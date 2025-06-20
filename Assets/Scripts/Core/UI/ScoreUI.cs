using CardGame.Core.Gameplay;
using CardGame.Core.Interfaces;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CardGame.Core.UI
{
    public class ScoreUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private TextMeshProUGUI totalScoreText;
        [SerializeField] private TextMeshProUGUI gameTimeText;
        
        [Header("Combo Visual Feedback")]
        [SerializeField] private GameObject comboContainer;
        [SerializeField] private Image comboBackground;
        [SerializeField] private TextMeshProUGUI comboMultiplierText;
        [SerializeField] private AnimationCurve comboPulseCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.2f);
        
        [Header("Score Animation")]
        [SerializeField] private TextMeshProUGUI scorePopupText;
        [SerializeField] private AnimationCurve scorePopupCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float scorePopupDuration = 1f;
        [SerializeField] private Vector2 scorePopupOffset = new Vector2(0, 50f);
        
        [Header("Colors")]
        [SerializeField] private Color normalComboColor = Color.white;
        [SerializeField] private Color highComboColor = Color.yellow;
        [SerializeField] private Color maxComboColor = Color.red;
        [SerializeField] private Color positiveScoreColor = Color.green;
        [SerializeField] private Color negativeScoreColor = Color.red;
        
        private IScoreSystem scoreSystem;
        private Vector3 originalComboScale;
        private Coroutine comboPulseCoroutine;
        private Coroutine scorePopupCoroutine;
        private Canvas parentCanvas;
        private GameStatsUI gameStatsUI;
        
        private void Awake()
        {
            originalComboScale = comboContainer != null ? comboContainer.transform.localScale : Vector3.one;
        }
        
        private void Start()
        {
            parentCanvas = FindObjectOfType<Canvas>();
            if (parentCanvas == null)
            {
                Debug.LogError("ScoreUI could not find a Canvas in its parent hierarchy!");
            }

            scoreSystem = FindObjectOfType<ScoreManager>();
            gameStatsUI = FindObjectOfType<GameStatsUI>();
            
            if (scoreSystem == null)
            {
                Debug.LogError("IScoreSystem implementation not found in scene!");
                return;
            }
            
            scoreSystem.OnScoreChanged += UpdateScoreDisplay;
            scoreSystem.OnComboChanged += UpdateComboDisplay;
            scoreSystem.OnComboMultiplierChanged += UpdateComboMultiplier;
            scoreSystem.OnTotalScoreChanged += UpdateTotalScoreDisplay;
            scoreSystem.OnComboStart += OnComboStart;
            scoreSystem.OnComboBreak += OnComboBreak;
            
            UpdateScoreDisplay(scoreSystem.CurrentScore);
            UpdateComboDisplay(scoreSystem.CurrentCombo);
            UpdateTotalScoreDisplay(scoreSystem.TotalScore);
        }
        
        private void Update()
        {
            if (scoreSystem != null)
            {
                UpdateGameTimeDisplay();
            }
        }
        
        private void OnDestroy()
        {
            if (scoreSystem != null)
            {
                scoreSystem.OnScoreChanged -= UpdateScoreDisplay;
                scoreSystem.OnComboChanged -= UpdateComboDisplay;
                scoreSystem.OnComboMultiplierChanged -= UpdateComboMultiplier;
                scoreSystem.OnTotalScoreChanged -= UpdateTotalScoreDisplay;
                scoreSystem.OnComboStart -= OnComboStart;
                scoreSystem.OnComboBreak -= OnComboBreak;
            }
        }
        
        private void UpdateScoreDisplay(int newScore)
        {
            if (scoreText != null)
            {
                scoreText.text = scoreSystem.GetScoreText() ;
            }
        }
        
        private void UpdateComboDisplay(int combo)
        {
            if (comboText != null)
            {
                comboText.text = scoreSystem.GetComboText();
                comboText.gameObject.SetActive(combo > 1);
            }
            
            if (comboContainer != null)
            {
                comboContainer.SetActive(combo > 1);
            }
        }
        
        private void UpdateComboMultiplier(int multiplier)
        {
            if (comboMultiplierText != null)
            {
                comboMultiplierText.text = $"x{multiplier}";
                
                if (multiplier >= 5)
                {
                    comboMultiplierText.color = maxComboColor;
                }
                else if (multiplier >= 3)
                {
                    comboMultiplierText.color = highComboColor;
                }
                else
                {
                    comboMultiplierText.color = normalComboColor;
                }
            }
        }
        
        private void UpdateTotalScoreDisplay(int totalScore)
        {
            if (totalScoreText != null)
            {
                totalScoreText.text = scoreSystem.GetTotalScoreText();
            }
        }
        
        private void UpdateGameTimeDisplay()
        {
            if (gameTimeText != null)
            {
                float gameTime = scoreSystem.GameTime;
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                gameTimeText.text = $"Time: {minutes:00}:{seconds:00}";
            }
        }
        
        private void OnComboStart()
        {
            if (comboContainer != null)
            {
                if (comboPulseCoroutine != null)
                {
                    StopCoroutine(comboPulseCoroutine);
                }
                comboPulseCoroutine = StartCoroutine(ComboPulseAnimation());
            }
        }
        
        private void OnComboBreak()
        {
            if (comboPulseCoroutine != null)
            {
                StopCoroutine(comboPulseCoroutine);
                comboPulseCoroutine = null;
            }
            
            if (comboContainer != null)
            {
                comboContainer.transform.localScale = originalComboScale;
            }
        }
        
        private System.Collections.IEnumerator ComboPulseAnimation()
        {
            float elapsed = 0f;
            float duration = 0.5f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;
                float scale = comboPulseCurve.Evaluate(progress);
                
                comboContainer.transform.localScale = originalComboScale * scale;
                yield return null;
            }
            
            comboContainer.transform.localScale = originalComboScale;
        }
        
        public void ShowScorePopup(int score, Vector3 position)
        {
            if (gameStatsUI != null && gameStatsUI.IsGameOverPanelActive())
            {
                return;
            }

            if (scorePopupText != null && parentCanvas != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
                
                TextMeshProUGUI popup = Instantiate(scorePopupText, parentCanvas.transform);
                popup.text = (score >= 0 ? "+" : "") + score.ToString();
                popup.color = score >= 0 ? positiveScoreColor : negativeScoreColor;
                
                if (scorePopupCoroutine != null)
                {
                    StopCoroutine(scorePopupCoroutine);
                }
                scorePopupCoroutine = StartCoroutine(ScorePopupAnimation(popup));
            }
        }
        
        private System.Collections.IEnumerator ScorePopupAnimation(TextMeshProUGUI popup)
        {
            Vector3 startPosition = popup.transform.position;
            Vector3 endPosition = startPosition + (Vector3)scorePopupOffset;
            Color startColor = popup.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
            
            float elapsed = 0f;
            
            while (elapsed < scorePopupDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / scorePopupDuration;
                float curveValue = scorePopupCurve.Evaluate(progress);
                
                popup.transform.position = Vector3.Lerp(startPosition, endPosition, curveValue);
                popup.color = Color.Lerp(startColor, endColor, curveValue);
                
                yield return null;
            }
            
            Destroy(popup.gameObject);
        }
        
        public void SetScoreSystem(IScoreSystem scoreSystem)
        {
            this.scoreSystem = scoreSystem;
        }
    }
} 