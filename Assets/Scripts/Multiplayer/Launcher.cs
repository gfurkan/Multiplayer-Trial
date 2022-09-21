using System.Collections.Generic;
using Multiplayer.Room.Info;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

namespace Multiplayer.Launcher
{
    public class Launcher: MonoBehaviourPunCallbacks
    {
        #region Fields

        public static Launcher Instance = null;

        [SerializeField] private GameObject menuButtons,loadingPanel,createRoomPanel,roomPanel,errorPanel,browsePanel;
        [SerializeField] private TextMeshProUGUI loadingText,roomNameText,errorText;
        [SerializeField] private TMP_InputField roomNameInput;
        [SerializeField] private RoomInfoController roomInfoController;
        [SerializeField] private int roomNameCharacterLimit = 0;
        
        private List<RoomInfoController> roomInfoList = new List<RoomInfoController>();
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
        }

        void ConnectToNetwork()
        {
            loadingPanel.SetActive(true);
            loadingText.text = "Connecting to network...";

            PhotonNetwork.ConnectUsingSettings();
        }
        #endregion

        #region Public Methods

        public override void OnConnectedToMaster()
        {
            loadingText.text = "Joining lobby...";
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {  
            CloseMenu();
            menuButtons.SetActive(true);
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
                options.MaxPlayers = 8;

                PhotonNetwork.CreateRoom(roomNameInput.text, options);
                loadingText.text = "Creating Room...";
                loadingPanel.SetActive(true);
            }
        }

        public override void OnJoinedRoom()
        {
            CloseMenu();

            roomNameText.text = PhotonNetwork.CurrentRoom.Name;
            roomPanel.SetActive(true);
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
                if (roomList[i].PlayerCount!=roomList[i].MaxPlayers && !roomList[i].RemovedFromList)
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
        #endregion
    }
  

}