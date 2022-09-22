using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Room.Info
{
    public class RoomInfoController: MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI buttonText;

        private RoomInfo roomInfo;
        private Button button;
        
        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
           
        }
        

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            buttonText.text = roomInfo.Name + "  " + roomInfo.PlayerCount+"/"+roomInfo.MaxPlayers;

            if (button == null)
            {
                button = GetComponent<Button>();
            }
            
            if (roomInfo.PlayerCount == roomInfo.MaxPlayers)
            {
               button.interactable = false;
            }
            
            else
            {
                button.interactable = true;
            }
        }

        public void OpenRoom()
        {
            Launcher.Launcher.Instance.JoinRoom(roomInfo);
        }
        #endregion
    }
  

}