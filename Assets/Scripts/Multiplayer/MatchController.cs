using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Player.Canvas;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Match
{
    public class MatchController: MonoBehaviourPunCallbacks,IOnEventCallback
    {
        #region Fields

        [SerializeField] private List<PlayerInfo> allPlayerInfos = new List<PlayerInfo>();

        private static MatchController _Instance;
        private int minePlayerIndex = 0;
        
        #endregion
        
        #region Properties

        public static MatchController Instance => _Instance;
        
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
            PlayerInfo newPlayer = new PlayerInfo((string)data[0], (int)data[1], (int)data[2], (int)data[3]);
            allPlayerInfos.Add(newPlayer);
            ListPlayerSend();
        }

        private void ListPlayerSend()
        {
            object[] package = new object[allPlayerInfos.Count];

            for (int i = 0; i < allPlayerInfos.Count; i++)
            {
                object[] piece = new object[4];
                
                piece[0] = allPlayerInfos[i].name;
                piece[1] = allPlayerInfos[i].actor;
                piece[2] = allPlayerInfos[i].kills;
                piece[3] = allPlayerInfos[i].deaths;

                package[i] = piece;
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
            for (int i = 0; i < dataRecevied.Length; i++)
            {
                object[] piece = (object[])dataRecevied[i];

                PlayerInfo newPlayer = new PlayerInfo(
                    (string)(piece[0]), (int)(piece[1]), (int)(piece[2]), (int)(piece[3])
                );
                allPlayerInfos.Add(newPlayer);

                if (PhotonNetwork.LocalPlayer.ActorNumber == newPlayer.actor)
                {
                    minePlayerIndex = i;
                }
            }
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
        NewPlayer,ListPlayers,UpdateStat
    }
}