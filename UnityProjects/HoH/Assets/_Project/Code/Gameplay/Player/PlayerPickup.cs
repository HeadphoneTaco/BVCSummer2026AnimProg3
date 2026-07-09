using UnityEngine;

namespace _Project.Code.Gameplay.Player
{
    /// <summary>
    ///     Hold-to-carry pickup. Hold the left mouse button to stick the nearest pickable
    ///     object (an Ingredient or Item) to the player's interaction sphere; release the button
    ///     to drop it back into the physics simulation.
    ///
    ///     Like a fridge magnet: while you press, the object clings to the sphere and follows it;
    ///     let go and gravity takes over again.
    /// </summary>
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
            if (Input.GetMouseButtonDown(0)) TryGrab();
            if (Input.GetMouseButtonUp(0)) Release();

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
        }

        private void Release()
        {
            if (heldBody == null) return;

            heldBody.isKinematic = false; // hand it back to gravity
            heldBody = null;
        }

        /// <summary>An object is pickable if it has a Rigidbody and is an Ingredient or an Item.</summary>
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
