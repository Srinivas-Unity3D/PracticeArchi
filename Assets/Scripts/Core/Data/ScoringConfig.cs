using UnityEngine;

namespace CardGame.Core.Data
{
    [CreateAssetMenu(fileName = "ScoringConfig", menuName = "Card Game/Scoring Config")]
    public class ScoringConfig : ScriptableObject
    {
        [Header("Base Scoring")]
        [Tooltip("Base points awarded for each successful match")]
        public int baseMatchScore = 100;
        
        [Tooltip("Bonus points for completing the game")]
        public int gameCompletionBonus = 1000;
        
        [Header("Combo System")]
        [Tooltip("Time window in seconds to maintain a combo")]
        public float comboTimeWindow = 3f;
        
        [Tooltip("Maximum combo multiplier")]
        public int maxComboMultiplier = 5;
        
        [Tooltip("Base combo multiplier (starts at 1, increases by this value each combo)")]
        public float comboMultiplierIncrement = 1f;
        
        [Tooltip("Minimum time between matches to start a combo")]
        public float minTimeBetweenMatches = 0.5f;
        
        [Header("Time Bonus")]
        [Tooltip("Bonus points per second remaining when game is completed")]
        public int timeBonusPerSecond = 10;
        
        [Tooltip("Maximum time bonus (caps the time bonus)")]
        public int maxTimeBonus = 5000;
        
        [Header("Moves Penalty")]
        [Tooltip("Penalty points for each failed match attempt")]
        public int failedMatchPenalty = -10;
        
        [Tooltip("Whether to apply penalties for failed matches")]
        public bool applyFailedMatchPenalty = true;
    }
} 