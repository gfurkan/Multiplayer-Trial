using Player.Canvas;
using UnityEngine;

namespace Player.Shoot.Sniper
{
    public class SniperScopeController: MonoBehaviour
    {

        #region Fields

        [SerializeField] private PlayerShootController shootController;

        #endregion
        
        #region Public Methods

        
        public void OpenSniperScope()
        {
            PlayerCanvasController.Instance.ControlSniperScope(true);
            shootController.ChangeFovOfCamera(true);
        }

        #endregion
    }
  

}