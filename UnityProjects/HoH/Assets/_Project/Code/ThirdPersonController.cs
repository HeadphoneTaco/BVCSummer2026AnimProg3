using UnityEngine;

namespace _Project.Code
{
  
    [DefaultExecutionOrder(-1)]
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Speeds (metres per second)")]
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float runSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;

        [Header("Ground handling")]
        [SerializeField] private float acceleration = 35f;
        [SerializeField] private float deceleration = 25f;

        [Header("Jump and gravity")]
        [SerializeField] private float gravity = -25f;
        [SerializeField] private float jumpSpeed = 8f;

        [Header("Rotation")]
        [Tooltip("Degrees per second the character turns toward its movement direction.")]
        [SerializeField] private float rotationSpeed = 540f;

        [Tooltip("Below this angle the character counts as already facing its target.")]
        [SerializeField] private float rotationTolerance = 5f;

        [Header("References")]
        [Tooltip("Leave empty. Resolved at runtime from the camera tagged MainCamera.")]
        [SerializeField] private Transform cameraTransform;

        private CharacterController _characterController;
        private Vector3 _horizontalVelocity;
        private PlayerLocomotionInput _locomotionInput;
        private PlayerState _playerState;
        private float _targetYaw;
        private float _verticalVelocity;

        // Read by PlayerAnimation. True while the character is still turning toward
        // the direction it has been asked to move in.
        public bool IsRotatingToTarget { get; private set; }

        // Read by PlayerAnimation. Signed angle in degrees from where the character is
        // facing to where the camera is looking. Negative is left, positive is right.
        public float RotationMismatch { get; private set; }

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _locomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();

            if (_locomotionInput == null)
                Debug.LogError("ThirdPersonController needs a PlayerLocomotionInput on the same object.", this);

            if (_playerState == null)
                Debug.LogError("ThirdPersonController needs a PlayerState on the same object.", this);

            if (cameraTransform == null)
            {
                if (Camera.main != null)
                {
                    cameraTransform = Camera.main.transform;
                }
                else
                {
                    Debug.LogError(
                        "ThirdPersonController found no camera. Tag the scene camera as MainCamera, " +
                        "or assign cameraTransform on this instance.", this);
                    enabled = false;
                    return;
                }
            }

            _targetYaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            if (_locomotionInput == null || _playerState == null)
                return;

            var moveDirection = GetCameraRelativeMoveDirection();

            UpdateMovementState(moveDirection);
            UpdateVerticalVelocity();
            UpdateHorizontalVelocity(moveDirection);
            UpdateRotation(moveDirection);

            var motion = _horizontalVelocity + Vector3.up * _verticalVelocity;
            _characterController.Move(motion * Time.deltaTime);
        }

      
        private Vector3 GetCameraRelativeMoveDirection()
        {
            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;

            cameraForward.y = 0f;
            cameraRight.y = 0f;

            cameraForward.Normalize();
            cameraRight.Normalize();

            var input = _locomotionInput.MovementInput;
            var direction = cameraForward * input.y + cameraRight * input.x;

            // A diagonal on a keyboard reads as (1, 1), which is longer than a full stick deflection.
            if (direction.sqrMagnitude > 1f)
                direction.Normalize();

            return direction;
        }

        private void UpdateMovementState(Vector3 moveDirection)
        {
            var isMoving = moveDirection.sqrMagnitude > 0.0001f;
            var isGrounded = _characterController.isGrounded;

            if (!isGrounded)
            {
                _playerState.SetPlayerMovementState(
                    _verticalVelocity > 0f ? PlayerMovementState.Jumping : PlayerMovementState.Falling);
                return;
            }

            if (!isMoving)
            {
                _playerState.SetPlayerMovementState(PlayerMovementState.Idling);
                return;
            }

            if (_locomotionInput.WalkToggledOn)
                _playerState.SetPlayerMovementState(PlayerMovementState.Walking);
            else if (_locomotionInput.SprintToggledOn)
                _playerState.SetPlayerMovementState(PlayerMovementState.Sprinting);
            else
                _playerState.SetPlayerMovementState(PlayerMovementState.Running);
        }

        private void UpdateVerticalVelocity()
        {
            // A small downward bias keeps isGrounded stable instead of flickering as the
            // controller drifts a hair off the floor.
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            if (_locomotionInput.JumpPressed && _characterController.isGrounded)
                _verticalVelocity = jumpSpeed;

            _verticalVelocity += gravity * Time.deltaTime;
        }

        private void UpdateHorizontalVelocity(Vector3 moveDirection)
        {
            var targetSpeed = GetTargetSpeed();
            var targetVelocity = moveDirection * targetSpeed;

            var rate = moveDirection.sqrMagnitude > 0.0001f ? acceleration : deceleration;

            _horizontalVelocity = Vector3.MoveTowards(
                _horizontalVelocity, targetVelocity, rate * Time.deltaTime);
        }

        private float GetTargetSpeed()
        {
            switch (_playerState.CurrentPlayerMovementState)
            {
                case PlayerMovementState.Walking:
                    return walkSpeed;
                case PlayerMovementState.Sprinting:
                    return sprintSpeed;
                case PlayerMovementState.Idling:
                    return 0f;
                default:
                    return runSpeed;
            }
        }

        private void UpdateRotation(Vector3 moveDirection)
        {
            if (moveDirection.sqrMagnitude > 0.0001f)
                _targetYaw = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            var currentYaw = transform.eulerAngles.y;

            IsRotatingToTarget = Mathf.Abs(Mathf.DeltaAngle(currentYaw, _targetYaw)) > rotationTolerance;

            var newYaw = Mathf.MoveTowardsAngle(currentYaw, _targetYaw, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newYaw, 0f);

            var cameraForward = cameraTransform.forward;
            cameraForward.y = 0f;

            if (cameraForward.sqrMagnitude > 0.0001f)
            {
                var cameraYaw = Mathf.Atan2(cameraForward.x, cameraForward.z) * Mathf.Rad2Deg;
                RotationMismatch = Mathf.DeltaAngle(newYaw, cameraYaw);
            }
        }
    }
}
