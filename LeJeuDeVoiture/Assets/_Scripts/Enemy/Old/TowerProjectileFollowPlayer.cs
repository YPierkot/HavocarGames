using System.Threading.Tasks;
using UnityEngine;

namespace EnemyNamespace
{
    public class TowerProjectileFollowPlayer : Tower
    {
        #region ModifyAiming
        protected override void TurretAiming()
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = projectileLaunchPos.position;
            positions[1] = playerPos.position;
            lr.SetPositions(positions);
            lr.startWidth = (1 - (aimingTimer / shootLoadingDuration)) * 0.55f;
            lr.endWidth = (1 - (aimingTimer / shootLoadingDuration)) * 0.4f;

            //ModifyMeshFormPlayerSpeed(car.speed);

            if (isAiming)
            {
                aimingTimer += Time.deltaTime;
                if (aimingTimer > shootLoadingDuration)
                {
                    isAiming = false;
                    // Shoot sur le joueur
                    TurretShoot();
                    SwitchState(TurretState.Sleep);
                }
            }
        }

        protected override Task TurretShoot()
        {
            var shootPos = playerPos.position; // Get la pos du player

            // Lancer le BulletBill
            var go = Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.identity);
            go.transform.LookAt(shootPos);
            go.GetComponent<BulletBill>().Setup(car);
            return Task.CompletedTask;
        }
        #endregion
    }
}
