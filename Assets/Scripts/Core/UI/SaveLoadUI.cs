using CardGame.Core.Persistence;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Core.UI
{
    public class SaveLoadUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Button saveButton;
        [SerializeField] private Button loadButton;
        
        private PersistenceManager persistenceManager;

        private void Start()
        {
            persistenceManager = FindObjectOfType<PersistenceManager>();
            
            if (persistenceManager == null)
            {
                Debug.LogError("PersistenceManager not found in scene!");
                return;
            }
            
            if (saveButton != null)
            {
                saveButton.onClick.AddListener(OnSaveButtonClicked);
            }
            
            if (loadButton != null)
            {
                loadButton.onClick.AddListener(OnLoadButtonClicked);
            }
        }
        
        private void OnDestroy()
        {
            if (saveButton != null)
            {
                saveButton.onClick.RemoveListener(OnSaveButtonClicked);
            }
            
            if (loadButton != null)
            {
                loadButton.onClick.RemoveListener(OnLoadButtonClicked);
            }
        }
        
        private void OnSaveButtonClicked()
        {
            if (persistenceManager != null)
            {
                persistenceManager.SaveGame();
            }
        }
        
        private void OnLoadButtonClicked()
        {
            if (persistenceManager != null)
            {
                persistenceManager.LoadGame();
            }
        }
    }
} 