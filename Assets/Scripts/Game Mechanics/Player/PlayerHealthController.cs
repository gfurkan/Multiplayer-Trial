using UnityEngine;

namespace Player.Health
{
    public class PlayerHealthController: MonoBehaviour
    {
        #region Fields

        [SerializeField] private int maxHealth = 0;

        private int currentHealth = 0;

        #endregion
        
        #region Properties
        
        
        #endregion

        #region Unity Methods

        void Start()
        {
            currentHealth = maxHealth;
        }
        

        #endregion

        #region Private Methods

        

        #endregion

        #region Public Methods

        public void TakeDamage(int damageDealt)
        {
            currentHealth -= damageDealt;
            print(currentHealth);
        }

        #endregion
    }
  

}