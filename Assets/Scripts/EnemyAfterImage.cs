using UnityEngine;
using Unity.Netcode;

public class EnemyAfterImageSpawner : NetworkBehaviour
{
    public GameObject ghostPrefab;

    [Header("Settings")]
    public float spawnDelay = 0.05f;

    Animator animator;

    float lastSpawnTime;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Animation Event
    public void SpawnAfterImage()
    {
        // DELAY CHECK
        if (Time.time < lastSpawnTime + spawnDelay)
            return;

        lastSpawnTime = Time.time;

        if (IsServer)
        {
            SpawnGhostClientRpc(transform.position, transform.rotation);
        }
    }

    [ClientRpc]
    void SpawnGhostClientRpc(Vector3 pos, Quaternion rot)
    {
        GameObject ghost = Instantiate(ghostPrefab, pos, rot);

        Animator ghostAnim = ghost.GetComponent<Animator>();

        if (ghostAnim != null && animator != null)
        {
            ghostAnim.Play(
                animator.GetCurrentAnimatorStateInfo(0).fullPathHash,
                0,
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime
            );

            ghostAnim.speed = 0f;
        }
    }
}