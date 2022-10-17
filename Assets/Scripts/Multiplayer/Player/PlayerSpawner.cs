using Managers.Singleton;
using Managers.Spawn;
using Photon.Pun;
using UnityEngine;

namespace Player.Spawn
{
    public class PlayerSpawner: SingletonManager<PlayerSpawner>
    {
        #region Fields

        [SerializeField] private GameObject playerPrefab;

        private GameObject _currentPlayer;
        #endregion
        
        #region Properties

        public GameObject currentPlayer
        {
            get => _currentPlayer;
            set
            {
                _currentPlayer = value;
            }
        }
        
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
            if (_currentPlayer == null)
            {
                Transform spawnPosition = SpawnManager.Instance.GetRandomSpawnPosition();
                _currentPlayer=PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition.position, spawnPosition.rotation);
            }
        }

        #endregion
    }
  

}