using Managers.Spawn;
using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;
using Cam.Movement;
using Photon.Pun;
using Player.Animation;


namespace Player.Movement
{
    public class PlayerMovement: MonoBehaviourPunCallbacks
    {
        #region Fields

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 0;
        [SerializeField] private float runSpeed = 0;
        [SerializeField] private float jumpForce = 0;
        [SerializeField] private float gravityMultiplier = 0;
        [SerializeField] private float scopeRotationDivider = 0;
        
        [Header("Rotation")]
        [SerializeField] private float rotationSensitivity = 0;
        [SerializeField] private bool invertMouseLook = false;
        [SerializeField] private Transform viewPoint;
        
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        
        private Vector3 movementDirection,movementVector;
        private Vector2 mouseInput;
        
        private float movementSpeed = 0;
        private float xRotation = 0;

        private bool isJumped = false;
        private CharacterController characterController;
        private PlayerAnimationController animationController;

        private bool _isScoping = false;
        
        #endregion
        
        #region Properties

        public bool isScoping
        {
            get => _isScoping;
            set
            {
                _isScoping = value;
            }
        }
        
        #endregion

        #region Unity Methods

        void Start()
        {
            if (photonView.IsMine)
            {
                Camera.main.GetComponent<CameraMovement>().GetViewPoint(viewPoint);
                SetCursorPos(Screen.width/2,Screen.height);
                
                movementSpeed = walkSpeed;

                characterController = GetComponent<CharacterController>();
                animationController = GetComponent<PlayerAnimationController>();
            }

        }
        void Update()
        {
            if (photonView.IsMine)
            {
                MovePlayer();
                RotatePlayer();

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    movementSpeed = runSpeed;
                }
                else if (Input.GetKeyUp(KeyCode.LeftShift))
                {
                    movementSpeed = walkSpeed;
                }
            }
        }

        #endregion

        #region Private Methods
        
        void MovePlayer()
        {
            CalculateMovementValues();
            
            float YVelocity = movementVector.y;
            movementVector = ((transform.forward * movementDirection.z)+(transform.right*movementDirection.x)).normalized * movementSpeed;
            
            movementVector.y = YVelocity;

            if (characterController.isGrounded)
            {
                movementVector.y = 0;
                isJumped = false;
                animationController.ControlPlayerGroundedn(true);
            }
            
            if (Input.GetButtonDown("Jump"))
            {
                if (!isJumped)
                {
                    JumpPlayer();
                    isJumped = true; 
                    animationController.ControlPlayerGroundedn(false);
                }
            }
            
            movementVector.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            characterController.Move(movementVector * Time.deltaTime);

            animationController.SetRunAnimation(movementVector.magnitude);
        }
        
        void RotatePlayer()
        {
            CalculateMouseInput();
            
            transform.rotation=Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y+mouseInput.x,transform.rotation.eulerAngles.z);
            
            xRotation +=mouseInput.y;
            xRotation = Mathf.Clamp(xRotation, -60, 60);

            if (invertMouseLook)
            {
                viewPoint.rotation=Quaternion.Euler(-xRotation,viewPoint.rotation.eulerAngles.y,viewPoint.rotation.eulerAngles.z);
            }
            else
            {
                viewPoint.rotation=Quaternion.Euler(xRotation,viewPoint.rotation.eulerAngles.y,viewPoint.rotation.eulerAngles.z);
            }

        }

        void JumpPlayer()
        {
            movementVector.y = jumpForce;
        }
        void CalculateMouseInput()
        {
            float rotator = rotationSensitivity;
            
            if (isScoping)
            {
                rotator /= scopeRotationDivider;
            }
      
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))*rotator;
        }
        void CalculateMovementValues()
        {
            movementDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
        
        #endregion

        #region Public Methods

        

        #endregion
    }
  

}