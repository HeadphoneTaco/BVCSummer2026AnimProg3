using System;
using _Project.Code.Core;
using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
    /// <summary>
    ///     Win condition: the house needs a quota of remedies crafted to end the day.
    ///     Another Observer on OnCombinationResolved — it counts Success outcomes only
    ///     and raises OnQuotaReached once. Knows nothing about UI or the chemistry internals.
    /// </summary>
    public class QuotaSystem : MonoBehaviour
    {
        [Tooltip("Successful remedies required to complete the day.")] [SerializeField]
        private int targetRemedies = 3;

        public int RemediesCrafted { get; private set; }
        public int TargetRemedies => targetRemedies;
        public bool QuotaReached => RemediesCrafted >= targetRemedies;

        /// <summary>Raised on every counted success with (crafted, target).</summary>
        public event Action<int, int> OnQuotaChanged;

        /// <summary>Raised once, when the day's quota is met.</summary>
        public event Action OnQuotaReached;

        private void OnEnable()
        {
            ChemistrySystem.OnCombinationResolved += HandleCombinationResolved;
        }

        private void OnDisable()
        {
            ChemistrySystem.OnCombinationResolved -= HandleCombinationResolved;
        }

        private void HandleCombinationResolved(OutcomeResult result)
        {
            if (result.OutcomeType != OutcomeType.Success || QuotaReached) return;

            RemediesCrafted++;
            Debug.Log($"[QuotaSystem] Remedies: {RemediesCrafted}/{targetRemedies}");
            OnQuotaChanged?.Invoke(RemediesCrafted, targetRemedies);

            if (QuotaReached)
            {
                Debug.Log("[QuotaSystem] Quota met — day complete!");
                OnQuotaReached?.Invoke();
            }
        }
    }
}
