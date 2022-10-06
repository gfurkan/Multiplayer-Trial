using TMPro;
using UnityEngine;

namespace Player.LeaderBoard
{
    public class PlayerLeaderBoardData: MonoBehaviour
    {
        #region Fields

        [SerializeField] private TextMeshProUGUI playerNameText, killsText, deathsText;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
             
        }
        
        void Update()
        {
             
        }

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        public void SetDetails(string playerName,int killCount,int deathCount)
        {
            playerNameText.text = playerName;
            killsText.text = killCount.ToString();
            deathsText.text = deathCount.ToString();
        }

        #endregion
    }
  

}