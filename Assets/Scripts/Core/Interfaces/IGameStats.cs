using System;

namespace CardGame.Core.Interfaces
{
    public interface IGameStats
    {
        int TotalMoves { get; }
        int SuccessfulMatches { get; }
        int FailedMatches { get; }
        float GameTime { get; }
        int FinalScore { get; }
        int MaxCombo { get; }
        float Accuracy { get; }
        
        void Reset();
        void AddMove(bool wasSuccessful);
        void SetGameTime(float time);
        void SetFinalScore(int score);
        void UpdateMaxCombo(int combo);
        string GetStatsSummary();
    }
    
    public interface IGameStatsTracker
    {
        event Action<IGameStats> OnGameStatsUpdated;
        event Action<IGameStats> OnGameCompleted;
        
        IGameStats CurrentStats { get; }
        
        void StartNewGame();
        void RecordMove(bool wasSuccessful);
        void CompleteGame();
        void SaveHighScore();
        int GetHighScore();
    }
} 