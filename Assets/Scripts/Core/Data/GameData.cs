using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Core.Data
{
    [System.Serializable]
    public struct CardData
    {
        public string cardSpriteName;
        public bool isFlipped;
        public bool isMatched;
        public float positionX;
        public float positionY;
        public float positionZ;

        public CardData(string spriteName, bool flipped, bool matched, Vector3 position)
        {
            cardSpriteName = spriteName;
            isFlipped = flipped;
            isMatched = matched;
            positionX = position.x;
            positionY = position.y;
            positionZ = position.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(positionX, positionY, positionZ);
        }
    }

    [System.Serializable]
    public class GameData
    {
        public List<CardData> cardsOnBoard;
        public int currentScore;
        public int currentCombo;
        public float gameTime;
        public int moves;

        public int highScore;

        public GameData()
        {
           
            cardsOnBoard = new List<CardData>();
            currentScore = 0;
            currentCombo = 0;
            gameTime = 0f;
            moves = 0;
            highScore = 0;
        }
    }
} 