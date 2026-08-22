using _Project.Code.Core.Interfaces;
using UnityEngine;

namespace _Project.Code.Gameplay.Strategies
{
 
    public class OrganicProcessingStrategy : IProcessingStrategy
    {
        public void Execute(IProcessable target)
        {
            Debug.Log("[OrganicProcessingStrategy] Washing / drying / pressing...");
            target.Process();
        }
    }
}