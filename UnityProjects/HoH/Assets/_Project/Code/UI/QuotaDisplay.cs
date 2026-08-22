using _Project.Code.Gameplay.Systems;
using TMPro;
using UnityEngine;

namespace _Project.Code.UI
{
   
    public class QuotaDisplay : MonoBehaviour
    {
        [SerializeField] private QuotaSystem quotaSystem;
        [SerializeField] private TMP_Text quotaText;

        [Tooltip("Shown when the quota is met.")] [SerializeField]
        private string dayCompleteMessage = "Day complete!";

        private void OnEnable()
        {
            if (quotaSystem != null)
            {
                quotaSystem.OnQuotaChanged += HandleQuotaChanged;
                quotaSystem.OnQuotaReached += HandleQuotaReached;

                // Match current state on enable (e.g. display activated mid-day).
                if (quotaSystem.QuotaReached) HandleQuotaReached();
                else HandleQuotaChanged(quotaSystem.RemediesCrafted, quotaSystem.TargetRemedies);
            }
        }

        private void OnDisable()
        {
            if (quotaSystem != null)
            {
                quotaSystem.OnQuotaChanged -= HandleQuotaChanged;
                quotaSystem.OnQuotaReached -= HandleQuotaReached;
            }
        }

        private void HandleQuotaChanged(int crafted, int target)
        {
            if (quotaText != null)
                quotaText.text = $"Remedies {crafted}/{target}";
        }

        private void HandleQuotaReached()
        {
            if (quotaText != null)
                quotaText.text = dayCompleteMessage;
        }
    }
}
