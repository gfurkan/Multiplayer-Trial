using Photon.Pun;
using UnityEngine;

namespace Cam.Movement
{
    public class CameraMovement: MonoBehaviourPunCallbacks
    {
        #region Fields
       
        [SerializeField] private bool invertMouseLook = false;

        private Transform followObject;
        private float xRotation = 0;
        private Vector2 mouseInput;

        #endregion
        
        #region Properties

        
        #endregion

        #region Unity Methods

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            
        }
        
        void LateUpdate()
        {
            if (followObject != null)
            {
                RotateCamera();
                FollowObject();
            }
           
        }

        #endregion

        #region Private Methods

        void FollowObject()
        {
            transform.position = followObject.position;
        }
        
        void RotateCamera()
        {
            transform.rotation = followObject.rotation;
        }

        #endregion

        #region Public Methods

        public void GetViewPoint(Transform obj)
        {
            followObject = obj;
        }

        #endregion
    }
  

}