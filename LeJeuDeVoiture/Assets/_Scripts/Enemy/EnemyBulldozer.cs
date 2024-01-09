using System;
using EnemyNamespace;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBulldozer : Enemy
{
    [Space(3)] [Header("Bulldozer")] 
    [SerializeField] private BulldozerState currentState;


    [SerializeField] private bool isAccelerating;
    [SerializeField] private float accelerateSpeed;
    [SerializeField] private float rollingStartTime;
    [SerializeField] private float durationToStartAccelerate = 4f;
    [SerializeField] private float rollingTime;
    
    [SerializeField] private float unitCurrentSpeed;
    [SerializeField] private float unitMaxSpeed = 40f;

    [SerializeField] private bool isImmobilize;
    [SerializeField] private Vector3 normalPos;
    [SerializeField] private float dizzyTime;
    [SerializeField] private float timeBeforeImmobilize = 1.5f;
    [SerializeField] private float immobilizeDuration = 3.5f;
    [SerializeField] private Image immobilizeDurationFeedback;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
        unitCurrentSpeed = unitBaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        ExecuteState();
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case BulldozerState.Rolling: ExecuteRolling(); break;
            case BulldozerState.Dizzy: ExecuteDizzy(); break;
            case BulldozerState.Death: break;
            case BulldozerState.None: break;
            default: throw new ArgumentOutOfRangeException();
        }
    }


    private void SwitchState(BulldozerState nextState)
    {
        switch (nextState)
        {
            case BulldozerState.Rolling: ToRolling(); break;
            case BulldozerState.Dizzy: ToDizzy(); break;
            case BulldozerState.Death: break;
            case BulldozerState.None: break;
        }

        currentState = nextState;
    }

    #region Dizzy
    private void ToDizzy()
    {
        isAccelerating = false;
        agent.speed = unitBaseSpeed;
        isImmobilize = false;
        dizzyTime = 0f;
        immobilizeDurationFeedback.fillAmount = 0;

        agent.SetDestination(normalPos);
    }

    private void ExecuteDizzy()
    {
        dizzyTime += Time.deltaTime;

        if (!isImmobilize && dizzyTime > timeBeforeImmobilize)
        {
            isImmobilize = true;
            agent.SetDestination(transform.position);
            agent.speed = 0;
            dizzyTime = 0;
        }

        if (isImmobilize)
        {
            immobilizeDurationFeedback.fillAmount = Mathf.Lerp(1, 0, dizzyTime / immobilizeDuration);
            if (dizzyTime > immobilizeDuration)
            {
                SwitchState(BulldozerState.Rolling);
            }
        }
    }

    #endregion
    
    #region Rolling

    private void ToRolling()
    {
        isImmobilize = false;
        isAccelerating = false;
        immobilizeDurationFeedback.fillAmount = 0;

        rollingStartTime = Time.time;
        rollingTime = 0;
        accelerateSpeed = 0;
        unitCurrentSpeed = unitBaseSpeed;
        agent.speed = unitBaseSpeed;
    }
    
    private void ExecuteRolling()
    {
        rollingTime += rollingStartTime + Time.deltaTime;
        if (isAccelerating)
        {
            accelerateSpeed = rollingTime - durationToStartAccelerate;
            if (unitCurrentSpeed < unitMaxSpeed)
            {
                unitCurrentSpeed = unitBaseSpeed + accelerateSpeed;
                agent.speed = unitCurrentSpeed;
            }
        }
        else
        {
            if (rollingTime > durationToStartAccelerate)
            {
                isAccelerating = true;
            }
        }
        
        // Set Destination
        Vector3 dir = (playerPos.position - transform.position).normalized;
        agent.SetDestination(transform.position + dir);
    }

    

    #endregion
    
    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Wall"))
        {
            normalPos = other.contacts[0].normal;
            Debug.DrawLine(other.contacts[0].point, normalPos, Color.red, 2f);
            SwitchState(BulldozerState.Dizzy);
        }
    }
    
    private enum BulldozerState
    {
        Rolling,
        Dizzy,
        Death,
        None
    }
}