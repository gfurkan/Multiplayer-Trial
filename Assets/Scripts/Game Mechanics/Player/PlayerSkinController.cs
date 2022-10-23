using Photon.Pun;
using UnityEngine;

namespace Player.Skin
{
    public class PlayerSkinController: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private SkinnedMeshRenderer renderer;
        [SerializeField] private Material[] skins;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            renderer.material = skins[photonView.Owner.ActorNumber % skins.Length];
        }

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        

        #endregion
    }
  

}