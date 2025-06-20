using CardGame.Core.Data;
using CardGame.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Core.Gameplay
{
    public struct BoardConfiguration 
    {
        public Sprite[] availableCardSprites;
        public int gridRows;
        public int gridColumns;
        public float matchCheckDelay;
        public int gridLayoutColumn;
    }

    public class GameBoardManager : MonoBehaviour, IGameBoard
    {
        [Header("Card Setup")]
        [SerializeField] private Card cardPrefab;
        [SerializeField] private GridLayoutGroup cardContainer;
        
        [Header("Dependencies")]
        [SerializeField] private IScoreSystem scoreSystem;
        [SerializeField] private IGameStatsTracker statsTracker;
        
        public event Action OnGameStarted;
        public event Action OnGameCompleted;
        public event Action<bool> OnMatchAttempted;
        
        public bool IsGameActive => isGameActive;
        public int MatchedPairsCount => matchedPairsCount;
        public int TotalPairs => allCards.Count / 2;
        
        private BoardConfiguration boardConfiguration;
        private List<Card> allCards = new List<Card>();
        private List<Sprite> cardSpritePairs = new List<Sprite>();
        private Card firstSelectedCard;
        private Card secondSelectedCard;
        private int matchedPairsCount = 0;
        private bool isGameActive = true;

        [SerializeField] private BoardConfig boardConfig;

        public void StartGame()
        {

        }

        private void Start()
        {
            InitializeDependencies();
            InitializeGame();
        }
        
        private void InitializeDependencies()
        {
            if (scoreSystem == null)
            {
                scoreSystem = FindObjectOfType<ScoreManager>();
            }
            
            if (statsTracker == null)
            {
                statsTracker = FindObjectOfType<GameStatsTracker>();
            }
        }
        
        private void InitializeGame()
        {
            InitializeBoardConfiguration(boardConfig);
            PrepareCardSprites();
            CreateGameBoard();
            RandomizeCardPositions();
            StartCoroutine(ShowAllCardsBriefly());
            
            if (scoreSystem != null)
            {
                scoreSystem.StartGame();
            }
            
            if (statsTracker != null)
            {
                statsTracker.StartNewGame();
            }
            
            OnGameStarted?.Invoke();
        }

        private void InitializeBoardConfiguration(BoardConfig boardConfig) 
        {
            boardConfiguration.availableCardSprites = boardConfig.availableCards;
            boardConfiguration.gridRows = boardConfig.rows;
            boardConfiguration.gridColumns = boardConfig.columns;
            boardConfiguration.matchCheckDelay = boardConfig.matchCheckDelay;
            boardConfiguration.gridLayoutColumn = boardConfig.gridColumn;
        }
        
        private void PrepareCardSprites()
        {
            cardSpritePairs.Clear();
            cardContainer.constraintCount = boardConfiguration.gridLayoutColumn;
            
            int totalCards = boardConfiguration.gridRows * boardConfiguration.gridColumns;
            int pairsNeeded = totalCards / 2;
            
            for (int i = 0; i < pairsNeeded; i++)
            {
                Sprite sprite = boardConfiguration.availableCardSprites[i % boardConfiguration.availableCardSprites.Length];
                cardSpritePairs.Add(sprite);
                cardSpritePairs.Add(sprite);
            }
            
            ShuffleCardSprites();
        }

        private void ShuffleCardSprites()
        {
            for (int i = cardSpritePairs.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Sprite temp = cardSpritePairs[i];
                cardSpritePairs[i] = cardSpritePairs[randomIndex];
                cardSpritePairs[randomIndex] = temp;
            }
        }

        private void CreateGameBoard()
        {
            for (int i = 0; i < cardSpritePairs.Count; i++)
            {
                Card newCard = Instantiate(cardPrefab, cardContainer.transform);
                newCard.SetCardFrontSprite(cardSpritePairs[i]);
                newCard.OnCardClicked += HandleCardSelection;
                newCard.OnCardRevealed += HandleCardRevealed;
                newCard.OnCardMatched += HandleCardMatched;
                
                allCards.Add(newCard);
            }
        }
        
        private void RandomizeCardPositions()
        {
            List<Vector3> gridPositions = GenerateGridPositions();
            
            ShufflePositions(gridPositions);
            
            for (int i = 0; i < allCards.Count; i++)
            {
                if (i < gridPositions.Count)
                {
                    allCards[i].transform.localPosition = gridPositions[i];
                }
            }
        }
        
        private List<Vector3> GenerateGridPositions()
        {
            List<Vector3> positions = new List<Vector3>();
            float spacing = 120f; 
            
            for (int row = 0; row < boardConfiguration.gridRows; row++)
            {
                for (int col = 0; col < boardConfiguration.gridColumns; col++)
                {
                    Vector3 position = new Vector3(
                        col * spacing - (boardConfiguration.gridColumns - 1) * spacing / 2f,
                        -row * spacing + (boardConfiguration.gridRows - 1) * spacing / 2f,
                        0f
                    );
                    positions.Add(position);
                }
            }
            
            return positions;
        }
        
        private void ShufflePositions(List<Vector3> positions)
        {
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Vector3 temp = positions[i];
                positions[i] = positions[randomIndex];
                positions[randomIndex] = temp;
            }
        }
        
        private IEnumerator ShowAllCardsBriefly()
        {
            SetAllCardsInteractable(false);
            
            foreach (Card card in allCards)
            {
                StartCoroutine(card.ShowCardBriefly());
            }
            
            yield return new WaitForSeconds(boardConfig.initialRevealDuration);
            
            SetAllCardsInteractable(true);
        }
        
        private void HandleCardSelection(Card selectedCard)
        {
            if (!isGameActive || selectedCard.IsMatched || selectedCard.IsRevealed)
                return;
                
            selectedCard.RevealCard();
            
            if (firstSelectedCard == null)
            {
                firstSelectedCard = selectedCard;
            }
            else if (secondSelectedCard == null)
            {
                secondSelectedCard = selectedCard;
                StartCoroutine(CheckForMatch());
            }
        }
        
        private void HandleCardRevealed(Card card)
        {
            Debug.Log($"Card revealed: {card.name}");
        }
        
        private void HandleCardMatched(Card card)
        {
            Debug.Log($"Card matched: {card.name}");
        }
        
        private IEnumerator CheckForMatch()
        {
            if (firstSelectedCard != null)
                firstSelectedCard.SetInteractable(false);
            if (secondSelectedCard != null)
                secondSelectedCard.SetInteractable(false);
            
            yield return new WaitForSeconds(boardConfiguration.matchCheckDelay);
            
            if (firstSelectedCard != null && secondSelectedCard != null)
            {
                bool isMatch = firstSelectedCard.CardFrontSprite == secondSelectedCard.CardFrontSprite;
                
                if (isMatch)
                {
                    HandleSuccessfulMatch();
                }
                else
                {
                    HandleFailedMatch();
                }
            }
            
            firstSelectedCard = null;
            secondSelectedCard = null;
        }
        
        private void HandleSuccessfulMatch()
        {
            firstSelectedCard.MarkAsMatched();
            secondSelectedCard.MarkAsMatched();
            
            matchedPairsCount++;
            boardConfig.OnMatchFound?.Invoke(matchedPairsCount);
            
            if (scoreSystem != null)
            {
                Vector3 midPoint = (firstSelectedCard.transform.position + secondSelectedCard.transform.position) / 2f;
                scoreSystem.OnSuccessfulMatch(midPoint);
            }
            
            if (statsTracker != null)
            {
                statsTracker.RecordMove(true);
            }
            
            OnMatchAttempted?.Invoke(true);
            
            if (matchedPairsCount >= allCards.Count / 2)
            {
                HandleGameCompletion();
            }
        }
        
        private void HandleFailedMatch()
        {
            firstSelectedCard.HideCard();
            secondSelectedCard.HideCard();
            
            if (scoreSystem != null)
            {
                Vector3 midPoint = (firstSelectedCard.transform.position + secondSelectedCard.transform.position) / 2f;
                scoreSystem.OnFailedMatch(midPoint);
            }
            
            if (statsTracker != null)
            {
                statsTracker.RecordMove(false);
            }
            
            OnMatchAttempted?.Invoke(false);
        }
        
        private void HandleGameCompletion()
        {
            isGameActive = false;
            
            if (scoreSystem != null)
            {
                scoreSystem.EndGame();
            }
            
            if (statsTracker != null)
            {
                statsTracker.CompleteGame();
                statsTracker.SaveHighScore();
            }
            
            PrimeTween.Sequence.Create()
                .Chain(PrimeTween.Tween.Scale(cardContainer.transform, Vector3.one * 1.2f, 0.2f, ease: PrimeTween.Ease.OutBack))
                .Chain(PrimeTween.Tween.Scale(cardContainer.transform, Vector3.one, 0.1f));

            boardConfig.OnGameCompleted?.Invoke();
            OnGameCompleted?.Invoke();
            Debug.Log("Game Completed! All pairs matched!");
        }
        
        private void SetAllCardsInteractable(bool interactable)
        {
            foreach (CardGame.Core.Gameplay.Card card in allCards)
            {
                card.SetInteractable(interactable);
            }
        }
        
        public void RestartGame()
        {
            ClearGameBoard();
            matchedPairsCount = 0;
            isGameActive = true;
            
            if (scoreSystem != null)
            {
                scoreSystem.RestartGame();
            }
            
            if (statsTracker != null)
            {
                statsTracker.StartNewGame();
            }
            
            InitializeGame();
        }
        
        public void EndGame()
        {
            if (isGameActive)
            {
                HandleGameCompletion();
            }
        }
        
        private void ClearGameBoard()
        {
            foreach (CardGame.Core.Gameplay.Card card in allCards)
            {
                if (card != null)
                {
                    DestroyImmediate(card.gameObject);
                }
            }
            allCards.Clear();
            cardSpritePairs.Clear();
        }
        
        public void SetScoreSystem(IScoreSystem scoreSystem)
        {
            this.scoreSystem = scoreSystem;
        }
        
        public void SetStatsTracker(IGameStatsTracker statsTracker)
        {
            this.statsTracker = statsTracker;
        }

       
    }
} 