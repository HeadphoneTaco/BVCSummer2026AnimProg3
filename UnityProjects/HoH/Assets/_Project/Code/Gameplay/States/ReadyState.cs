using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.States
{
    /// <summary>
    ///     Before the day starts. The player is frozen and the clock is full.
    ///     Exists so a run has a defined beginning rather than starting the instant
    ///     the scene finishes loading, which is what made the old build impossible
    ///     to describe as a run at all.
    /// </summary>
    public class ReadyState : IState
    {
        private readonly GameManager game;

        public ReadyState(GameManager game)
        {
            this.game = game;
        }

        public void Enter()
        {
            game.SetGameplayEnabled(false);
            game.ResetDayTimer();
            Debug.Log("[GameManager] Ready. Waiting for the start key.");
        }

        public void Update()
        {
            if (game.StartPressed)
                game.GoToPlaying();
        }

        public void Exit()
        {
        }
    }
}
