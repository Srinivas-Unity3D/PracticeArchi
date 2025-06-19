using UnityEngine;
using System;

namespace CardGame.Core.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Card Game/Game Config")]
    public class BoardConfig : ScriptableObject
    {
        [Header("Game Settings")]
        public int rows = 2;
        public int columns = 2;
        public int gridColumn = 2;
        public float matchCheckDelay = 0.3f;


        public float cardFlipDuration = 0.5f;
        public float mismatchDelay = 1.0f;
        public float initialRevealDuration = 1f;
        
        [Header("Card Types")]
        public Sprite[] availableCards;


        public Action OnGameCompleted;
        public Action<int> OnMatchFound;
    }
} 