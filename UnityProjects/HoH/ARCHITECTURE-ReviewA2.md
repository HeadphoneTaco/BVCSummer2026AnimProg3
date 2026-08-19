# Architecture Review: HoH Gameplay Systems

**Status:** Reviewed against DESIGN-A2Plan.md
**Date:** July 14, 2026 (A2 due July 22)
**Scope:** `_Project/Code/Gameplay/Systems`, `Chores/ChemistryWorkbench`, `Core`, `Utilities`, `Testing`

## Verdict

The architecture the plan called for is built and sound. All six build-order steps are in the code: the gym exists, `Evaluate` is implemented with the raw-ingredient rule checked first, the Observer chain is live with four listeners (Inventory, Cleaning, Quota, Gym), the inventory view subscribes instead of polling, and the quota win condition raises once. `CombinationLookup` and the auto-trigger pairing are gone as decided. Per the plan: **go find Sean — the event chain is running.**

One real architectural issue, a couple of judgment calls to document in the reflection, and that's it.

## The One Real Issue: `Instantiate` inside `Evaluate`

`ChemistrySystem.Evaluate` does two jobs: it *decides* the outcome (pure logic) and it *spawns the result item into the world* (side effect). Kitchen analogy: the recipe card shouldn't also plate the food.

Consequences today:

- Every `ChemistryGymRunner` pass spawns physical items into the gym scene — F5 a few times and the test scene fills with salads. The gym is supposed to prove logic, not litter.
- The event chain already exists for exactly this: a tiny `ResultSpawner` listening to `OnCombinationResolved` (payload would need the `resultItem` reference added to `OutcomeResult`) keeps `Evaluate` a pure decision.

**Recommendation:** move spawning to a listener before submission if time allows (~30 min); otherwise note it in the reflection as known debt. It's the same Observer move you already made three times.

## Judgment Calls Worth Defending in the Reflection

**Static events.** `OnCombinationResolved` and `OnIngredientProcessed` are `static` — global channels. With one workbench this is the simplest thing that works, and every listener correctly unsubscribes in `OnDisable`. The trade-off: a second workbench would feed the same quota and inventory, and static events survive scene loads. Fine at A2 scale; say so explicitly rather than letting it look unexamined.

**Linear rule scan.** Already well-documented in-code ("a dictionary lookup buys nothing but a class"). The multiset comparison in `CombinationRuleData.Matches` is correct — duplicates counted, order ignored. Keep that comment; it *is* the reflection paragraph.

**Ingredients consumed regardless of outcome.** `ConsumeStagedIngredients` runs on Fail too. That's a design stance (failure costs materials), not a bug — spec should state it.

**Destroy-vs-TriggerExit ordering.** Destroying staged ingredients fires `OnTriggerExit`, but the list is already cleared so `Remove` no-ops harmlessly. Works, but it works by accident of ordering — a one-line comment in `ConsumeStagedIngredients` would save future-you a debugging session.

## Minor (post-A2, don't touch now)

- `InventorySystem.GetBuildingInventoryText` puts text formatting (view concern) in the model. Return the list, let `InventoryDisplay` format.
- `OutcomeResult` carries only type + name. Adding the matched rule (or `resultItem`) makes the ResultSpawner refactor and richer UI possible later.
- `CleaningSystem` mess is a single bool — one mess at a time, no location. Matches the deferred-VFX scope; fine.

## Action Items

1. [ ] Show Sean the running event chain (plan's own gate)
2. [ ] Optional: extract `ResultSpawner` listener; keep `Evaluate` pure
3. [ ] Update spec: interaction layer, consume-on-fail rule, quota win condition (plan step 6)
4. [ ] Reflection: static events trade-off, linear scan vs lookup, workbench-owns-its-systems
5. [ ] Comment the destroy/trigger-exit ordering in `ConsumeStagedIngredients`
