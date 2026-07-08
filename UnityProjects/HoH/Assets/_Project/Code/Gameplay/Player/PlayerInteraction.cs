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

        private IInteractable currentTarget;
        private readonly Collider[] overlapBuffer = new Collider[8];

        private void Update()
        {
            FindClosestInteractable();

            if (currentTarget != null && interactAction != null && interactAction.action.WasPressedThisFrame())
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

            var prompt = $"[E] {currentTarget.GetInteractionPrompt()}";
            var style = new GUIStyle(GUI.skin.box) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            GUI.Box(new Rect(Screen.width / 2f - 120, Screen.height - 100, 240, 40), prompt, style);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }

        private void FindClosestInteractable()
        {
            var count = Physics.OverlapSphereNonAlloc(transform.position, interactionRadius, overlapBuffer,
                interactableLayer);

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