using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionsMapActions
{
    public bool AttackPressed { get; private set; }
    public bool GatherPressed { get; private set; }
    private PlayerControls controls;


    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot enable");
            return;
        }

        controls = PlayerInputManager.Instance.PlayerControls;
        controls.PlayerActionsMap.Enable();
        controls.PlayerActionsMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        // Unsubscribing goes through the reference captured in OnEnable, not through the
        // singleton. Unity destroys scene objects in an unspecified order, so on play mode
        // exit and on every scene reload the manager can already be gone by the time this
        // runs. Asking a destroyed manager for controls this component already holds turns
        // a normal shutdown into a logged error. PlayerControls is a plain C# object, so it
        // outlives the MonoBehaviour that created it and is still safe to disable here.
        if (controls == null) return;

        controls.PlayerActionsMap.Disable();
        controls.PlayerActionsMap.RemoveCallbacks(this);
        controls = null;
    }

    private void LateUpdate()
    {
        AttackPressed = false;
        GatherPressed = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        AttackPressed = true;
    }

    public void OnGather(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        GatherPressed = true;
    }
}