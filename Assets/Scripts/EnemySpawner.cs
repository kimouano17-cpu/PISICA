using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
   public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // 1. Create the local instance on the Server
        GameObject enemyInstance = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // 2. This is the "Magic Button" that makes it appear for Player 2
        NetworkObject netObj = enemyInstance.GetComponent<NetworkObject>();
        netObj.Spawn();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
