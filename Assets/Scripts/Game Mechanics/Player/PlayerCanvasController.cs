using System;
using Managers.Singleton;
using Multiplayer.Match;
using Photon.Pun;
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
        [SerializeField] private GameObject pausePanel;
        
        [SerializeField] private PlayerLeaderBoardData _playerLeaderBoardData;
        
        [SerializeField] private TextMeshProUGUI deathText;
        [SerializeField] private TextMeshProUGUI spawnTimeText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI killsText;
        [SerializeField] private TextMeshProUGUI deathsText;
        [SerializeField] private TextMeshProUGUI timeText;

        private bool isTimerActive = true;
        private bool _isPaused = false;
        
        #endregion

        #region Properties

        public GameObject leaderBoard => _leaderBoard;
        public GameObject roundEndPanel => _roundEndPanel;
        public PlayerLeaderBoardData playerLeaderBoardData => _playerLeaderBoardData;
        public bool isPaused => _isPaused;
        
        #endregion
        
        #region Unity Methods

        private void Start()
        {
            SetKillsText(0);
            SetDeathsText(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ControlPausePanel();
            }
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

        public void ControlTimerActivity(bool val)
        {
            if (isTimerActive != val)
            {
                timeText.gameObject.SetActive(val);
                isTimerActive = val;
            }
        }
        public void ControlPausePanel()
        {
            if (pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(false);
                _isPaused = false;
                
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                pausePanel.SetActive(true);
                _isPaused = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        public void GoToMainMenu()
        {
            MatchController.Instance.OnPlayerLeft(PhotonNetwork.NickName);
            PlayerPrefs.DeleteKey("roundTime");
            PlayerPrefs.DeleteKey("averagePoint");
            PhotonNetwork.LeaveRoom();
        }

        public void QuitGame()
        {
            MatchController.Instance.OnPlayerLeft(PhotonNetwork.NickName);
            PlayerPrefs.DeleteKey("roundTime");
            PlayerPrefs.DeleteKey("averagePoint");
            Application.Quit();
        }
        #endregion
    }
  

}