using System.Threading.Tasks;
using ManagerNameSpace;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemyNamespace
{
    public class Tower : Enemy, IDamageable
    {
        [Space(3)]
        [Header("Turret Section")]
        [SerializeField] private TurretState currentState = TurretState.Sleep;
        public EnemyAttribute enemyAttribute = EnemyAttribute.None;
        
        [SerializeField] protected bool isAiming;
        [Tooltip("Distance à laquelle la tourelle détecte le joueur")] 
        [SerializeField] protected float detectionDst;
        
        [FormerlySerializedAs("timeBeforeShootInSeconds")]
        [Tooltip("Délai de tir une fois la cible verrouillé")] 
        [SerializeField] protected float shootLoadingDuration = 5f;
        
        [SerializeField] protected GameObject turretProjectilePrefab;
        [SerializeField] protected Transform projectileLaunchPos;
        [Tooltip("Délai pendant lequel la tourelle ne peux pas ciblé le joueur")] 
        [SerializeField] protected float turretShotCooldown = 5f;
        [SerializeField] protected bool isInCooldown;
        
        [SerializeField] protected MeshRenderer meshRenderer;
        [SerializeField] protected Material[] mats;
        
        [SerializeField] protected TextMeshProUGUI lifeText;
        [SerializeField] protected LineRenderer lr;

        [SerializeField] private int damageTakenByDoorOnDeath = 90;

        [SerializeField] private ParticleSystem fluidSplashFx;

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
            
            //UpdateRegen();
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
            aimingTimer = 0;
            lr.enabled = true;
        }

        protected virtual async Task TurretAiming()
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
            aimingTimer = 0;
            lr.enabled = false;
            isInCooldown = true;
        }

        protected virtual void TurretSleep()
        {
            if (isInCooldown)
            {
                aimingTimer += Time.deltaTime;
                if (aimingTimer > turretShotCooldown)
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
            //LevelManager.Instance.OnTowerDie(this, damageTakenByDoorOnDeath);
            //isDead = true;
            //SwitchState(TurretState.None);
        }
        #endregion

        // protected override void UpdateRegen()
        // {
        //     base.UpdateRegen();
        //     UpdateCanvas();
        // }
        
        protected override void UpdateCanvas() => lifeText.text = $"{currentHealthPoints}/{maxHealthPoints}";
        protected void ModifyMeshFormPlayerSpeed(float playerSpeed) => meshRenderer.material = playerSpeed < currentHealthPoints ? mats[0] : mats[1];
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (Application.isPlaying) return;
            
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
        
        protected enum TurretState
        {
            Aiming,
            Sleep,
            Dead,
            None
        }
        
        public enum EnemyAttribute
        {
            None,
            Fragile,
            Regeneration
        }

        #region IDamageable Implementation

        public override void CollideWithPlayer()
        {
            if(!car.abilitiesManager.isInBulletMode) return;
            EnemyTakeDamage(Mathf.FloorToInt(car.speed));
        }
        
        protected override async void OnDie() {
            GetComponent<Collider>().enabled = false;
            Debug.Log(this.gameObject);
            base.OnDie();
            await LevelManager.Instance.OnTowerDie(this, damageTakenByDoorOnDeath);
            Destroy(gameObject); // TODO -> Passer en state mort quand on aura des assets & gamefeel pour différencier les deux states
            Pooler.instance.SpawnTemporaryInstance(Key.FX_FluidSplash, new Vector3(transform.position.x, 3.4f, transform.position.z), Quaternion.identity,15);
        }

        // TODO -> Connecter avec le nv system de collision directement sur la voiture
        public void TakeDamage(int damages) => EnemyTakeDamage(Mathf.FloorToInt(car.speed));
        public void Kill() => OnDie();

        #endregion
    }
}