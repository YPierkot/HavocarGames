using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;

namespace EnemyNamespace
{
    public class GuardianTower : Tower
    {
        [SerializeField] private int bulletInRumbleCount;
        [SerializeField] private int timeBewteenShotsInMilliseconds = 100;
        [SerializeField] private float speedLooseByPlayer = 1.5f;
        [SerializeField] private float bulletScattering = 1;
        
        public LayerMask playerMask;
        public LayerMask groundMask;
        public LayerMask wallMask;

        protected override void Spawn()
        {
            base.Spawn();
            beamLr.SetPosition(0, projectileLaunchPos.position);
        }

        #region AimingModify
        
        protected override async void TurretAiming()
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = projectileLaunchPos.position;
            positions[1] = playerPos.position;
            lr.SetPositions(positions);
            lr.startWidth = (1 - (timer / timeBeforeShootInSeconds)) * 0.55f;
            lr.endWidth = (1 - (timer / timeBeforeShootInSeconds)) * 0.4f;
            
            if (isAiming)
            {
                timer += Time.deltaTime;
                if (timer > timeBeforeShootInSeconds)
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
                await DoShoot();
                await Task.Delay(timeBewteenShotsInMilliseconds);
            }
        }

        private async Task DoShoot()
        {
            var shootPos = playerPos.position; // Get la pos du player
            shootPos += new Vector3(Random.Range(-bulletScattering, bulletScattering), 0, Random.Range(-bulletScattering, bulletScattering));
            
            // Lancer le tir 
            Vector3 dir = (shootPos - projectileLaunchPos.position).normalized;
            if (Physics.Raycast(projectileLaunchPos.position, dir, out var hitWall, Mathf.Infinity, wallMask))
            {
                ActivateBeam(hitWall.point, hitWall.normal);
                Instantiate(explosionFeedback, hitWall.point, Quaternion.LookRotation(hitWall.normal));
                return;
            }

            if (Physics.Raycast(projectileLaunchPos.position, dir, out var hitPlayer, Mathf.Infinity, playerMask))
            {
                ActivateBeam(hitPlayer.point, hitPlayer.normal);
                Instantiate(explosionFeedback, hitPlayer.point, Quaternion.LookRotation(hitPlayer.normal));
                if (car.abilitiesManager.isShielded) return;
                if (car.enabled)
                {
                    // Appliquer les dégats
                    car.maxSpeed -= speedLooseByPlayer;
                }

                return;
            }

            if (Physics.Raycast(projectileLaunchPos.position, dir, out var hitGround, Mathf.Infinity, groundMask))
            {
                ActivateBeam(hitGround.point, hitGround.normal);
                Instantiate(explosionFeedback, hitGround.point, Quaternion.LookRotation(hitGround.normal));
                return;
            }
        }

        #endregion
        
        #region Vfx
        [Header("Gamefeel Section")] 
        [SerializeField] private LineRenderer beamLr;
        [SerializeField] private ParticleSystem muzzleParticuleSystem;
        [SerializeField] private ParticleSystem hitPointParticeSystem;
        [SerializeField] private GameObject explosionFeedback;

        private void ActivateBeam(Vector3 hitPoint, Vector3 normalPoint)
        {
            beamLr.SetPosition(1, hitPoint);
            beamLr.enabled = true;
            
            hitPointParticeSystem.transform.position = hitPoint;
            hitPointParticeSystem.transform.LookAt(projectileLaunchPos.position);
            muzzleParticuleSystem.transform.LookAt(hitPoint);
            
            muzzleParticuleSystem.Play();
            hitPointParticeSystem.Play();
            
            Invoke("DesactivateBeam", 0.35f);
        }

        private void DesactivateBeam()
        {
            beamLr.enabled = false;
            muzzleParticuleSystem.Stop();
            hitPointParticeSystem.Stop();
        }

        #endregion
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(car.transform.position, Vector3.up, bulletScattering * 2, 5f);
        }
#endif
    }
}
