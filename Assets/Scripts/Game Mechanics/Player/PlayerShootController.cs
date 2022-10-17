using System.Threading.Tasks;
using Gun.Settings;
using Photon.Pun;
using Player.Canvas;
using Player.Health;
using Player.Movement;
using UnityEngine;

namespace Player.Shoot
{
    public class PlayerShootController: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private GunSettings[] guns;
        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField] private GameObject playerImpact;
        [SerializeField] private Transform gunHolderInHand, gunHolderInPlayer;
        [SerializeField] private float shootingDelay = 0,muzzleCounter=0,scopedFOV=0;
        
        private float normalFOV = 0;
        private GameObject weaponCam;
        private Animator sniperAnimator;
        private Camera cam;
        private float time = 0;
        private float flashRotationY = 0;

        private int damageToDeal = 0;
        private int gunIndex = 0;
        private bool isAutoFireEnabled = false;
        private float tempValue = 0;
        
        private GunSettings activeGun;
        private PlayerHealthController healthController;
        private PlayerMovement playerMovement;
        
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {

            cam = Camera.main;
            
            time = shootingDelay;
            tempValue = muzzleCounter;
            
            photonView.RPC("SetGun",RpcTarget.All,gunIndex);
            healthController = GetComponent<PlayerHealthController>();
            playerMovement=GetComponent<PlayerMovement>();
            
            if (photonView.IsMine)
            {
                weaponCam = cam.transform.GetChild(0).gameObject;
                normalFOV = cam.fieldOfView;
                sniperAnimator = guns[2].GetComponent<Animator>();
            }
            
            SetGunPosition();
        }
         
        void Update()
        {
            if (photonView.IsMine)
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

                if (gunIndex == 2)
                {
                    if (Input.GetButtonDown("Fire2"))
                    {
                        sniperAnimator.SetBool("Aim", true);
                    }
                
                    if (Input.GetButtonUp("Fire2"))
                    {
                        sniperAnimator.SetBool("Aim", false);
                        PlayerCanvasController.Instance.ControlSniperScope(false);
                        ChangeFovOfCamera(false);
                    }
                }
            
                
                if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
                {
                    gunIndex++;
                    if (gunIndex > guns.Length-1)
                    {
                        gunIndex = 0;
                    }

                    photonView.RPC("SetGun",RpcTarget.All,gunIndex);
                }
                else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
                {
                    gunIndex--;
                    if (gunIndex <0)
                    {
                        gunIndex = guns.Length-1;
                    }

                    photonView.RPC("SetGun",RpcTarget.All,gunIndex);
                }
                ChangeGunWithNumberKeys();  
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
                if (raycastHit.transform.CompareTag("Player"))
                {
                    PhotonNetwork.Instantiate(playerImpact.name, raycastHit.point, Quaternion.identity);
                    raycastHit.collider.GetComponent<PhotonView>().RPC("DealDamage",RpcTarget.All,damageToDeal,photonView.Owner.NickName,PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else
                {
                    CreateImpact(raycastHit.point,raycastHit.normal);
                    photonView.RPC("ControlMuzzleFlash",RpcTarget.All,true);
                }
                
            }
        }
        private Ray CalculateShootingDirection()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            ray.origin = cam.transform.position;
            return ray;
        }

        private async void CreateImpact(Vector3 hitPosition,Vector3 hitNormal)
        {
            GameObject bulletImpact=PhotonNetwork.Instantiate(bulletImpactPrefab.name, hitPosition+(hitNormal*0.002f), Quaternion.LookRotation(hitNormal, Vector3.up));
            await Task.Delay(2 * 1000);
            PhotonNetwork.Destroy(bulletImpact);
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
            
            SetDamageToDeal(activeGun);
            activeGun.gameObject.SetActive(true);
        }

        [PunRPC]
        private void SetGun(int index)
        {
            if (index < guns.Length)
            {
                ChangeGun(index);
            }
        }

        [PunRPC]
        private void ControlMuzzleFlash(bool val)
        {
            activeGun.muzzleFlash.SetActive(val);
        }
        void DisableMuzzleFlash()
        {
            if (activeGun.muzzleFlash.activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                
                if (muzzleCounter <= 0)
                {
                    photonView.RPC("ControlMuzzleFlash",RpcTarget.All,false);
                    muzzleCounter = tempValue;
                }
            }
        }

        void ChangeGunWithNumberKeys()
        {
            if (Input.GetKeyDown("1"))
            {
                if (gunIndex != 0)
                {
                    gunIndex = 0;
                    photonView.RPC("SetGun",RpcTarget.All,gunIndex);
                }
            }
            if (Input.GetKeyDown("2"))
            {
                if (gunIndex != 1)
                {
                    gunIndex = 1;
                    photonView.RPC("SetGun",RpcTarget.All,gunIndex);
                }
            }
            if (Input.GetKeyDown("3"))
            {
                if (gunIndex != 2)
                {
                    gunIndex = 2;
                    photonView.RPC("SetGun",RpcTarget.All,gunIndex);
                }
            }
        }

        [PunRPC]
        void DealDamage(int damageValue,string name,int actor)
        {
            healthController.TakeDamage(damageValue,name,actor);
        }

        private void SetDamageToDeal(GunSettings settings)
        {
            damageToDeal=settings.damageValue;
        }

        private void SetGunPosition()
        {
            if (!photonView.IsMine)
            {
                gunHolderInPlayer.parent = gunHolderInHand;
                gunHolderInPlayer.transform.localPosition = Vector3.zero;
                gunHolderInPlayer.transform.localRotation=Quaternion.identity;
            }
        }
        #endregion

        #region Public Methods

        public void ChangeFovOfCamera(bool isAiming)
        {
            if (isAiming)
            {
                weaponCam.gameObject.SetActive(false);
                cam.fieldOfView = scopedFOV;
                playerMovement.isScoping = true;
            }
            else
            {
                weaponCam.gameObject.SetActive(true);
                cam.fieldOfView = normalFOV;
                playerMovement.isScoping = false;
            }
        }

        #endregion
    }
  

}