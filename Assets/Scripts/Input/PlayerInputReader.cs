using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputReader : NetworkBehaviour, InputSystem_Actions.IPlayerActions
{

    [SerializeField] InputActionAsset inputActionAsset;
    public UnityAction<Vector2> onMove;
    public UnityAction jumpStarted;
    public UnityAction onDodgeStarted;
    public UnityAction onDodgeFinished;
    public UnityAction onBlockStarted;
    public UnityAction onBlockFinished;
    public UnityAction onHeal;
    public UnityAction onUniqueSkillStarted;
    public UnityAction onLightAttackStarted;
    public UnityAction onLightAttackFinished;
    public UnityAction onHeavyAttackStarted;
    public UnityAction onHeavyAttackFinished;
    public UnityAction<bool> onSprintStarted;
    public UnityAction<bool> onSprintFinished;
    public UnityAction onLockOn;

    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction dodgeAction;
    public InputAction blockAction;
    public InputAction skillAction;
    public InputAction healAction;
    public InputAction lAttackAction;
    public InputAction hAttackAction;
    public InputAction sprintAction;
    public InputAction lockOnAction;

   public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
       
            lockOnAction = inputActionAsset.FindAction("Lock On");
            sprintAction = inputActionAsset.FindAction("Sprint");
            moveAction = inputActionAsset.FindAction("Move");
            jumpAction = inputActionAsset.FindAction("Jump");
            dodgeAction = inputActionAsset.FindAction("Dodge");
            blockAction = inputActionAsset.FindAction("Block");
            healAction = inputActionAsset.FindAction("Heal");
            skillAction = inputActionAsset.FindAction("Unique Skill");
            lAttackAction = inputActionAsset.FindAction("Light Attack");
            hAttackAction = inputActionAsset.FindAction("Heavy Attack");

            lockOnAction.started += OnLockOn;

            sprintAction.started += OnSprint;
            sprintAction.performed += OnSprint;
            sprintAction.canceled += OnSprint;

            moveAction.started += OnMove;
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;


            jumpAction.started += OnJump;
            jumpAction.performed += OnJump;
            jumpAction.canceled += OnJump;


            dodgeAction.started += OnDodge;
            dodgeAction.performed += OnDodge;
            dodgeAction.canceled += OnDodge;


            blockAction.started += OnBlock;
            blockAction.performed += OnBlock;
            blockAction.canceled += OnBlock;


            healAction.started += OnHeal;
            healAction.performed += OnHeal;
            healAction.canceled += OnHeal;

            skillAction.started += OnUniqueSkill;
            skillAction.performed += OnUniqueSkill;
            skillAction.canceled += OnUniqueSkill;

            lAttackAction.started += OnLightAttack;
            lAttackAction.performed += OnLightAttack;
            lAttackAction.canceled += OnLightAttack;

            hAttackAction.started += OnHeavyAttack;
            hAttackAction.performed += OnHeavyAttack;
            hAttackAction.canceled += OnHeavyAttack;

            moveAction.Enable();
            jumpAction.Enable();
            dodgeAction.Enable();
            blockAction.Enable();
            healAction.Enable();
            skillAction.Enable();
            lAttackAction.Enable();
            hAttackAction.Enable();
            lockOnAction.Enable();
            sprintAction.Enable();
        }
      
    }

   public override void OnNetworkDespawn()
    {
        if(IsOwner)
        {
            lockOnAction.started -= OnLockOn;
            lockOnAction.performed -= OnLockOn;
            lockOnAction.canceled -= OnLockOn;

            sprintAction.started -= OnSprint;
            sprintAction.performed -= OnSprint;
            sprintAction.canceled -= OnSprint;

            moveAction.started -= OnMove;
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;


            jumpAction.started -= OnJump;
            jumpAction.performed -= OnJump;
            jumpAction.canceled -= OnJump;


            dodgeAction.started -= OnDodge;
            dodgeAction.performed -= OnDodge;
            dodgeAction.canceled -= OnDodge;


            blockAction.started -= OnBlock;
            blockAction.performed -= OnBlock;
            blockAction.canceled -= OnBlock;


            healAction.started -= OnHeal;
            healAction.performed -= OnHeal;
            healAction.canceled -= OnHeal;

            skillAction.started -= OnUniqueSkill;
            skillAction.performed -= OnUniqueSkill;
            skillAction.canceled -= OnUniqueSkill;

            lAttackAction.started -= OnLightAttack;
            lAttackAction.performed -= OnLightAttack;
            lAttackAction.canceled -= OnLightAttack;

            hAttackAction.started -= OnHeavyAttack;
            hAttackAction.performed -= OnHeavyAttack;
            hAttackAction.canceled -= OnHeavyAttack;

        }

        //FUNCTION
    }
    public void OnBlock(InputAction.CallbackContext context)
    {
        if (context.started)
        {
           onBlockStarted?.Invoke();
        }
        
        if (context.canceled)

        {
            onBlockFinished?.Invoke();
        }
        else
        {
            return;
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (onDodgeStarted != null && context.performed) onDodgeStarted.Invoke();
        else return;
    }

    public void OnHeal(InputAction.CallbackContext context)
    {
        if (onHeal != null && context.started) onHeal.Invoke();
        else return;
    }

    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (onHeavyAttackStarted != null && context.performed) onHeavyAttackStarted.Invoke();
        else return;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (jumpStarted != null && context.started) jumpStarted.Invoke();
        else return;
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (onLightAttackStarted != null && context.performed) onLightAttackStarted.Invoke();
        else return;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        onMove?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnUniqueSkill(InputAction.CallbackContext context)
    {
        if (onUniqueSkillStarted != null && context.started) onUniqueSkillStarted?.Invoke();
        else return;
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (onSprintStarted != null && context.performed)
        {
            onSprintStarted?.Invoke(context.ReadValueAsButton());
        }
        else if(onSprintFinished != null && context.canceled)
        {
            onSprintFinished?.Invoke(context.ReadValueAsButton());
        }
            return;
        
    }
    public void OnLockOn(InputAction.CallbackContext context)
    {
        if (onLockOn != null && context.started) onLockOn?.Invoke();
        else return;
    }

}