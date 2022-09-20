using Managers.Spawn;
using TMPro;
using UnityEngine;
using System.Runtime.InteropServices;


namespace Player.Movement
{
    public class PlayerMovement: MonoBehaviour
    {
        #region Fields

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 0;
        [SerializeField] private float runSpeed = 0;
        [SerializeField] private float jumpForce = 0;
        [SerializeField] private float gravityMultiplier = 0;
        
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
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            SpawnPlayer();
            SetCursorPos(Screen.width/2,Screen.height);
            movementSpeed = walkSpeed;
            Cursor.lockState = CursorLockMode.Locked;
            characterController = GetComponent<CharacterController>();
        }
        void Update()
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
            }
            
            if (Input.GetButtonDown("Jump"))
            {
                if (!isJumped)
                {
                    JumpPlayer();
                    isJumped = true;
                }
            }
            
            movementVector.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
            characterController.Move(movementVector * Time.deltaTime);
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
            mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"))*rotationSensitivity;
        }
        void CalculateMovementValues()
        {
            movementDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        void SpawnPlayer()
        {
            Transform spawnTransform = SpawnManager.Instance.GetRandomSpawnPosition();
            
            transform.position = spawnTransform.position;
            transform.rotation = spawnTransform.rotation;
        }
        #endregion

        #region Public Methods

        

        #endregion
    }
  

}