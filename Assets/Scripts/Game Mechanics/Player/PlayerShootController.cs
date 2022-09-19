using System;
using UnityEngine;

namespace Player.Shoot
{
    public class PlayerShootController: MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField] private float shootingDelay = 0;
        
        private Camera cam;
        private float time = 0;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            cam = Camera.main;
            time = shootingDelay;
        }
        
        void Update()
        {
            if (Input.GetButton("Fire1"))
            { 
                time += Time.deltaTime;
                
                if (time > shootingDelay)
                { 
                    Shoot(); 
                    time = 0;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                time = shootingDelay;
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
                CreateImpact(raycastHit.point,raycastHit.normal);
            }
        }
        private Ray CalculateShootingDirection()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            ray.origin = cam.transform.position;
            return ray;
        }

        private void CreateImpact(Vector3 hitPosition,Vector3 hitNormal)
        {
            var bulletImpact=Instantiate(bulletImpactPrefab, hitPosition+(hitNormal*0.002f), Quaternion.LookRotation(hitNormal, Vector3.up));
            Destroy(bulletImpact,2);
        }
        #endregion

        #region Public Methods

        

        #endregion
    }
  

}