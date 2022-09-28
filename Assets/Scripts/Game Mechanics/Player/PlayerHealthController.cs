using DG.Tweening;
using Photon.Pun;
using Player.Canvas;
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
            
            if (photonView.IsMine)
            {
                currentHealth = maxHealth;
                PlayerCanvasController.Instance.SetHealthText(currentHealth);
                spawner = GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();
            }
           
        }


        #endregion

        #region Private Methods

        private void KillPlayer(string shooterName)
        {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
            
            PlayerCanvasController.Instance.OpenDeathPanel(shooterName);
            ControlSpawningTime();
        }

        private void ControlSpawningTime()
        {
            DOVirtual.Int(5, 1, 5, v => PlayerCanvasController.Instance.SetSpawnTimeText(v)).OnComplete(SpawnPlayer);
        }

        private void SpawnPlayer()
        {
            spawner.SpawnPlayer();
            PlayerCanvasController.Instance.CloseDeathPanel();
        }

        #endregion

        #region Public Methods

        public void TakeDamage(int damageDealt,string shooterName)
        {
            if (photonView.IsMine)
            {
                if (currentHealth > damageDealt)
                {
                    currentHealth -= damageDealt;
                    PlayerCanvasController.Instance.SetHealthText(currentHealth);
                }
                else
                {
                    currentHealth = 0;
                    PlayerCanvasController.Instance.SetHealthText(currentHealth);
                    KillPlayer(shooterName);
                }
            }
        }

        #endregion
    }
  

}