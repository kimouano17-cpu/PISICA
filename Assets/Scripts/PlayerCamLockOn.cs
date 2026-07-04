using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamLockOn : NetworkBehaviour
{
    [Header("References")]
    public CinemachineTargetGroup targetGroup;
    public Animator cameraStateAnimator;

    [Header("Settings")]
    public float detectionRadius = 15f;
    public LayerMask enemyLayer;

    public Transform currentEnemy;

    void Update()
    {
        if(currentEnemy == null)
        {
            cameraStateAnimator.SetBool("IsLockedOn", false);
        }

        if (!IsOwner) return;

        // Logic: If we have a reference, we MUST validate it
        if (currentEnemy != null)
        {
            float dist = Vector3.Distance(transform.position, currentEnemy.position);
            if (dist > detectionRadius + 2f)
            {
                ClearLockOn();
            }
        }
    }

    public void ToggleLockOn()
    {
        if (currentEnemy != null)
        {
            ClearLockOn();
            currentEnemy = null;
            return;
        }

        AttemptLockOn();
    }

    void AttemptLockOn()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);
        float closestDist = Mathf.Infinity;
        Transform bestTarget = null;

        if (enemies.Length == 0) return;

        foreach (var col in enemies)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = col.transform;
            }
        }

        if (bestTarget != null)
        {
            currentEnemy = bestTarget;
            // Add the enemy to the Cinemachine Target Group
            targetGroup.AddMember(currentEnemy, 1f, 2f);

            // Switch the State-Driven Camera to the LockOn state
            cameraStateAnimator.SetBool("IsLockedOn", true);
        }
    }

    public void ClearLockOn()
    {
        // Ensure we tell the animator to switch back FIRST
        if (cameraStateAnimator != null)
        {
            cameraStateAnimator.SetBool("IsLockedOn", false);
        }

        if (currentEnemy != null)
        {
            targetGroup.RemoveMember(currentEnemy);
            currentEnemy = null;
        }
        Debug.Log("Lock On Cleared.");

    }
}