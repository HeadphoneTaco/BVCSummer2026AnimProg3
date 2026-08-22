using System.Collections.Generic;
using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Utilities
{
    public static class IngredientValidator
    {
        private const int minIngredients = 2;
        private const int maxIngredients = 3;

        public static bool ValidateCount(List<IIngredient> ingredients)
        {
            if (ingredients == null || ingredients.Count < minIngredients || ingredients.Count > maxIngredients)
            {
                Debug.LogWarning(
                    $"[IngredientValidator] Invalid ingredient count: {ingredients?.Count}. Expected {minIngredients}–{maxIngredients}.");
                return false;
            }

            return true;
        }

        public static bool ValidateAllProcessed(List<IIngredient> ingredients)
        {
            foreach (var ingredient in ingredients)
                if (!ingredient.IsProcessed())
                {
                    // Plain Log, not LogWarning: a raw ingredient in the mix is normal
                    // gameplay (it's how Fail messes happen), not a program error.
                    Debug.Log(
                        $"[IngredientValidator] {ingredient.GetData().ingredientName} is not processed. Combination blocked.");
                    return false;
                }

            return true;
        }
    }
}