using CardGame.Core.SaveLoad;
using UnityEngine;

namespace CardGame.Core.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        private ISaveLoadSystem saveLoadSystem;

        private void Awake()
        {
            saveLoadSystem = new BinarySaveLoadSystem();
        }

        public void SaveGame()
        {
            Debug.Log("PersistenceManager: SaveGame called, but not implemented yet.");
        }

        public void LoadGame()
        {
            Debug.Log("PersistenceManager: LoadGame called, but not implemented yet.");
        }

        public bool HasSaveData()
        {
            return saveLoadSystem.SaveExists();
        }
    }
} 