using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EnemyNamespace
{ 
    public class GuardianVariant : Tower
    {
        [SerializeField] private float aimAngle = 35f;
        [SerializeField] private float angleOffset = 35f;
        [SerializeField] private float aimRadius = 35f;
        [SerializeField] private int bulletInRumbleCount;
        [SerializeField] private int timeBewteenShotsInMilliseconds;

        protected override void Spawn()
        {
            base.Spawn();
            SetupAimingPos();
        }

        private Vector3[] aimPositions = Array.Empty<Vector3>();
        private void SetupAimingPos()
        {
            aimPositions = new Vector3[bulletInRumbleCount];
            for (int i = 0; i < bulletInRumbleCount; i++)
            {
                //   2 * Math.PI -> ça q'il faut modifier
                float angle = angleOffset / 2 + i * (aimAngle * Mathf.Deg2Rad / bulletInRumbleCount);
                //Debug.Log(angle);
                float x = Mathf.Cos(angle) * aimRadius;
                float z = Mathf.Sin(angle) * aimRadius;

                aimPositions[i] = new Vector3(transform.position.x + x, 1.5f, transform.position.z + z);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            SetupAimingPos();
            if (aimPositions.Length != 0 || aimPositions != Array.Empty<Vector3>())
            {
                for (int i = 0; i < aimPositions.Length; i++)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(aimPositions[i], 0.5f);
                }
            }
            
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, detectionDst, 8f);
            
            Handles.color = Color.blue;
            Handles.DrawWireDisc(transform.position, Vector3.up, spawningRadius, 4f);
            
            var positions = new Vector3[currentSentinelCount];
            var currentPos = transform.position;
            
            for (int i = 0; i < currentSentinelCount; i++)
            {
                float angle = i * (2 * Mathf.PI / currentSentinelCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;
            
                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + z);
                Handles.color = Color.green;
                Handles.DrawWireDisc(positions[i], Vector3.up, 2f, 4f);
            }
        }
#endif
        
        #region AimingModify
        protected override async void TurretAiming()
        {
            if (isAiming)
            {
                aimingTimer += Time.deltaTime;
                if (aimingTimer > shootLoadingDuration)
                {
                    isAiming = false;
                    await TurretShoot();
                    SwitchState(TurretState.Sleep);
                }
            }
        }

        protected override async Task TurretShoot()
        {
            for (int i = 0; i < bulletInRumbleCount; i++)
            { 
                DoShoot(i);
                await Task.Delay(timeBewteenShotsInMilliseconds);
            }
        }
        
        private void DoShoot(int it)
        {
            //var shootPos = playerPos.position; // Get la pos du player
            //shootPos += new Vector3(Random.Range(-bulletScattering, bulletScattering), 0, Random.Range(-bulletScattering, bulletScattering));
            
            // Balayage
            
            // Choisir une position sur la range
            
            // Lancer le projectile
            Vector3 dir = (aimPositions[it] - projectileLaunchPos.position).normalized;
            Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.LookRotation(dir));
        }

        #endregion
        
        #region Vfx
        #endregion
    }
}
