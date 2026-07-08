using UnityEngine;

namespace _Project.Code.Gameplay.Player
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField]private Transform target;
    
        //   [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float distance;
        [SerializeField] private float minAngle = -45;
        [SerializeField] private float maxAngle = 90;
        [SerializeField] private float sensitivity = 3f;
    
        [SerializeField] private float smoothing = 0.12f;
    
        private float rotationX;
        private float rotationY;

        public Vector3 currentRotation;
        private Vector3 rotationVelocity;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            Vector3 angles = transform.eulerAngles;
            rotationX = angles.y;
            rotationY = angles.x;
        }

        private void LateUpdate()
        {
            if(target == null)  return; 
        
            rotationX += Input.GetAxis("Mouse Y") * sensitivity;
            rotationY += Input.GetAxis("Mouse X") * sensitivity;
        
            rotationX = Mathf.Clamp(rotationX, minAngle, maxAngle);
        
            currentRotation = Vector3.SmoothDamp(currentRotation, new  Vector3(rotationX, rotationY, 0), ref rotationVelocity, smoothing);
            transform.eulerAngles = currentRotation;
            transform.position = target.position - (transform.forward * distance);
        }
    }
}
