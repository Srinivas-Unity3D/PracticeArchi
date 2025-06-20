using System;

namespace CardGame.Core.Interfaces
{
    public interface IScoreSystem
    {
        event Action<int> OnScoreChanged;
        event Action<int> OnComboChanged;
        event Action<int> OnComboMultiplierChanged;
        event Action<int> OnTotalScoreChanged;
        event Action OnComboBreak;
        event Action OnComboStart;
        
       
        int CurrentScore { get; }
        int TotalScore { get; }
        int CurrentCombo { get; }
        float CurrentComboMultiplier { get; }
        float GameTime { get; }
        
      
        void StartGame();
        void EndGame();
        void OnSuccessfulMatch();
        void OnFailedMatch();
        void RestartGame();
        
        string GetScoreText();
        string GetComboText();
        string GetTotalScoreText();
    }
} 