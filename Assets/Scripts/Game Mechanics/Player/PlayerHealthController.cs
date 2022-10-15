using System.Threading.Tasks;
using DG.Tweening;
using Multiplayer.Match;
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

        private bool isGameEnded = false;
        
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

        private void KillPlayer(string shooterName,int actor)
        {
            PhotonNetwork.Instantiate(deathParticle.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);
            
            PlayerCanvasController.Instance.OpenDeathPanel(shooterName);
            ControlSpawningTime();
            
            MatchController.Instance.UpdateStatsSend(actor,0,1);
            MatchController.Instance.UpdateStatsSend(PhotonNetwork.LocalPlayer.ActorNumber,1,1);
        }

        private void ControlSpawningTime()
        {
            DOVirtual.Int(5, 1, 5, v => PlayerCanvasController.Instance.SetSpawnTimeText(v)).OnComplete(SpawnPlayer);
        }

        private async void SpawnPlayer()
        {
            PlayerCanvasController.Instance.CloseDeathPanel();
            spawner.SpawnPlayer();
        }

        #endregion

        #region Public Methods

        public void TakeDamage(int damageDealt,string shooterName,int actor)
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
                    KillPlayer(shooterName,actor);
                }
            }
        }

        #endregion
    }
  

}