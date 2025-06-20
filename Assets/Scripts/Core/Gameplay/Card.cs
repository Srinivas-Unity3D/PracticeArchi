using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using System.Collections;
using UnityEngine.EventSystems;
using CardGame.Core.Audio;

namespace CardGame.Core.Gameplay
{
    public class Card : MonoBehaviour, IPointerClickHandler
    {
        [Header("Card Components")]
        [SerializeField] private Image cardIconImage;
        
        [Header("Card Sprites")]
        [SerializeField] private Sprite cardBackSprite;
        [SerializeField] private Sprite cardFrontSprite;
        
        [Header("Animation Settings")]
        [SerializeField] private float flipAnimationDuration = 0.2f;
        [SerializeField] private float flipDelay = 0.1f;
        
        private bool isRevealed = false;
        private bool isMatched = false;
        private bool isInteractable = true;
        
        public System.Action<Card> OnCardClicked;
        public System.Action<Card> OnCardRevealed;
        public System.Action<Card> OnCardMatched;
        
        public bool IsRevealed => isRevealed;
        public bool IsMatched => isMatched;
        public bool IsInteractable => isInteractable;
        public Sprite CardFrontSprite => cardFrontSprite;
        
        private SoundManager soundManager;

        private void Awake()
        {
            InitializeCard();
            soundManager = FindObjectOfType<SoundManager>();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        
        private void InitializeCard()
        {
            SetCardToBack();
        }
        
        public void SetCardFrontSprite(Sprite sprite)
        {
            cardFrontSprite = sprite;
        }
        
        public void HandleCardClick()
        {
            if (!isInteractable || isMatched || isRevealed)
                return;
                
            OnCardClicked?.Invoke(this);
        }
        
        public void RevealCard()
        {
            if (isRevealed || isMatched) return;

            soundManager?.PlayCardFlipSound();

            isRevealed = true;
            OnCardClicked?.Invoke(this);
            
            StartCoroutine(AnimateCardReveal());
        }
        
        public void HideCard()
        {
            if (isMatched)
                return;
                
            StartCoroutine(AnimateCardHide());
        }
        
        public void MarkAsMatched()
        {
            isMatched = true;
            isInteractable = false;
            OnCardMatched?.Invoke(this);
        }
        
        public IEnumerator ShowCardBriefly()
        {
            yield return StartCoroutine(AnimateCardReveal());
            yield return new WaitForSeconds(1.5f);
            yield return StartCoroutine(AnimateCardHide());
        }
        
        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
        }
        
        private IEnumerator AnimateCardReveal()
        {
            Tween.Rotation(transform, new Vector3(0f, 180f, 0f), flipAnimationDuration);
            
            yield return new WaitForSeconds(flipDelay);
            SetCardToFront();
            isRevealed = true;
            
            OnCardRevealed?.Invoke(this);
        }
        
        private IEnumerator AnimateCardHide()
        {
            Tween.Rotation(transform, new Vector3(0f, 0f, 0f), flipAnimationDuration);
            
            yield return new WaitForSeconds(flipDelay);
            SetCardToBack();
            isRevealed = false;
            
            if (!isMatched)
            {
                isInteractable = true;
            }
        }
        
        private void SetCardToFront()
        {
            if (cardIconImage != null && cardFrontSprite != null)
            {
                cardIconImage.sprite = cardFrontSprite;
            }
        }
        
        private void SetCardToBack()
        {
            if (cardIconImage != null && cardBackSprite != null)
            {
                cardIconImage.sprite = cardBackSprite;
            }
        }
        
        public void ResetCard()
        {
            isRevealed = false;
            isMatched = false;
            isInteractable = true;
            transform.rotation = Quaternion.identity;
            SetCardToBack();
        }

        public void RestoreCardState(bool isRevealed, bool isMatched)
        {
            this.isRevealed = isRevealed;
            this.isMatched = isMatched;
            
            if (isMatched)
            {
                SetCardToFront();
                isInteractable = false;
            }
            else if (isRevealed)
            {
                SetCardToFront();
                isInteractable = true;
            }
            else
            {
                SetCardToBack();
                isInteractable = true;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            HandleCardClick();
        }
    }
} 