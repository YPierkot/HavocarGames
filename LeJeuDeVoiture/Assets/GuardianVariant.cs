using System;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyNamespace
{ 
    public class GuardianVariant : Tower
    {
        [SerializeField] private float aimAngle = 35f;
        [SerializeField] private int bulletInRumbleCount;
        [SerializeField] private int timeBewteenShotsInMilliseconds;

        private Vector3[] positions = Array.Empty<Vector3>();
        [ContextMenu("Balayage")]
        public void Balayage()
        {
            positions = new Vector3[bulletInRumbleCount];
            for (int i = 0; i < bulletInRumbleCount; i++)
            {
                //   2 * Math.PI -> ça q'il faut modifier
                float angle = i * ( aimAngle * Mathf.Deg2Rad / bulletInRumbleCount);
                Debug.Log(angle);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(transform.position.x + transform.forward.x + + x, 0, transform.forward.z + transform.forward.z + z);
            }
        }

        private void OnDrawGizmos()
        {
            if (positions.Length != 0 || positions != Array.Empty<Vector3>())
            {
                for (int i = 0; i < positions.Length; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(positions[i], 0.5f);
                }
            }
        }

        #region AimingModify
        
        protected override async void TurretAiming()
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = projectileLaunchPos.position;
            positions[1] = playerPos.position;
            lr.SetPositions(positions);
            lr.startWidth = (1 - (aimingTimer / shootLoadingDuration)) * 0.55f;
            lr.endWidth = (1 - (aimingTimer / shootLoadingDuration)) * 0.4f;
            
            if (isAiming)
            {
                Vector3 dir = (playerPos.position - projectileLaunchPos.position).normalized;
                if(Physics.Raycast(projectileLaunchPos.position, dir, out var hit, 1000))
                {
                    aimingTimer = hit.collider.CompareTag("Player") || hit.collider.CompareTag("Player")
                        ? aimingTimer += Time.deltaTime
                        : aimingTimer -= Time.deltaTime;
                }
                
                if (aimingTimer > shootLoadingDuration)
                {
                    isAiming = false;
                    await TurretShoot();
                    SwitchState(TurretState.Sleep);
                }
                else if (aimingTimer < 0)
                {
                    isAiming = false;
                    SwitchState(TurretState.Sleep);
                }
            }
        }

        protected override async Task TurretShoot()
        {
            DoShoot();
            
            // for (int i = 0; i < bulletInRumbleCount; i++)
            // {
            //     await Task.Delay(timeBewteenShotsInMilliseconds);
            // }
        }
        
        private void DoShoot()
        {
            //var shootPos = playerPos.position; // Get la pos du player
            //shootPos += new Vector3(Random.Range(-bulletScattering, bulletScattering), 0, Random.Range(-bulletScattering, bulletScattering));
            
            // Balayage
            
            // Choisir une position sur la range
            
            // Lancer le projectile
            //Vector3 dir = (shootPos - projectileLaunchPos.position).normalized;
            //var tempProjectile = Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.LookRotation(dir));
        }

        #endregion
        
        #region Vfx
        #endregion
    }
}
