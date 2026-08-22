using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.States
{
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
