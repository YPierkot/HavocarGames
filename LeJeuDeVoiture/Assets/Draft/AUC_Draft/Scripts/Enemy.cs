using System.Collections.Generic;
using CarNameSpace;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace EnemyNamespace
{
    public abstract class Enemy : MonoBehaviour
    {
        // Variables
        [SerializeField] protected string name;
        [SerializeField] protected int healthPoints;
        [SerializeField] protected float unitBaseSpeed;
        [SerializeField] protected float updatePath = 0.35f;

        protected NavMeshAgent agent;
        protected bool isDead;
        protected float timer = 0;
        
        protected CarController car;
        public Transform playerPos => car.transform;

        [Space(8)] [Header("Sentinels")] protected List<Sentinels> sentinelsList = new();
        public int sentinelCount;
        public float spawningRadius = 0;
        public GameObject sentinelsPrefab;
        [SerializeField] private bool isAutoRegen = false;

        /// <summary>
        /// Method appelé à la mort de l'entitée
        /// </summary>
        public abstract void Death();

        /// <summary>
        /// Method appelé au spawn de l'entitée
        /// </summary>
        protected virtual void Spawn()
        {
            if (GameManager.instance) car = GameManager.instance.controller;

            if (GetComponent<NavMeshAgent>() != null)
            {
                agent = GetComponent<NavMeshAgent>();
                agent.speed = unitBaseSpeed;
            }

            isDead = false;

            if (sentinelCount > 0) SetupSentinel();
        }

        /// <summary>
        /// Méthod appelé lorsque l'entité est en collision avec la voiture (joueur)
        /// </summary>
        public abstract void CollideWithPlayer();

        private void SetupSentinel()
        {
            isAutoRegen = true;
            
            var positions = new Vector3[sentinelCount];
            var currentPos = transform.position;

            for (int i = 0; i < sentinelCount; i++)
            {
                float angle = i * (2 * Mathf.PI / sentinelCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + z);
            }

            for (int i = 0; i < sentinelCount; i++)
            {
                Sentinels tempS = Instantiate(sentinelsPrefab, positions[i], Quaternion.identity, transform)
                    .GetComponent<Sentinels>();
                sentinelsList.Add(tempS);
                tempS.parentEnemy = this;
            }
        }

        protected internal void OnSentinelDie(int health)
        {
            healthPoints -= health;
            sentinelCount--;
            if (sentinelCount == 0) isAutoRegen = false;
        }
    }
}