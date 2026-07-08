using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
    /// <summary>
    ///     Processing strategy for Solvent ingredients: Water.
    ///     Simulates boiling and filtering.
    /// </summary>
    public class SolventProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[SolventProcessingStrategy] Boiling / filtering...");
            target.Process();
        }
    }
}