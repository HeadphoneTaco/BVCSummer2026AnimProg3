using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class ThirdPersonInput : MonoBehaviour, PlayerControls.IThirdPersonMapActions
{
    public Vector2 ScrollInput {  get; private set; }
    private PlayerControls controls;


    //[SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private float _cameraZoomSpeed;
    [SerializeField] private float _cameraMinZoom;
    [SerializeField] private float _cameraMaxZoom;

    //private Cinemachine3rdPersonFollow _thirdPersonFollow;

    private void Awake()
    {
      //  _thirdPersonFollow = _virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot enable");
            return;
        }

        controls = PlayerInputManager.Instance.PlayerControls;
        controls.ThirdPersonMap.Enable();
        controls.ThirdPersonMap.SetCallbacks(this);

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

        controls.ThirdPersonMap.Disable();
        controls.ThirdPersonMap.RemoveCallbacks(this);
        controls = null;
    }

    private void Update()
    {
        //_thirdPersonFollow.CameraDistance = Mathf.Clamp(_thirdPersonFollow.CameraDistance + ScrollInput.y, _cameraMinZoom, _cameraMaxZoom);
        //_thirdPersonFollow.CameraDistance = Mathf.Clamp(_thirdPersonFollow.CameraDistance, _cameraMinZoom, _cameraMaxZoom);
    }

    private void LateUpdate()
    {
        ScrollInput = Vector2.zero;
    }


    public void OnScrollCamera(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        Vector2 scrollInput = context.ReadValue<Vector2>();
        //ScrollInput = -1f * scrollInput.normalized * cameraZoomSpeed;
    }

}


