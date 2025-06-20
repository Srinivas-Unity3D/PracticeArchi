using UnityEngine;

namespace CardGame.Core.Data
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Card Game/Sound Config")]
    public class SoundConfig : ScriptableObject
    {
        [Header("Game Sounds")]
        public AudioClip cardFlip;
        public AudioClip cardMatch;
        public AudioClip cardMismatch;
        public AudioClip gameOver;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        public float defaultVolume = 0.7f;
    }
} 