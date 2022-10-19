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
        [SerializeField] private GameObject _roundEndPanel;
        [SerializeField] private GameObject scopeImage;
        [SerializeField] private GameObject crossHair;
        
        [SerializeField] private PlayerLeaderBoardData _playerLeaderBoardData;
        
        [SerializeField] private TextMeshProUGUI deathText;
        [SerializeField] private TextMeshProUGUI spawnTimeText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private TextMeshProUGUI timeText;
        
        #endregion

        #region Properties

        public GameObject leaderBoard => _leaderBoard;
        
        public GameObject roundEndPanel => _roundEndPanel;
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

        public void ControlSniperScope(bool val)
        {
            crossHair.SetActive(!val);
            scopeImage.SetActive(val);
        }

        public void SetTimeText(TimeSpan span)
        {
            timeText.text = span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00");
        }
        
        #endregion
    }
  

}