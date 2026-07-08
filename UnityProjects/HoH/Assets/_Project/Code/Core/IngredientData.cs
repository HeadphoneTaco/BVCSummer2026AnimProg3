using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Core
{
    /// <summary>
    ///     ScriptableObject defining a single ingredient type.
    ///     Author one asset per ingredient in ScriptableObjects/GameData/.
    /// </summary>
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "House of Healing/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
         public string ingredientName;
         public IngredientCategory category;
         public Sprite rawSprite;
         public Sprite processedSprite;
         public bool isProcessed;
         public CombinationRuleData combinationRuleData;
         public IngredientType ingredientType;
    }
}