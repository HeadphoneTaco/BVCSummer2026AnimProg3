using _Project.Code.Core.Enums;
using UnityEngine;

namespace _Project.Code.Core
{
   
    [CreateAssetMenu(fileName = "NewIngredient", menuName = "House of Healing/Ingredient Data")]
    public class IngredientData : ScriptableObject
    {
         public string ingredientName;
         public IngredientCategory category;
         public Sprite rawSprite;
         public Sprite processedSprite;
         public CombinationRuleData combinationRuleData;
         public IngredientType ingredientType;
         // NOTE: 'isProcessed' was removed from this SO deliberately. Processed-ness is
         // runtime state on the Ingredient instance; a flag on shared data claims to
         // speak for every instance in the game at once, and can only mislead.
    }
}