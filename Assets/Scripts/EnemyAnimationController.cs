using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAnimationController : NetworkBehaviour
{
    Animator animator;
    EnemyStateHandler stateHandler;
    EnemyAI enemyAI;

    void Start()
    {
        animator = GetComponent<Animator>();
        stateHandler = GetComponent<EnemyStateHandler>();
        enemyAI = GetComponent<EnemyAI>();
    }

   public override void OnNetworkSpawn()
    {

    }
    void Update()
    {
      if(enemyAI.targetPlayer == null)
        {
            animator.ResetControllerState();
        }

        if (stateHandler.CurrentState == EnemyStateHandler.EnemyState.IsStrafing)
        {
            PlayStrafe();
        }



    }
    public void PlayStrafe()
    {
      if (enemyAI.strafeDirection == 1)
        {
            animator.SetInteger("isStrafing", 0);
        }
      else if (enemyAI.strafeDirection == -1)
        {
            animator.SetInteger("isStrafing", 1);
        }

    }
    public void PlayIdle()
    {
       bool isIdle = stateHandler.enemyState.Value == EnemyStateHandler.EnemyState.Idle;
        animator.SetBool("isIdle", isIdle);
    }
 
    public void PlayExhaust()
    {
    animator.SetBool("isExhausted", true);     
    }
    public void PlayDead()
    {
        animator.SetTrigger("isDead");
    }
    public void RecoverExhaust()
    {
        animator.SetBool("isExhausted", false);
    }
    public void CheckComboExtend(float duration)
    {
        StartCoroutine(ComboExtend(duration));
    }
    private IEnumerator ComboExtend(float duration)
    {
        float timer = 0f;
        while(timer < duration)
        {

         
            timer += Time.deltaTime;
            if(PlayComboExtender())
            {
                yield break;
            }

            yield return null;
        }
    }
    public bool PlayComboExtender()
    {
        if (!IsServer || enemyAI.targetPlayer == null) return false;
        Vector3 distance = (enemyAI.targetPlayer.position - transform.position);

        if(distance.magnitude <= 5.5f)
        {
            animator.SetTrigger("isComboExtend");
            return true;
        }
        return false;
      
    }
}
