using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [Tooltip("Prefab du système de particules à jouer quand un légume entre dans le but")]
    public ParticleSystem particlesPrefab;
    [Tooltip("Décalage de position pour l'instanciation des particules")]
    public Vector3 particlesOffset = new Vector3(0, 0.1f, 0);
    GameManager gameManager;
    [Min(0)] public int pointsOnGoal = 1;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            if (particlesPrefab != null)
            {
                Vector3 spawnPosition = transform.position + particlesOffset;
                ParticleSystem particlesInstance = Instantiate(particlesPrefab, spawnPosition, Quaternion.identity);
                particlesInstance.Play();
                Destroy(particlesInstance.gameObject, particlesInstance.main.duration + 1f);
            }
            
            if (gameManager != null)
                gameManager.AddScore(pointsOnGoal);

            Destroy(rb.gameObject);
        }
    }
}
