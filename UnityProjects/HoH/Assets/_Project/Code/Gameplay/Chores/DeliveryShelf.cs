using System;
using UnityEngine;

namespace _Project.Code.Gameplay.Chores
{
    /// <summary>
    ///     The delivery shelf: the far end of the production line. Combined items
    ///     dropped into its trigger zone are delivered — consumed from the world
    ///     and announced via OnItemDelivered for InventorySystem and QuotaSystem.
    ///
    ///     Only RELEASED items count. A held item is kinematic (PlayerPickup suspends
    ///     its physics while carried), so walking past the shelf with a remedy in hand
    ///     does nothing — like carrying a plate past the serving window: nothing is
    ///     served until you actually set it down.
    /// </summary>
    public class DeliveryShelf : MonoBehaviour
    {
        /// <summary>
        ///     Observer hook, mirroring ChemistrySystem.OnCombinationResolved.
        ///     Carries the delivered item's name; the item itself is destroyed
        ///     immediately after this is raised.
        /// </summary>
        public static event Action<string> OnItemDelivered;

        // A thrown item enters the zone already released — deliver on entry.
        private void OnTriggerEnter(Collider other)
        {
            TryDeliver(other);
        }

        // A carried item enters kinematic and is ignored; when the player releases
        // it inside the zone, no new Enter fires — Stay catches the handoff.
        private void OnTriggerStay(Collider other)
        {
            TryDeliver(other);
        }

        private static void TryDeliver(Collider col)
        {
            var body = col != null ? col.attachedRigidbody : null;
            if (body == null || body.isKinematic) return; // held or not a pickup

            var item = body.GetComponent<Item>();
            if (item == null) return; // raw ingredients don't belong on the shelf

            // Destroy() only takes effect at end of frame, and physics can tick more
            // than once before that — so claim the item NOW or a second OnTriggerStay
            // in the same frame delivers it again (the double-count bug).
            if (!item.enabled) return; // already claimed
            item.enabled = false;

            Debug.Log($"[DeliveryShelf] Delivered: {item.ItemName}");
            OnItemDelivered?.Invoke(item.ItemName);
            Destroy(item.gameObject);
        }
    }
}
