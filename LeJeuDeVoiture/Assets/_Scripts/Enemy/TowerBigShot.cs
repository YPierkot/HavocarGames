using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EnemyNamespace
{
    public class TowerBigShot : Tower
    {
        public int shootDelayInMilliseconds = 50;
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

            //ModifyMeshFormPlayerSpeed(car.speed);

            if (isAiming)
            {
                timer += Time.deltaTime;
                if (timer > timeBeforeShootInSeconds)
                {
                    isAiming = false;
                    // Shoot sur le joueur
                    await TurretShoot();
                    SwitchState(TurretState.Sleep);
                }
            }
        }

        protected override async Task TurretShoot()
        {
            var shootPos = playerPos.position; // Get la pos du player
            await Task.Delay(shootDelayInMilliseconds); // Attendre le delay
            
            // Lancer le beam à la position puis désactiver 0.3f plus tard
            
            
            // Lancer le bullet feedback
            
            // Lancer le tir 
            Vector3 dir = ( shootPos - projectileLaunchPos.position).normalized;
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
                    car.enabled = false;
                    await Task.Delay(4000);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        [SerializeField] private Image aimImage;

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
    }
}