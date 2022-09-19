using System;
using Gun.Settings;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player.Shoot
{
    public class PlayerShootController: MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField] private float shootingDelay = 0,muzzleCounter=0;
        [SerializeField] private GunSettings[] guns;
        
        private Camera cam;
        private float time = 0;
        private float flashRotationY = 0;
            
        private int gunIndex = 0;
        private GunSettings activeGun;
        
        float tempValue = 0;
        private bool isAutoFireEnabled = false;
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            cam = Camera.main;
            time = shootingDelay;
            tempValue = muzzleCounter;
            
            ChangeGun(0);
        }
        
        void Update()
        {
            DisableMuzzleFlash();
            
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isAutoFireEnabled)
                {
                    Shoot(); 
                }
            }
            
            if (Input.GetButton("Fire1"))
            {
                if (isAutoFireEnabled)
                {
                    time += Time.deltaTime;
                    if (time > shootingDelay)
                    { 
                        Shoot(); 
                        time = 0;
                    }
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                time = shootingDelay;
                activeGun.muzzleFlash.SetActive(false);
            }

            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
            {
                gunIndex++;
                if (gunIndex > guns.Length-1)
                {
                    gunIndex = 0;
                }

                ChangeGun(gunIndex);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
            {
                gunIndex--;
                if (gunIndex <0)
                {
                    gunIndex = guns.Length-1;
                }

                ChangeGun(gunIndex);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color=Color.red;
            if (cam != null)
            {
                Debug.DrawRay(cam.transform.position,CalculateShootingDirection().direction*10);
            }
        }

        #endregion

        #region Private Methods

        private void Shoot()
        {
            RaycastHit raycastHit;

            if (Physics.Raycast(CalculateShootingDirection(), out raycastHit,100))
            {
                CreateImpact(raycastHit.point,raycastHit.normal);
                activeGun.muzzleFlash.SetActive(true);
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

        void ChangeGun(int index)
        {
            if (activeGun != null)
            {
                activeGun.gameObject.SetActive(false);
            }

            activeGun = guns[index];
            shootingDelay = activeGun.shootingDelay;
            isAutoFireEnabled = activeGun.isAutoFireEnabled;
            
            activeGun.gameObject.SetActive(true);
        }

        void DisableMuzzleFlash()
        {
            if (activeGun.muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                
                if (muzzleCounter <= 0)
                {
                    activeGun.muzzleFlash.SetActive(false);
                    muzzleCounter = tempValue;
                }
            }
        }
        #endregion

        #region Public Methods

        

        #endregion
    }
  

}