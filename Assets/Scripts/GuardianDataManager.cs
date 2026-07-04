using UnityEngine;

[CreateAssetMenu(fileName = "GuardianData", menuName = "Guardian Data")]
public class GuardianDataManager : ScriptableObject
{
   
    public enum PoiseLevel
    {
        low,
        medium,
        high
    }

    [Header("Poise Level")]
    public PoiseLevel poise;

    [Header("Enemy Name")]
    public string enemyName;

    [Header("Enemy Stats")]
    public int enemyHealth;
    public int enemyStamina;
    public int enemyDefense;
    public float enemySpeed;

    [Header("Enemy Base Attack")]
    public int enemyMinAttackPower;
    public int enemyMaxAttackPower;
    

    public int enemyID;
    public string enemyKey;
    
   

    
}
