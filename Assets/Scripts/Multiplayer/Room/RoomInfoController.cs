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
          //  button = GetComponent<Button>();
        }
        

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        public void SetRoomInfo(RoomInfo roomInfo)
        {
            this.roomInfo = roomInfo;
            buttonText.text = roomInfo.Name;
        }

        public void OpenRoom()
        {
            Launcher.Launcher.Instance.JoinRoom(roomInfo);
        }
        #endregion
    }
  

}