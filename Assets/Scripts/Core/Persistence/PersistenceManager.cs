using CardGame.Core.Data;
using CardGame.Core.Gameplay;
using CardGame.Core.Interfaces;
using CardGame.Core.SaveLoad;
using UnityEngine;

namespace CardGame.Core.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        private ISaveLoadSystem saveLoadSystem;
        private GameBoardManager gameBoardManager;
        private ScoreManager scoreManager;
        private GameStatsTracker statsTracker;

        private void Awake()
        {
            saveLoadSystem = new BinarySaveLoadSystem();
        }

        private void Start()
        {
            gameBoardManager = FindObjectOfType<GameBoardManager>();
            scoreManager = FindObjectOfType<ScoreManager>();
            statsTracker = FindObjectOfType<GameStatsTracker>();
        }

        public void SaveGame()
        {
            if (gameBoardManager == null || scoreManager == null || statsTracker == null)
            {
                Debug.LogError("Cannot save game - required managers not found!");
                return;
            }

            var gameData = new GameData
            {
                cardsOnBoard = gameBoardManager.GetCardsData(),
                currentScore = scoreManager.CurrentScore,
                currentCombo = scoreManager.CurrentCombo,
                gameTime = scoreManager.GameTime,
                moves = statsTracker.CurrentStats.TotalMoves,
                highScore = statsTracker.GetHighScore()
            };

            saveLoadSystem.SaveGame(gameData);
            Debug.Log("Game saved successfully!");
        }

        public void LoadGame()
        {
            if (gameBoardManager == null || scoreManager == null || statsTracker == null)
            {
                Debug.LogError("Cannot load game - required managers not found!");
                return;
            }

            if (!saveLoadSystem.SaveExists())
            {
                Debug.Log("No save file found to load!");
                return;
            }

            try
            {
                GameData loadedData = saveLoadSystem.LoadGame();
                
                // Restore the game state
                gameBoardManager.RestoreBoardState(loadedData.cardsOnBoard);
                scoreManager.RestoreGameState(loadedData.currentScore, loadedData.currentCombo, loadedData.gameTime);
                statsTracker.RestoreGameState(loadedData.moves, loadedData.highScore);
                
                Debug.Log("Game loaded successfully!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error loading game: {e.Message}");
            }
        }

        public bool HasSaveData()
        {
            return saveLoadSystem.SaveExists();
        }
    }
} 