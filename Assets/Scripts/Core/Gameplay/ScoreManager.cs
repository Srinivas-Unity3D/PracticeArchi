using CardGame.Core.Data;
using CardGame.Core.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using CardGame.Core.UI;

namespace CardGame.Core.Gameplay
{
    public class ScoreManager : MonoBehaviour, IScoreSystem
    {
        [Header("Configuration")]
        [SerializeField] private ScoringConfig scoringConfig;
        
        private ScoreUI scoreUI;
        
        public event Action<int> OnScoreChanged;
        public event Action<int> OnComboChanged;
        public event Action<int> OnComboMultiplierChanged;
        public event Action<int> OnTotalScoreChanged;
        public event Action OnComboBreak;
        public event Action OnComboStart;
       
        private int currentScore = 0;
        private int totalScore = 0;
        private int currentCombo = 0;
        private float currentComboMultiplier = 1f;
        
        private float gameStartTime;
        private float lastMatchTime;
        private bool isGameActive = false;
        
        private Coroutine comboDecayCoroutine;
        
        public int CurrentScore => currentScore;
        public int TotalScore => totalScore;
        public int CurrentCombo => currentCombo;
        public float CurrentComboMultiplier => currentComboMultiplier;
        public float GameTime => isGameActive ? Time.time - gameStartTime : 0f;
        
        private void Awake()
        {
            ValidateConfiguration();
        }
        
        private void ValidateConfiguration()
        {
            if (scoringConfig == null)
            {
                Debug.LogError("ScoringConfig is not assigned to ScoreManager!");
            }
        }
        
        public void StartGame()
        {
            ResetScore();
            gameStartTime = Time.time;
            isGameActive = true;
            lastMatchTime = -scoringConfig.comboTimeWindow; 
        }
        
        public void EndGame()
        {
            isGameActive = false;
            CalculateFinalScore();
        }
        
        public void OnSuccessfulMatch(Vector3 matchPosition)
        {
            if (!isGameActive) return;
            
            Debug.Log("Successful Match Detected by ScoreManager! Calculating score...");
            
            float timeSinceLastMatch = Time.time - lastMatchTime;
            lastMatchTime = Time.time;
            
            if (ShouldContinueCombo(timeSinceLastMatch))
            {
                ContinueCombo();
            }
            else
            {
                StartNewCombo();
            }
            
            int matchScore = CalculateMatchScore();
            AddScore(matchScore, matchPosition);
            
            StartComboDecayTimer();
        }
        
        public void OnFailedMatch(Vector3 failPosition)
        {
            if (!isGameActive) return;
            
            if (scoringConfig.applyFailedMatchPenalty)
            {
                AddScore(scoringConfig.failedMatchPenalty, failPosition);
            }
            
            BreakCombo();
        }
        
        private bool ShouldContinueCombo(float timeSinceLastMatch)
        {
            return timeSinceLastMatch <= scoringConfig.comboTimeWindow && 
                   timeSinceLastMatch >= scoringConfig.minTimeBetweenMatches;
        }
        
        private void StartNewCombo()
        {
            currentCombo = 1;
            currentComboMultiplier = 1f;
            OnComboStart?.Invoke();
            OnComboChanged?.Invoke(currentCombo);
            OnComboMultiplierChanged?.Invoke((int)currentComboMultiplier);
        }
        
        private void ContinueCombo()
        {
            currentCombo++;
            currentComboMultiplier = Mathf.Min(
                currentComboMultiplier + scoringConfig.comboMultiplierIncrement,
                scoringConfig.maxComboMultiplier
            );
            
            OnComboChanged?.Invoke(currentCombo);
            OnComboMultiplierChanged?.Invoke((int)currentComboMultiplier);
        }
        
        private void BreakCombo()
        {
            if (currentCombo > 1)
            {
                OnComboBreak?.Invoke();
            }
            
            currentCombo = 0;
            currentComboMultiplier = 1f;
            
            if (comboDecayCoroutine != null)
            {
                StopCoroutine(comboDecayCoroutine);
                comboDecayCoroutine = null;
            }
            
            OnComboChanged?.Invoke(currentCombo);
            OnComboMultiplierChanged?.Invoke((int)currentComboMultiplier);
        }
        
        private void StartComboDecayTimer()
        {
            if (comboDecayCoroutine != null)
            {
                StopCoroutine(comboDecayCoroutine);
            }
            
            comboDecayCoroutine = StartCoroutine(ComboDecayTimer());
        }
        
        private IEnumerator ComboDecayTimer()
        {
            yield return new WaitForSeconds(scoringConfig.comboTimeWindow);
            BreakCombo();
        }
        
        private int CalculateMatchScore()
        {
            int baseScore = scoringConfig.baseMatchScore;
            int comboBonus = Mathf.RoundToInt(baseScore * (currentComboMultiplier - 1f));
            
            return baseScore + comboBonus;
        }
        
        private void AddScore(int scoreToAdd, Vector3 position)
        {
            currentScore += scoreToAdd;
            totalScore += scoreToAdd;
            
            if (scoreUI != null)
            {
                scoreUI.ShowScorePopup(scoreToAdd, position);
            }
            
            OnScoreChanged?.Invoke(currentScore);
            OnTotalScoreChanged?.Invoke(totalScore);
        }
        
        private void CalculateFinalScore()
        {
            float gameTime = Time.time - gameStartTime;
            int timeBonus = Mathf.Min(
                Mathf.RoundToInt(gameTime * scoringConfig.timeBonusPerSecond),
                scoringConfig.maxTimeBonus
            );
            
            if (timeBonus > 0)
            {
                AddScore(timeBonus, Vector3.zero);
            }
            
            // Add completion bonus
            AddScore(scoringConfig.gameCompletionBonus, Vector3.zero);
        }
        
        private void ResetScore()
        {
            currentScore = 0;
            totalScore = 0;
            currentCombo = 0;
            currentComboMultiplier = 1f;
            
            if (comboDecayCoroutine != null)
            {
                StopCoroutine(comboDecayCoroutine);
                comboDecayCoroutine = null;
            }
            
            OnScoreChanged?.Invoke(currentScore);
            OnTotalScoreChanged?.Invoke(totalScore);
            OnComboChanged?.Invoke(currentCombo);
            OnComboMultiplierChanged?.Invoke((int)currentComboMultiplier);
        }
        
        public void RestartGame()
        {
            ResetScore();
            StartGame();
        }
        
        // Public methods for external access
        public string GetScoreText()
        {
            return $"Score: {currentScore:N0}";
        }
        
        public string GetComboText()
        {
            if (currentCombo <= 1) return "";
            return $"Combo: {currentCombo}x";
        }
        
        public string GetTotalScoreText()
        {
            return $"Total: {totalScore:N0}";
        }

        public void SetScoreUI(ScoreUI ui)
        {
            this.scoreUI = ui;
        }
    }
} 