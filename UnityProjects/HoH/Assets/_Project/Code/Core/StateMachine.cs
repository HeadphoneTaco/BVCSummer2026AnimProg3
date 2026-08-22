using _Project.Code.Core.Interfaces;

namespace _Project.Code.Core
{
    /// <summary>
    ///     Minimal state machine driver. It holds the current IState, routes Enter,
    ///     Update and Exit, and knows nothing about what the states are for.
    ///     No reference to chemistry, quota, ingredients or UI appears anywhere in
    ///     this file, which is what makes it reusable: dropping it into a different
    ///     project only requires new IState implementations, not an edit here.
    /// </summary>
    public class StateMachine
    {
        public IState Current { get; private set; }

        /// <summary>
        ///     Exits the current state and enters the next one. Re-entering the state
        ///     that is already current is treated as a no-op, so a transition raised
        ///     twice in one frame cannot run Enter twice.
        /// </summary>
        public void ChangeState(IState next)
        {
            if (ReferenceEquals(Current, next)) return;

            Current?.Exit();
            Current = next;
            Current?.Enter();
        }

        /// <summary>Called once per frame by whoever owns the machine.</summary>
        public void Tick()
        {
            Current?.Update();
        }
    }
}
