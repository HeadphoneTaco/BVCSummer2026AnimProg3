using _Project.Code.Core;
using _Project.Code.Gameplay.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace _Project.Code.Gameplay.Systems
{
       public enum GamePhase
    {
        Ready,
        Playing,
        Won,
        Lost
    }

  
    public class GameManager : MonoBehaviour
    {
        [Header("Win condition")]
        [Tooltip("Deliveries are counted here. Win fires when this reports QuotaReached.")]
        [SerializeField] private QuotaSystem quotaSystem;

        [Header("Lose condition")]
        [Tooltip("Seconds in a day. The run is lost when this expires with the quota unmet.")]
        [SerializeField] private float dayLengthSeconds = 180f;

        [Header("Player")]
        [Tooltip("Behaviours switched off outside Playing: controller, interaction, pickup. " +
                 "Listed rather than found, so nothing is disabled by accident.")]
        [SerializeField] private MonoBehaviour[] gameplayBehaviours;

        [Tooltip("Optional. Velocity is zeroed when gameplay stops, so the player does not " +
                 "coast across the room behind the win screen.")]
        [SerializeField] private Rigidbody playerBody;

        [Header("Cursor")]
        [Tooltip("Lock and hide the cursor while the day is running, release it on the Ready, Won " +
                 "and Lost screens. In the editor, Escape releases it at any time.")]
        [SerializeField] private bool lockCursorDuringPlay = true;

        [Header("Keys")]
        [Tooltip("Input System Key, not the legacy KeyCode. The project runs Input System only, " +
                 "so the legacy Input class throws at runtime.")]
        [SerializeField] private Key startKey = Key.Space;

        [SerializeField] private Key restartKey = Key.R;

        private readonly StateMachine machine = new();

        private ReadyState ready;
        private PlayingState playing;
        private WonState won;
        private LostState lost;

        private GUIStyle bannerStyle;

        public GamePhase Phase { get; private set; } = GamePhase.Ready;
        public float TimeRemaining { get; private set; }
        public float DayLengthSeconds => dayLengthSeconds;
        public Key StartKey => startKey;
        public Key RestartKey => restartKey;

            public bool StartPressed => WasPressedThisFrame(startKey);

            public bool RestartPressed => WasPressedThisFrame(restartKey);

              public bool QuotaReached => quotaSystem != null && quotaSystem.QuotaReached;

        private void Awake()
        {
            ready = new ReadyState(this);
            playing = new PlayingState(this);
            won = new WonState(this);
            lost = new LostState(this);
        }

        private void Start()
        {
            machine.ChangeState(ready);
        }

        private void Update()
        {
            machine.Tick();
        }

        public void GoToReady()
        {
            Phase = GamePhase.Ready;
            machine.ChangeState(ready);
        }

        public void GoToPlaying()
        {
            Phase = GamePhase.Playing;
            machine.ChangeState(playing);
        }

        public void GoToWon()
        {
            Phase = GamePhase.Won;
            machine.ChangeState(won);
        }

        public void GoToLost()
        {
            Phase = GamePhase.Lost;
            machine.ChangeState(lost);
        }

       
        private static bool WasPressedThisFrame(Key key)
        {
            var keyboard = Keyboard.current;
            return keyboard != null && keyboard[key].wasPressedThisFrame;
        }

        public void ResetDayTimer()
        {
            TimeRemaining = dayLengthSeconds;
        }

              public void TickDayTimer(float deltaTime)
        {
            TimeRemaining = Mathf.Max(0f, TimeRemaining - deltaTime);
        }

     
        public void SetGameplayEnabled(bool enabledState)
        {
            if (gameplayBehaviours != null)
                foreach (var behaviour in gameplayBehaviours)
                    if (behaviour != null)
                        behaviour.enabled = enabledState;

            if (!enabledState && playerBody != null)
            {
                playerBody.linearVelocity = Vector3.zero;
                playerBody.angularVelocity = Vector3.zero;
            }

            ApplyCursorMode(enabledState);
        }

      
        private void ApplyCursorMode(bool playing)
        {
            if (!lockCursorDuringPlay) return;

            Cursor.lockState = playing ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !playing;
        }

        // Leaving play mode, or unloading the scene on restart, must not strand a hidden cursor.
        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

      
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

              private void OnGUI()
        {
            bannerStyle ??= new GUIStyle(GUI.skin.box)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            var banner = Phase switch
            {
                GamePhase.Ready => $"HOUSE OF HEALING\nPress {startKey} to start the day",
                GamePhase.Playing => $"Time {Mathf.CeilToInt(TimeRemaining)}s",
                GamePhase.Won => $"DAY COMPLETE\nQuota filled with {Mathf.CeilToInt(TimeRemaining)}s to spare" +
                                 $"\nPress {restartKey} to run it again",
                GamePhase.Lost => $"OUT OF TIME\nThe quota went unfilled\nPress {restartKey} to try again",
                _ => string.Empty
            };

            var width = Phase == GamePhase.Playing ? 200f : 520f;
            var height = Phase == GamePhase.Playing ? 44f : 130f;
            var y = Phase == GamePhase.Playing ? 12f : Screen.height / 2f - height / 2f;

            GUI.Box(new Rect((Screen.width - width) / 2f, y, width, height), banner, bannerStyle);
        }
    }
}
