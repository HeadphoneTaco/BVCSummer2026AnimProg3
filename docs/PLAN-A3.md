# A3 Plan: Programming 3 + Animation

Revised Tue 2026-08-18 evening after scope clarification. Both due Fri 2026-08-21 (verify the minute on D2L; notes say 11:50 PM, you say 11:55 PM). Target 9:00 PM Friday.

This version is deliberately lighter than the first draft. Optimization is demoted, Animation turned out to be much smaller than the brief implied, and the marking is lenient. What is left is the stuff that actually moves a mark or that you were told directly to do.

---

## The one thing that is not elastic

The Prog3 A3 brief opens with "The playable game is in place." It is not. From A2 feedback:

> There is no lose state anywhere in the project, no timer, no cap and no fail pressure of any kind, and there is no restart, since no script calls SceneManager. IState.cs is still an interface with no implementations.

And his closing line:

> For A3 the push is narrow. Build the loop by hand, close it so a run can start, be won, be lost and be restarted.

That is a direct instruction, and it also happens to satisfy Part 1's "refactor so core systems are reusable," because a state machine driver that knows nothing about chemistry is exactly that. Everything else on this page is negotiable. This is not.

---

## Programming 3 (GAME 2401)

### Must do

**1. GameManager + state machine.** Implement the dormant `IState` in `Core/Interfaces/IState.cs`. States: Ready, Playing, Won, Lost. Day timer ticked in Playing. Win when quota is met before it expires, lose when it does not. `SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)` to restart.

Write this by hand and commit it before any AI touches it. Not for purity, just because the commit order is the cheapest possible evidence for the Part 4 AI-boundary section, and it costs nothing to work in that order.

**2. Finite stock.** Delete the six `Decor_Spawner_*` objects from Main. Your own spec calls them test tooling excluded from game scenes, so their presence contradicts the document. Replace with a fixed manifest of ingredients delivered at day start. Without this the timer has nothing to squeeze and the loop is hollow.

**3. Entry document.** `UnityProjects/HoH/README.md`, required by Part 1's third bullet. What each reusable piece is for and how to extend it: state machine, event channel, `IProcessingStrategy`, `CombinationRuleData`, the tool. Put the controls in it too (mouse hold to carry, E to interact) and the three recipes, which also answers "no readme, no controls screen" from A2.

**4. Recipe authoring + validation tool** (Part 3). Editor window that lists `CombinationRuleData` assets, creates new ones from an `IngredientData` picker, and validates: duplicate rule sets (reuse the multiset `Matches` you already wrote), missing ingredient references, unset outcome enum. If it can also emit gym cases, good, but that is a bonus not a requirement.

Write a short `TOOL-SPEC.md` before the code and a `TOOL-README.md` after, with a couple of screenshots. The brief asks for adoption without reading source, and screenshots do most of that work.

Skipping the "generalise the validator to a second asset type" idea from the first draft. It was chasing a rubric line, not solving a problem.

**5. Spec revision + reflection** (Part 4). Continue the revision log from R8 in the same format that already scored well: what changed, alternative considered, trade-off accepted. Break the divergence summary into its own section since Part 1 now asks for it by name. Reflection covers refactor and tool.

### Worth doing, cheap

**6. Comment purge.** Delete comments describing things that do not exist: the Clean button, the UI, the `ScriptableObjects/GameData` folder. Two TODO markers still sitting there: `Ingredient.cs:48` and `CleaningSystem.cs:44`. He called this out under both Knowledge and Skills, and it is half an hour.

**7. Re-attach `ChemistryGymRunner`.** Attached to nothing since 2026-07-17 while spec section 6 still claims five cases run. Ten minutes to re-attach in `ChemistryGym.unity`.

**8. Split `ChemistrySystem.Evaluate`.** Evaluate returns an `OutcomeResult` and stops instantiating; a listener spawns the result. This is the fix your own A2 architecture review proposed and did not ship, so shipping it is a revision traceable to a document you wrote.

### Optional

**9. De-static the two event chains** so a second bench or shelf does not feed the same quota. Clean, but nothing in the submission has a second bench.

**10. Finish the input migration.** `activeInputHandler: 2` (Both) is still set and half your input never moved to the new system. Correct to fix, but it is the change most likely to silently break a build, so only if the rest is done and it is not Friday.

### Part 2: Optimization, reduced

You said this is not really needed, so it drops to one item, about thirty minutes:

`PlayerInteraction.OnGUI` runs `new GUIStyle(GUI.skin.box)` plus an interpolated string on every OnGUI pass, and OnGUI fires more than once per frame. That is a per-frame managed allocation for a text label. Its own comment already says "replace with UIToolkit in A2."

Reason to keep this one rather than zero: fixing it is the comment purge (item 6) and the optimization section at the same time, and it hands you one honest before/after GC number instead of a section that says nothing. Open the Profiler, note GC alloc per frame, fix it, note it again, write three sentences. That is a real measurement, cheaply.

Skip the draw call and batching work entirely.

---

## Animation (GAME 1307 A3)

### Do it in `FinalCharacterController`, not `AA3`

`FinalCharacterController` is clean, sits at "Part 7 final" with a branch per episode, and runs **Cinemachine 2.9.7**, which is the version the tutorial is written against. `PlayerAnimator.controller` already has the full parameter set and a Base/Upper layer split, the animation clips are imported, and Timeline 1.7.6 is installed.

`AA3` is a partial copy of this same tutorial dropped into a project running Cinemachine 3.1.7, which is why `ThirdPersonInput` is commented out, and it has a second Rigidbody controller stack tangled through it. Every blocker I listed for AA3 in the first draft was a symptom of that mismatch, and none of them exist here. Retire AA3.

### Actual scope
- Keep following the playlist: https://www.youtube.com/watch?v=SwWZ-pklT9I&list=PLYvjPIZvaz-o-DIBhiHzSrrau9HKSmeEz
- A short Timeline sequence authored in Unity
- The animator controller from the tutorial, which is largely built

### Environment
Export the Tech Art modular kit from HoH as a `.unitypackage` into `FinalCharacterController`. Rebake there, which is a few minutes. This keeps the Prog3 build frozen while you work on the animation side.

---

## Schedule

Loose on purpose. The video-following is the part you want time for, so it gets real hours rather than leftovers.

| When | Work |
|---|---|
| **Tue night** | Prog3 item 1: GameManager and state machine, by hand, committed |
| **Wed AM** | Map enlarge + rebake (~1h). Then Prog3 items 2, 7, 8 |
| **Wed PM** | Animation: export kit into FinalCharacterController, rebake, continue the playlist |
| **Thu AM** | Prog3 item 4: tool spec, then the tool |
| **Thu PM** | Animation: playlist + start the Timeline sequence |
| **Fri AM** | Prog3 items 3, 5, 6, and the one optimization. Build and play it. |
| **Fri PM** | Finish Timeline, record, export PDFs, zip both |
| **Fri 9:00 PM** | Submit both |

Items 9 and 10 go in only if Friday morning finishes early.

---

## Two things to keep an eye on

**Do not build the tool before the loop closes.** At A2 the tooling shipped and the game did not, and that was the headline of the feedback. Order matters more than either piece.

**The input migration is the one change that can quietly break a build.** If you do it, do it with a commit to revert to and play an actual build afterward, and not on Friday.

---

## Housekeeping, after the term

- One repo holding four courses, branch per course, Unity project two folders deep. He is right, and it is a next-term fix. Splitting it this week would destroy the commit history Part 2 of the brief asks you to show.
- `Programming3` branch is 0 ahead of master and 10+ behind. All the work is on master. Just know that before Friday night.
- `DEADLINES.md` still says A2 is upcoming and everything else is TBD.
