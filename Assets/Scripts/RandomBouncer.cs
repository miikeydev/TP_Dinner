using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBouncer : MonoBehaviour
{
    [Header("Objets")]
    [Tooltip("Objets qui peuvent rebondir aléatoirement")]
    public GameObject[] objectsToBounce;

    [Header("Paramètres de saut")]
    [Range(0.05f, 0.5f)]
    [Tooltip("Hauteur maximale du saut")]
    public float bounceHeight = 0.1f;
    
    [Range(0.1f, 2f)]
    [Tooltip("Durée de l'animation complète (montée + descente)")]
    public float bounceDuration = 0.5f;
    
    [Range(0f, 0.9f)]
    [Tooltip("Probabilité qu'un objet commence à sauter chaque seconde")]
    public float bounceChance = 0.1f;
    
    [Range(0f, 10f)]
    [Tooltip("Temps minimal entre deux sauts du même objet")]
    public float minTimeBetweenBounces = 2f;
    
    [Range(0, 3)]
    [Tooltip("Nombre maximal d'objets qui peuvent sauter simultanément (0 = illimité)")]
    public int maxSimultaneousBounces = 1;

    [Header("Animation Cartoon")]
    [Tooltip("Courbe d'animation pour le rebond")]
    public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
    
    [Range(0f, 1f)]
    [Tooltip("Facteur d'aléatoire appliqué à la hauteur (0 = hauteur fixe, 1 = entre 0 et hauteur max)")]
    public float randomHeightFactor = 0.3f;
    
    [Range(0f, 0.5f)]
    [Tooltip("Intensité de l'effet d'écrasement (squash) lors du décollage et atterrissage")]
    public float squashStrength = 0.2f;
    
    [Range(0f, 0.5f)]
    [Tooltip("Intensité de l'effet d'étirement (stretch) pendant le saut")]
    public float stretchStrength = 0.15f;

    [Tooltip("Courbe d'animation pour l'écrasement (squash & stretch)")]
    public AnimationCurve squashCurve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.1f, 1),
        new Keyframe(0.2f, 0),
        new Keyframe(0.5f, -0.5f),
        new Keyframe(0.9f, 0),
        new Keyframe(1, 1)
    );

    private Dictionary<GameObject, float> nextBounceTime = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();
    private int currentlyBouncing = 0;

    void Start()
    {
        foreach (GameObject obj in objectsToBounce)
        {
            if (obj != null)
            {
                nextBounceTime[obj] = Time.time + Random.Range(0f, minTimeBetweenBounces);
                originalScales[obj] = obj.transform.localScale;
            }
        }
    }

    void Update()
    {
        if (maxSimultaneousBounces > 0 && currentlyBouncing >= maxSimultaneousBounces)
            return;

        float currentTime = Time.time;
        
        foreach (GameObject obj in objectsToBounce)
        {
            if (obj != null && currentTime >= nextBounceTime[obj])
            {
                if (Random.value < bounceChance * Time.deltaTime)
                {
                    nextBounceTime[obj] = currentTime + bounceDuration + minTimeBetweenBounces;
                    
                    float actualHeight = bounceHeight * (1f - randomHeightFactor * Random.value);
                    
                    StartCoroutine(BounceObjectCartoon(obj, actualHeight));
                }
            }
        }
    }

    IEnumerator BounceObjectCartoon(GameObject obj, float height)
    {
        currentlyBouncing++;
        
        
        Vector3 startPos = obj.transform.position;
        Vector3 originalScale = originalScales[obj];
        float startTime = Time.time;
        float endTime = startTime + bounceDuration;
        
        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / bounceDuration;
            
            
            float bounceEval = bounceCurve.Evaluate(normalizedTime);
            Vector3 newPos = startPos + new Vector3(0, bounceEval * height, 0);
            
            
            float squashEval = squashCurve.Evaluate(normalizedTime);
            
            
            Vector3 newScale = originalScale;
            if (squashEval > 0) 
            {
                newScale.y = originalScale.y * (1 - squashEval * squashStrength);
                newScale.x = originalScale.x * (1 + squashEval * squashStrength * 0.5f);
                newScale.z = originalScale.z * (1 + squashEval * squashStrength * 0.5f);
            }
            else 
            {
                float stretchEval = -squashEval;
                newScale.y = originalScale.y * (1 + stretchEval * stretchStrength);
                newScale.x = originalScale.x * (1 - stretchEval * stretchStrength * 0.3f);
                newScale.z = originalScale.z * (1 - stretchEval * stretchStrength * 0.3f);
            }
            
            obj.transform.position = newPos;
            obj.transform.localScale = newScale;
            
            yield return null;
        }
        
        obj.transform.position = startPos;
        obj.transform.localScale = originalScale;
        
        currentlyBouncing--;
    }
    
    void OnValidate()
    {
        if (bounceCurve.length == 0)
        {
            bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
            bounceCurve.AddKey(0.5f, 1);
        }
        
        if (squashCurve.length == 0)
        {
            squashCurve = new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.1f, 1),
                new Keyframe(0.2f, 0),
                new Keyframe(0.5f, -0.5f),
                new Keyframe(0.9f, 0),
                new Keyframe(1, 1)
            );
        }
    }
}
