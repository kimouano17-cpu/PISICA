using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using static Player;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class EnemyAI : NetworkBehaviour, IDamageable
{

    public Transform targetPlayer = null;
    public GuardianDataManager guardianData;
    EnemyAttackController attackController;
    EnemyStateHandler stateHandler;
    EnemyAnimationController animationController;

    [Header("EnemyHPBar")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image damageBar;  
    private Coroutine damageRoutine;
    
   

    [Header("EnemyStats")]
    [SerializeField] private int enemyMaxHealth;
    [SerializeField] private int enemyMaxStamina;
    [SerializeField] private int enemyDefense;
    [SerializeField] private int enemyMinAP;
    [SerializeField] private int enemyMaxAP;
    [SerializeField] private float enemySpeed;
    [SerializeField] private float attackTimer;
    [SerializeField] private float attackCooldown;

    [SerializeField] private bool enemyAttackStarted;
  
    public int enemyDamage;

    private bool isTrackingPlayer;

    public NetworkVariable<int> enemyCurrentHealth = new NetworkVariable<int>(0,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<int> enemyCurrentStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Animator anim;
    private UnityEngine.AI.NavMeshAgent navMeshAgent;


    public int strafeDirection = 1;
    public float strafeDistance = 6f;
    [Header("Raycast Settings")]
    public float sphereRadius = 0f;
    public Vector3 sphereOffset;

    public LayerMask playerLayer;


    public Vector3 dirToPlayer;


    private void Awake()
    {
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        attackController = GetComponent<EnemyAttackController>();
        stateHandler = GetComponent<EnemyStateHandler>();
        animationController = GetComponent<EnemyAnimationController>();

        InvokeRepeating("SwitchStrafeDirection", 3.2f, 4.7f);
        InvokeRepeating("RandomizeStrafeDistance", 2.5f, 4.2f);
    }
    public override void OnNetworkSpawn()
    {
        enemyMaxHealth = guardianData.enemyHealth;
        enemyMaxStamina = guardianData.enemyStamina;

        // Only set initial values on the server to prevent network race conditions
        if (IsServer)
        {
            enemyCurrentHealth.Value = enemyMaxHealth;
            enemyCurrentStamina.Value = enemyMaxStamina;
        }

        // SUBSCRIBE ALL CLIENTS TO HEALTH CHANGES
        enemyCurrentHealth.OnValueChanged += OnHealthChanged;

        enemyDefense = guardianData.enemyDefense;
        enemySpeed = guardianData.enemySpeed;
        enemyMinAP = guardianData.enemyMinAttackPower;
        enemyMaxAP = guardianData.enemyMaxAttackPower;
        navMeshAgent.speed = enemySpeed;

        stateHandler.enemyState.OnValueChanged += OnStateChanged;
        OnStateChanged(stateHandler.enemyState.Value, stateHandler.enemyState.Value);
    }
    private void Start()
    {
        navMeshAgent.updateRotation = false;
        hpBar.transform.parent.gameObject.SetActive(false);
    }
    public override void OnNetworkDespawn()
    {
        stateHandler.enemyState.OnValueChanged -= OnStateChanged;
        enemyCurrentHealth.OnValueChanged -= OnHealthChanged; // CLEAN UP
    }
    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // Ensure the HP Bar is visible to the client who just landed a hit
        ShowHPBar();

        // Update the visual fills on the local screen
        UpdateHPBar(newHealth);
    }
    void Update()
    {
        if (!IsServer) return;

     
        if (targetPlayer == null) return;

        CalculateDistancefromPlayer();

        dirToPlayer = (targetPlayer.transform.position - transform.position).normalized;
        if (isTrackingPlayer && targetPlayer != null)
        {
            ResetEnemyRotation();
        }
        var playerDead = targetPlayer.gameObject.GetComponent<Player>();
        if (playerDead.playerState == Player.PlayerState.IsDead)
        {
            targetPlayer = null;
            return;
        }

    }
    private void OnStateChanged(EnemyStateHandler.EnemyState oldState, EnemyStateHandler.EnemyState newState)
    {
        if (!IsServer) return;
        if (newState == EnemyStateHandler.EnemyState.OnDeath)
        {
            animationController.PlayDead();
            StartCoroutine(PlayDeath());
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
        }
        else if (newState == EnemyStateHandler.EnemyState.IsAttacking)
        {
            SetTarget();
            enemyDamage = Random.Range(enemyMinAP, enemyMaxAP);
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
            attackController.ChooseAction();
     
        }
        else if (newState == EnemyStateHandler.EnemyState.IsStrafing)
        {
           

            StopCoroutine(AttackRoutine());
            StartCoroutine(AttackRoutine());

            navMeshAgent.isStopped = false;
            navMeshAgent.nextPosition = transform.position;
            navMeshAgent.updatePosition = true;


        }
        else if(newState == EnemyStateHandler.EnemyState.Idle)
        {
            animationController.PlayIdle();
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
        }
        else if(newState == EnemyStateHandler.EnemyState.IsExhausted)
        {
          
            animationController.PlayExhaust();
            navMeshAgent.isStopped = true;
            navMeshAgent.updatePosition = false;
        }
    }
    public void EnemyStrafing()
    {
        if (!IsServer) return;

       
        Vector3 sideVector = Vector3.Cross(Vector3.up, dirToPlayer);

        Vector3 orbitPoint = targetPlayer.transform.position - (dirToPlayer * strafeDistance);
        Vector3 targetpos = orbitPoint + (sideVector * strafeDirection * 5f);
  
        navMeshAgent.SetDestination(targetpos);

        Quaternion lookrotation = Quaternion.LookRotation(new Vector3(dirToPlayer.x, 0, dirToPlayer.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookrotation, Time.deltaTime * 5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stateHandler.CurrentState == EnemyStateHandler.EnemyState.IsExhausted) return;
            SetTarget();
            stateHandler.CurrentState = EnemyStateHandler.EnemyState.IsStrafing;
            
        }
    }
    void SetTarget()
    {
        float closestPlayer = Mathf.Infinity;

        Collider[] playersHit = Physics.OverlapSphere(transform.position, sphereRadius, playerLayer);
        ShowHPBar();
        foreach (Collider  player in playersHit)
        {
          
            Vector3 directionToPlayer = (player.transform.position - transform.position);
            var deadPlayer = player.GetComponent<Player>();
            if (deadPlayer.playerState == Player.PlayerState.IsDead) return;
            float distance = directionToPlayer.sqrMagnitude;
            if (distance < closestPlayer)
            {
                closestPlayer = distance;
                targetPlayer = player.transform;
            }
        }
    }
  
    void SwitchStrafeDirection()
    {
        strafeDirection *= -1;

    }
    void RandomizeStrafeDistance()
    {
        int distanceValue = Random.Range(4, 7);
        strafeDistance = distanceValue;
    }

    //TakeDamage 
    public void TakeDamage(int damage, Vector3 hitDir)
    {
        // Only the server calculates damage numbers
        if (!IsServer) return;

        int healthDamage = (damage * damage) / (damage + enemyDefense);
        enemyCurrentHealth.Value -= healthDamage;
        if (stateHandler.CurrentState == EnemyStateHandler.EnemyState.OnDeath) return;
        if (enemyCurrentHealth.Value <= 0)
        {
            stateHandler.CurrentState = EnemyStateHandler.EnemyState.OnDeath;
            return;
        }

        if (stateHandler.CurrentState == EnemyStateHandler.EnemyState.IsExhausted)
        {
            Debug.Log("Enemy hit while exhausted - taking damage but staying in state");
            return;
        }

        int staminaDamage = damage / 3;
        enemyCurrentStamina.Value -= staminaDamage;

        // REMOVED: UpdateHPBar() and ShowHPBar() are gone from here!
        // The OnValueChanged event handler takes care of it safely now.

        if (enemyCurrentStamina.Value <= 0)
        {
            stateHandler.CurrentState = EnemyStateHandler.EnemyState.IsExhausted;
        }
    }



    //HPBarFuntion

    void UpdateHPBar(int currentHealth)
    {
      
        if(hpBar.isActiveAndEnabled)
        {
            hpBar.fillAmount = (float)currentHealth / enemyMaxHealth;
        }
       

        StartCoroutine(SmoothDamageBar((float)currentHealth / enemyMaxHealth));
        
    }   

IEnumerator SmoothDamageBar(float target)
{
       
    yield return new WaitForSeconds(0.5f);

    while (damageBar.fillAmount > target)
    {
        damageBar.fillAmount =
            Mathf.Lerp(damageBar.fillAmount, target, Time.deltaTime * 3f);

        yield return null;
    }

    damageBar.fillAmount = target;
}
void ShowHPBar()
{
    hpBar.transform.parent.gameObject.SetActive(true);
}
    //AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA

    void CalculateDistancefromPlayer()
    {
        dirToPlayer = (targetPlayer.transform.position - transform.position).normalized;
    }
    public void ResetEnemyState()
    {
        stateHandler.enemyState.Value = EnemyStateHandler.EnemyState.IsStrafing;
        attackController.isBusy = false;
    }
    private void SetTracking(int state)
    {
        isTrackingPlayer = (state == 1);
    }
    void ResetEnemyRotation() 
    {
        dirToPlayer.y = 0; 
        Quaternion enemyLookrotation = Quaternion.LookRotation(dirToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, enemyLookrotation, Time.deltaTime * 5f);
    }
    void ResetEnemyExhaustState()
    {
        enemyCurrentStamina.Value = enemyMaxStamina;
        animationController.RecoverExhaust();
      
        
    }
  
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, sphereRadius);
      
    }
    private IEnumerator AttackRoutine()
    {
        while(attackController.isBusy || stateHandler.isBusy )
        {
            yield return null;
        }
        enemyAttackStarted = true;
        if (stateHandler.CurrentState != EnemyStateHandler.EnemyState.IsStrafing)
            yield break;

        float randomInterval = Random.Range(1.4f, 2.4f);
        yield return new WaitForSeconds(randomInterval);
        if (stateHandler.CurrentState == EnemyStateHandler.EnemyState.IsStrafing)
        {
            stateHandler.CurrentState = EnemyStateHandler.EnemyState.IsAttacking;
        }
       
        enemyAttackStarted = false;
    }
    private IEnumerator PlayDeath()
    {
        yield return new WaitForSeconds(5.5f);
        GetComponent<NetworkObject>().Despawn();
    }

   
}