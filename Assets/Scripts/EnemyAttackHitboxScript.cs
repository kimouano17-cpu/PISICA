using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyAttackHitboxScript : NetworkBehaviour
{
    PlayerVfx vfx;
    PlayerSoundFX sfx;
    public List<Collider> ignoredColliders = new List<Collider>();
    [SerializeField] EnemyAI enemyAI = null;
    [SerializeField] Player player = null;
    [SerializeField] private Vector3 attackPointOffset;
    [SerializeField] private float attackRange;
    [SerializeField] private string targetTag, hitBoxType;
    [SerializeField] private LayerMask targetMask, enemyMask, playerMask;

    private Coroutine attackRoutine;

    private void Awake()
    {
        enemyAI = GetComponentInParent<EnemyAI>();
        player = GetComponentInParent<Player>();
        vfx = GetComponentInParent<PlayerVfx>();
    }

    void Start()
    {
        if (hitBoxType == "Player")
        {
            targetTag = "Enemy";
            targetMask = enemyMask;
        }
        else if (hitBoxType == "Enemy")
        {
            targetTag = "Player";
            targetMask = playerMask;
        }
        sfx = GetComponentInParent<PlayerSoundFX>();
    }

    public void StartDamageWindow(float duration)
    {
        // FIX: IsOwner check is correct — only the owner starts the coroutine
        if (!IsOwner) return;

        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = StartCoroutine(DamageWindow(duration));
    }

    private IEnumerator DamageWindow(float duration)
    {
        ResetIgnoredList();
        float timer = 0f;

        while (timer < duration)
        {
            DealDamage();
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public void DealDamage()
    {
        // FIX: IsOwner check is correct — only the owner does the physics query
        if (!IsOwner) return;

        Vector3 spherePos = transform.TransformPoint(attackPointOffset);
        Collider[] hitEntities = Physics.OverlapSphere(spherePos, attackRange, targetMask, QueryTriggerInteraction.Ignore);

        foreach (Collider entity in hitEntities)
        {
            if (ignoredColliders.Contains(entity)) continue;

            // FIX: Get the NetworkObject to send its ID to the server
            if (entity.TryGetComponent(out NetworkObject targetNetObj))
            {
                int damage = (hitBoxType == "Player") ? player.playerDamage : enemyAI.enemyDamage;
                Vector3 hitDir = (entity.transform.position - transform.parent.position).normalized;
                hitDir.y = 0.15f;

                // FIX: Send damage request to server so it works for ALL clients including Player 2
                RequestDamageServerRpc(targetNetObj.NetworkObjectId, damage, hitDir);

                // Play SFX and VFX locally on the owner (feels responsive)
                PlayHitEffects();

                ignoredColliders.Add(entity);

                Debug.Log("Hit registered, sending to server!");
            }
            // Fallback: if no NetworkObject but has IDamageable (local-only objects)
            else if (entity.TryGetComponent(out IDamageable localDamageable))
            {
                int damage = (hitBoxType == "Player") ? player.playerDamage : enemyAI.enemyDamage;
                Vector3 hitDir = (entity.transform.position - transform.parent.position).normalized;
                hitDir.y = 0.15f;

                localDamageable.TakeDamage(damage, hitDir);
                PlayHitEffects();
                ignoredColliders.Add(entity);
            }
        }
    }

    // FIX: ServerRpc — this runs on the HOST/SERVER, so damage is always applied correctly
    // RequireOwnership = true means only the owner of this object can call this (default, safe)
    [ServerRpc]
    private void RequestDamageServerRpc(ulong targetNetworkObjectId, int damage, Vector3 hitDir)
    {
        // Find the target NetworkObject on the server
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetNetworkObjectId, out NetworkObject targetNetObj))
        {
            // Apply damage via IDamageable — this runs on the server so it syncs to all clients
            if (targetNetObj.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage, hitDir);
                Debug.Log($"Server applied {damage} damage to {targetNetObj.name}");
            }
        }
    }

    private void PlayHitEffects()
    {
        if (hitBoxType == "Player")
        {
            sfx.LAttackSFX();
            vfx.PlayImpactLeft();
        }
        else
        {
            sfx.RAttackSFX();
            vfx.PlayImpactRight();
        }
        sfx.HeavyAttackSFX();
        vfx.ReleaseHeavy();
    }

    public void ResetIgnoredList()
    {
        ignoredColliders.Clear();
    }

    public void ResetHitbox()
    {
        ignoredColliders.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 spherePos = transform.TransformPoint(attackPointOffset);
        Gizmos.DrawWireSphere(spherePos, attackRange);
    }
}