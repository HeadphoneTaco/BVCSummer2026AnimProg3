using _Project.Code.Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction")] [SerializeField]
        private float interactionRadius = 2f;

        [SerializeField] private LayerMask interactableLayer;

        [Header("Input")] [SerializeField] private InputActionReference interactAction;

        [Tooltip("Key name shown in the prompt. Must match the interact action's actual binding.")]
        [SerializeField] private string promptKeyLabel = "E";

        private IInteractable currentTarget;
        private readonly Collider[] overlapBuffer = new Collider[8];
        private bool warnedAboutLayerMask;

        // Cached prompt drawing state. The text is refreshed once per frame in Update; the
        // GUIStyle, GUIContent and measured rect are rebuilt only when that text or the screen
        // size changes, so the OnGUI path itself allocates nothing.
        private GUIStyle promptStyle;
        private GUIContent promptContent;
        private Rect promptRect;
        private string promptText;
        private bool promptDirty;
        private int cachedScreenWidth;
        private int cachedScreenHeight;

        private void Update()
        {
            FindClosestInteractable();
            RefreshPromptText();

            // Prefer the assigned InputAction; fall back to the E key so an unassigned
            // reference degrades to working defaults instead of a silent dead button.
            // The fallback exists so an unassigned reference degrades to a working default
            // instead of a silent dead button. It reads the keyboard directly, because the
            // legacy Input class throws in an Input System only project.
            var pressed = interactAction != null
                ? interactAction.action.WasPressedThisFrame()
                : Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;

            if (currentTarget != null && pressed)
                currentTarget.Interact(gameObject);
        }

        private void OnEnable()
        {
            interactAction?.action.Enable();
        }

        private void OnDisable()
        {
            interactAction?.action.Disable();
        }

        private void RefreshPromptText()
        {
            if (currentTarget == null)
            {
                promptText = null;
                return;
            }

            var latest = currentTarget.GetInteractionPrompt();
            if (latest == promptText) return;

            promptText = latest;
            promptDirty = true;
        }

        // Rudimentary prompt, visible in Game view without any UI setup.
        // The draw path allocates nothing: the style is built once ever, and the content and
        // rect are rebuilt only when the label or the window size changes.
        private void OnGUI()
        {
            if (promptText == null) return;

            // GUI.skin is only valid inside an OnGUI call, so the style cannot be built earlier.
            promptStyle ??= new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            if (Screen.width != cachedScreenWidth || Screen.height != cachedScreenHeight)
                promptDirty = true;

            if (promptDirty || promptContent == null)
                RebuildPromptLayout();

            GUI.Box(promptRect, promptContent, promptStyle);
        }

        private void RebuildPromptLayout()
        {
            promptContent = new GUIContent($"[{promptKeyLabel}] {promptText}");

            var width = Mathf.Min(promptStyle.CalcSize(promptContent).x + 24f, 440f);
            var height = promptStyle.CalcHeight(promptContent, width) + 12f;
            promptRect = new Rect((Screen.width - width) / 2f, Screen.height - 100, width, height);

            cachedScreenWidth = Screen.width;
            cachedScreenHeight = Screen.height;
            promptDirty = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        private void FindClosestInteractable()
        {
            // An unassigned LayerMask is "Nothing", which silently finds nothing.
            // Treat that as "Everything" and say so once, so the misconfiguration is visible.
            var mask = interactableLayer.value;
            if (mask == 0)
            {
                if (!warnedAboutLayerMask)
                {
                    Debug.LogWarning("[PlayerInteraction] Interactable Layer is set to Nothing — " +
                                     "searching all layers. Assign a layer to silence this.", this);
                    warnedAboutLayerMask = true;
                }

                mask = ~0;
            }

            var count = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, overlapBuffer,
                mask);

            IInteractable best = null;
            var bestDist = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                // GetComponentInParent, not GetComponent: a station's visual mesh usually
                // carries its own collider on a child object, and the script lives on the parent.
                // Asking only the collider's own object finds nothing in that very common setup.
                var candidate = overlapBuffer[i].GetComponentInParent<IInteractable>();
                if (candidate == null) continue;

                var dist = Vector3.Distance(transform.position, overlapBuffer[i].transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = candidate;
                }
            }

            currentTarget = best;
        }
    }
}