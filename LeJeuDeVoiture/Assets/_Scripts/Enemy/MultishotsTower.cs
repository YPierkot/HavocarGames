using System.Threading.Tasks;
using UnityEngine;

namespace EnemyNamespace
{
    public class MultishotsTower : Tower
    {
        [Space] [Header("MultiShots Turret Section")] [SerializeField]
        private int bulletInRumbleCount;

        [SerializeField] private float bombCastingDuration;
        [SerializeField] private float damageToApply;
        [SerializeField] private float explosionSize;
        [SerializeField] private int waitingDurationBetweenShotsInMilliseconds;
        [SerializeField] private float attackRadius;
        [SerializeField] private GameObject bombPrefab;

        protected override async Task TurretAiming()
        {
            // Faire  comprendre que la tourelle va tirer sans LR

            if (isAiming)
            {
                Vector3 dir = (playerPos.position - projectileLaunchPos.position).normalized;
                if (Physics.Raycast(projectileLaunchPos.position, dir, out var hit, 1000))
                {
                    aimingTimer = hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy")
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
            for (int i = 0; i < bulletInRumbleCount; i++)
            {
                if (this == null) break;
                DoShoot();
                await Task.Delay(waitingDurationBetweenShotsInMilliseconds);
            }
        }
        
        private void DoShoot()
        {
            Vector2 v = Random.insideUnitCircle * attackRadius;
            Vector3 nextShootPos = new Vector3(transform.position.x + v.x, transform.position.y + 0.2f, transform.position.z + v.y);
            var bomb = Instantiate(bombPrefab, nextShootPos, Quaternion.identity);
            var bill = Instantiate(turretProjectilePrefab, transform.position, Quaternion.identity).GetComponent<BulletBill>();
            bomb.GetComponent<Enemy_Bomb>().Setup(bombCastingDuration, damageToApply, explosionSize, projectileLaunchPos.position, bill);
        }
    }
}