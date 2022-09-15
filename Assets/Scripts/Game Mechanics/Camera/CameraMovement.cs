using UnityEngine;

namespace Cam.Movement
{
    public class CameraMovement: MonoBehaviour
    {
        #region Fields
        [SerializeField] private Transform followObject;
        [SerializeField] private bool invertMouseLook = false;
        
        private float xRotation = 0;
        private Vector2 mouseInput;
        private Vector3 distance;
        
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            distance = transform.position - followObject.position;
        }
        
        void LateUpdate()
        {
            RotateCamera();
            FollowObject();
        }

        #endregion

        #region Private Methods

        void FollowObject()
        {
            transform.position = followObject.position + distance;
        }
        
        void RotateCamera()
        {
            transform.rotation = followObject.rotation;
        }

        #endregion

        #region Public Methods

        

        #endregion
    }
  

}