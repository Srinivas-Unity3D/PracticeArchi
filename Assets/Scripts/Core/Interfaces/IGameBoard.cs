using System;

namespace CardGame.Core.Interfaces
{
    public interface IGameBoard
    {
        event Action OnGameStarted;
        event Action OnGameCompleted;
        event Action<bool> OnMatchAttempted; 
        
        bool IsGameActive { get; }
        int MatchedPairsCount { get; }
        int TotalPairs { get; }
        
        void StartGame();
        void RestartGame();
        void EndGame();
    }
} 