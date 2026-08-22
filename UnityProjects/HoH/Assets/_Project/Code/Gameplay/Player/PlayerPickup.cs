using UnityEngine;
using UnityEngine.InputSystem;

namespace _Project.Code.Gameplay.Player
{
  
    public class PlayerPickup : MonoBehaviour
    {
        [Header("Interaction sphere")]
        [Tooltip("Where held objects stick and where the grab check is centred. Assign the player's InteractSphere.")]
        [SerializeField] private Transform interactSphere;

        [Tooltip("Grab radius around the interaction sphere. Auto-filled from a SphereCollider on it if present.")]
        [SerializeField] private float pickupRadius = 0.5f;

        [Tooltip("Only objects on these layers can be picked up.")]
        [SerializeField] private LayerMask pickableLayers = ~0;

        private Rigidbody heldBody;
        private readonly Collider[] overlapBuffer = new Collider[16];

        private void Awake()
        {
            if (interactSphere == null) interactSphere = transform;

            // If the interaction sphere has a SphereCollider, match the grab radius to it.
            if (interactSphere.TryGetComponent<SphereCollider>(out var sphere))
            {
                var scale = interactSphere.lossyScale;
                var maxAxis = Mathf.Max(scale.x, Mathf.Max(scale.y, scale.z));
                pickupRadius = sphere.radius * maxAxis;
            }
        }
        private void Update()
        {
            // Input System only project: the legacy Input class throws here, it does not
            // silently return false. Mouse.current is null when no mouse is attached.
            var mouse = Mouse.current;
            if (mouse != null)
            {
                if (mouse.leftButton.wasPressedThisFrame) TryGrab();
                if (mouse.leftButton.wasReleasedThisFrame) Release();
            }

            // Keep the held object glued to the interaction sphere each frame.
            if (heldBody != null)
                heldBody.transform.position = interactSphere.position;
        }

        private void TryGrab()
        {
            if (heldBody != null) return;

            var count = Physics.OverlapSphereNonAlloc(
                interactSphere.position, pickupRadius, overlapBuffer, pickableLayers, QueryTriggerInteraction.Collide);

            Rigidbody best = null;
            var bestDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                if (!IsPickable(overlapBuffer[i], out var body)) continue;

                var distance = (overlapBuffer[i].transform.position - interactSphere.position).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = body;
                }
            }

            if (best == null) return;

            heldBody = best;
            heldBody.isKinematic = true; // suspend physics while carried so it doesn't fall

            // A carried object must also stop COLLIDING, not just falling. A solid
            // kinematic box glued to the player overlaps the character capsule, the
            // controller depenetrates, the box teleports back in — a feedback loop
            // that compounds until the physics solver blows up (editor crash).
            // While held it's a ghost; the world reacts when you set it down.
            heldBody.detectCollisions = false;
        }

        private void Release()
        {
            if (heldBody == null) return;

            heldBody.detectCollisions = true;  // solid again — triggers re-see it here
            heldBody.isKinematic = false;      // hand it back to gravity
            heldBody.linearVelocity = Vector3.zero;  // drop clean, no inherited launch
            heldBody.angularVelocity = Vector3.zero;
            heldBody = null;
        }

            private static bool IsPickable(Collider col, out Rigidbody body)
        {
            body = col != null ? col.attachedRigidbody : null;
            if (body == null) return false;

            return body.GetComponent<Ingredient>() != null || body.GetComponent<Item>() != null;
        }

        private void OnDrawGizmosSelected()
        {
            var centre = interactSphere != null ? interactSphere.position : transform.position;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(centre, pickupRadius);
        }
    }
}
