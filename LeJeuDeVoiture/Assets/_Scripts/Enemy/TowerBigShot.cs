using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnemyNamespace
{
    public class TowerBigShot : Tower
    {
        public int shootDelayInMilliseconds = 70;
        public float bulletSpeed = 12500;
        
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
            
            // Lancer le bullet feedback
            var go = Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.identity);
            go.transform.LookAt(shootPos);
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * bulletSpeed);
            
            // Lancer le tir 
            Vector3 dir = ( shootPos - projectileLaunchPos.position).normalized;
            if (Physics.Raycast(projectileLaunchPos.position, dir, out var hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (car.abilitiesManager.isShielded) return;
                    if (car.enabled)
                    {
                        car.enabled = false;
                        await Task.Delay(4000);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    
                }
            }
        }
        #endregion
    }
}