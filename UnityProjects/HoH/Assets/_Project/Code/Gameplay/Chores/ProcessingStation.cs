using System.Collections.Generic;
using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.Chores
{
    /// <summary>
    ///     A world-space processing station. The player drops ingredients here;
    ///     interacting processes all unprocessed ingredients in the station's queue.
    /// </summary>
    public class ProcessingStation : MonoBehaviour, IInteractable
    {
        [SerializeField] private ProcessingSystem processingSystem;

        [Tooltip("Ingredients placed at this station, assigned via Inspector or at runtime.")] [SerializeField]
        private List<Ingredient> ingredientsAtStation = new();

        // --- IInteractable ---

        public string GetInteractionPrompt()
        {
            var rawCount = ingredientsAtStation.FindAll(i => i != null && !i.IsProcessed()).Count;
            return rawCount > 0
                ? $"Process ({rawCount} ingredient{(rawCount > 1 ? "s" : "")})"
                : "Station (nothing to process)";
        }

        public void Interact(GameObject interactor)
        {
            if (processingSystem == null)
            {
                Debug.LogWarning("[ProcessingStation] No ProcessingSystem assigned.");
                return;
            }

            var processed = 0;
            foreach (var ingredient in ingredientsAtStation)
                if (ingredient != null && !ingredient.IsProcessed())
                {
                    processingSystem.ProcessIngredient(ingredient);
                    processed++;
                }

            if (processed == 0)
                Debug.Log("[ProcessingStation] Nothing left to process.");
        }

        /// <summary>Called by IngredientPickup or drag-and-drop when the player places an ingredient here.</summary>
        public void AddIngredient(Ingredient ingredient)
        {
            if (!ingredientsAtStation.Contains(ingredient))
                ingredientsAtStation.Add(ingredient);
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            ingredientsAtStation.Remove(ingredient);
        }
    }
}