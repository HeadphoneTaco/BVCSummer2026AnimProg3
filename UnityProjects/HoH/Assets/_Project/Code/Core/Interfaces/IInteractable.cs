using UnityEngine;

namespace _Project.Code.Core.Interfaces
{
   
    public interface IInteractable
    {
               string GetInteractionPrompt();

              void Interact(GameObject interactor);
    }
}