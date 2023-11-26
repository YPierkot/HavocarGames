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
        [SerializeField] public Transform playerPos; //TODO - Link avec le Gamemanager

        /// <summary>
        /// Method appelé à la mort de l'entitée
        /// </summary>
        public abstract void Death();

        /// <summary>
        /// Method appelé au spawn de l'entitée
        /// </summary>
        protected virtual void Spawn()
        {
            if (GameManager.instance)
            {
                playerPos = GameManager.instance.controller.transform;
            }
           
            
            if (GetComponent<NavMeshAgent>() != null)
            {
                agent = GetComponent<NavMeshAgent>();
                agent.speed = unitBaseSpeed;
            }
            
            isDead = false;
        }

        /// <summary>
        /// Méthod appelé lorsque l'entité est en collision avec la voiture (joueur)
        /// </summary>
        public abstract void CollideWithPlayer();
    }
}