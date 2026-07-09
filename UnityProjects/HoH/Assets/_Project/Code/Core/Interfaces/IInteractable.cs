using UnityEngine;

namespace _Project.Code.Core.Interfaces
{
    /// <summary>
    ///     Implemented by any world-space object the player can interact with.
    ///     PlayerInteraction detects these via sphere overlap and calls Interact() on E press.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Short label shown in the interaction prompt UI (e.g. "Process", "Combine").</summary>
        string GetInteractionPrompt();

        /// <summary>Called by PlayerInteraction when the player presses the interact key.</summary>
        void Interact(GameObject interactor);
    }
}