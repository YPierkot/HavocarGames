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
        [SerializeField] private GameObject turretHead;

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

                aimPositions[i] = new Vector3(transform.position.x + x, projectileLaunchPos.transform.position.y, transform.position.z + z);
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
            Handles.DrawWireDisc(transform.position, Vector3.up, detectionDst, 1f);
            
            Handles.color = Color.blue;
            Handles.DrawWireDisc(transform.position, Vector3.up, spawningRadius, .5f);
            
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
        protected override async Task TurretAiming()
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
            // Visual
            
            // Vector3 rot = Quaternion.LookRotation(aimPositions[it] - projectileLaunchPos.position).eulerAngles;
            // rot.x = -90;
            // rot.y = 0;
            // turretHead.transform.rotation = Quaternion.Euler(rot);
            visual.Shoot();
            
            // Lancer le projectile
            Vector3 dir = ( aimPositions[it] - projectileLaunchPos.position).normalized;
            float angle = Vector3.SignedAngle(turretHead.transform.forward,  dir, Vector3.up);
            turretHead.transform.rotation = Quaternion.Euler(-90, 0, angle);
            Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.LookRotation(dir));
        }

        #endregion
        
        #region Vfx
        #endregion
    }
}
