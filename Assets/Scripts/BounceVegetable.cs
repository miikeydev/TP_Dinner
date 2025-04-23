using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceVegetable : MonoBehaviour
{
    Animator anim;
    [Range(0, 10)] public float bounceForce = 5;
    [Range(0, 1)] public float bounceUpward = 0.5f;
    public AnimationCurve bounceAnimCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float animDuration = 0.3f;
    public float squashAmount = 0.4f;
    public float rotationAmount = 10f;     
    public float jumpHeight = 0.2f;        
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 0); 

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            Vector3 toObject = (collision.transform.position - transform.position).normalized;
            Vector3 direction = Vector3.Lerp(toObject, Vector3.up, bounceUpward).normalized;
            rb.AddForce(direction * bounceForce, ForceMode.Impulse);
            Vector3 squashDir = toObject;
            StartCoroutine(BounceAnimation(squashDir));
        }
    }

    IEnumerator BounceAnimation(Vector3 squashDir)
    {
        float t = 0;
        Vector3 baseScale = transform.localScale;
        Vector3 basePosition = transform.position;
        Quaternion baseRotation = transform.rotation;
        
        
        Vector3 squashAxis = squashDir;
        if (Mathf.Abs(squashAxis.x) > Mathf.Abs(squashAxis.y) && Mathf.Abs(squashAxis.x) > Mathf.Abs(squashAxis.z))
            squashAxis = new Vector3(Mathf.Sign(squashAxis.x), 0, 0);
        else if (Mathf.Abs(squashAxis.y) > Mathf.Abs(squashAxis.z))
            squashAxis = new Vector3(0, Mathf.Sign(squashAxis.y), 0);
        else
            squashAxis = new Vector3(0, 0, Mathf.Sign(squashAxis.z));
            
        Vector3 randomRotAxis = new Vector3(
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f)
        ).normalized;

        float crushPhase = animDuration * 0.25f;
        while (t < crushPhase)
        {
            float normalizedTime = t / crushPhase;
            float eval = Mathf.Pow(normalizedTime, 0.6f); 
            
            Vector3 crushVec = new Vector3(
                squashAxis.x != 0 ? -eval * squashAmount * 1.8f : 0,
                -eval * squashAmount * 1.9f, 
                squashAxis.z != 0 ? -eval * squashAmount * 1.8f : 0
            );
            
            Vector3 expandVec = new Vector3(
                squashAxis.x == 0 ? eval * squashAmount * 1.4f : 0,
                0, 
                squashAxis.z == 0 ? eval * squashAmount * 1.4f : 0
            );
            
            transform.localScale = baseScale + crushVec + expandVec;
            transform.position = basePosition - Vector3.up * eval * 0.05f;
            
            t += Time.deltaTime;
            yield return null;
        }
        
        while (t < animDuration)
        {
            float normalizedTime = (t - crushPhase) / (animDuration - crushPhase);
            
            float reboundCurve = Mathf.Sin(normalizedTime * Mathf.PI);
            float heightFactor = 1.0f - normalizedTime; 
            
            Vector3 stretchVec = new Vector3(
                0,
                reboundCurve * squashAmount * 1.5f, 
                0
            );
            
            Vector3 compressVec = new Vector3(
                -reboundCurve * squashAmount * 0.3f,
                0,
                -reboundCurve * squashAmount * 0.3f
            );
            
            transform.localScale = baseScale + stretchVec + compressVec;
            
            float rotationEval = reboundCurve * heightFactor;
            transform.rotation = baseRotation * Quaternion.AngleAxis(
                rotationEval * rotationAmount * 1.2f,
                randomRotAxis
            );
            
            float jumpHeight2 = jumpHeight * 2.2f; 
            float jumpEval = reboundCurve;
            transform.position = basePosition + Vector3.up * jumpEval * jumpHeight2;
            
            t += Time.deltaTime;
            yield return null;
        }
        
        float settlingTime = animDuration * 0.5f;
        float settlingT = 0;
        
        while (settlingT < settlingTime)
        {
            float normalizedTime = settlingT / settlingTime;
            float bounceFactor = Mathf.Exp(-normalizedTime * 4) * Mathf.Sin(normalizedTime * 15);
            
            transform.position = basePosition + Vector3.up * bounceFactor * 0.05f;
            
            Vector3 settleScale = baseScale + new Vector3(
                bounceFactor * 0.03f,
                -bounceFactor * 0.05f,
                bounceFactor * 0.03f
            );
            transform.localScale = settleScale;
            
            settlingT += Time.deltaTime;
            yield return null;
        }
        
        transform.localScale = baseScale;
        transform.position = basePosition;
        transform.rotation = baseRotation;
    }
}
