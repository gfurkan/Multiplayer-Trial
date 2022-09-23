using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Multiplayer.Match
{
    public class MatchController: MonoBehaviour
    {
        #region Fields

        

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(0);
            }
        }
        
        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        

        #endregion
    }
  

}