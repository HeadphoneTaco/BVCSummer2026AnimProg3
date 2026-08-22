using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
    public class SolventProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[SolventProcessingStrategy] Boiling / filtering...");
            target.Process();
        }
    }
}