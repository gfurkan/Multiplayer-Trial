using Managers.Singleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers.Spawn
{
    public class SpawnManager: SingletonManager<SpawnManager>
    {
        #region Fields

        [SerializeField] private Transform[] spawnPositions;

        #endregion

        #region Public Methods

        public Transform GetRandomSpawnPosition()
        {
            return spawnPositions[Random.Range(0, spawnPositions.Length)];
        }

        #endregion
    }
  

}