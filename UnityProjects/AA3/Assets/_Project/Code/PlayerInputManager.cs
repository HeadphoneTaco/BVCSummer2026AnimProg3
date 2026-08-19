using UnityEngine;

[DefaultExecutionOrder(-3)]
public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance;

    public PlayerControls PlayerControls { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Built here rather than in OnEnable. Every input script asks for this from its own
        // OnEnable, and Awake at this execution order is the only place guaranteed to have run
        // first. Creating it in OnEnable is what produces "Player controls is not initialized".
        PlayerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        PlayerControls?.Enable();
    }

    private void OnDisable()
    {
        PlayerControls?.Disable();
    }

    private void OnDestroy()
    {
        // Static fields survive play mode exit when domain reload is disabled, which would
        // otherwise leave a stale reference to a destroyed object on the next run.
        if (Instance == this)
            Instance = null;
    }
}
