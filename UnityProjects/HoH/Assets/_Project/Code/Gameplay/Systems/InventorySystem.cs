using System;
using System.Collections.Generic;
using System.Text;
using _Project.Code.Core;
using _Project.Code.Gameplay.Chores;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
   
    public class InventorySystem : MonoBehaviour
    {
        private readonly List<string> buildingInventory = new();
        private readonly List<IngredientData> ingredientStorage = new();

       
        public event Action OnInventoryChanged;

        private void OnEnable()
        {
            DeliveryShelf.OnItemDelivered += HandleItemDelivered;
        }

        private void OnDisable()
        {
            DeliveryShelf.OnItemDelivered -= HandleItemDelivered;
        }

        private void HandleItemDelivered(string itemName)
        {
            buildingInventory.Add(itemName);
            Debug.Log($"[InventorySystem] Added to building inventory: {itemName}");
            OnInventoryChanged?.Invoke();
        }

        public void StoreIngredient(IngredientData ingredient)
        {
            ingredientStorage.Add(ingredient);
            Debug.Log($"[InventorySystem] Stored ingredient: {ingredient.ingredientName}");
            OnInventoryChanged?.Invoke();
        }

              public string GetBuildingInventoryText()
        {
            if (buildingInventory.Count == 0) return "(empty)";
            var sb = new StringBuilder();
            foreach (var item in buildingInventory) sb.AppendLine(item);
            return sb.ToString().TrimEnd();
        }
    }
}
