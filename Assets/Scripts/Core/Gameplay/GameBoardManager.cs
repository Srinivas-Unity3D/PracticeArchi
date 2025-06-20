using CardGame.Core.Audio;
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
        private SoundManager soundManager;
        
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
            UpdateGridLayout(boardConfig.rows, boardConfig.columns);
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
            
            soundManager?.PlayCardMatchSound();
            
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
            
            soundManager?.PlayCardMismatchSound();
            
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
            
            soundManager?.PlayGameOverSound();
            
            if (statsTracker != null)
            {
                statsTracker.CompleteGame();
                statsTracker.SaveHighScore();
            }
            
            if (scoreSystem != null)
            {
                scoreSystem.EndGame();
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
            
            if (cardContainer != null) cardContainer.enabled = true;
            
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
            foreach (Transform child in cardContainer.transform)
            {
                Destroy(child.gameObject);
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

        public void SetSoundManager(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public List<CardData> GetCardsData()
        {
            var data = new List<CardData>();
            foreach (var card in allCards)
            {
                data.Add(new CardData(card.CardFrontSprite.name, card.IsRevealed, card.IsMatched, card.transform.localPosition));
            }
            return data;
        }

        public int GetRows()
        {
            return boardConfig != null ? boardConfig.rows : 2;
        }

        public int GetColumns()
        {
            return boardConfig != null ? boardConfig.columns : 2;
        }

        public int GetConstraintCount()
        {
            if (cardContainer != null)
            {
                return cardContainer.constraintCount;
            }
            else if (boardConfig != null)
            {
                return boardConfig.gridColumn;
            }
            
            return 2;
        }

        public void RestoreBoardState(List<CardData> savedCardsData, int rows, int columns, int constraintCount)
        {
            if (savedCardsData == null || savedCardsData.Count == 0)
            {
                Debug.LogWarning("No saved card data to restore!");
                return;
            }

            if (rows <= 0 || columns <= 0)
            {
                Debug.LogWarning("Loaded board dimensions are invalid. Falling back to current BoardConfig.");
                rows = boardConfig.rows;
                columns = boardConfig.columns;
            }

            ClearGameBoard();
            
            if (cardContainer != null)
            {
                cardContainer.enabled = true;
                cardContainer.constraintCount = constraintCount;
                UpdateGridLayout(rows, columns);
            }
            
            foreach (var cardData in savedCardsData)
            {
                var cardSprite = FindSpriteByName(cardData.cardSpriteName);
                if (cardSprite == null) continue;

                var newCard = Instantiate(cardPrefab, cardContainer.transform);
                newCard.SetCardFrontSprite(cardSprite);
                newCard.OnCardClicked += HandleCardSelection;
                newCard.OnCardRevealed += HandleCardRevealed;
                newCard.OnCardMatched += HandleCardMatched;

                newCard.RestoreCardState(cardData.isFlipped, cardData.isMatched);
                
                allCards.Add(newCard);
            }
            
            matchedPairsCount = 0;
            foreach (var card in allCards)
            {
                if (card.IsMatched) matchedPairsCount++;
            }
            matchedPairsCount /= 2;

            Card singleFlippedCard = null;
            int flippedCount = 0;
            foreach (var card in allCards)
            {
                if (card.IsRevealed && !card.IsMatched)
                {
                    flippedCount++;
                    singleFlippedCard = card;
                }
            }

            if (flippedCount == 1)
            {
                firstSelectedCard = singleFlippedCard;
            }
            
            Debug.Log($"Board restored with {allCards.Count} cards and constraint count {constraintCount}");
        }

        private Sprite FindSpriteByName(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return null;
                
            foreach (Sprite sprite in boardConfiguration.availableCardSprites)
            {
                if (sprite != null && sprite.name == spriteName)
                {
                    return sprite;
                }
            }
            
            return null;
        }

        private void UpdateGridLayout(int rows, int cols)
        {
            if (cardContainer == null) return;
            if (rows <= 0 || cols <= 0)
            {
                Debug.LogError($"Invalid layout configuration: Rows ({rows}) and Columns ({cols}) must be greater than 0.");
                return;
            }

            var rectTransform = cardContainer.transform as RectTransform;
            if (rectTransform == null) return;

            float containerWidth = rectTransform.rect.width;
            float containerHeight = rectTransform.rect.height;

            float xPadding = cardContainer.padding.left + cardContainer.padding.right;
            float yPadding = cardContainer.padding.top + cardContainer.padding.bottom;
            float xSpacing = (cols - 1) * cardContainer.spacing.x;
            float ySpacing = (rows - 1) * cardContainer.spacing.y;

            float availableWidth = containerWidth - xPadding - xSpacing;
            float availableHeight = containerHeight - yPadding - ySpacing;
            
            if (availableWidth <= 0 || availableHeight <= 0)
            {
                cardContainer.cellSize = Vector2.zero;
                Debug.LogWarning("Container size is too small for the specified padding and spacing. Cell size set to zero.");
                return;
            }

            float cellWidth = availableWidth / cols;
            float cellHeight = availableHeight / rows;

            float cellSize = Mathf.Min(cellWidth, cellHeight);

            if (cellSize < 0)
            {
                cellSize = 0;
            }

            cardContainer.cellSize = new Vector2(cellSize, cellSize);
        }
    }
} 