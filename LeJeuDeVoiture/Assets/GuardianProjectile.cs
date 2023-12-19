using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GuardianProjectile : MonoBehaviour
{
    [SerializeField] public float projectileSpeed;
    [SerializeField] private int damages;

    [SerializeField] private ParticleSystem muzzlePS;
    [SerializeField] private GameObject explosionPS;
    
    private void Start()
    {
        muzzlePS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * (projectileSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(explosionPS, transform.position + new Vector3(0, 0, -0.4f), Quaternion.LookRotation(-other.ClosestPoint(other.transform.position)));
            other.GetComponent<IDamageable>()?.TakeDamage(damages);
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Wall"))
        {
            Instantiate(explosionPS, transform.position + new Vector3(0, 0, -0.4f), Quaternion.LookRotation(-other.ClosestPoint(other.transform.position)));
            Destroy(gameObject);
        }
    }
}
