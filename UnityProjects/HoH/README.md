# House of Healing

A chore game. You are the apprentice in a healing house: gather ingredients, process
them at the right station, combine them at the workbench into remedies, and deliver the remedies to
the shelf before the day runs out.

Unity 6000.3.15f1. Open `Assets/_Project/Scenes/Main.unity`.

---

## Controls

| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look |
| Left mouse, hold | Pick up and carry. Release to drop |
| E | Interact with the station or bench you are standing at |
| R | Start the day, from the Ready screen |
| T | Restart, from the win or lose screen |

Carried items are kinematic while held. Nothing counts as delivered until you release it
inside the shelf's trigger, the same way nothing is served until the plate is set down on the pass.

---

## The run

`GameManager` drives four states: Ready, Playing, Won, Lost.

- **Ready.** Player frozen, clock full. R starts the day.
- **Playing.** The day clock counts down. Delivering the quota wins. Reaching zero loses.
- **Won / Lost.** Player frozen, T reloads the scene.

The day length, the quota target and both key bindings are serialized fields, on `GameManager` and
`QuotaSystem`, so difficulty and controls are tuned in the Inspector rather than in code. The
on-screen banners read their key names from those same fields, so rebinding does not leave the
prompt telling the player the wrong key.

The cursor is locked and hidden while the day is running and released on the Ready, Won and Lost
screens. Escape releases it at any time in the editor.

---

## Recipes

Every ingredient must be processed at a station before it goes on the bench. A raw ingredient fails
the mix before any recipe is even looked up.

| Result | Ingredients | Outcome |
|---|---|---|
| Saline | Water + Powders | Success |
| Salad | Herbs + Oils | Neutral |
| LaxativeBob | Fruit + Gunpowder | Fail |

Ingredient order does not matter. `CombinationRuleData.Matches` compares as a multiset, so a rule
authored with two Waters and one Herbs matches only three ingredients containing exactly that,
in any order.

All three results are physical items and all three count toward the quota once delivered. A salad
is still something you made, even when it is not a remedy.

---

## The reusable pieces, and how to extend each one

### `Core/StateMachine.cs` and `Core/Interfaces/IState.cs`

The driver. `StateMachine` holds one `IState`, routes `Enter`, `Update` and `Exit`, and ignores a
transition into the state that is already current so a double-raised transition cannot run `Enter`
twice. Neither file references chemistry, quota, ingredients or UI.

**To extend:** write a class implementing `IState`, give it a reference to whatever context it needs,
and call `ChangeState`. To reuse the driver in another project, copy these two files and nothing else.
The four states in `Gameplay/States/` are the game-specific half and are meant to be replaced.

### `Gameplay/Systems/GameManager.cs`

The context the states operate on, and the only script in the project that touches scene loading.
It exposes the day clock, the quota check, the player freeze, and restart.

**To extend:** add a state class, add a `GoToX` method, and add its banner text to the `OnGUI`
switch. A pause state is roughly ten lines.

### Event channels

Systems talk through C# events, never through direct references to each other:

- `DeliveryShelf.OnItemDelivered(string)` is the payout signal. `InventorySystem` and `QuotaSystem`
  both listen. Neither knows the other exists.
- `ChemistrySystem.OnCombinationResolved(OutcomeResult)` announces a mix. `CleaningSystem` listens
  and creates a mess on a Neutral or Fail outcome.
- `QuotaSystem.OnQuotaChanged` and `OnQuotaReached` feed the display and the win check.

**To extend:** subscribe in `OnEnable`, unsubscribe in `OnDisable`. Every listener in the project
does this, and it is what keeps the scene reload on restart clean: a static event with subscribers
that never detach would carry dead references across the reload.

### `Core/Interfaces/IProcessingStrategy` and `Gameplay/Strategies/`

One strategy per ingredient category: organic, inorganic, solvent. `ProcessingStation` picks the
strategy that matches the ingredient rather than running a chain of type checks.

**To extend:** add a value to `IngredientCategory`, add a strategy class implementing
`IProcessingStrategy`, and assign it on the station. No existing file needs an edit.

### `Core/CombinationRuleData` and `Core/IngredientData`

Recipes and ingredients are ScriptableObject assets in `_Project/ScriptableObjects/`. Adding a
recipe is authoring an asset, not writing code.

**To extend:** right click in `ScriptableObjects/Recipes/` → Create → House of Healing → Combination
Rule. Fill in the ingredient array, the outcome type, the result name and the result item prefab,
then add the asset to the `combinationRules` array on `ChemistrySystem`.

### `Testing/ChemistryGymRunner`

A test harness scene, `_Project/Scenes/Test/ChemistryGym.unity`, that runs combination cases
without walking around the level.

**To extend:** add a case to the runner's list. It is the cheapest way to check a new recipe
resolves correctly before building the physical version.

---

## Known gaps

- The Ready, timer and end-of-run banners are `OnGUI`, not a Canvas. It needs no UI setup, which is
  what makes the loop droppable into any scene, but it is not the final presentation layer.
- The recipe authoring and validation tool specified for this assignment is not built. Recipes are
  currently authored directly in the Inspector, and nothing checks for a duplicate rule set or an
  unassigned ingredient reference before runtime.
- Ingredient supply is not yet finite. Without a fixed delivery at day start, the clock is the only
  pressure in the run.
