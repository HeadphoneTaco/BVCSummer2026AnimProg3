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
        [Tooltip("Auto-filled from this object if left empty — the system lives on the station.")]
        [SerializeField] private ProcessingSystem processingSystem;

        private void Awake()
        {
            // The equipment owns its systems: prefer a sibling on this same object.
            if (processingSystem == null) processingSystem = GetComponent<ProcessingSystem>();
        }

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

        /// <summary>
        ///     Physical registration, same pattern as ChemistryWorkbench: an ingredient dropped
        ///     into the station's trigger zone joins the queue; removing it leaves the queue.
        ///     Requires a collider with Is Trigger enabled on this object.
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            var ingredient = FindIngredient(other);
            if (ingredient != null) AddIngredient(ingredient);
        }

        private void OnTriggerExit(Collider other)
        {
            var ingredient = FindIngredient(other);
            if (ingredient != null) RemoveIngredient(ingredient);
        }

        private static Ingredient FindIngredient(Collider col)
        {
            if (col == null) return null;
            return col.attachedRigidbody != null
                ? col.attachedRigidbody.GetComponent<Ingredient>()
                : col.GetComponent<Ingredient>();
        }

        /// <summary>Adds an ingredient to the station queue (trigger zone or drag-and-drop).</summary>
        public void AddIngredient(Ingredient ingredient)
        {
            if (ingredientsAtStation.Contains(ingredient)) return;

            ingredientsAtStation.Add(ingredient);
            Debug.Log($"[ProcessingStation] Queued: {ingredient.GetData().ingredientName} " +
                      $"({ingredientsAtStation.Count} at station)");
        }

        public void RemoveIngredient(Ingredient ingredient)
        {
            if (ingredientsAtStation.Remove(ingredient))
                Debug.Log($"[ProcessingStation] Removed: {ingredient.GetData().ingredientName} " +
                          $"({ingredientsAtStation.Count} at station)");
        }
    }
}