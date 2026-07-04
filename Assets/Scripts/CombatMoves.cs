using UnityEngine;

[CreateAssetMenu(fileName = "enemyCombatMoves", menuName = "Enemy Moves")]
public class CombatMove : ScriptableObject
{
    public string moveName;
    public float minRange;
    public float maxRange;
    public float baseWeight;
    public string animTrigger;
    public float cooldown;
}
