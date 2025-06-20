using CardGame.Core.Interfaces;
using System;
using UnityEngine;

namespace CardGame.Core.Gameplay
{
    public class GameStatsTracker : MonoBehaviour, IGameStatsTracker
    {
        public event Action<IGameStats> OnGameStatsUpdated;
        public event Action<IGameStats> OnGameCompleted;

        private GameStats currentStats;
        private IScoreSystem scoreSystem;

        public IGameStats CurrentStats => currentStats;

        private void Start()
        {
            currentStats = new GameStats();
            if (scoreSystem == null)
            {
                scoreSystem = FindObjectOfType<ScoreManager>();
            }

            if (scoreSystem != null)
            {
                scoreSystem.OnComboChanged += OnComboChanged;
            }
        }

        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnComboChanged -= OnComboChanged;
            }
        }

        public void StartNewGame()
        {
            currentStats.Reset();
            OnGameStatsUpdated?.Invoke(currentStats);
        }

        public void RecordMove(bool wasSuccessful)
        {
            currentStats.AddMove(wasSuccessful);
            OnGameStatsUpdated?.Invoke(currentStats);
        }

        public void CompleteGame()
        {
            if (scoreSystem != null)
            {
                currentStats.SetGameTime(scoreSystem.GameTime);
                currentStats.SetFinalScore(scoreSystem.TotalScore);
            }

            OnGameCompleted?.Invoke(currentStats);
        }

        private void OnComboChanged(int combo)
        {
            currentStats.UpdateMaxCombo(combo);
        }

        public void SaveHighScore()
        {
            int currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (currentStats.FinalScore > currentHighScore)
            {
                PlayerPrefs.SetInt("HighScore", currentStats.FinalScore);
                PlayerPrefs.Save();
            }
        }

        public int GetHighScore()
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }

        public void SetScoreSystem(IScoreSystem scoreSystem)
        {
            this.scoreSystem = scoreSystem;
        }

        public void RestoreGameState(int savedMoves, int savedHighScore)
        {
            currentStats.Reset();
            
            for (int i = 0; i < savedMoves; i++)
            {
                currentStats.AddMove(true);
            }
            
            if (savedHighScore > GetHighScore())
            {
                PlayerPrefs.SetInt("HighScore", savedHighScore);
                PlayerPrefs.Save();
            }
            
            OnGameStatsUpdated?.Invoke(currentStats);
            
            Debug.Log($"Stats restored: Moves={savedMoves}, HighScore={savedHighScore}");
        }
    }
} 