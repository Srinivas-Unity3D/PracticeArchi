using CardGame.Core.Data;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CardGame.Core.SaveLoad
{
    public class BinarySaveLoadSystem : ISaveLoadSystem
    {
        private readonly string savePath;

        public BinarySaveLoadSystem()
        {
            savePath = Path.Combine(Application.persistentDataPath, "savedata.bin");
        }

        public void SaveGame(GameData data)
        {
            try
            {
                using (FileStream stream = new FileStream(savePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, data);
                }
                Debug.Log($"Game data saved to: {savePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error saving game data: {e.Message}");
            }
        }

        public GameData LoadGame()
        {
            if (!SaveExists())
            {
                Debug.Log("No save file found. Creating new game data.");
                return new GameData();
            }

            try
            {
                using (FileStream stream = new FileStream(savePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    GameData data = formatter.Deserialize(stream) as GameData;
                    Debug.Log("Game data loaded successfully.");
                    return data;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading game data: {e.Message}. Returning new game data.");
                return new GameData();
            }
        }

        public bool SaveExists()
        {
            return File.Exists(savePath);
        }

        public void DeleteSaveData()
        {
            if (SaveExists())
            {
                File.Delete(savePath);
                Debug.Log("Save data deleted.");
            }
        }
    }
} 