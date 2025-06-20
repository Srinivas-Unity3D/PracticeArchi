using CardGame.Core.Gameplay;
using CardGame.Core.Interfaces;
using UnityEngine;
using TMPro;

namespace CardGame.Core.UI
{
    public class GameStatsUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI movesText;
        [SerializeField] private TextMeshProUGUI accuracyText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI statsSummaryText;
        
        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI gameTimeText;
        [SerializeField] private TextMeshProUGUI maxComboText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        
        private IGameStatsTracker statsTracker;
        private IScoreSystem scoreSystem;
        
        private void Start()
        {
            statsTracker = FindObjectOfType<GameStatsTracker>();
            scoreSystem = FindObjectOfType<ScoreManager>();
            
            if (statsTracker != null)
            {
                statsTracker.OnGameStatsUpdated += UpdateStatsDisplay;
                statsTracker.OnGameCompleted += ShowGameOverScreen;
            }
           
            UpdateHighScoreDisplay();
            
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        private void OnDestroy()
        {
            if (statsTracker != null)
            {
                statsTracker.OnGameStatsUpdated -= UpdateStatsDisplay;
                statsTracker.OnGameCompleted -= ShowGameOverScreen;
            }
        }
        
        private void UpdateStatsDisplay(IGameStats stats)
        {
            if (movesText != null)
            {
                movesText.text = $"Moves: {stats.TotalMoves}";
            }
            
            if (accuracyText != null)
            {
                accuracyText.text = $"Accuracy: {stats.Accuracy:F1}%";
            }
        }
        
        private void UpdateHighScoreDisplay()
        {
            if (highScoreText != null && statsTracker != null)
            {
                int highScore = statsTracker.GetHighScore();
                highScoreText.text = $"High Score: {highScore:N0}";
            }
        }
        
        private void ShowGameOverScreen(IGameStats finalStats)
        {
            if (gameOverPanel == null) return;
            
            if (finalScoreText != null)
            {
                finalScoreText.text = $"Final Score: {finalStats.FinalScore:N0}";
            }
            
            if (gameTimeText != null)
            {
                int minutes = Mathf.FloorToInt(finalStats.GameTime / 60f);
                int seconds = Mathf.FloorToInt(finalStats.GameTime % 60f);
                gameTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
            
            if (maxComboText != null)
            {
                maxComboText.text = $"Max Combo: {finalStats.MaxCombo}x";
            }
            
            if (statsSummaryText != null)
            {
                statsSummaryText.text = finalStats.GetStatsSummary();
            }
            
            if (newHighScoreText != null)
            {
                int currentHighScore = statsTracker.GetHighScore();
                bool isNewHighScore = finalStats.FinalScore > currentHighScore;
                newHighScoreText.gameObject.SetActive(isNewHighScore);
                
                if (isNewHighScore)
                {
                    newHighScoreText.text = "NEW HIGH SCORE!";
                }
            }
            
            gameOverPanel.SetActive(true);
            
            UpdateHighScoreDisplay();
        }
        
        public void HideGameOverScreen()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }
        
        public void RestartGame()
        {
            HideGameOverScreen();
            
            GameBoardManager gameBoardManager = FindObjectOfType<GameBoardManager>();
            if (gameBoardManager != null)
            {
                gameBoardManager.RestartGame();
            }
        }
        
        public void SetStatsTracker(IGameStatsTracker statsTracker)
        {
            this.statsTracker = statsTracker;
        }
        
        public void SetScoreSystem(IScoreSystem scoreSystem)
        {
            this.scoreSystem = scoreSystem;
        }

        public bool IsGameOverPanelActive()
        {
            return gameOverPanel != null && gameOverPanel.activeInHierarchy;
        }
    }
} 