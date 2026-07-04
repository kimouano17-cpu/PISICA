using System;
using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour
{
    public ItemManager item;
    public GameObject itemVisual;
    public GameObject equipPrompt;
    PlayerSoundFX sfx;

    private bool playerNear;

    private void Start()
    {
        if (equipPrompt != null)
            equipPrompt.SetActive(false);
    }

    void Update()
    {
        // Only allow the player who is actually near the item to interact
        if (playerNear)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RequestEquipServerRPC();
                sfx.ItemPickUpSFX();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    void RequestEquipServerRPC(RpcParams rpcParams = default)
    {
        ulong clientID = rpcParams.Receive.SenderClientId;

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientID, out var client))
        {
            var playerObject = client.PlayerObject;
            EquipmentManager manager = playerObject.GetComponent<EquipmentManager>();

            if (manager != null)
            {
                if (manager.primaryAugment != null && manager.secondaryAugment != null) return;
                int index = manager.allEquipment.IndexOf(item);

                manager.FindItemByIndexRpc(index);
            }
        }

        // Hide prompt globally on the server if it's being despawned
        HidePromptClientRpc();

        if (itemVisual != null && itemVisual.GetComponent<NetworkObject>().IsSpawned)
        {
            itemVisual.GetComponent<NetworkObject>().Despawn();
        }
    }

    // ClientRpc ensures that when an item is picked up, any active prompt disappears immediately
    [ClientRpc]
    private void HidePromptClientRpc()
    {
        if (equipPrompt != null) equipPrompt.SetActive(false);
        playerNear = false;
        this.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (itemVisual == null) return;

        // 1. Verify if the object that entered the trigger is a Player
        if (other.CompareTag("Player"))
        {
            // 2. Grab the network properties of that specific player character
            if (other.TryGetComponent<NetworkObject>(out var playerNetObj))
            {
                // CRITICAL FIX: Only set the prompt active if THIS specific client owns the character entering the zone
                if (playerNetObj.IsOwner)
                {
                    Debug.Log("LOCAL PLAYER DETECTED - Showing Prompt");
                    playerNear = true;
                    if (equipPrompt != null) equipPrompt.SetActive(true);
                     sfx = other.GetComponent<PlayerSoundFX>();
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<NetworkObject>(out var playerNetObj))
            {
                // CRITICAL FIX: Only hide it if the local player is the one walking away
                if (playerNetObj.IsOwner)
                {
                    playerNear = false;
                    if (equipPrompt != null) equipPrompt.SetActive(false);
                }
            }
        }
    }
}