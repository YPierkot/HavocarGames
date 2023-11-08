using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    // Variables
    [SerializeField] protected string name;
    [SerializeField] protected int healthPoints;
    [SerializeField] protected float speed;
    [SerializeField] protected float updatePath = 0.35f;
    
    protected NavMeshAgent agent;
    protected bool isDead;
    protected float timer = 0;
    protected Transform playerPos;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public abstract void Death();
    public abstract void Spawn();
}
