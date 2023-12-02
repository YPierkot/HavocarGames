using UnityEngine;

namespace EnemyNamespace
{
    public class TurretBullet : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Destroy(gameObject);
        }
    }
}