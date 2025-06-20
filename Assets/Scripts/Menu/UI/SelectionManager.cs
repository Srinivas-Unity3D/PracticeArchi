using CardGame.Core.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private Toggle twobyTwo;
    [SerializeField] private Toggle twobyThree;
    [SerializeField] private Toggle fivebySix;
    [SerializeField] private Toggle sixbyEight;
    [SerializeField] private Button play;

    [SerializeField] private BoardConfig _2X2;
    [SerializeField] private BoardConfig _2X3;
    [SerializeField] private BoardConfig _5X6;
    [SerializeField] private BoardConfig _6X8;

    [SerializeField] private SelectedConfifuration selectedConfiguration;

    void Start()
    {
        if (twobyTwo.isOn)
        {
            SetSelectionConfig(_2X2);
        }

        twobyTwo.onValueChanged.AddListener((value) =>
        {
            SetSelectionConfig(_2X2);
            ColorBlock cb = twobyTwo.colors;
            cb.normalColor = Color.white;
            twobyTwo.colors = cb;
        });

        twobyThree.onValueChanged.AddListener((value) =>
        {
            SetSelectionConfig(_2X3);
        });

        fivebySix.onValueChanged.AddListener((value) =>
        {
            SetSelectionConfig(_5X6);
        });

        sixbyEight.onValueChanged.AddListener((value) =>
        {
            SetSelectionConfig(_6X8);
        });

        play.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Main");
        });
    }

    void SetSelectionConfig(BoardConfig grid) 
    {
        selectedConfiguration.selectedGrid = grid;
    }
    
}
