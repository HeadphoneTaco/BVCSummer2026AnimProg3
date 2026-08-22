using System;
using _Project.Code.Core.Enums;
using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Strategies;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{

    public class ProcessingSystem : MonoBehaviour
    {
        private readonly InorganicProcessingStrategy inorganicStrategy = new();

        private readonly OrganicProcessingStrategy organicStrategy = new();
        private readonly SolventProcessingStrategy solventStrategy = new();
        public static event Action<IIngredient> OnIngredientProcessed;

    
        public void ProcessIngredient(Ingredient ingredient)
        {
            if (ingredient.IsProcessed())
            {
                Debug.LogWarning("Ingredient is already processed.");
                return;
            }

            var strategy = SelectStrategy(ingredient.GetData().category);
            strategy.Execute(ingredient);

            OnIngredientProcessed?.Invoke(ingredient);
        }

        private IProcessingStrategy SelectStrategy(IngredientCategory category)
        {
            return category switch
            {
                IngredientCategory.Organic => organicStrategy,
                IngredientCategory.Inorganic => inorganicStrategy,
                IngredientCategory.Solvent => solventStrategy,
                _ => throw new ArgumentOutOfRangeException(nameof(category), category,
                    "No strategy registered for category.")
            };
        }
    }
}