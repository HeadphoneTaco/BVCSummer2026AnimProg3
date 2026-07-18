using System.Collections.Generic;
using _Project.Code.Core;
using _Project.Code.Core.Enums;
using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Testing
{
    /// <summary>
    ///     Gym for ChemistrySystem. Runs a fixed set of combination cases once on
    ///     Start and logs PASS/FAIL per case plus a summary. No rerun key — the
    ///     F row belongs to the ingredient spawners now; re-enter play mode to rerun.
    ///     Also subscribes to OnCombinationResolved to prove the Observer event fires —
    ///     this gym is the first listener the event ever had.
    /// </summary>
    public class ChemistryGymRunner : MonoBehaviour
    {
        [Header("System under test")] [SerializeField]
        private ChemistrySystem chemistrySystem;

        [Header("Ingredient data (assign all six)")] [SerializeField]
        private IngredientData water;

        [SerializeField] private IngredientData powders;
        [SerializeField] private IngredientData herbs;
        [SerializeField] private IngredientData oils;
        [SerializeField] private IngredientData fruits;
        [SerializeField] private IngredientData gunpowder;

        private int eventCount;

        private void Start()
        {
            RunAllCases();
        }

        private void OnEnable()
        {
            ChemistrySystem.OnCombinationResolved += HandleResolved;
        }

        private void OnDisable()
        {
            ChemistrySystem.OnCombinationResolved -= HandleResolved;
        }

        private void HandleResolved(OutcomeResult result)
        {
            eventCount++;
            Debug.Log($"[Gym] OnCombinationResolved received: {result}");
        }

        private void RunAllCases()
        {
            eventCount = 0;
            var passed = 0;
            const int total = 5;

            // Each case: staged ingredients (processed unless noted) + expected outcome.
            passed += RunCase("Saline recipe",
                OutcomeType.Success, "Saline",
                (water, true), (powders, true));

            passed += RunCase("Raw ingredient blocked before lookup",
                OutcomeType.Fail, "Ruined Mixture (raw ingredient)",
                (water, false), (powders, true));

            passed += RunCase("Salad recipe (harmless)",
                OutcomeType.Neutral, "Salad",
                (herbs, true), (oils, true));

            passed += RunCase("Explosive laxative (fail recipe)",
                OutcomeType.Fail, "LaxativeBob",
                (fruits, true), (gunpowder, true));

            passed += RunCase("Unknown mixture",
                OutcomeType.Fail, "Unknown Mixture",
                (water, true), (oils, true));

            var eventsOk = eventCount == total;
            Debug.Log($"[Gym] ===== {passed}/{total} cases passed. " +
                      $"Events raised: {eventCount}/{total} {(eventsOk ? "OK" : "MISMATCH")} =====");
        }

        /// <summary>Returns 1 on pass, 0 on fail, so results can be summed.</summary>
        private int RunCase(string label, OutcomeType expectedType, string expectedName,
            params (IngredientData data, bool processed)[] setup)
        {
            var spawned = new List<GameObject>();
            var staged = new List<IIngredient>();

            foreach (var (data, processed) in setup)
            {
                var go = new GameObject($"Gym_{data.ingredientName}");
                go.transform.SetParent(transform);
                var ingredient = go.AddComponent<Gameplay.Ingredient>();
                ingredient.data = data;
                if (processed) ingredient.Process();
                spawned.Add(go);
                staged.Add(ingredient);
            }

            var result = chemistrySystem.Evaluate(staged);

            foreach (var go in spawned) Destroy(go);

            var pass = result != null &&
                       result.OutcomeType == expectedType &&
                       result.ResultName == expectedName;

            Debug.Log(pass
                ? $"[Gym] PASS — {label}: {result}"
                : $"[Gym] FAIL — {label}: expected [{expectedType}] {expectedName}, got {result?.ToString() ?? "null"}");

            return pass ? 1 : 0;
        }
    }
}
