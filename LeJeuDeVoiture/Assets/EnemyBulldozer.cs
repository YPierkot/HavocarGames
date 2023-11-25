using System;
using EnemyNamespace;
using UnityEngine;

public class EnemyBulldozer : Enemy
{
    [Space(3)] 
    [Header("Bulldozer")] 
    [SerializeField] private BulldozerState currentState;
    
    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ExecuteState()
    {
        
    }

    private void SwitchState(BulldozerState nextState)
    {
        switch (nextState)
        {
            case BulldozerState.Rolling: break;
            case BulldozerState.Dizzy: break;
            case BulldozerState.Death: break;
            case BulldozerState.None: break;
            default: throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
        }

        currentState = nextState;
    }
    

    public override void Death()
    {
    }

    public override void CollideWithPlayer()
    {
    }

    private enum BulldozerState
    {
        Rolling,
        Dizzy,
        Death,
        None
    }
}
