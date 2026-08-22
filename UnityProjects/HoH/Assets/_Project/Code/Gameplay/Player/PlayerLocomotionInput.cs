using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        [SerializeField] private bool holdToSprint = true;

        private PlayerControls controls;

 
        public Vector2 MovementInput { get; private set; }
 
        public Vector2 LookInput { get; private set; }
 
        public bool JumpPressed { get; private set; }
 
        public bool SprintToggledOn { get; private set; }
 
        public bool WalkToggledOn { get; private set; }
 
        private void OnEnable()
        {
            if(PlayerInputManager.Instance?.PlayerControls == null)
            {
                Debug.LogError("Player controls is not initialized - cannot enable");
                return;
            }
 
            controls = PlayerInputManager.Instance.PlayerControls;
            controls.PlayerLocomotionMap.Enable();
            controls.PlayerLocomotionMap.SetCallbacks(this);
 
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
 
            controls.PlayerLocomotionMap.Disable();
            controls.PlayerLocomotionMap.RemoveCallbacks(this);
            controls = null;
        }
 
        private void LateUpdate()
        {
            JumpPressed = false;
        }
 
        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
        }
 
        public void OnLook(InputAction.CallbackContext context)
        {
            {
                LookInput = context.ReadValue<Vector2>();
            }
        }
 
        public void OnToggleSprint(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SprintToggledOn = holdToSprint || !SprintToggledOn;
            }
            else if (context.canceled)
            {
                SprintToggledOn = !holdToSprint && SprintToggledOn;
            }
        }
 
        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
 
            JumpPressed = true;
        }
 
        public void OnToggleWalk(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
 
            WalkToggledOn = !WalkToggledOn;
        }
 
    }
}
 