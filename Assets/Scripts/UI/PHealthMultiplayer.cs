using Unity.Netcode;
using UnityEngine;
using System.Collections; // ✅ Fix sa IEnumerator error

public class PlayerHealthUIConnector : NetworkBehaviour
{
    [Header("UI Reference")]
    public PlayerHealthUI playerHealthUI;

    Player player;

    // ✅ Isang OnNetworkSpawn lang — fixed duplicate
    public override void OnNetworkSpawn()
    {
        
        player = GetComponent<Player>();
        if (!IsOwner) return;
        player.playerBaseCurrentHealth.OnValueChanged += OnHealthChanged;
        player.playerBaseCurrentStamina.OnValueChanged += OnStaminaChanged;

      
        
            StartCoroutine(InitUIDelay());
        
    }

    public override void OnNetworkDespawn()
    {
        player.playerBaseCurrentHealth.OnValueChanged -= OnHealthChanged;
        player.playerBaseCurrentStamina.OnValueChanged -= OnStaminaChanged;
    }

    IEnumerator InitUIDelay()
    {
        yield return new WaitForSeconds(0.1f);

        playerHealthUI.SetHealth(
            player.playerBaseCurrentHealth.Value,
            player.playerBaseMaxHealth
        );
        playerHealthUI.SetStamina(
            player.playerBaseCurrentStamina.Value,
            player.playerBaseMaxStamina
        );
    }

    void OnHealthChanged(int oldVal, int newVal)
    {
        if (!IsOwner) return;
        playerHealthUI.SetHealth(newVal, player.playerBaseMaxHealth);
    }

    void OnStaminaChanged(int oldVal, int newVal)
    {
        if (!IsOwner) return;
        playerHealthUI.SetStamina(newVal, player.playerBaseMaxStamina);
    }
}