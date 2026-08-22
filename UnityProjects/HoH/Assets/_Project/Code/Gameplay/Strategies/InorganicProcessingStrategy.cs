using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
    public class InorganicProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[InorganicProcessingStrategy] Refining / grinding...");
            target.Process();
        }
    }
}