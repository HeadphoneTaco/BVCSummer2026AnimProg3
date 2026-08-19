# Raw material for the A3 Part 4 reflection

Notes from the working session of Tue 2026-08-18, evening. Not a draft. These are the specifics worth drawing on, with the parts where the tool was wrong kept in, because those are the parts that show a boundary being enforced rather than described.

Part 4 asks for the development workflow and how AI was used against the specification, across refactor, optimization and tool work. The A2 feedback asked for the AI boundary to be reasoned from properties of the problem rather than stated as a policy. Most of what follows is usable for that.

---

## 1. The brief and the spec disagreed, and the spec lost

The A3 brief opens "The playable game is in place." It was not. The A2 feedback recorded that there was no lose state, no timer, no fail pressure and no restart, and that `IState.cs` had been carried unchanged since A1 with zero implementations.

The resolution was not to pick one document over the other. A generic state machine driving Ready / Playing / Won / Lost satisfies Part 1's "refactor so core systems are reusable" and closes the loop at the same time, so the two requirements turned out to be one piece of work seen from two angles. Worth saying explicitly in the reflection, because it is a spec-reading decision rather than a coding one.

Scope was cut twice during planning. Optimization dropped from a full section to a single measured item. A proposal to generalise the tool's validator to a second asset type was cut outright on the grounds that it chased a rubric descriptor without solving a problem.

## 2. Where the assistant was wrong, and how it got caught

Five errors in one session, all caught by checking rather than by intuition:

- **Planned around a constraint that did not exist.** An entire risk section was built on the belief that the lighting bake needed a booked school VM and took a long time. Both were false, from a stale note. The bake is about five minutes and the VM is available on demand. Correcting it moved a task from "half a day with an external dependency" to "an hour."
- **Recommended the wrong repository.** Proposed moving the animation assignment into the tutorial-follow repo, having read its Cinemachine version but not asked what the repo was for. It is reference material for learning, not a submission target.
- **Predicted `.git` would land at 680 MB. It landed at 468 MB.** Cause: added a working-tree figure (317 MB of Maya files on disk) to a packed-repository figure (366 MB), which are different units. Maya packed down to about 102 MB.
- **Asserted an expected LFS file count of 26 without ever running the count.** The real answer was 13. This was the exact failure mode the assistant had described an hour earlier: a checkpoint number is only worth anything if it came from an independent measurement, and this one came from memory.
- **Broke a tool's safety check by giving a safer-sounding instruction.** Removing the `origin` remote immediately after cloning meant nothing could accidentally push a rewrite over the original repo, which was the right instinct, but `git filter-repo` reads a missing origin as evidence the repo is not a fresh clone and refuses to run.

Useful framing: none of these were caught by reading the output and finding it plausible. They were caught because a number was stated in advance and then measured.

## 3. What the division of labour actually was

Every destructive command was run by hand, one at a time, after declining an offer of a single script that would do the whole job. The reason is legible in hindsight: a script that clones, filters, repacks and pushes produces one line of output at the end and gives you nothing to disagree with in the middle. Six separate commands produced five opportunities to notice something wrong, and four of them were taken.

The pattern that carried the session:

```bash
wc -l /tmp/big-exr.txt        # expect 68
```

A verification command with a predicted value attached stops being output and becomes a test. The prediction has to come from a different route than the thing it checks, or it is circular.

The strongest check of the session was not a number at all. Comparing `git ls-tree -r HEAD --long` between the original and rewritten repositories across the kept paths gave 1461 files, byte-for-byte identical. Checking against a known-good reference beat every absolute figure that got guessed at.

## 4. The technical finding worth keeping

The repository was 4.0 GB. Roughly 2 GB of that was raw `.exr` lightmap data, and about 1 GB was Maya scene revisions.

The lightmaps had been added to Git LFS partway through the term. LFS only affects commits made after it is configured; everything baked before that point sits raw in history permanently, and no amount of later configuration retrieves it. Around 1.65 GB of it was under a folder path that no longer exists in the current project at all, having been left behind by a scene reorganisation.

Two mechanisms that are easy to conflate:

- **Delta compression** stores version N of a file as a difference against version N-1. It fails completely on `.exr` and `.mb`, because re-saving an already-compressed binary format rearranges the bytes wholesale. Ten saves of a 67 MB Maya scene cost roughly ten times 67 MB.
- **zlib compression** operates within a single file and works fine on `.mb`, roughly three to one, because a Maya scene has a lot of internal repetition.

So "binary does not compress" is too broad. Binary does not *delta*. That distinction is why collapsing the file history worked while truncating the commit history would not have: cutting back to 2026-08-01 would have discarded 79 commits and still left 1.88 GB, because the large blobs were all recent.

Ended at 67 commits and under 500 MB, with the tip state provably identical to the original.

## 5. Threads to pick up when drafting

- Tie section 2 to the Part 2 optimization writeup. Both are the same claim: a stated expectation followed by a measurement is evidence, and a number on its own is not.
- The GameManager is being written by hand and committed before any assistance touches it. The commit ordering is the receipt, so reference the commit rather than asserting the practice.
- Section 4 is a real finding about the project's own history and is more interesting than a generic statement about tooling. It also explains a decision (why history was filtered rather than truncated) with a measurement behind it.
