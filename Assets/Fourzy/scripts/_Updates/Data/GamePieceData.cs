//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;

namespace Fourzy
{
    [System.Serializable]
    public class GamePieceData
    {
        public static System.Action<GamePieceData> onUpgrade;

        public int ID;
        public string name;
        public bool enabled;
        public int rarity = 30;
        public Color outlineColor = Color.blue;
        public Color borderColor = Color.green;
        public int piecesToUnlock = 30;
        [List]
        public ProgressionCollection piecesProgression;

        public int pieces = 0;
        public int champions;

        [System.NonSerialized]
        public bool foldout = false;

        //champions count from numberOfPieces
        public int ChampionsFromPieces
        {
            get
            {
                int starsCount = 0;

                if (pieces < piecesToUnlock)
                    return starsCount;

                for (int count = 0; count < 5; count++)
                    if (count < piecesProgression.list.Count)
                    {
                        if (piecesProgression.list[count] > pieces)
                        {
                            if (count > 0)
                                starsCount = count;

                            break;
                        }
                    }
                    else
                        return count;

                return starsCount;
            }
        }

        public int GetCurrentTierProgression
        {
            get
            {
                if (pieces < piecesToUnlock)
                    return piecesToUnlock;
                else
                    return piecesProgression.list[champions];
            }
        }

        public int GetTierProgression
        {
            get
            {
                if (pieces < piecesToUnlock)
                    return piecesToUnlock;
                else
                    return piecesProgression.list[ChampionsFromPieces];
            }
        }

        public GamePieceState State
        {
            get
            {
                if (pieces == 0)
                    return GamePieceState.NotFound;
                else if (pieces < piecesToUnlock)
                    return GamePieceState.FoundAndLocked;
                else
                    return GamePieceState.FoundAndUnlocked;
            }
        }

        public bool CanUpgrade
        {
            get
            {
                return pieces >= GetCurrentTierProgression;
            }
        }

        public void Initialize()
        {
            pieces = PlayerPrefsWrapper.GetGamePiecePieces(this);
            champions = PlayerPrefsWrapper.GetGamePieceChampions(this);
        }

        public void AddPieces(int quantity)
        {
            pieces += quantity;

            PlayerPrefsWrapper.GamePieceUpdatePiecesCount(this);
        }

        public void Upgrade()
        {
            if (!CanUpgrade)
                return;

            champions++;

            if (onUpgrade != null)
                onUpgrade.Invoke(this);

            PlayerPrefsWrapper.GamePieceUpdateChampionsCount(this);
        }
    }

    [System.Serializable]
    public class ProgressionCollection
    {
        public List<int> list = new List<int>(new int[] { 80, 150, 240, 300, 550 });
    }

    public enum GamePieceState
    {
        FoundAndUnlocked,
        FoundAndLocked,
        NotFound
    }
}