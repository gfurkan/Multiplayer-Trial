using System;
using Photon.Pun;
using UnityEngine;

namespace Player.Animation
{
    public class PlayerAnimationController: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private Animator animator;
        [SerializeField] private GameObject playerModel;
        
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        private void Start()
        {
            DisableModelForPlayer();
        }

        #endregion

        #region Private Methods

        private void DisableModelForPlayer()
        {
            if (photonView.IsMine)
            {
                playerModel.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        public void ControlPlayerGroundedn(bool value)
        {
            animator.SetBool("grounded",value);
        }

        public void SetRunAnimation(float speed)
        {
            animator.SetFloat("speed",speed);
        }
        #endregion
    }
  

}