using CardGame.Core.Data;
using UnityEngine;

[CreateAssetMenu(fileName ="SelectedConfig", menuName ="Card Game/SelectionConfig")]
public class SelectedConfifuration : ScriptableObject
{
    public BoardConfig selectedGrid;
}
