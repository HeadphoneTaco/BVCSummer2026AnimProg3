using System;
using System.Collections.Generic;
using _Project.Code.Core;
using _Project.Code.Core.Enums;
using _Project.Code.Core.Interfaces;
using _Project.Code.Utilities;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
    /// <summary>
    ///     Evaluates ingredient combinations and raises OnCombinationResolved.
    ///     Does not manage inventory — that is InventorySystem's responsibility.
    ///     Accepts 2 or 3 processed ingredients only.
    ///     Spec rule enforced here: no raw ingredient can succeed, and that check
    ///     happens BEFORE any rule lookup is attempted.
    /// </summary>
    public class ChemistrySystem : MonoBehaviour
    {
        [Tooltip("All authored CombinationRuleData assets.")] [SerializeField]
        private CombinationRuleData[] combinationRules;

        [Tooltip("Where a successful combination's result item is spawned.")]
        public Transform itemSpawnTransform;

        /// <summary>
        ///     Observer hook. InventorySystem stores Success results;
        ///     CleaningSystem creates a mess on Neutral/Fail.
        /// </summary>
        public static event Action<OutcomeResult> OnCombinationResolved;

        /// <summary>
        ///     Entry point called by ChemistryWorkbench with the staged ingredients.
        ///     Returns the resolved outcome, or null when the input was not a valid
        ///     combination attempt (wrong count) — no event is raised in that case.
        /// </summary>
        public OutcomeResult Evaluate(List<IIngredient> stagedIngredients)
        {
            // Not a combination attempt — reject without an outcome or event.
            if (!IngredientValidator.ValidateCount(stagedIngredients))
                return null;

            // Central spec rule: any raw ingredient fails the mix BEFORE lookup.
            if (!IngredientValidator.ValidateAllProcessed(stagedIngredients))
                return Resolve(new OutcomeResult(OutcomeType.Fail, "Ruined Mixture (raw ingredient)"));

            // Rule lookup. Linear scan by design: with a handful of authored rules,
            // a dictionary lookup (the cut CombinationLookup) buys nothing but a class.
            var stagedData = new List<IngredientData>(stagedIngredients.Count);
            foreach (var ingredient in stagedIngredients)
                stagedData.Add(ingredient.GetData());

            foreach (var rule in combinationRules)
            {
                if (!rule.Matches(stagedData)) continue;

                if (rule.outcomeType == OutcomeType.Success && rule.resultItem != null)
                    Instantiate(rule.resultItem, itemSpawnTransform);

                return Resolve(new OutcomeResult(rule.outcomeType, rule.resultName));
            }

            // Processed ingredients but no known rule: a botched experiment.
            return Resolve(new OutcomeResult(OutcomeType.Fail, "Unknown Mixture"));
        }

        private static OutcomeResult Resolve(OutcomeResult result)
        {
            Debug.Log($"[ChemistrySystem] Resolved: {result}");
            OnCombinationResolved?.Invoke(result);
            return result;
        }
    }
}
