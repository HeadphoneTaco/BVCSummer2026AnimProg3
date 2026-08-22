using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float runningSpeedMultiplier;

        [SerializeField] private Transform cameraTransform;

        [SerializeField] private InputActionReference moveInputAction;
        [SerializeField] private InputActionReference runInputAction;
        private readonly int runningAnimatorHash = Animator.StringToHash("Running");

        private readonly int walkingAnimatorHash = Animator.StringToHash("Walking");

        private float activeRunningSpeedMultiplier = 1f;

        private Animator anim;

        private Vector2 moveInput;

        private Rigidbody rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            anim = GetComponent<Animator>();

            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            moveInput = moveInputAction.action.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0;
            cameraRight.y = 0;

            cameraForward.Normalize();
            cameraRight.Normalize();

            var moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            var velocity = moveDirection * movementSpeed * activeRunningSpeedMultiplier;

            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);

            //Checks if the character is moving
            if (moveDirection == Vector3.zero)
            {
                anim.SetBool(walkingAnimatorHash, false);
                anim.SetBool(runningAnimatorHash, false);
                return;
            }

            anim.SetBool(walkingAnimatorHash, true);

            //Checking if 'Shift' is being pressed to execute running. If not, then the character is walking.
            if (runInputAction.action.IsPressed())
            {
                anim.SetBool(runningAnimatorHash, true);
                activeRunningSpeedMultiplier = runningSpeedMultiplier;
            }
            else
            {
                anim.SetBool(runningAnimatorHash, false);
                activeRunningSpeedMultiplier = 1;
            }

            var targetRotation = Quaternion.LookRotation(moveDirection);

            var finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);

            rb.MoveRotation(finalRotation);
        }
    }
}
