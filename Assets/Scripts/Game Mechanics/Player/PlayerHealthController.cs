using Photon.Pun;
using Player.Spawn;
using UnityEngine;

namespace Player.Health
{
    public class PlayerHealthController: MonoBehaviourPunCallbacks
    {
        #region Fields

        [SerializeField] private int maxHealth = 0;
        [SerializeField] private ParticleSystem deathParticle;
        
        private int currentHealth = 0;
        private PlayerSpawner spawner;
        
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            currentHealth = maxHealth;
            spawner = GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();
        }


        #endregion

        #region Private Methods

        private void KillPlayer()
        {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
            
            spawner.SpawnPlayer();
        }

        #endregion

        #region Public Methods

        public void TakeDamage(int damageDealt)
        {
            if (photonView.IsMine)
            {
                if (currentHealth > damageDealt)
                {
                    currentHealth -= damageDealt;
                }
                else
                {
                    currentHealth = 0;
                    KillPlayer();
                }
            }
        }

        #endregion
    }
  

}