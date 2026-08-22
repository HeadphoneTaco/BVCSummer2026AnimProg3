using System;
using System.Collections.Generic;
using _Project.Code.Core.Enums;
using _Project.Code.Gameplay;
using UnityEngine;

namespace _Project.Code.Core
{
    /// <summary>
    ///     ScriptableObject defining one combination rule.
    ///     Author one asset per known combination in _Project/ScriptableObjects/Recipes/.
    ///     Order of ingredients does not matter — lookup normalises before matching.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCombinationRule", menuName = "House of Healing/Combination Rule")]
    public class CombinationRuleData : ScriptableObject
    {
        
        public IngredientData[]  ingredients;
        public OutcomeType outcomeType;
        public string resultName;
        public Item resultItem;

        /// <summary>
        ///     True when the staged ingredients are exactly this rule's ingredients,
        ///     in any order, duplicates counted. A multiset comparison: Water+Water+Herbs
        ///     only matches a rule authored with two Waters and one Herbs.
        /// </summary>
        public bool Matches(IReadOnlyList<IngredientData> stagedData)
        {
            if (stagedData == null || stagedData.Count != ingredients.Length) return false;

            // Tick off each staged ingredient against an unclaimed slot in the rule.
            var claimed = new bool[ingredients.Length];
            foreach (var staged in stagedData)
            {
                var found = false;
                for (var i = 0; i < ingredients.Length; i++)
                {
                    if (claimed[i] || ingredients[i] != staged) continue;
                    claimed[i] = true;
                    found = true;
                    break;
                }

                if (!found) return false;
            }

            return true;
        }
    }
}