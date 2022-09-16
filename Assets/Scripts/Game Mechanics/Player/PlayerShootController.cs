using System;
using UnityEngine;

namespace Player.Shoot
{
    public class PlayerShootController: MonoBehaviour
    {
        #region Fields

        private Camera cam;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            cam = Camera.main;
        }
        
        void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }

        private void OnDrawGizmos()
        {
            Debug.DrawRay(cam.transform.position,CalculateShootingDirection().direction*10);
            Gizmos.color=Color.red;
        }

        #endregion

        #region Private Methods

        private void Shoot()
        {
            RaycastHit raycastHit;
            if (Physics.Raycast(CalculateShootingDirection(), out raycastHit,100))
            {
                print("hit");
            }
        }
        private Ray CalculateShootingDirection()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            ray.origin = cam.transform.position;
            return ray;
        }

        #endregion

        #region Public Methods

        

        #endregion
    }
  

}