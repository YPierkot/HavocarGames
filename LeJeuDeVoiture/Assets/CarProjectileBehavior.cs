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
        void FixedUpdate()
        {
            transform.position += movement * Time.fixedDeltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("HIT SOMETHING");
            if (other.gameObject)
            {
                
            }
        }
    }   
}
