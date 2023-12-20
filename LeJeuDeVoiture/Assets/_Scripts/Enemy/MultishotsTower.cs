using System.Threading.Tasks;
using EnemyNamespace;
using UnityEngine;

public class MultishotsTower : Tower
{
    [Space]
    [Header("MultiShots Turret Section")]
    [SerializeField] private int bulletInRumbleCount;
    [SerializeField] private float Y;
    [SerializeField] private float Z;
    [SerializeField] private int waitingDurationBetweenShotsInMilliseconds;
    [SerializeField] private float shootMaxDisplacement = 8f;
    [SerializeField] private GameObject bombPrefab;

    protected override async Task TurretAiming()
    {
        base.TurretAiming();
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
        base.TurretShoot();
        for (int i = 0; i < bulletInRumbleCount; i++)
        {
            if (this == null) break;
            DoShoot();
            await Task.Delay(waitingDurationBetweenShotsInMilliseconds);
        }
    }

    private void DoShoot()
    {
        Vector3 nextShootPos = new Vector3(Random.Range(-shootMaxDisplacement, shootMaxDisplacement), 0, Random.Range(-shootMaxDisplacement, shootMaxDisplacement));
        Instantiate(bombPrefab, nextShootPos, Quaternion.identity);
    }
}
