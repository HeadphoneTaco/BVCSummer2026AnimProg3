using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
    /// <summary>
    ///     Processing strategy for Organic ingredients: Fruit, Herbs, Oils.
    ///     Simulates washing, drying, or pressing depending on category.
    /// </summary>
    public class OrganicProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[OrganicProcessingStrategy] Washing / drying / pressing...");
            target.Process();
        }
    }
}