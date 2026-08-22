using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.States
{
    /// <summary>
    ///     The run itself. Ticks the day clock and holds both exits: quota met is a
    ///     win, clock expired is a loss. The quota is checked first, so filling it on
    ///     the final frame wins rather than draws.
    /// </summary>
    public class PlayingState : IState
    {
        private readonly GameManager game;

        public PlayingState(GameManager game)
        {
            this.game = game;
        }

        public void Enter()
        {
            game.SetGameplayEnabled(true);
            Debug.Log($"[GameManager] Day started. {Mathf.CeilToInt(game.TimeRemaining)}s on the clock.");
        }

        public void Update()
        {
            game.TickDayTimer(Time.deltaTime);

            if (game.QuotaReached)
            {
                game.GoToWon();
                return;
            }

            if (game.TimeRemaining <= 0f)
                game.GoToLost();
        }

        public void Exit()
        {
        }
    }
}
