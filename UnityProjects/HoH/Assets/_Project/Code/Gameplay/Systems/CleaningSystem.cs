using _Project.Code.Core;
using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
  
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

               public void Clean()
        {
            if (!messPresent)
            {
                Debug.Log("[CleaningSystem] Nothing to clean.");
                return;
            }

            messPresent = false;
            Debug.Log("[CleaningSystem] Mess cleared.");
        }

        public bool HasMess()
        {
            return messPresent;
        }
    }
}