using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFoddler : Enemy
{
    //Variables
    
    [SerializeField] public Rigidbody[] ragdollHandler;
    [SerializeField] public List<Collider> ragdollColliders;
    
    // Start is called before the first frame update
    void Start()
    {
        ragdollHandler = GetComponentsInChildren<Rigidbody>();
        for (int i = 0; i < ragdollHandler.Length; i++)
        {
            ragdollColliders.Add(ragdollHandler[i].GetComponent<Collider>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        
        timer += Time.deltaTime;
        if (timer > updatePath)
        {
            agent.SetDestination(playerPos.position);
            timer = 0;
        }
    }
    
    public override void Spawn()
    {
        throw new System.NotImplementedException();
    }

    public override void Death()
    {
        throw new System.NotImplementedException();
    }
    
    public void EnableRagdoll()
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
