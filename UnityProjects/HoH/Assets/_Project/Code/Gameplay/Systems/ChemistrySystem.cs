using System;
using System.Collections.Generic;
using _Project.Code.Core;
using _Project.Code.Core.Enums;
using _Project.Code.Core.Interfaces;
using _Project.Code.Utilities;
using UnityEngine;

namespace _Project.Code.Gameplay.Systems
{
    
    public class ChemistrySystem : MonoBehaviour
    {
        [Tooltip("All authored CombinationRuleData assets.")] [SerializeField]
        private CombinationRuleData[] combinationRules;

        [Tooltip("Where result items appear. Leave empty to spawn beside this component — " +
                 "which, with the system living on the workbench, is exactly where the work happens.")]
        public Transform itemSpawnTransform;

        [Tooltip("Offset from the spawn transform, so items pop out beside the bench instead of inside it.")]
        [SerializeField] private Vector3 spawnOffset = new(0f, 1f, 0.75f);

    
        public static event Action<OutcomeResult> OnCombinationResolved;

    
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

                               if (rule.resultItem != null)
                {
                    var anchor = itemSpawnTransform != null ? itemSpawnTransform : transform;
                    var spawnPosition = anchor.position + anchor.TransformVector(spawnOffset);
                    var spawned = Instantiate(rule.resultItem, spawnPosition, Quaternion.identity);

                                       spawned.ItemName = rule.resultName;
                }

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
