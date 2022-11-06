using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Player.Canvas;
using Player.LeaderBoard;
using Player.Spawn;
using UnityEngine;

namespace Multiplayer.Match
{
    public class MatchController: MonoBehaviourPunCallbacks,IOnEventCallback
    {
        #region Fields

        [SerializeField] private List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();
        [SerializeField] private GameStates _currentState;
        
        [SerializeField] private int _roundTime = 0;
        [SerializeField] private int _averagePoint = 0;
        
        [SerializeField] private float endingDelay;
        [SerializeField] private bool isPerpetual = false;
        public static event Action<GameStates> onGameStateChanged;
        private static MatchController _Instance;
        private int minePlayerIndex = 0;
        private List<PlayerLeaderBoardData> leaderBoardDatas = new List<PlayerLeaderBoardData>();
        private float currentTime = 0;
        private float sendTimer = 0;
        
        #endregion
        
        #region Properties

        public static MatchController Instance => _Instance;
        public GameStates currentState => _currentState;

        public int averagePoint=> PlayerPrefs.GetInt("averagePoint", _averagePoint);


        public int roundTime => PlayerPrefs.GetInt("roundTime", _roundTime*60);
  
        #endregion

        #region Unity Methods

        public override void OnEnable()
        {
           PhotonNetwork.AddCallbackTarget(this);
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Awake()
        {
            _Instance = this;
        }

        void Start()
        {
            PlayerCanvasController.Instance.ControlTimerActivity(false);
            currentTime = roundTime;
            
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                NewPlayerSend(PhotonNetwork.NickName);
                UpdateGameState(GameStates.Playing);
                SetupTimer();
            }
        }

        private void Update()
        {
            if (_currentState == GameStates.Playing)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    ShowLeaderBoard();
                }
                if (Input.GetKeyUp(KeyCode.Tab))
                {
                    if (PlayerCanvasController.Instance.leaderBoard.activeInHierarchy)
                    {
                        PlayerCanvasController.Instance.leaderBoard.SetActive(false);
                    }
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    if (currentTime > 0f)
                    {
                        currentTime -= Time.deltaTime;
                        if (currentTime <= 0f)
                        {
                            currentTime = 0;
                            UpdateGameState(GameStates.End);
                            ListPlayerSend();
                        }

                        ControlTime();

                        sendTimer -= Time.deltaTime;
                        if (sendTimer <= 0)
                        {
                            sendTimer += 1;
                            TimerSend();
                        }

                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void NewPlayerSend(string username)
        {
            object[] package = new object[4];
            package[0] = username;
            package[1] = PhotonNetwork.LocalPlayer.ActorNumber;
            package[2] = 0;
            package[3] = 0;

          /*  if (allPlayerInfos.Count > 0)
            {
                print("newp " +(int)package[1] + "actor " +allPlayerInfos[0].actor + "allp " +allPlayerInfos.Count );
            }
            
            
            for (int i = 0; i < allPlayerInfos.Count; i++)
            {
                if (allPlayerInfos[i].actor == (int)package[1])
                {
                   return;
                }
            }*/
            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NewPlayer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
            );
        }

        private void NewPlayerReceive(object[] data)
        {
            PlayerInfo newPlayer = new PlayerInfo((string)data[0], (int)data[1], (int)data[2], (int)data[3]);
            allPlayerInfos.Add(newPlayer);
            ListPlayerSend();
        }
        private void ListPlayerSend()
        {
            object[] package = new object[allPlayerInfos.Count+1];
            package[0] = _currentState;
            
            for (int i = 0; i < allPlayerInfos.Count; i++)
            {
                object[] piece = new object[4];
                
                piece[0] = allPlayerInfos[i].name;
                piece[1] = allPlayerInfos[i].actor;
                piece[2] = allPlayerInfos[i].kills;
                piece[3] = allPlayerInfos[i].deaths;

                package[i+1] = piece;
            }
            
            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.ListPlayers,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
        }

        private void ListPlayerReceive(object[] dataRecevied)
        {
            allPlayerInfos.Clear();
            UpdateGameState((GameStates)dataRecevied[0]);

            for (int i = 1; i < dataRecevied.Length; i++)
            {
                object[] piece = (object[])dataRecevied[i];

                PlayerInfo newPlayer = new PlayerInfo(
                    (string)(piece[0]), (int)(piece[1]), (int)(piece[2]), (int)(piece[3])
                );
                allPlayerInfos.Add(newPlayer);

                if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.actor)
                {
                    minePlayerIndex = i-1;
                }
            }
            
            CheckState();
        }

        public void UpdateStatsSend(int actorSending,int statToUpdate,int amountToChange)
        {
            object[] package = new object[] { actorSending, statToUpdate, amountToChange };

            PhotonNetwork.RaiseEvent((byte)EventCodes.UpdateStat,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true });
        }

        private void UpdateStatsReceive(object[] dataReceived)
        {
            int actor = (int)dataReceived[0];
            int statType = (int)dataReceived[1];
            int amount = (int)dataReceived[2];

            for (int i = 0; i < allPlayerInfos.Count; i++)
            {
                if (allPlayerInfos[i].actor == actor)
                {
                    switch (statType)
                    {
                        case 0: // Kills

                            allPlayerInfos[i].kills += amount;   
                            break;
                        
                        case 1: // Deaths

                            allPlayerInfos[i].deaths += amount;   
                            break;
                    }

                    if (i == minePlayerIndex)
                    {
                        UpdateStateDisplay();
                    }
                    break;
                }
            }
            
            CheckScore();
        }
        
        private void UpdateStateDisplay()
        {
            if (allPlayerInfos.Count > minePlayerIndex)
            {
                PlayerCanvasController.Instance.SetKillsText(allPlayerInfos[minePlayerIndex].kills);
                PlayerCanvasController.Instance.SetDeathsText(allPlayerInfos[minePlayerIndex].deaths);
            }
            else
            {
                PlayerCanvasController.Instance.SetKillsText(0);
                PlayerCanvasController.Instance.SetDeathsText(0);
            }
        }

        private void ShowLeaderBoard()
        {
            PlayerCanvasController.Instance.leaderBoard.SetActive(true);
            foreach (PlayerLeaderBoardData data in leaderBoardDatas)
            {
                Destroy(data.gameObject);
            }
            leaderBoardDatas.Clear();
            PlayerCanvasController.Instance.playerLeaderBoardData.gameObject.SetActive(false);

            List<PlayerInfo> sortedList = SortPlayers(allPlayerInfos);
            
            foreach (PlayerInfo player in sortedList)
            {
                PlayerLeaderBoardData newData = Instantiate(PlayerCanvasController.Instance.playerLeaderBoardData,PlayerCanvasController.Instance.playerLeaderBoardData.transform.parent);
                newData.SetDetails(player.name,player.kills,player.deaths);
                newData.gameObject.SetActive(true);

                leaderBoardDatas.Add(newData);
            }

        }

        private List<PlayerInfo> SortPlayers(List<PlayerInfo> players)
        {
            List<PlayerInfo> sortedList = new List<PlayerInfo>();

            while (sortedList.Count<players.Count)
            {
                int highest = -1;
                PlayerInfo selectedPlayer = players[0];

                foreach (PlayerInfo player in players)
                {
                    if (!sortedList.Contains(player))
                    {
                        int averageKills = player.kills - player.deaths;
                        
                        if (averageKills > highest)
                        {
                            selectedPlayer = player;
                            highest = averageKills; 
                        }
                    }
                }

                sortedList.Add(selectedPlayer);
            }

            return sortedList;
        }

        private void CheckScore()
        {
            bool isWinnerFound = false;

            foreach (PlayerInfo player in allPlayerInfos)
            {
                if ((player.kills - player.deaths) >= averagePoint)
                {
                    isWinnerFound = true;
                    break;
                }
            }

            if (isWinnerFound)
            {
                if (PhotonNetwork.IsMasterClient && _currentState != GameStates.End)
                {
                    
                    UpdateGameState(GameStates.End);
                    
                    ListPlayerSend();
                }
            }
        }

        private void CheckState()
        {
            if (_currentState == GameStates.End)
            {
                FinishGame();
            }
        }

        private void FinishGame()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.DestroyAll();
            }

            PlayerCanvasController.Instance.roundEndPanel.SetActive(true);
            ShowLeaderBoard();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            StartCoroutine(GoToMainMenu(endingDelay));
        }

        private IEnumerator GoToMainMenu(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (!isPerpetual)
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                PhotonNetwork.LeaveRoom();
                
                PlayerPrefs.DeleteKey("roundTime");
                PlayerPrefs.DeleteKey("averagePoint");
            }
            else
            {
                if (PhotonNetwork.IsMasterClient)
                { 
                    NextMatchSend();
                }
            }
        }

        private void NextMatchSend()
        {
            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NextMatch,
                null,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
        }

        private void NextMatchReceive()
        {
            UpdateGameState(GameStates.Playing);
            
            PlayerCanvasController.Instance.roundEndPanel.SetActive(false);
            PlayerCanvasController.Instance.leaderBoard.SetActive(false);

            foreach (PlayerInfo player in allPlayerInfos)
            {
                player.kills = 0;
                player.deaths = 0;
            }
            
            UpdateStateDisplay();
            PlayerSpawner.Instance.SpawnPlayer();
            
            SetupTimer();
            Cursor.lockState = CursorLockMode.Locked;

        }

        private void SetupTimer()
        {
            if (roundTime > 0)
            {
                currentTime = roundTime;
                ControlTime();
            }
        }
        private void ControlTime()
        {
            var timeToDisplay = System.TimeSpan.FromSeconds(currentTime);
            PlayerCanvasController.Instance.SetTimeText(timeToDisplay);
        }

        private void TimerSend()
        {
            object[] package = new object[] {(int)currentTime, _currentState};
            
            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.TimeSync,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                new SendOptions { Reliability = true }
            );
        }

        private void TimerReceive(object[] data)
        {
            currentTime = (int)data[0];
            _currentState = (GameStates)data[1];
            
            ControlTime();
            PlayerCanvasController.Instance.ControlTimerActivity(true);
        }
        
        #endregion

        #region Public Methods

        public void UpdateGameState(GameStates state)
        {
            if (_currentState != state)
            {
                _currentState = state;
                onGameStateChanged?.Invoke(state);
            }
        }
        
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code < 200)
            {
                EventCodes currentEvent = (EventCodes)photonEvent.Code;
                object[] data = (object[])photonEvent.CustomData;
                
                switch (currentEvent)
                {
                    case EventCodes.NewPlayer:
                        
                        NewPlayerReceive(data);
                        break;

                    case EventCodes.ListPlayers:
                        
                        ListPlayerReceive(data);
                        break;
                    case EventCodes.UpdateStat:
                        
                        UpdateStatsReceive(data);
                        break;
                    
                    case EventCodes.NextMatch:
                        
                        NextMatchReceive();
                        break;
                    
                    case EventCodes.TimeSync:
                        
                        TimerReceive(data);
                        break;
                }
            }
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            SceneManager.LoadScene(0);
        }
        
        public void OnPlayerLeft(string username)
        {
            if (allPlayerInfos.Count > 1)
            {
                for (int i = 1; i < allPlayerInfos.Count; i++)
                {
                    if (allPlayerInfos[i].name == username)
                    {
                        allPlayerInfos.Remove(allPlayerInfos[i]);
                        break;
                    }
                }
                ListPlayerSend();
            }
        }
        #endregion

    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public int actor = 0, kills = 0, deaths = 0;

        public PlayerInfo(string _name,int _actor,int _kills, int _deaths)
        {
            name = _name;
            actor = _actor;
            kills = _kills;
            deaths = _deaths;
        }
    }

    public enum EventCodes : byte
    {
        NewPlayer,PlayerLeft,ListPlayers,UpdateStat,NextMatch,TimeSync
    }

    public enum GameStates
    {
        Waiting,Playing,End
    }
}