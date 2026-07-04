using UnityEngine;

public class HealingLightScript : MonoBehaviour
{
    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
           var player = other.GetComponent<Player>();
            if(player.currentPotionCount != player.maxPotionCount)
            {
                player.currentPotionCount = player.maxPotionCount;
            }
           if(player.playerBaseCurrentHealth.Value != player.playerBaseMaxHealth)
            {
                player.playerBaseCurrentHealth.Value = player.playerBaseMaxHealth;
            }
           
        }
    }
}
