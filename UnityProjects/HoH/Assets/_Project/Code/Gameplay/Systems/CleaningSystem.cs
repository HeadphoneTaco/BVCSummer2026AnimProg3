using _Project.Code.Core;
using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
    /// <summary>
    ///     Resolves mess states produced by Neutral and Fail chemistry outcomes.
    ///     Foundational build: clears mess state without animation or feedback.
    /// </summary>
    public class CleaningSystem : MonoBehaviour
    {
        private bool messPresent;
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
            if (result.OutcomeType == OutcomeType.Neutral || result.OutcomeType == OutcomeType.Fail)
            {
                messPresent = true;
                Debug.Log($"[CleaningSystem] Mess created: {result.ResultName}. Cleaning required.");
            }
        }

        /// <summary>Entry point called by the Clean button in the UI.</summary>
        public void Clean()
        {
            if (!messPresent)
            {
                Debug.Log("[CleaningSystem] Nothing to clean.");
                return;
            }

            messPresent = false;
            Debug.Log("[CleaningSystem] Mess cleared.");
            // TODO A2: trigger animation and sound
        }

        public bool HasMess()
        {
            return messPresent;
        }
    }
}