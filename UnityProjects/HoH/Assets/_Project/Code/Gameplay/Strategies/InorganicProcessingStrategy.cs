using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
    /// <summary>
    ///     Processing strategy for Inorganic ingredients: Saltpeter, Crystals.
    ///     Simulates refinement or grinding.
    /// </summary>
    public class InorganicProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[InorganicProcessingStrategy] Refining / grinding...");
            target.Process();
        }
    }
}