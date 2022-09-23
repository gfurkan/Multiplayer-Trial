using System.Collections.Generic;
using Multiplayer.Room.Info;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Multiplayer.Launcher
{
    public class Launcher: MonoBehaviourPunCallbacks
    {
        #region Fields

        public static Launcher Instance = null;

        [SerializeField] private GameObject menuButtons,loadingPanel,createRoomPanel,roomPanel,errorPanel,browsePanel,nicknamePanel;
        [SerializeField] private TextMeshProUGUI loadingText, roomNameText, errorText;
        [SerializeField] private TMP_InputField roomNameInput,nicknameInput;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private RoomInfoController roomInfoController;
        [SerializeField] private int roomNameCharacterLimit = 0;
        [SerializeField] private byte maxPlayerCount = 0;
        [SerializeField] private string levelName;
        [SerializeField] private GameObject startGameButton,testButton;
        
        private List<RoomInfoController> roomInfoList = new List<RoomInfoController>();
        private List<TextMeshProUGUI> playerNameList = new List<TextMeshProUGUI>();

        private bool isNicknameSetted = false;
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        private void Awake()
        {
            Instance = this;
        }

        void Start()
        {
             CloseMenu();
             ConnectToNetwork();
             roomNameInput.characterLimit = roomNameCharacterLimit;

#if UNITY_EDITOR
            testButton.SetActive(true);
#endif
        }

        #endregion

        #region Private Methods

        void CloseMenu()
        {
            loadingPanel.SetActive(false);
            menuButtons.SetActive(false);
            createRoomPanel.SetActive(false);
            roomPanel.SetActive(false);
            browsePanel.SetActive(false);
            nicknamePanel.SetActive(false);
        }

        void ConnectToNetwork()
        {
            loadingPanel.SetActive(true);
            loadingText.text = "Connecting to network...";

            PhotonNetwork.ConnectUsingSettings();
        }

        void ListAllPlayers()
        {
            foreach (var player in playerNameList)
            {
                Destroy(player.gameObject);
            }

            Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
            
            playerNameList.Clear();
            playerNameText.gameObject.SetActive(false);

            for (int i = 0; i < players.Length; i++)
            {
                var text = Instantiate(playerNameText, playerNameText.transform.parent);
                text.text = players[i].NickName;
                playerNameList.Add(text);
                text.gameObject.SetActive(true);
            }
        }
        #endregion

        #region Public Methods

        public override void OnConnectedToMaster()
        {
            loadingText.text = "Joining lobby...";
            
            PhotonNetwork.JoinLobby();
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        public override void OnJoinedLobby()
        {  
            CloseMenu();
            menuButtons.SetActive(true);

            if (!isNicknameSetted)
            {
                CloseMenu();
                nicknamePanel.SetActive(true);
                
                if (PlayerPrefs.HasKey("nickname"))
                {
                    nicknameInput.text = PlayerPrefs.GetString("nickname");
                }
            }
            else
            {
                PhotonNetwork.NickName = PlayerPrefs.GetString("nickname");
            }
        }

        public void OpenRoomCreatePanel()
        {
            CloseMenu();
            createRoomPanel.SetActive(true);
        }

        public void CreateRoom()
        {
            if (!string.IsNullOrEmpty(roomNameInput.text))
            {
                CloseMenu();
                RoomOptions options = new RoomOptions();
                options.MaxPlayers = maxPlayerCount;

                PhotonNetwork.CreateRoom(roomNameInput.text, options);
                loadingText.text = "Creating Room...";
                loadingPanel.SetActive(true);
            }
        }
        
        public void CloseRoomCreate()
        {
            CloseMenu();
            menuButtons.SetActive(true);
        }
        public override void OnJoinedRoom()
        {
            CloseMenu();

            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            roomPanel.SetActive(true);
            ListAllPlayers();

            if (PhotonNetwork.IsMasterClient)
            {
                startGameButton.SetActive(true);
            }
            else
            {
                startGameButton.SetActive(false);
            }
        }

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            CloseMenu();
            loadingText.text = "Leaving Room...";
            loadingPanel.SetActive(true);
        }

        public override void OnLeftRoom()
        {
            CloseMenu();
            menuButtons.SetActive(true);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            errorText.text = "Failed to Create Room:" + message;
            CloseMenu();
            errorPanel.SetActive(true);
        }

        public void CloseErrorPanel()
        {
            CloseMenu();
            menuButtons.SetActive(true);
        }

        public void OpenRoomBrowser()
        {
            CloseMenu();
            browsePanel.SetActive(true);

        }

        public void CloseRoomBrowser()
        {
            CloseMenu();
            menuButtons.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            foreach (RoomInfoController roomInfo in roomInfoList)
            {
                Destroy(roomInfo.gameObject);
            }
            
            roomInfoList.Clear();
            roomInfoController.gameObject.SetActive(false);
            
            for (int i = 0; i < roomList.Count; i++)
            {
                if (!roomList[i].RemovedFromList)
                {
                    RoomInfoController newButton = Instantiate(roomInfoController, roomInfoController.transform.parent);
                    newButton.SetRoomInfo(roomList[i]);
                    newButton.gameObject.SetActive(true);
                    roomInfoList.Add(newButton);
                }
            }
        }

        public void JoinRoom(RoomInfo roomInfo)
        {
            PhotonNetwork.JoinRoom(roomInfo.Name);
            
            CloseMenu();
            loadingText.text = "Joining Room...";
            loadingPanel.SetActive(true);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            var text = Instantiate(playerNameText, playerNameText.transform.parent);
            text.text = newPlayer.NickName;
            playerNameList.Add(text);
            text.gameObject.SetActive(true);
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            ListAllPlayers();
        }

        public void SetNickname()
        {
            if (!string.IsNullOrEmpty(nicknameInput.text))
            {
                PhotonNetwork.NickName = nicknameInput.text;
                isNicknameSetted = true;
                
                PlayerPrefs.SetString("nickname",nicknameInput.text);
                CloseMenu();
                menuButtons.SetActive(true);
            }
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startGameButton.SetActive(true);
            }
            else
            {
                startGameButton.SetActive(false);
            }
        }

        public void StartGame()
        {
            PhotonNetwork.LoadLevel(levelName);
        }

        public void QuickJoin()
        {
            PhotonNetwork.CreateRoom("test");
            CloseMenu();
            loadingText.text = "Creating Room";
            loadingPanel.SetActive(true);
        }
        #endregion
    }
  

}