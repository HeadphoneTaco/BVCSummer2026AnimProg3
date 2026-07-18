# A2 Reflection — DRAFT for Mike to edit

> **Mike:** this is a draft in your voice for you to rewrite where it doesn't sound like you.
> Everything factual in here actually happened — nothing is invented — but the judgement
> calls about what mattered most should be yours. Sean's Values rubric rewards disclosure
> "at the granularity at which it was used, including the prompts that did not produce
> useful output," so the failures section is deliberately specific. Cut, reword, reorder.

## My workflow this assignment

A1's feedback was blunt in the useful way: my spec described a game my build didn't run.
So A2 ran in the opposite direction — no new systems until the specified ones worked,
decisions locked in writing before any code, and a test scene (gym) standing before I
trusted anything. The order came from Sean's check-in and it held: chemistry gym first,
finish Evaluate, wire the event chain, then the displays and quota, updating the spec as
each step forced a decision.

The pattern that emerged is that I work like an editor. The AI drafts; I cut, correct,
and make the calls. Every design decision in the spec's revision log — delivery-based
quota, the drop-to-deliver shelf, keeping outcome variety in the recipes, even the F-key
order on the test spawners — was a choice I made between alternatives the AI laid out,
and several times against its default.

## Where the AI boundary sat

AI-authored: most system code (DeliveryShelf, the quota/inventory rewiring, the spawner
tooling), written from decisions already locked in the plan doc, and the first draft of
this spec revision.

Mine: all Unity-side work (scenes, prefabs, colliders, playtesting), all design calls,
and — this matters — most of the bug *discovery*. The AI never saw the game run. I found
the shelf not firing, the double-counted Saline, and the editor crash; the AI's value was
turning my symptom descriptions into causes: trigger messages route to the collider's
GameObject (why the shelf was deaf), Destroy deferring to end-of-frame while physics
ticks twice (why Saline counted twice), and a kinematic box fighting the character
controller's depenetration every frame (why carrying a cube crashed the editor).

## What didn't work

Honesty section. Three failures worth recording:

1. **The AI's memory rotted.** It confidently carried a wrong due date (July 22) from an
   earlier session note. The fix wasn't a better prompt — it was moving dates out of the
   AI's memory into a file in the repo (DEADLINES.md) that gets checked against the real
   clock. Same lesson as the spec itself: the artifact wins, recall loses.
2. **I let it write code that already existed.** Asked for a "bucket of ingredients," it
   built a custom ScriptableObject bucket — a worse copy of the CoreUtils AssetBuckets
   already embedded in the project. I caught it and we deleted the homemade version.
   The boundary lesson: the AI defaults to writing code; knowing what *not* to write is
   currently my job.
3. **Spec/data drift survived two artifacts agreeing.** The gym asserted Salad=Neutral
   while my authored data said Success — the gym and the data had drifted, and both
   looked fine in isolation. It was caught in review while updating the spec, which is
   Sean's point about lockstep made concrete: three things must agree (spec, data, tests),
   not two. An earlier version of me also let a triage/RequestSystem redesign start —
   scope creep dressed as architecture — and killing it was the right call.

## The trade-off I'd defend

Cutting CombinationLookup. The O(1) dictionary is objectively better at 35 recipes; at
three, it was a dead class costing understanding. The commitment: linear scan until
profiling — not intuition — says otherwise. Its strongest counter-case is that authoring
all 35 combinations makes the cut wrong; the revision log records exactly that tripwire.

## What A2 changed about how I work

The gym habit is the keeper. The crash, the double-count, and the deaf shelf were all
found *because* there was a scene whose only job was to make one system tell the truth.
And the two-event split (crafting vs. earning) is the first time a pattern decision came
from playtesting rather than from the pattern catalogue: carrying the item to the shelf
felt meaningless while quota counted at the bench, and the architecture followed the feel.
