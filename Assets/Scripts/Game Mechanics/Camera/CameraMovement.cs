using Multiplayer.Match;
using Photon.Pun;
using UnityEngine;

namespace Cam.Movement
{
    public class CameraMovement: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private Transform endScreenCameraPos;
        [SerializeField] private bool invertMouseLook = false;

        private Transform followObject;
        private float xRotation = 0;
        private Vector2 mouseInput;

        private GameStates currentState;
        private bool isGameEnded = false;
        
        #endregion
        
        #region Properties

        
        #endregion

        #region Unity Methods

        public override void OnEnable()
        {
            MatchController.onGameStateChanged += ControlCameraMovement;
        }

        public override void OnDisable()
        {
            MatchController.onGameStateChanged -= ControlCameraMovement;
        }

        void Start()
        { 
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        void LateUpdate()
        {
            if (followObject != null)
            {
                if (isGameEnded)
                {
                    RotateCamera(endScreenCameraPos);
                    FollowObject(endScreenCameraPos);
                }
                else
                {
                    RotateCamera(followObject);
                    FollowObject(followObject);
                }
            }
        }

        #endregion

        #region Private Methods

        private void ControlCameraMovement(GameStates state)
        {
            if (state == GameStates.End)
            {
                isGameEnded = true;
            }
            else
            {
                isGameEnded = false;
            }
        }
        void FollowObject(Transform objectToFollow)
        {
            transform.position = objectToFollow.position;
        }
        
        void RotateCamera(Transform objectToFollow)
        {
            transform.rotation = objectToFollow.rotation;
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