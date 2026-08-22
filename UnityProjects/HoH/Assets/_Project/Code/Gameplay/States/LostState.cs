using _Project.Code.Core.Interfaces;
using _Project.Code.Gameplay.Systems;
using UnityEngine;

namespace _Project.Code.Gameplay.States
{
       public class LostState : IState
    {
        private readonly GameManager game;

        public LostState(GameManager game)
        {
            this.game = game;
        }

        public void Enter()
        {
            game.SetGameplayEnabled(false);
            Debug.Log("[GameManager] Lost. The day ended with the quota unmet.");
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
