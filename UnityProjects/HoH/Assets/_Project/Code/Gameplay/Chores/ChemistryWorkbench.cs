using System.Collections.Generic;
using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.Chores
{
 
    public class ChemistryWorkbench : MonoBehaviour, IInteractable
    {
        [Tooltip("Auto-filled from this object if left empty — the system lives on the bench.")]
        [SerializeField] private ChemistrySystem chemistrySystem;

        [Tooltip("Auto-filled from this object if left empty.")]
        [SerializeField] private CleaningSystem cleaningSystem;

        private readonly List<IIngredient> stagedIngredients = new();

        private void Awake()
        {
            // The equipment owns its systems: prefer siblings on this same object.
            if (chemistrySystem == null) chemistrySystem = GetComponent<ChemistrySystem>();
            if (cleaningSystem == null) cleaningSystem = GetComponent<CleaningSystem>();
        }

        public IReadOnlyList<IIngredient> StagedIngredients => stagedIngredients;

        // --- IInteractable ---

        public string GetInteractionPrompt()
        {
            if (cleaningSystem != null && cleaningSystem.HasMess())
                return "Clean up mess";

            return stagedIngredients.Count switch
            {
                0 => "Workbench (add ingredients first)",
                1 => $"Workbench ({stagedIngredients.Count}/2 — need at least 2)",
                _ => $"Combine {stagedIngredients.Count} ingredient{(stagedIngredients.Count > 1 ? "s" : "")}"
            };
        }

        public void Interact(GameObject interactor)
        {
            // Prioritise cleaning if there's a mess
            if (cleaningSystem != null && cleaningSystem.HasMess())
            {
                cleaningSystem.Clean();
                return;
            }

            if (stagedIngredients.Count < 2)
            {
                Debug.Log("[ChemistryWorkbench] Not enough ingredients to combine.");
                return;
            }

            if (chemistrySystem != null)
            {
                chemistrySystem.Evaluate(stagedIngredients);
                ConsumeStagedIngredients();
            }
            else
            {
                Debug.LogWarning("[ChemistryWorkbench] No ChemistrySystem assigned.");
            }
        }

            private void OnTriggerEnter(Collider other)
        {
            var ingredient = FindIngredient(other);
            if (ingredient == null || stagedIngredients.Contains(ingredient)) return;

            StageIngredient(ingredient);
        }

        private void OnTriggerExit(Collider other)
        {
            // Picking an ingredient back up (or it rolling off) unstages it.
            var ingredient = FindIngredient(other);
            if (ingredient == null) return;

            if (stagedIngredients.Remove(ingredient))
                Debug.Log($"[ChemistryWorkbench] Unstaged: {ingredient.GetData().ingredientName} " +
                          $"({stagedIngredients.Count}/3)");
        }

        private static Ingredient FindIngredient(Collider col)
        {
            if (col == null) return null;
            return col.attachedRigidbody != null
                ? col.attachedRigidbody.GetComponent<Ingredient>()
                : col.GetComponent<Ingredient>();
        }

     
        public bool StageIngredient(IIngredient ingredient)
        {
            if (stagedIngredients.Count >= 3)
            {
                Debug.LogWarning("[ChemistryWorkbench] Workbench is full (max 3 ingredients).");
                return false;
            }

            stagedIngredients.Add(ingredient);
            Debug.Log(
                $"[ChemistryWorkbench] Staged: {ingredient.GetData().ingredientName} ({stagedIngredients.Count}/3)");
            return true;
        }

        public void ClearStaged()
        {
            stagedIngredients.Clear();
        }

        /// <summary>
        ///     A combination consumes its ingredients regardless of outcome —
        ///     destroy the staged ingredient objects and empty the list.
        /// </summary>
        private void ConsumeStagedIngredients()
        {
            foreach (var ingredient in stagedIngredients)
                if (ingredient is Component component && component != null)
                    Destroy(component.gameObject);

            stagedIngredients.Clear();
        }
    }
}