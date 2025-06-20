using CardGame.Core.Interfaces;
using System;
using UnityEngine;

namespace CardGame.Core.Gameplay
{
    [System.Serializable]
    public class GameStats : IGameStats
    {
        public int totalMoves;
        public int successfulMatches;
        public int failedMatches;
        public float gameTime;
        public int finalScore;
        public int maxCombo;
        public float accuracy;
        
        // IGameStats Properties
        public int TotalMoves => totalMoves;
        public int SuccessfulMatches => successfulMatches;
        public int FailedMatches => failedMatches;
        public float GameTime => gameTime;
        public int FinalScore => finalScore;
        public int MaxCombo => maxCombo;
        public float Accuracy => accuracy;
        
        public GameStats()
        {
            Reset();
        }
        
        public void Reset()
        {
            totalMoves = 0;
            successfulMatches = 0;
            failedMatches = 0;
            gameTime = 0f;
            finalScore = 0;
            maxCombo = 0;
            accuracy = 0f;
        }
        
        public void AddMove(bool wasSuccessful)
        {
            totalMoves++;
            if (wasSuccessful)
            {
                successfulMatches++;
            }
            else
            {
                failedMatches++;
            }
            
            CalculateAccuracy();
        }
        
        public void SetGameTime(float time)
        {
            gameTime = time;
        }
        
        public void SetFinalScore(int score)
        {
            finalScore = score;
        }
        
        public void UpdateMaxCombo(int combo)
        {
            if (combo > maxCombo)
            {
                maxCombo = combo;
            }
        }
        
        private void CalculateAccuracy()
        {
            if (totalMoves > 0)
            {
                accuracy = (float)successfulMatches / totalMoves * 100f;
            }
        }
        
        public string GetStatsSummary()
        {
            return $"Final Score: {finalScore:N0}\n" +
                   $"Time: {Mathf.FloorToInt(gameTime / 60f):00}:{Mathf.FloorToInt(gameTime % 60f):00}\n" +
                   $"Moves: {totalMoves}\n" +
                   $"Accuracy: {accuracy:F1}%\n" +
                   $"Max Combo: {maxCombo}x";
        }
    }
} 