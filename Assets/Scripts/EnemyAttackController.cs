using System;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;


public class EnemyAttackController : NetworkBehaviour
{

    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private EnemyStateHandler stateHandler;

  
    public List<CombatMove> moveset;
    public List<CombatMove> movesInCooldown = new List<CombatMove>();
    public List<CombatMove> availableMoves = new List<CombatMove>();
    private Animator animator;
    public bool isBusy;
    public bool isTrackingPlayer;
    public float distance1;
    public bool inCooldown;
    private float distanceToPlayer;

    

    private void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        stateHandler = GetComponent<EnemyStateHandler>();
        animator = GetComponent<Animator>();
       
    }
    private void Start()
    { 
      
    }
    private void OnEnable()
    {
  
    }
    private void OnDisable()
    {
      
    }
    private void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, enemyAI.targetPlayer.position);
       
    }
    public  void ChooseAction()
    {
        if (!IsServer) return;
        if (isBusy || stateHandler.enemyState.Value == EnemyStateHandler.EnemyState.Idle || stateHandler.enemyState.Value == EnemyStateHandler.EnemyState.IsExhausted || stateHandler.enemyState.Value == EnemyStateHandler.EnemyState.OnDeath)
        {
            return;
        }
          
            DecideAction(distanceToPlayer);
        

    }
    void DecideAction(float distance)
    {
        availableMoves.Clear();
        if(!isBusy)
        {
            foreach (var move in moveset)
            {

                if (distance >= move.minRange && distance <= move.maxRange && !movesInCooldown.Contains(move))
                {

                    availableMoves.Add(move);
                }
            }
            if (availableMoves.Count > 0)
            {
                CalculateTotalWeight();
            }
            else if( availableMoves.Count == 0)
            {
                stateHandler.CurrentState = EnemyStateHandler.EnemyState.IsStrafing;

            }
                Debug.Log(availableMoves.Count);
        }
      
    }
    void CalculateTotalWeight()
    {
        float totalWeight = 0;
        foreach (var move in availableMoves) totalWeight += move.baseWeight;

        float randomWeight = UnityEngine.Random.Range(0, totalWeight);
        float iterations = 0;

        foreach (var move in availableMoves)
        {
            iterations += move.baseWeight;
            if (randomWeight <= iterations && !inCooldown)
            {
               
                ExecuteMove(move);
                return;
            }
        }
    }
   public void ExecuteMove(CombatMove move)
    {

        isBusy = true;
        animator.SetTrigger(move.animTrigger);
        StartCoroutine(MoveCooldown(move));
        Debug.Log(move.moveName);
        
    }
   IEnumerator MoveCooldown(CombatMove move)
    {
        movesInCooldown.Add(move);
        yield return new WaitForSeconds(move.cooldown);
        movesInCooldown.Remove(move);

    }
}
