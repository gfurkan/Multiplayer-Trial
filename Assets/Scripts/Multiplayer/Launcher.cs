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

        [SerializeField] private GameObject menuButtons,loadingPanel,createRoomPanel,roomPanel,errorPanel;
        [SerializeField] private TextMeshProUGUI loadingText,roomNameText,errorText;
        [SerializeField] private TMP_InputField roomNameInput;
        
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
        }

        #endregion

        #region Private Methods

        void CloseMenu()
        {
            loadingPanel.SetActive(false);
            menuButtons.SetActive(false);
            createRoomPanel.SetActive(false);
            roomPanel.SetActive(false);
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
        #endregion
    }
  

}