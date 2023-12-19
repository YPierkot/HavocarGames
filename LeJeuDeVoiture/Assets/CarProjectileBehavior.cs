using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AbilityNameSpace
{
    public class CarProjectileBehavior : MonoBehaviour
    {
        public Vector3 movement;
        public TrailRenderer trail;
        public int damages;
        void FixedUpdate()
        {
            transform.position += movement * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) return;
            
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damages);
            }
        }
    }   
}
