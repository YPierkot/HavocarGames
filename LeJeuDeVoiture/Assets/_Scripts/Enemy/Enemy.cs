using System.Collections.Generic;
using CarNameSpace;
using ManagerNameSpace;
using UnityEngine;
using UnityEngine.AI;

namespace EnemyNamespace
{
    public abstract class Enemy : MonoBehaviour
    {
        // Variables
        [SerializeField] protected int currentHealthPoints;
        [SerializeField] protected int maxHealthPoints;
        
        [SerializeField] protected float unitBaseSpeed;
        [SerializeField] protected float updatePath = 0.35f;

        protected NavMeshAgent agent;
        protected bool isDead;
        protected float aimingTimer = 0;
        
        protected CarController car;
        public Transform playerPos => car.transform;

        [Space(8)] [Header("SENTINEL SECTION")] 
        protected List<Sentinels> sentinelsList = new();
        [SerializeField] private Vector2Int sentinelRandomRange;
        [SerializeField] protected int currentSentinelCount;
        public float spawningRadius = 14;
        public GameObject sentinelsPrefab;
        //[SerializeField] private bool isAutoRegen = false;

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

            currentHealthPoints = maxHealthPoints;
            isDead = false;

            if (sentinelRandomRange.y > 0) SetupSentinel();
        }
        
        private void SetupSentinel()
        {
            //isAutoRegen = true;
            currentSentinelCount = (int)Random.Range(sentinelRandomRange.x, sentinelRandomRange.y + 1);
            var positions = new Vector3[currentSentinelCount];
            var currentPos = transform.position;

            for (int i = 0; i < currentSentinelCount; i++)
            {
                float angle = i * (2 * Mathf.PI / currentSentinelCount);
                float x = Mathf.Cos(angle) * spawningRadius;
                float z = Mathf.Sin(angle) * spawningRadius;

                positions[i] = new Vector3(currentPos.x + x, 0, currentPos.z + z);
            }

            for (int i = 0; i < currentSentinelCount; i++)
            {
                Sentinels tempS = Instantiate(sentinelsPrefab, positions[i], Quaternion.identity, transform)
                    .GetComponent<Sentinels>();
                sentinelsList.Add(tempS);
                tempS.parentEnemy = this;
            } 
        }

        protected virtual void UpdateCanvas()
        {
        }

        protected virtual void EnemyTakeDamage(int damages)
        {
            currentHealthPoints -= damages;
            
            if (currentHealthPoints < 1)
            {
                OnDie();
            }
            
            UpdateCanvas();
        }

        [SerializeField] private float energyBonusAfterKillingEnemy = 4.5f;
        protected virtual async void OnDie()
        {
            GameManager.instance.abilitiesManager.AddEnergy(energyBonusAfterKillingEnemy);
        }
        
        protected internal void OnSentinelDie(int sentinelHealth)
        {
            EnemyTakeDamage(sentinelHealth);
            currentSentinelCount--;
            //if (currentHealthPoints > maxHealthPoints) currentHealthPoints = maxHealthPoints;
            //if (sentinelCount == 0) isAutoRegen = false;
        }   

        // private double regenTimer;
        // public int hpRegenPerSeconds = 2;
        // protected virtual void UpdateRegen()
        // {
        //     if (!isAutoRegen) return;
        //     if (currentHealthPoints >= maxHealthPoints) return;
        //     
        //     regenTimer += Time.deltaTime;
        //     if (regenTimer > 1f)
        //     {
        //         currentHealthPoints += hpRegenPerSeconds;
        //         regenTimer = 0f;
        //     }
        // }
    }
}