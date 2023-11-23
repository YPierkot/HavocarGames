using UnityEngine;

namespace EnemyNamespace 
{
    public class TurretBullet : MonoBehaviour
    {
        public Rigidbody rb;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hit collide with player");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Hit collide {other.gameObject.name}");
                Destroy(gameObject);
            }
        }

        public void Setup(Vector3 aimedPos, float projectileSpeed)
        {
            //transform.LookAt(aimedPos);
            Debug.DrawLine(transform.position, aimedPos, Color.green, 1);
            rb.AddForce(transform.forward * projectileSpeed);
        }
    }
}

