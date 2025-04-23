using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VegetableThrower : MonoBehaviour
{
    [Tooltip("Collection of vegetables that can be thrown")] public Rigidbody[] vegetables;
    [Header("Throw")]
    [Range(0, 20), Tooltip("Minimum force to apply to a vegetable when throwing")] public float minThrowForce = 2;
    [Range(0, 20), Tooltip("Maximum force to apply to a vegetable when throwing")] public float maxThrowForce = 10;
    [Range(0.5f, 3), Tooltip("Time the player has to hold the button to throw at max strengh")] public float throwForceChargeTime = 1;
    float throwForce01;
    [Range(0,30), Tooltip("Maximum torque to apply to a vegetable when throwing")] public float maxRandomTorque = 10;

    [Header("UI")]
    public Image chargeBarFilling;

    float chargeStartTime;
    bool isCharging = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isCharging = true;
            chargeStartTime = Time.time;
        }
        if (Input.GetMouseButton(0) && isCharging)
        {
            float elapsed = Time.time - chargeStartTime;
            throwForce01 = Mathf.Clamp01(elapsed / throwForceChargeTime);
        }
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, throwForce01);
            Rigidbody veg = InstantiateRandomVegetable();
            ThrowVegetable(veg, throwForce);
            throwForce01 = 0;
            isCharging = false;
        }

        chargeBarFilling.fillAmount = throwForce01;
    }

    Rigidbody InstantiateRandomVegetable()
    {
        int idx = Random.Range(0, vegetables.Length);
        Rigidbody prefab = vegetables[idx];
        Rigidbody instance = Instantiate(prefab, transform.position, transform.rotation);
        return instance;
    }

    void ThrowVegetable(Rigidbody vegetable, float throwForce)
    {
        Vector3 direction = transform.forward;
        vegetable.AddForce(direction * throwForce, ForceMode.Impulse);
        Vector3 randomTorque = Random.insideUnitSphere * maxRandomTorque;
        vegetable.AddTorque(randomTorque, ForceMode.Impulse);
    }
}
