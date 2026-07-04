using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static EnemyAI;
using static Player;

public class EnemyStateHandler : NetworkBehaviour
{
    public NetworkVariable<EnemyState> enemyState = new NetworkVariable<EnemyState>(EnemyState.Idle, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyAttackController attackController;
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private EnemyAnimationController animationController;

    public bool isBusy => CurrentState != EnemyState.IsStrafing;
    public event Action onIdle;
    public event Action onStrafe;
    public event Action onAttack;
    public event Action onExhaust;
    public event Action onDeath;

    public enum EnemyState
    {
        Idle,
        IsStrafing,
        IsAttacking,
        IsExhausted,
        OnDeath
    }
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAI = GetComponent<EnemyAI>();
        attackController = GetComponent<EnemyAttackController>();
        animationController = GetComponent<EnemyAnimationController>();
    }
    private void OnEnable()
    {
        onIdle += OnEnemyIdle;
        onStrafe += OnEnemyStrafe;
        onAttack += OnEnemyAttack;
        onExhaust += OnEnemyExhaust;
        onDeath += OnEnemyDeath;
    }
    private void OnDisable()
    {
        onIdle -= OnEnemyIdle;
        onStrafe -= OnEnemyStrafe;
        onAttack -= OnEnemyAttack;
        onExhaust -= OnEnemyExhaust;
        onDeath -= OnEnemyDeath;
    }
    public override void OnNetworkSpawn()
    {
        enemyState.OnValueChanged += OnStateChanged;

        SetEnemyState(enemyState.Value);
    }
    public override void OnNetworkDespawn()
    {
        enemyState.OnValueChanged -= OnStateChanged;
    }

    public EnemyState CurrentState
    {
        get => enemyState.Value;
        set 
        {
            if(IsServer)
            {
                if (enemyState.Value == value) return;
                enemyState.Value = value;

                SetEnemyState(value);
            }
            
        } 
    }
    private void Update()
    {
        if (!IsServer) return;
        if (enemyState.Value == EnemyState.IsStrafing)
        {
            enemyAI.EnemyStrafing();
            
           
           

        }


    }
   
    private void OnStateChanged(EnemyState oldState, EnemyState newState)
    {    if(!IsServer )
        {
            SetEnemyState(newState);
        }
          
    }
    void SetEnemyState(EnemyState newState)
    {
        switch (newState)
        {
            case EnemyState.Idle:
                onIdle?.Invoke();
                break;
            case EnemyState.IsStrafing:       
                if(IsServer)
                {
                 
                   
                }

                onStrafe?.Invoke();

                break;
            case EnemyState.IsAttacking:
                if(IsServer)
                {
                  
                   

                }
                onAttack?.Invoke();


                break;
            case EnemyState.IsExhausted:
                onExhaust?.Invoke();

                break;
            case EnemyState.OnDeath:
                onDeath?.Invoke();
                break;
        }
    }
    void OnEnemyIdle()
    {
        if (enemyState.Value == EnemyState.Idle)
        {
           
        }
    }
    void OnEnemyStrafe()
    {
        if (enemyState.Value == EnemyState.IsStrafing)
        {
           
        }
    }
    void OnEnemyAttack()
    {
        if(enemyState.Value == EnemyState.IsAttacking)
        {
           
        }
    }
    void OnEnemyExhaust()
    {
        if (enemyState.Value == EnemyState.IsExhausted)
        {
           
        }
    }
    void OnEnemyDeath()
    {
        if (enemyState.Value == EnemyState.OnDeath)
        {
            
        }
    }
   
  
}
