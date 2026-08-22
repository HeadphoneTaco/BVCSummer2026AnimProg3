using System;
using _Project.Code.Gameplay.Chores;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
  
    public class QuotaSystem : MonoBehaviour
    {
        [Tooltip("Delivered remedies required to complete the day.")] [SerializeField]
        private int targetRemedies = 3;

        public int RemediesCrafted { get; private set; }
        public int TargetRemedies => targetRemedies;
        public bool QuotaReached => RemediesCrafted >= targetRemedies;

        /// <summary>Raised on every counted delivery with (delivered, target).</summary>
        public event Action<int, int> OnQuotaChanged;

        /// <summary>Raised once, when the day's quota is met.</summary>
        public event Action OnQuotaReached;

        private void OnEnable()
        {
            DeliveryShelf.OnItemDelivered += HandleItemDelivered;
        }

        private void OnDisable()
        {
            DeliveryShelf.OnItemDelivered -= HandleItemDelivered;
        }

        // Only combination results spawn Items and only Items can be delivered,
        // so anything arriving here counts — per the design call that all three
        // authored recipes count toward quota.
        private void HandleItemDelivered(string itemName)
        {
            if (QuotaReached) return;

            RemediesCrafted++;
            Debug.Log($"[QuotaSystem] Delivered: {RemediesCrafted}/{targetRemedies}");
            OnQuotaChanged?.Invoke(RemediesCrafted, targetRemedies);

            if (QuotaReached)
            {
                Debug.Log("[QuotaSystem] Quota met — day complete!");
                OnQuotaReached?.Invoke();
            }
        }
    }
}
