using _Project.Code.Core;
using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay
{
    /// <summary>
    ///     MonoBehaviour representing a single ingredient instance in the scene.
    ///     Implements both IIngredient (for ChemistrySystem) and IProcessable (for ProcessingSystem).
    ///     These are separate interfaces by design — each system only sees what it needs.
    /// </summary>
    public class Ingredient : MonoBehaviour, IIngredient, IProcessable
    {
        public IngredientData data;

        private bool isProcessed;

        // --- IIngredient ---

        public IngredientData GetData()
        {
            return data;
        }

        public bool IsProcessed()
        {
            return isProcessed;
        }

        // --- IProcessable ---

        public void Process()
        {
            if (isProcessed)
            {
                Debug.LogWarning($"{data.ingredientName} is already processed.");
                return;
            }

            isProcessed = true;
            Debug.Log($"{data.ingredientName} processed. State: processed.");

            // TODO A2: trigger sprite swap, animation, sound
        }
    }
}