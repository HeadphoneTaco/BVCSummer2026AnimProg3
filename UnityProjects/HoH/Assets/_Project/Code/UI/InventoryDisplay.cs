using _Project.Code.Gameplay.Systems;
using TMPro;
using UnityEngine;

namespace _Project.Code.UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private InventorySystem inventorySystem;
        [SerializeField] private TMP_Text inventoryText;

        private void OnEnable()
        {
            if (inventorySystem != null)
                inventorySystem.OnInventoryChanged += Refresh;

            Refresh();
        }

        private void OnDisable()
        {
            if (inventorySystem != null)
                inventorySystem.OnInventoryChanged -= Refresh;
        }

        private void Refresh()
        {
            if (inventoryText == null || inventorySystem == null) return;
            inventoryText.text = $"Remedies\n{inventorySystem.GetBuildingInventoryText()}";
        }
    }
}
