using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.Chores
{
    /// <summary>
    ///     A world-space ingredient object the player can walk up to and interact with.
    ///     On Interact: if raw, attempts processing. If processed, stores in InventorySystem.
    ///     Requires a Collider on the Interactable layer.
    /// </summary>
    public class IngredientPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private Ingredient ingredient;
        [SerializeField] private ProcessingSystem processingSystem;
        [SerializeField] private InventorySystem inventorySystem;

        private void OnValidate()
        {
            if (ingredient == null)
                ingredient = GetComponent<Ingredient>();
        }

        // --- IInteractable ---

        public string GetInteractionPrompt()
        {
            if (ingredient == null) return "Ingredient";
            return ingredient.IsProcessed()
                ? $"Store {ingredient.GetData().ingredientName}"
                : $"Process {ingredient.GetData().ingredientName}";
        }

        public void Interact(GameObject interactor)
        {
            if (ingredient == null) return;

            if (!ingredient.IsProcessed())
            {
                if (processingSystem != null)
                    processingSystem.ProcessIngredient(ingredient);
                else
                    Debug.LogWarning("[IngredientPickup] No ProcessingSystem assigned.");
            }
            else
            {
                if (inventorySystem != null)
                {
                    inventorySystem.StoreIngredient(ingredient.GetData());
                    Debug.Log($"[IngredientPickup] Stored {ingredient.GetData().ingredientName}.");
                    gameObject.SetActive(false); // Remove from world once stored
                }
                else
                {
                    Debug.LogWarning("[IngredientPickup] No InventorySystem assigned.");
                }
            }
        }
    }
}