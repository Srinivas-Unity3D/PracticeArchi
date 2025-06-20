using CardGame.Core.Interfaces;
using CardGame.Core.Gameplay;
using UnityEngine;
using CardGame.Core.UI;

namespace CardGame.Core.DependencyInjection
{
    public class GameDependencyContainer : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameStatsTracker statsTracker;
        [SerializeField] private GameBoardManager gameBoardManager;
        
        [Header("UI Components")]
        [SerializeField] private ScoreUI scoreUI;
        [SerializeField] private GameStatsUI gameStatsUI;
        
        private void Awake()
        {
            InitializeDependencies();
        }
        
        private void InitializeDependencies()
        {
            if (scoreManager == null)
            {
                scoreManager = FindObjectOfType<ScoreManager>();
            }
            
            if (statsTracker == null)
            {
                statsTracker = FindObjectOfType<GameStatsTracker>();
            }
            
            if (gameBoardManager == null)
            {
                gameBoardManager = FindObjectOfType<GameBoardManager>();
            }
            
            if (scoreUI == null)
            {
                scoreUI = FindObjectOfType<ScoreUI>();
            }
            
            if (gameStatsUI == null)
            {
                gameStatsUI = FindObjectOfType<GameStatsUI>();
            }
            
            InjectDependencies();
        }
        
        private void InjectDependencies()
        {
            if (gameBoardManager != null)
            {
                gameBoardManager.SetScoreSystem(scoreManager);
                gameBoardManager.SetStatsTracker(statsTracker);
            }
            
            if (scoreManager != null && scoreUI != null)
            {
                scoreManager.SetScoreUI(scoreUI);
            }
            
            if (scoreUI != null)
            {
                scoreUI.SetScoreSystem(scoreManager);
            }
            
            if (gameStatsUI != null)
            {
                gameStatsUI.SetStatsTracker(statsTracker);
                gameStatsUI.SetScoreSystem(scoreManager);
            }
        }
        
        public void SetScoreManager(ScoreManager scoreManager)
        {
            this.scoreManager = scoreManager;
            InjectDependencies();
        }
        
        public void SetStatsTracker(GameStatsTracker statsTracker)
        {
            this.statsTracker = statsTracker;
            InjectDependencies();
        }
        
        public void SetGameBoardManager(GameBoardManager gameBoardManager)
        {
            this.gameBoardManager = gameBoardManager;
            InjectDependencies();
        }
        
        public void SetScoreUI(ScoreUI scoreUI)
        {
            this.scoreUI = scoreUI;
            InjectDependencies();
        }
        
        public void SetGameStatsUI(GameStatsUI gameStatsUI)
        {
            this.gameStatsUI = gameStatsUI;
            InjectDependencies();
        }
        
        public IScoreSystem GetScoreSystem()
        {
            return scoreManager;
        }
        
        public IGameStatsTracker GetStatsTracker()
        {
            return statsTracker;
        }
        
        public IGameBoard GetGameBoard()
        {
            return gameBoardManager;
        }
    }
} 