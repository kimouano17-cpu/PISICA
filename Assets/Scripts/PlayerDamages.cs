using UnityEngine;

public class PlayerDamages : MonoBehaviour
{
    public int lightDamage = 25;
    public int heavyDamage = 50;

    public PlayerHitbox leftHand;
    public PlayerHitbox rightHand;

    public void DealLightDamage()
    {
        leftHand.Hit(lightDamage);
        rightHand.Hit(lightDamage);
    }

    public void DealHeavyDamage()
    {
        leftHand.Hit(heavyDamage);
        rightHand.Hit(heavyDamage);
    }
}