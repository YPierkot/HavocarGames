using System;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyTurret : Enemy
    {
        [Space(3)]
        [Header("Turret Section")]
        [SerializeField] private TurretState currentState = TurretState.Sleep;
        [SerializeField] private bool isAiming;
        [SerializeField] private float detectionDst;
        [SerializeField] private float timeBeforeShootInSeconds = 5f;
        [SerializeField] private int shootDelayInMilliseconds = 140;
        
        [SerializeField] private GameObject turretProjectilePrefab;
        [SerializeField] private Transform projectileLaunchPos;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private float turretShotCooldown = 5f;
        [SerializeField] private bool isInCooldown;
        
        private LineRenderer lr;
        //private float timer = 0f;
        
        // Start is called before the first frame update
        void Start()
        {
            Spawn();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            ExecuteState();
        }

        private void ExecuteState()
        {
            switch (currentState)
            {
                case TurretState.Aiming: TurretAiming(); break;
                case TurretState.Sleep: TurretSleep(); break;
                case TurretState.Dead: Death(); break;
                default: Debug.Log(currentState); break;
            }
        }
        
        private void SwitchState(TurretState turretState)
        {
            switch (turretState)
            {
                case TurretState.Aiming: ToAiming(); break;
                case TurretState.Sleep: ToSleep(); break;
                case TurretState.Dead: ToDead(); break;
                case TurretState.None: /*ToNone();*/ break;
                default: Debug.Log(turretState); break;
            }

            currentState = turretState;
        }

        #region Aiming

        private void ToAiming()
        {
            isAiming = true;
            timer = 0;
            lr.enabled = true;
        }

        private async void TurretAiming()
        {
            Vector3[] positions = new Vector3[2];
            positions[0] = projectileLaunchPos.position;
            positions[1] = playerPos.position;
            lr.SetPositions(positions);

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

        private async Task TurretShoot()
        {
            var shootPos = playerPos.position;
            await Task.Delay(shootDelayInMilliseconds);
            var go = Instantiate(turretProjectilePrefab, projectileLaunchPos.position, Quaternion.identity);
            go.transform.LookAt(shootPos);
            go.GetComponent<Rigidbody>().AddForce(go.transform.forward * bulletSpeed);
            Task.Yield();
        }

        #endregion
        
        #region Sleep
        private void ToSleep()
        {
            isAiming = false;
            timer = 0;
            lr.enabled = false;
            isInCooldown = true;
        }

        private void TurretSleep()
        {
            if (isInCooldown)
            {
                timer += Time.deltaTime;
                if (timer > turretShotCooldown)
                {
                    isInCooldown = false;
                }
                else
                {
                    return;
                }
            }

            if (Vector3.Distance(transform.position, playerPos.position) < detectionDst)
                SwitchState(TurretState.Aiming);
        }

        #endregion

        #region Death

        private void ToDead()
        {
            throw new NotImplementedException();
        }

        public override void Death()
        {
            Debug.Log($"Turret {gameObject.name} is dead");
            isDead = true;
            SwitchState(TurretState.None);
        }
        #endregion
        
        private TurretState CurrentState() => currentState;

        private void OnDrawGizmos()
        {
           Gizmos.DrawWireSphere(transform.position, detectionDst);
        }

        public override void CollideWithPlayer()
        {
            
        }

        private enum TurretState
        {
            Aiming,
            Sleep,
            Dead,
            None
        }
    }
}