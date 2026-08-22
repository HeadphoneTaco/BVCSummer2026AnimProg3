using UnityEngine;

namespace _Project.Code
{
    public class PlayerAnimation : MonoBehaviour
    {
        //Locomotion
        // Casing must match HandcraftedCharacterController exactly. StringToHash is case
        // sensitive and a mismatch fails silently, so the blend tree just never moves.
        private static readonly int inputXHash = Animator.StringToHash("InputX");
        private static readonly int inputYHash = Animator.StringToHash("InputY");
        private static readonly int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");

        private static readonly int isGroundedHash = Animator.StringToHash("isGrounded");
        private static readonly int isFallingHash = Animator.StringToHash("isFalling");
        private static readonly int isJumpingHash = Animator.StringToHash("isJumping");
        private static readonly int isIdlingHash = Animator.StringToHash("isIdling");

        //Actions
        private static readonly int isAttackingHash = Animator.StringToHash("isAttacking");
        private static readonly int isGatheringHash = Animator.StringToHash("isGathering");
        private static readonly int isPlayingActionHash = Animator.StringToHash("isPlayingAction");

        //Camera + Rotation
        private static readonly int isRotatingToTargetHash = Animator.StringToHash("isRotatingToTarget");
        private static readonly int rotationMismatchHash = Animator.StringToHash("rotationMismatch");
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private Vector2 _currentBlendInput = Vector3.zero;
        private PlayerActionsInput _playerActionsInput;
        private ThirdPersonController _thirdPersonController;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;
        private float _rotationMismatch;
        private readonly float _runMaxBlendValue = 1.0f;

        private readonly float _sprintMaxBlendValue = 1.5f;
        private readonly float _walkMaxBlendValue = 0.5f;
        private int[] actionHashes;


        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
            _thirdPersonController = GetComponent<ThirdPersonController>();
            _playerActionsInput = GetComponent<PlayerActionsInput>();

            actionHashes = new[] { isGatheringHash, isAttackingHash };
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            var isIdling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
            var isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
            var isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            var isJumping = _playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
            var isFalling = _playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
            var isGrounded = _playerState.IsGroundedState();
            var isPlayingAction = false;

            foreach (var hash in actionHashes)
                if (_animator.GetBool(hash))
                {
                    isPlayingAction = true;
                    break;
                }

            var isRunBlendValue = isRunning || isJumping || isFalling;
            var inputTarget = isSprinting ? _playerLocomotionInput.MovementInput * _sprintMaxBlendValue :
                isRunBlendValue ? _playerLocomotionInput.MovementInput * _runMaxBlendValue :
                _playerLocomotionInput.MovementInput * _walkMaxBlendValue;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _animator.SetBool(isGroundedHash, isGrounded);
            _animator.SetBool(isIdlingHash, isIdling);
            _animator.SetBool(isFallingHash, isFalling);
            _animator.SetBool(isJumpingHash, isJumping);
            _animator.SetBool(isRotatingToTargetHash, _thirdPersonController.IsRotatingToTarget);
            _animator.SetBool(isAttackingHash, _playerActionsInput.AttackPressed);
            _animator.SetBool(isGatheringHash, _playerActionsInput.GatherPressed);
            _animator.SetBool(isPlayingActionHash, isPlayingAction);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
            _animator.SetFloat(rotationMismatchHash, _thirdPersonController.RotationMismatch);
        }
    }
}