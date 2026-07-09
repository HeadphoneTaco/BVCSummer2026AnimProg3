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

        [Tooltip("Multiplied into every renderer's colour on Process(), so state is visible. " +
                 "Darker = processed. Replace with real art/VFX later.")]
        [SerializeField] private Color processedTint = new(0.55f, 0.5f, 0.65f);

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

            ApplyProcessedVisual();
            // TODO: animation and sound on top of the tint
        }

        /// <summary>
        ///     Placeholder state feedback: tint every renderer so raw and processed
        ///     are distinguishable at a glance. Instance materials only — the shared
        ///     material assets are never touched.
        /// </summary>
        private void ApplyProcessedVisual()
        {
            foreach (var rend in GetComponentsInChildren<Renderer>())
                rend.material.color *= processedTint;
        }
    }
}