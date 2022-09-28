using Managers.Spawn;
using Photon.Pun;
using UnityEngine;

namespace Player.Spawn
{
    public class PlayerSpawner: MonoBehaviour
    {
        #region Fields

        [SerializeField] private GameObject playerPrefab;

        private GameObject currentPlayer;
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                SpawnPlayer();
            }
        }
        #endregion

        #region Private Methods
        

        #endregion

        #region Public Methods

        public void SpawnPlayer()
        {
            Transform spawnPosition = SpawnManager.Instance.GetRandomSpawnPosition();
            currentPlayer=PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
        }

        #endregion
    }
  

}