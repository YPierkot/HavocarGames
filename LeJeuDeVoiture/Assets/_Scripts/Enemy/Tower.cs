using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyNamespace
{
    public class Tower : Enemy
    {
        [Space(3)]
        [Header("Turret Section")]
        [SerializeField] private TurretState currentState = TurretState.Sleep;
        
        [SerializeField] protected bool isAiming;
        [Tooltip("Distance à laquelle la tourelle détecte le joueur")] 
        [SerializeField] protected float detectionDst;
        [Tooltip("Délai de tir une fois la cible verrouillé")] 
        [SerializeField] protected float timeBeforeShootInSeconds = 5f;
        
        [SerializeField] protected GameObject turretProjectilePrefab;
        [SerializeField] protected Transform projectileLaunchPos;
        [Tooltip("Délai pendant lequel la tourelle ne peux pas ciblé le joueur")] 
        [SerializeField] protected float turretShotCooldown = 5f;
        [SerializeField] protected bool isInCooldown;
        
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected Material[] mats;
        
        [SerializeField] protected TextMeshProUGUI lifeText;
        [SerializeField] protected LineRenderer lr;
        
        // Start is called before the first frame update
        void Start()
        {
            Spawn();
            lr = GetComponent<LineRenderer>();
            lr.enabled = false;
        }
        
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
                default:
                    break;
            }
            
            UpdateRegen();
        }
        
        protected void SwitchState(TurretState turretState)
        {
            switch (turretState)
            {
                case TurretState.Aiming: ToAiming(); break;
                case TurretState.Sleep: ToSleep(); break;
                case TurretState.Dead: ToDead(); break;
                case TurretState.None: /*ToNone();*/ break;
                default: break;
            }

            currentState = turretState;
        }
        
        #region Aiming

        protected virtual void ToAiming()
        {
            isAiming = true;
            timer = 0;
            lr.enabled = true;
        }

        protected virtual void TurretAiming()
        {
        }

        protected virtual async Task TurretShoot()
        {
        }

        #endregion
        
        #region Sleep
        protected virtual void ToSleep()
        {
            isAiming = false;
            timer = 0;
            lr.enabled = false;
            isInCooldown = true;
        }

        protected virtual void TurretSleep()
        {
            //ModifyMeshFormPlayerSpeed(car.speed);
            
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

        protected virtual void ToDead()
        {
        }

        public override void Death() // Perso
        {
            isDead = true;
            SwitchState(TurretState.None);
        }
        #endregion
        
        public override void CollideWithPlayer()
        {
            if(!car.abilitiesManager.isInBulletMode) return;
            currentHealthPoints -= Mathf.FloorToInt(car.speed);
            UpdateCanvas();
            
            if (currentHealthPoints < 1) Destroy(gameObject); // TODO -> Passer en state mort quand on aura des assets & gamefeel pour différencier les deux states
        }
        
        protected override void UpdateRegen()
        {
            base.UpdateRegen();
            UpdateCanvas();
        }
        
        private void UpdateCanvas()
        {
            lifeText.text = $"{currentHealthPoints}/{maxHealthPoints}";
        }
        
        protected void ModifyMeshFormPlayerSpeed(float playerSpeed) => meshRenderer.material = playerSpeed < currentHealthPoints ? mats[0] : mats[1];

        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.up, detectionDst, 8f);

            Handles.color = Color.blue;
            Handles.DrawWireDisc(transform.position, Vector3.up, spawningRadius, 4f);
            
            var positions = new Vector3[sentinelCount];
            var currentPos = transform.position;
            
            for (int i = 0; i < sentinelCount; i++)
            {
                float angle = i * (2 * Mathf.PI / sentinelCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + z);
                Handles.color = Color.green;
                Handles.DrawWireDisc(positions[i], Vector3.up, 2f, 4f);
            }
        }
#endif
        
        protected enum TurretState
        {
            Aiming,
            Sleep,
            Dead,
            None
        }
    }

    
}