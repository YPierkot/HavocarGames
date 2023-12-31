using System.Collections.Generic;
using UnityEngine;

namespace EnemyNamespace
{
    public class EnemyFoddler : Enemy
    {
        //Variables
        [SerializeField] public Rigidbody[] ragdollHandler;
        [SerializeField] public List<Collider> ragdollColliders;
        
        void Start()
        {
            Spawn();
        }
        
        void Update()
        {
            if (isDead) return;

            aimingTimer += Time.deltaTime;
            if (aimingTimer > updatePath)
            {
                agent.SetDestination(playerPos.position);
                aimingTimer = 0;
            }
        }

        protected override void Spawn()
        {
            base.Spawn();

            //playerPos = FindObjectOfType<WaveManager>().gameObject.transform;
            
            ragdollHandler = GetComponentsInChildren<Rigidbody>();
            
            for (int i = 0; i < ragdollHandler.Length; i++)
            {
                ragdollColliders.Add(ragdollHandler[i].GetComponent<Collider>());
            }
        }
        
        public override void Death()
        {
        }

        // public override void CollideWithPlayer()
        // {
        //     GetComponent<Collider>().enabled = false;
        //     EnableRagdoll();
        // }

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
    }
}
