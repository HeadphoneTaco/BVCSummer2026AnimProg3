# House of Healing — A2 Plan

**Course:** GAME 2401 · **Due:** July 22, 2026 · **Source:** Sean's A1 feedback + July 2 check-in

**Guiding rule (Sean's words):** the aim is not more systems — it is the game already specified, running and proven. Nothing in this plan adds a system. Everything closes the gap between spec and build.

## Decisions Made (spec must be updated to match)

1. **Which game:** the 3D player game. The player/camera/pickup code works; the spec's "no player character" clause gets removed and the interaction layer written into the spec as a real system.
2. **Which chemistry design:** the staged workbench (`ChemistryWorkbench` — player places 2–3 processed ingredients, interacts to combine). The auto-trigger bench logic in `ChemistrySystem.OnTriggerEnter` gets **deleted**. Rationale: combining should be done *by* the player, not *to* the player, and the workbench is the design the spec's `Evaluate` path describes.
3. **CombinationLookup:** **cut.** With a handful of recipes, `Evaluate` scans the rule array directly. One less dead class; the trade-off (O(n) scan vs O(1) dictionary at this scale) goes in the reflection.
4. **Win/lose:** simple quota. The building needs N remedies delivered to the shelf to end the day. Known recipes, clear goal, finishable. Discovery-mode (hidden recipes, experiment at your own risk) is noted in the spec as a post-A2 variant, not built.

## Current Wiring Gaps (from check-in, verified in code)

- `ChemistrySystem.Evaluate` throws `NotImplementedException`; the trigger-based auto-pairing runs instead
- `ChemistryWorkbench.Interact` line 54: the `Evaluate` call is commented out
- `OnCombinationResolved` event does not exist; subscriptions in `InventorySystem` and `CleaningSystem` are commented out (handlers already written)
- `CleaningSystem` never receives a mess; `InventorySystem` building inventory never receives a remedy
- One Playground scene; no per-system test scenes ("gyms")

## Build Order (Sean's order, kept)

1. **Chemistry gym.** New scene: a few ingredients, the three recipes, nothing else. Run it before and after every step below. This is the habit A2 is partly grading.
2. **Finish `Evaluate`.** Takes the staged list; raw-ingredient block checked *first* (the spec's central rule); match against `CombinationRuleData` array; return/raise `OutcomeResult` with Success / Neutral / Fail. Delete `OnTriggerEnter` pairing and `CombinationLookup`.
3. **Wire the event chain.** Add `ChemistrySystem.OnCombinationResolved`, uncomment both subscriptions, uncomment the workbench's `Evaluate` call. Success → inventory; Neutral/Fail → mess. The Observer chain Sean called "the part that holds the rest together."
4. **Inventory display.** Simple world-space or screen UI reading from `InventorySystem` — view separate from data (the MVC hook from the check-in table).
5. **Quota win condition.** Counter + end-of-day state when N remedies delivered. Smallest possible implementation.
6. **Spec + reflection as you go.** Update the spec at each step, not at the end; commit per step so the history shows iteration (an explicit A2 deliverable).

Then: find Sean — "come find me once the event chain is running."

## Deferred (per check-in: Later)

Undoable actions (Command), pause, menus, tutorial, settings, audio, save/load, outcome VFX beyond debug text.
