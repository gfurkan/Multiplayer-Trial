using System;
using System.Threading.Tasks;
using DG.Tweening;
using Gun.Settings;
using Multiplayer.Match;
using Photon.Pun;
using Player.Canvas;
using Player.Health;
using Player.Movement;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Player.Shoot
{
    public class PlayerShootController: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private GunSettings[] guns;
        [SerializeField] private GameObject bulletImpactPrefab;
        [SerializeField] private GameObject playerImpact;
        [SerializeField] private Transform gunHolderInHand, gunHolderInPlayer;
        [SerializeField] private float shootingDelay = 0,muzzleCounter=0,sniperScopedFOV=0,normalGunScopedFOV=0;
        [SerializeField] private float normalGunScopeSpeed = 0;
        
        private float normalFOV = 0;
        private GameObject weaponCam;
        private Animator sniperAnimator;
        private Camera cam;
        private float time = 0;
        private float flashRotationY = 0;

        private int damageToDeal = 0;
        private int gunIndex = 0;
        private bool isAutoFireEnabled = false;
        private bool isScoped = false;
        public bool isShootingDelayFinished = true;
        private bool isCalculatingDelay = false;
        
        private float tempValue = 0;
        
        private GunSettings activeGun;
        private PlayerHealthController healthController;
        private PlayerMovement playerMovement;

        private Sequence scopeZoomIn, scopeZoomOut;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        public override void OnEnable()
        {
            MatchController.onGameStateChanged += ControlAimingWhenGameEnded;
        }

        public override void OnDisable()
        {
            MatchController.onGameStateChanged -= ControlAimingWhenGameEnded;
        }

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
                normalFOV = cam.fieldOfView;
                sniperAnimator = guns[2].GetComponent<Animator>();
            }
            
            SetGunPosition();
        }
         
        void Update()
        {
            if (photonView.IsMine)
            {
                if (!PlayerCanvasController.Instance.isPaused)
                {
                    DisableMuzzleFlash();
                    ControlShooting(); 
                    ControlScoping();
                    ControlGunChanging();

                    if (!isShootingDelayFinished && isCalculatingDelay)
                    {
                        ControlShootingDelay();
                    }
                }
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
                photonView.RPC("PlayGunSound",RpcTarget.All);
                
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

        private void ControlShooting()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isAutoFireEnabled)
                {
                    if (isShootingDelayFinished)
                    {
                        Shoot();
                        isShootingDelayFinished = false;
                    }
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
                if (!isShootingDelayFinished && !isCalculatingDelay)
                {
                    time = 0;
                    activeGun.muzzleFlash.SetActive(false);
                    isCalculatingDelay = true;
                }
            }

        }

        private void ControlShootingDelay()
        {
            time += Time.deltaTime;

            if (time > shootingDelay)
            {
                isShootingDelayFinished = true;
                time = 0;
            }
        }
        private void ControlScoping()
        {
            if (gunIndex == 2)
            {
                if (Input.GetButton("Fire2"))
                {
                    sniperAnimator.SetBool("Aim", true);
                }
                
                else
                {
                    sniperAnimator.SetBool("Aim", false);
                    PlayerCanvasController.Instance.ControlSniperScope(false);
                    ChangeFovOfCameraForSniper(false);
                }
            }

            else
            {
                if (Input.GetButton("Fire2"))
                {
                    if (!isScoped)
                    {
                        ChangeFovOfCameraForOthers(true);
                    }
                }
                
                else
                {
                    if (isScoped)
                    {
                        ChangeFovOfCameraForOthers(false);
                    }
                }
            }
        }

        private void ControlGunChanging()
        {
             
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
        private Ray CalculateShootingDirection()
        {
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            ray.origin = cam.transform.position;
            return ray;
        }

        private void CreateImpact(Vector3 hitPosition,Vector3 hitNormal)
        { 
            PhotonNetwork.Instantiate(bulletImpactPrefab.name, hitPosition+(hitNormal*0.002f), Quaternion.LookRotation(hitNormal, Vector3.up));
        }

        void ChangeGun(int index)
        {
            if (activeGun != null)
            {
                activeGun.gameObject.SetActive(false);
            }

            activeGun = guns[index];
            shootingDelay = activeGun.shootingDelay;
            time = shootingDelay;
          
            isCalculatingDelay = false;
            isShootingDelayFinished = true;
            isAutoFireEnabled = activeGun.isAutoFireEnabled;
            
            SetDamageToDeal(activeGun);
            activeGun.gameObject.SetActive(true);
            activeGun.ControlRenderer(true);
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

        [PunRPC]
        private void PlayGunSound()
        {
            activeGun.StopGunSound();
            activeGun.PlayGunSound();
        }

        private void ControlAimingWhenGameEnded(GameStates state)
        {
            if (state == GameStates.End)
            {
                PlayerCanvasController.Instance.CloseDeathPanel();
                if (gunIndex == 2)
                {
                    sniperAnimator.SetBool("Aim", false);
                    PlayerCanvasController.Instance.ControlSniperScope(false);
                    ChangeFovOfCameraForSniper(false);
                }
                else
                {
                    ChangeFovOfCameraForOthers(false);
                }
            }
        }
        #endregion

        #region Public Methods

        public void ChangeFovOfCameraForSniper(bool isAiming)
        {
            if (isAiming)
            {
               // weaponCam.gameObject.SetActive(false);
               activeGun.ControlRenderer(false);
                cam.fieldOfView = sniperScopedFOV;
                playerMovement.ControlScopeMovement(true);
            }
            else
            {
              //  weaponCam.gameObject.SetActive(true);
              activeGun.ControlRenderer(true);
              cam.fieldOfView = normalFOV;
              playerMovement.ControlScopeMovement(false);
            }
        }

        public void ChangeFovOfCameraForOthers(bool isAiming)
        {
            DOTween.PauseAll();
            
            if (isAiming && !isScoped)
            {
                isScoped = true;
                playerMovement.ControlScopeMovement(true);
                scopeZoomIn = DOTween.Sequence();
                scopeZoomIn.Append(DOVirtual.Float(cam.fieldOfView, normalGunScopedFOV, normalGunScopeSpeed, v => cam.fieldOfView = v));
            }
            else if(!isAiming && isScoped)
            {
                isScoped = false;
                playerMovement.ControlScopeMovement(false);
                scopeZoomOut = DOTween.Sequence();
                scopeZoomOut.Append(DOVirtual.Float(cam.fieldOfView, normalFOV, normalGunScopeSpeed, v => cam.fieldOfView = v));

            }
        }
        #endregion
    }
  

}