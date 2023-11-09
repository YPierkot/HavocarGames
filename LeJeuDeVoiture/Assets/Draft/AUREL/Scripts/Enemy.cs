using UnityEngine;
using UnityEngine.AI;

namespace EnemyNamespace
{
    public abstract class Enemy : MonoBehaviour
    {
        // Variables
        [SerializeField] protected string name;
        [SerializeField] protected int healthPoints;
        [SerializeField] protected float unitSpeed;
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
            agent = GetComponent<NavMeshAgent>();
            agent.speed = unitSpeed;
            isDead = false;
        }

        /// <summary>
        /// Méthod appelé lorsque l'entité est en collision avec la voiture (joueur)
        /// </summary>
        public abstract void CollideWithPlayer();
    }
}