using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCup : MonoBehaviour
{
    public Transform[] cups;
    public float liftHeight = 0.67f;
    public float liftSpeed = 1f;
    public float fallSpeed = 1f;
    public float rotationSpeed = 180f;
    public float scaleAtTop = 1.5f;
    public float animationInterval = 10f;
    public int bonusPoints = 3;
    public GameManager gameManager;
    public bool animateChildren = true;
    public bool keepCollidersActive = true;

    float nextAnimationTime;
    bool isAnimating = false;
    Transform currentCup;
    Vector3 originalPosition;
    Quaternion originalRotation;
    Vector3 originalScale;
    Goal cupGoal;
    bool gameStarted = false;
    float gameStartTime;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        gameStartTime = Time.time;
        nextAnimationTime = gameStartTime + 1f;
    }

    void Update()
    {
        if (!gameStarted && Time.time >= gameStartTime + 1f)
        {
            gameStarted = true;
            StartCoroutine(AnimateRandomCup());
        }
        if (gameStarted && !isAnimating)
        {
            StartCoroutine(AnimateRandomCup());
        }
    }

    IEnumerator AnimateRandomCup()
    {
        isAnimating = true;
        int randomIndex;
        if (cups.Length > 1 && currentCup != null)
        {
            int previousIndex = System.Array.IndexOf(cups, currentCup);
            do {
                randomIndex = Random.Range(0, cups.Length);
            } while (randomIndex == previousIndex);
        }
        else
        {
            randomIndex = Random.Range(0, cups.Length);
        }
        currentCup = cups[randomIndex];
        cupGoal = currentCup.GetComponentInChildren<Goal>();
        Transform cupToAnimate = currentCup;
        Vector3 originalPos = cupToAnimate.position;
        Quaternion originalRot = cupToAnimate.rotation;
        Vector3 originalScale = cupToAnimate.localScale;
        Dictionary<Collider, bool> collidersState = new Dictionary<Collider, bool>();
        if (!keepCollidersActive)
        {
            Collider[] colliders = cupToAnimate.GetComponentsInChildren<Collider>();
            foreach (Collider collider in colliders)
            {
                collidersState[collider] = collider.enabled;
                collider.enabled = false;
            }
        }
        if (cupGoal != null)
        {
            cupGoal.pointsOnGoal += bonusPoints;
        }
        float liftTime = liftHeight / Mathf.Max(0.01f, liftSpeed);
        float t = 0;
        while (t < liftTime)
        {
            float progress = t / liftTime;
            float ease = Mathf.SmoothStep(0, 1, progress);
            cupToAnimate.position = Vector3.Lerp(originalPos, originalPos + Vector3.up * liftHeight, ease);
            cupToAnimate.localScale = Vector3.Lerp(originalScale, originalScale * scaleAtTop, ease);
            cupToAnimate.rotation = originalRot;
            t += Time.deltaTime;
            yield return null;
        }
        cupToAnimate.position = originalPos + Vector3.up * liftHeight;
        cupToAnimate.localScale = originalScale * scaleAtTop;
        cupToAnimate.rotation = originalRot;
        float rotationTime = 360f / Mathf.Max(1f, rotationSpeed);
        t = 0;
        float startAngle = cupToAnimate.eulerAngles.y;
        float targetAngle = startAngle + 360f;
        while (t < rotationTime)
        {
            float progress = t / rotationTime;
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, progress);
            Vector3 currentEuler = cupToAnimate.eulerAngles;
            currentEuler.y = currentAngle;
            cupToAnimate.eulerAngles = currentEuler;
            cupToAnimate.position = originalPos + Vector3.up * liftHeight;
            cupToAnimate.localScale = originalScale * scaleAtTop;
            t += Time.deltaTime;
            yield return null;
        }
        Vector3 finalEuler = cupToAnimate.eulerAngles;
        finalEuler.y = startAngle;
        cupToAnimate.eulerAngles = finalEuler;
        float fallTime = liftHeight / Mathf.Max(0.01f, fallSpeed);
        t = 0;
        while (t < fallTime)
        {
            float progress = t / fallTime;
            float ease = Mathf.SmoothStep(0, 1, progress);
            cupToAnimate.position = Vector3.Lerp(originalPos + Vector3.up * liftHeight, originalPos, ease);
            cupToAnimate.localScale = Vector3.Lerp(originalScale * scaleAtTop, originalScale, ease);
            cupToAnimate.rotation = Quaternion.Slerp(Quaternion.Euler(finalEuler), originalRot, progress);
            t += Time.deltaTime;
            yield return null;
        }
        cupToAnimate.position = originalPos;
        cupToAnimate.localScale = originalScale;
        cupToAnimate.rotation = originalRot;
        if (!keepCollidersActive)
        {
            foreach (KeyValuePair<Collider, bool> entry in collidersState)
            {
                entry.Key.enabled = entry.Value;
            }
        }
        if (cupGoal != null)
        {
            cupGoal.pointsOnGoal -= bonusPoints;
        }
        isAnimating = false;
        nextAnimationTime = Time.time + animationInterval;
    }
}
