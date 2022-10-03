using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Multiplayer.Match
{
    public class MatchController: MonoBehaviourPunCallbacks,IOnEventCallback
    {
        #region Fields

        [SerializeField] private List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();

        #endregion
        
        #region Properties
        
        
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

        void Start()
        {
            if (!PhotonNetwork.IsConnected)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                NewPlayerSend(PhotonNetwork.NickName);
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


            PhotonNetwork.RaiseEvent(
                (byte)EventCodes.NewPlayer,
                package,
                new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                new SendOptions { Reliability = true }
            );
        }

        private void NewPlayerReceive(object[] data)
        {
            
        }

        private void ListPlayerSend(object[] data)
        {
            
        }

        private void ListPlayerReceive(object[] data)
        {
            
        }

        private void UpdateStatsSend(object[] data)
        {
            
        }

        private void UpdateStatsReceive(object[] data)
        {
            
        }
        #endregion

        #region Public Methods

        
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
                }
            }
        }

        #endregion

    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public int actor = 0, kills = 0, deaths = 0;

        public PlayerInfo(string _name,int _actor,int _deaths, int _kills)
        {
            name = _name;
            actor = _actor;
            kills = _kills;
            deaths = _deaths;
        }
    }

    public enum EventCodes : byte
    {
        NewPlayer,ListPlayers,UpdateStat
    }
}