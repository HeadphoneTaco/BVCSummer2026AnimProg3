using System;
using System.Collections.Generic;
using System.Text;
using _Project.Code.Core;
using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
    /// <summary>
    ///     Manages the building inventory and ingredient storage.
    ///     Subscribes to ChemistrySystem.OnCombinationResolved and stores successful outcomes.
    ///     Foundational build: flat list, no sorting, filtering, or quantity tracking.
    /// </summary>
    public class InventorySystem : MonoBehaviour
    {
        private readonly List<string> buildingInventory = new();
        private readonly List<IngredientData> ingredientStorage = new();

        /// <summary>
        ///     Raised whenever the inventory contents change. The display (view) subscribes
        ///     to this instead of polling — the model never references the UI.
        /// </summary>
        public event Action OnInventoryChanged;
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
            if (result.OutcomeType == OutcomeType.Success)
            {
                buildingInventory.Add(result.ResultName);
                Debug.Log($"[InventorySystem] Added to building inventory: {result.ResultName}");
                OnInventoryChanged?.Invoke();
            }
        }

        public void StoreIngredient(IngredientData ingredient)
        {
            ingredientStorage.Add(ingredient);
            Debug.Log($"[InventorySystem] Stored ingredient: {ingredient.ingredientName}");
            OnInventoryChanged?.Invoke();
        }

        /// <summary>Returns a newline-separated list for the rudimentary UI text display.</summary>
        public string GetBuildingInventoryText()
        {
            if (buildingInventory.Count == 0) return "(empty)";
            var sb = new StringBuilder();
            foreach (var item in buildingInventory) sb.AppendLine(item);
            return sb.ToString().TrimEnd();
        }
    }
}