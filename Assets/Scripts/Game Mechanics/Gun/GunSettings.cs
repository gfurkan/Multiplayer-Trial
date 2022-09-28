using UnityEngine;

namespace Gun.Settings
{
    public class GunSettings: MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _isAutoFireEnabled = false;
        [SerializeField] private float _shootingDelay = 0.15f;
        [SerializeField] private GameObject _muzzleFlash;
        [SerializeField] private int _damageValue = 0;
        
        #endregion
        
        #region Properties

        public bool isAutoFireEnabled => _isAutoFireEnabled;

        public float shootingDelay => _shootingDelay;

        public GameObject muzzleFlash => _muzzleFlash;
        
        public int damageValue => _damageValue;
        
        #endregion

        #region Unity Methods

        void Start()
        {
             
        }
        
        void Update()
        {
             
        }

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        

        #endregion
    }
  

}