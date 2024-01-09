using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyFoddler : Enemy, IDamageable
    {
        //Variables
        [Space] [Header("Foddler Section")] 
        [SerializeField] private Animator animator;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private Rigidbody[] ragdollHandler;
        [SerializeField] private List<Collider> ragdollColliders;
        [SerializeField] public FoddlerState State = FoddlerState.Spawning;
        [SerializeField] private float deadTimer = 0;
        
        void Start()
        {
            State = FoddlerState.Spawning;
            Spawn();
        }
        
        void Update()
        {
            ExecuteState();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ExecuteState() // Update 
        {
            switch (State)
            {
                case FoddlerState.Spawning: Spawn(); break;
                case FoddlerState.FollowPlayer: FollowPlayer(); break;
                case FoddlerState.AttackPlayer: AttackPlayer(); break;
                case FoddlerState.Dead: DeadState(); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
        
        private void FollowPlayer()
        {
            float velocity = agent.velocity.magnitude/agent.speed;
            animator.SetFloat("Speed", velocity);
            aimingTimer += Time.deltaTime;
            if (aimingTimer > updatePath)
            {
                agent.SetDestination(playerPos.position);
                aimingTimer = 0;
            }
        }
        
        private void AttackPlayer()
        {
            // Attack en direction du joueur au moment ou il trigger l'attaque
            // Si voiture présente après l'anim dans un radius -> Apply dégats
            // Switch state

            agent.SetDestination(transform.position);
            agent.transform.LookAt(playerPos);
        }
        
        private void DeadState()
        {
            // timer & au bout de 10 secondes dépop
            // Lerp du mat opacity 255 -> 0
            
            deadTimer += Time.deltaTime;
            Color col = new Color(255, 0, 0, Mathf.Lerp(255, 0, 10 - deadTimer));
            meshRenderer.material.color = col;
            if (deadTimer > 10)
            {
                Pooler.instance.DestroyInstance(Key.OBJ_Foddler, transform);
            }
        }

        private void SwitchState(FoddlerState nextState)
        {
            switch (nextState) // 
            {
                case FoddlerState.Spawning: break;
                case FoddlerState.FollowPlayer: ToFollow(); break;
                case FoddlerState.AttackPlayer: ToAttack(); break;
                case FoddlerState.Dead: Kill(); break;
                default: throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
            }

            State = nextState;
        }

        private async void ToAttack()
        {
            animator.SetBool("isAttack", true);
            animator.SetFloat("RandomAttack", UnityEngine.Random.Range(0, 3));
            await Task.Delay(450);
        }

        private void ToFollow()
        {
            animator.SetBool("isAttack", false);
        }

        protected override void Spawn()
        {
            base.Spawn();
            
            ragdollHandler = GetComponentsInChildren<Rigidbody>();
            
            for (int i = 0; i < ragdollHandler.Length; i++)
            {
                ragdollColliders.Add(ragdollHandler[i].GetComponent<Collider>());
            }
            
            SwitchState(FoddlerState.FollowPlayer);
        }
        
        private void EnableRagdoll()
        {
            foreach (var r in ragdollHandler)
            {
                r.isKinematic = false;
            }

            foreach (var r in ragdollColliders)
            {
                r.enabled = true;
            }

            isDead = true;
            agent.enabled = false;
        }

        public void TakeDamage(int damages)
        {
            EnemyTakeDamage(damages);
        }

        public void Kill() => OnDie();

        protected override void OnDie()
        {
            base.OnDie();
            EnableRagdoll();
        }
        
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
          
            if (agent.destination != null)
            {
                Gizmos.color = Color.white;
                {
                    // Draw lines joining each path corner
                    Vector3[] pathCorners = agent.path.corners;
                
                    for (int i = 0; i < pathCorners.Length - 1; i++)
                    {
                        Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
                    }
                }
            }
        }
    }
}

public enum FoddlerState
{
    Spawning,
    FollowPlayer,
    AttackPlayer,
    Dead
}

public static class ExtensionMethods {
 
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
   
}