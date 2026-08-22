using _Project.Code.Core.Interfaces;

namespace _Project.Code.Core
{
    public class StateMachine
    {
        public IState Current { get; private set; }

        public void ChangeState(IState next)
        {
            if (ReferenceEquals(Current, next)) return;

            Current?.Exit();
            Current = next;
            Current?.Enter();
        }

        public void Tick()
        {
            Current?.Update();
        }
    }
}
