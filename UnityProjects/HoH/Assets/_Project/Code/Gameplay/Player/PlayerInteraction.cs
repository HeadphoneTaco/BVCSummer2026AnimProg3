using _Project.Code.Core.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Player
{
    /// <summary>
    ///     Detects nearby IInteractable objects via sphere overlap and triggers them on Interact press.
    ///     Displays a world-space prompt label via OnGUI (rudimentary — replace with UIToolkit in A2).
    /// </summary>
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

        private void Update()
        {
            FindClosestInteractable();

            // Prefer the assigned InputAction; fall back to the E key so an unassigned
            // reference degrades to working defaults instead of a silent dead button.
            var pressed = interactAction != null
                ? interactAction.action.WasPressedThisFrame()
                : Input.GetKeyDown(KeyCode.E);

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

        // Rudimentary prompt — visible in Game view without any UI setup
        private void OnGUI()
        {
            if (currentTarget == null) return;

            var prompt = $"[{promptKeyLabel}] {currentTarget.GetInteractionPrompt()}";
            var style = new GUIStyle(GUI.skin.box)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            // Size the box to its content instead of clipping: grow up to a max width,
            // then wrap and grow downward.
            var content = new GUIContent(prompt);
            var width = Mathf.Min(style.CalcSize(content).x + 24f, 440f);
            var height = style.CalcHeight(content, width) + 12f;
            GUI.Box(new Rect((Screen.width - width) / 2f, Screen.height - 100, width, height), prompt, style);
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
                var candidate = overlapBuffer[i].GetComponent<IInteractable>();
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