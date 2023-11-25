using System;
using System.Threading.Tasks;
using CarNameSpace;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField] private CarController car;
        [SerializeField] private MeshRenderer MeshRenderer;
        [SerializeField] private Material[] mats;
        [SerializeField] private float speedToExecuteTower;
        
        private LineRenderer lr;
        //private float timer = 0f;
        
        // Start is called before the first frame update
        void Start()
        {
            Spawn();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
            car = playerPos.GetComponent<CarController>();
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

            ModifyMeshFormPlayerSpeed(car.speed);

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
                    if (hit.collider.GetComponent<CarController>().enabled)
                    {
                        hit.collider.GetComponent<CarController>().enabled = false;
                        await Task.Delay(4000);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                    
                }
            }
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
            ModifyMeshFormPlayerSpeed(car.speed);
            
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

        private void ModifyMeshFormPlayerSpeed(float playerSpeed)
        {
            MeshRenderer.material = playerSpeed < speedToExecuteTower ? mats[0] : mats[1];
        }

        private void OnDrawGizmos()
        {
           Gizmos.DrawWireSphere(transform.position, detectionDst);
        }

        public override void CollideWithPlayer()
        {
            Debug.Log("COLLIDE");
            
            if (car.speed < speedToExecuteTower) return;
            Debug.Log("Enemy Dies");
            Destroy(this.gameObject);
            // TODO -> Passer en state mort quand on aura des assets & gamefeel pour différencier les deux states
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