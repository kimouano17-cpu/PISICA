using UnityEngine;

public class BattleDoorScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    
     public void OpenBattleGate()
    {
        animator.SetBool("IsOpen", true);
       
    }
    public void CloseBattleGate()
    {
        animator.SetBool("IsOpen", false);
        
    }

}
