using CardGame.Core.Data;
using UnityEngine;

namespace CardGame.Core.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private SoundConfig soundConfig;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if (soundConfig == null)
            {
                Debug.LogError("SoundConfig is not assigned in the SoundManager!");
            }
        }

        public void PlayCardFlipSound()
        {
            PlaySound(soundConfig.cardFlip);
        }

        public void PlayCardMatchSound()
        {
            PlaySound(soundConfig.cardMatch);
        }

        public void PlayCardMismatchSound()
        {
            PlaySound(soundConfig.cardMismatch);
        }

        public void PlayGameOverSound()
        {
            PlaySound(soundConfig.gameOver);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip, soundConfig.defaultVolume);
            }
            else
            {
                Debug.LogWarning("Tried to play a sound, but the AudioClip is missing in the SoundConfig.");
            }
        }
    }
} 