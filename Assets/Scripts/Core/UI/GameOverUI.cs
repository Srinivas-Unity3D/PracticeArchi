using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    void Start()
    {
        homeButton.onClick.AddListener(() =>
        {
            LoadScene("Menu");
        });

        restartButton.onClick.AddListener(() =>
        {
            LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    void LoadScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName);
    }
}
