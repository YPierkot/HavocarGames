using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilityNameSpace
{
    public class CarProjectileCollision : MonoBehaviour
    {
        public CarProjectileBehavior projectile;
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Collision !!!");
            
            projectile.movement = Vector3.Reflect(projectile.movement, other.contacts[0].normal);
            projectile.transform.rotation = Quaternion.LookRotation(projectile.movement);
        }
    }
}
