using CardGame.Core.Data;
using CardGame.Core.Gameplay;
using CardGame.Core.Interfaces;
using CardGame.Core.SaveLoad;
using System.Collections;
using TMPro;
using UnityEngine;

namespace CardGame.Core.Persistence
{
    public class PersistenceManager : MonoBehaviour
    {
        private ISaveLoadSystem saveLoadSystem;
        private GameBoardManager gameBoardManager;
        private ScoreManager scoreManager;
        private GameStatsTracker statsTracker;

        [SerializeField] private TextMeshProUGUI message;

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
                rows = gameBoardManager.GetRows(),
                columns = gameBoardManager.GetColumns(),
                gridConstraintCount = gameBoardManager.GetConstraintCount(),
                currentScore = scoreManager.CurrentScore,
                currentCombo = scoreManager.CurrentCombo,
                gameTime = scoreManager.GameTime,
                moves = statsTracker.CurrentStats.TotalMoves,
                highScore = statsTracker.GetHighScore()
            };

            saveLoadSystem.SaveGame(gameData);
            StartCoroutine(ToggleMessage(1f, "Game saved successfully!"));
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
                
                gameBoardManager.RestoreBoardState(loadedData.cardsOnBoard, loadedData.rows, loadedData.columns, loadedData.gridConstraintCount);
                scoreManager.RestoreGameState(loadedData.currentScore, loadedData.currentCombo, loadedData.gameTime);
                statsTracker.RestoreGameState(loadedData.moves, loadedData.highScore);
                StartCoroutine(ToggleMessage(1f, "Game loaded successfully!"));
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

        IEnumerator ToggleMessage(float time,string msg) 
        {
            message.text = msg;
            message.gameObject.SetActive(true);
            yield return new WaitForSeconds(time);
            message.gameObject.SetActive(false);
        }
    }
} 