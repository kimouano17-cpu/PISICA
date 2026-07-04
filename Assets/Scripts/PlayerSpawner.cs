using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
   
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        // Listen for connections
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        // Spawn anyone already here (including Host)
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            StartCoroutine(SpawnWithDelay(client.ClientId));
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        StartCoroutine(SpawnWithDelay(clientId));
    }

    private IEnumerator SpawnWithDelay(ulong clientId)
    {
        // 1. THE "SIMPLE" FIX: Wait a moment for the scene to stabilize
        // You can increase this to 1.0f if the client is still 'frozen'
        yield return new WaitForSeconds(0.3f);

        // 2. Double-check they don't already have a body
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
            yield break;

        // 3. Spawn logic
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject player = Instantiate(playerPrefab, point.position, point.rotation);

        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);

        Debug.Log($"Spawned player for Client: {clientId}");
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }
}