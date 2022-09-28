using Managers.Singleton;
using TMPro;
using UnityEngine;

namespace Player.Canvas
{
    public class PlayerCanvasController: SingletonManager<PlayerCanvasController>
    {
        #region Fields

        [SerializeField] private GameObject deathPanel;
        [SerializeField] private TextMeshProUGUI deathText;
        [SerializeField] private TextMeshProUGUI spawnTimeText;
        
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
        #endregion
    }
  

}