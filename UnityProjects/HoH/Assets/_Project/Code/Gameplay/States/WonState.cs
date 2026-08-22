using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.States
{
    public class WonState : IState
    {
        private readonly GameManager game;

        public WonState(GameManager game)
        {
            this.game = game;
        }

        public void Enter()
        {
            game.SetGameplayEnabled(false);
            Debug.Log("[GameManager] Won. Quota filled before the day ended.");
        }

        public void Update()
        {
            if (game.RestartPressed)
                game.Restart();
        }

        public void Exit()
        {
        }
    }
}
