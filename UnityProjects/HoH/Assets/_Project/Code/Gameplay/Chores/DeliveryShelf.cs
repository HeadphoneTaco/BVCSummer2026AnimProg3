using System;
using UnityEngine;

namespace _Project.Code.Gameplay.Chores
{
 
    public class DeliveryShelf : MonoBehaviour
    {
      
        public static event Action<string> OnItemDelivered;

             private void OnTriggerEnter(Collider other)
        {
            TryDeliver(other);
        }

  
        private void OnTriggerStay(Collider other)
        {
            TryDeliver(other);
        }

        private static void TryDeliver(Collider col)
        {
            var body = col != null ? col.attachedRigidbody : null;
            if (body == null || body.isKinematic) return; 

            var item = body.GetComponent<Item>();
            if (item == null) return; 

                    if (!item.enabled) return; 
            item.enabled = false;

            Debug.Log($"[DeliveryShelf] Delivered: {item.ItemName}");
            OnItemDelivered?.Invoke(item.ItemName);
            Destroy(item.gameObject);
        }
    }
}
