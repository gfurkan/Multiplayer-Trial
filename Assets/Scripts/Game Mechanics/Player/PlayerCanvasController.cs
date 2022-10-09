using System;
using Managers.Singleton;
using Player.LeaderBoard;
using TMPro;
using UnityEngine;

namespace Player.Canvas
{
    public class PlayerCanvasController: SingletonManager<PlayerCanvasController>
    {
        #region Fields

        [SerializeField] private GameObject deathPanel;
        [SerializeField] private GameObject _leaderBoard;
        [SerializeField] private GameObject roundEndPanel;
        
        [SerializeField] private PlayerLeaderBoardData _playerLeaderBoardData;
        
        [SerializeField] private TextMeshProUGUI deathText;
        [SerializeField] private TextMeshProUGUI spawnTimeText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI deathsText;
        
        #endregion

        #region Properties

        public GameObject leaderBoard => _leaderBoard;
        public PlayerLeaderBoardData playerLeaderBoardData => _playerLeaderBoardData;

        #endregion
        
        #region Unity Methods

        private void Start()
        {
            SetKillsText(0);
            SetDeathsText(0);
        }

        #endregion
        
        #region Public Methods

        public void OpenDeathPanel(string killerName)
        {
            deathPanel.SetActive(true);
            deathText.text = "You were kılled by " + killerName;
        }

        public void SetSpawnTimeText(int time)
        {
            spawnTimeText.text="Respawnıng ın " + time;
        }

        public void CloseDeathPanel()
        {
            deathPanel.SetActive(false);
            deathText.text = "";
            spawnTimeText.text = "";
        }

        public void SetHealthText(int value)
        {
            healthText.text = value.ToString();
        }

        public void SetKillsText(int value)
        {
            killsText.text = "KILLS " + value;
        }
        
        public void SetDeathsText(int value)
        {
            deathsText.text = "DEATHS " + value;
        }
        #endregion
    }
  

}