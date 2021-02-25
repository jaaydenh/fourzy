//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using StackableDecorator;
using FourzyGameModel.Model;

namespace Fourzy
{
    [System.Serializable]
    public class GamePieceData
    {
        public static System.Action<GamePieceData> onUpgrade;

        public string ID;
        public string name;
        public bool enabled;
        public int rarity = 30;
        public int startingMagic = 100;
        [SerializeField]
        private List<SpellId> spells;
        public Color outlineColor = Color.blue;
        public Color borderColor = Color.green;
        public int piecesToUnlock = 40;
        public Sprite profilePicture;
        public Vector2 profilePictureOffset;
        [List]
        public ProgressionCollection piecesProgression;

        public int pieces = 0;

        [System.NonSerialized]
        public bool foldout = false;

        public int Pieces
        {
            get => PlayerPrefsWrapper.GetGamePiecePieces(ID);
            set => PlayerPrefsWrapper.GamePieceUpdatePiecesCount(ID, value);
        }

        public int Champions
        {
            get => PlayerPrefsWrapper.GetGamePieceChampions(ID);
            set => PlayerPrefsWrapper.GamePieceUpdateChampionsCount(ID, value);
        }

        /// <summary>
        /// Temp returns these values
        /// </summary>
        public List<SpellId> Spells
        {
            get
            {
                return new List<SpellId>()
                {
                    SpellId.HEX,
                    SpellId.PLACE_LURE,
                    SpellId.DARKNESS
                };
            }
        }

        public int ChampionsFromPieces
        {
            get
            {
                int starsCount = 0;
                int piecesCount = Pieces;

                if (piecesCount < piecesToUnlock) return 0;

                for (int count = 0; count < 5; count++)
                    if (count < piecesProgression.list.Count)
                    {
                        if (piecesProgression.list[count] > piecesCount)
                        {
                            if (count > 0) starsCount = count;

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
                if (Pieces < piecesToUnlock)
                    return piecesToUnlock;
                else
                    return piecesProgression.list[Champions];
            }
        }

        public GamePieceState State
        {
            get
            {
                int piecesCount = Pieces;

                if (piecesCount == 0)
                    return GamePieceState.NotFound;
                else if (piecesCount < piecesToUnlock)
                    return GamePieceState.FoundAndLocked;
                else
                    return GamePieceState.FoundAndUnlocked;
            }
        }

        public bool CanUpgrade => Pieces >= GetCurrentTierProgression;

        public void AddPieces(int quantity) => Pieces += quantity;

        public void Upgrade()
        {
            if (!CanUpgrade) return;

            Champions++;

            onUpgrade?.Invoke(this);
        }

        public void Initialize()
        {
            if (!PlayerPrefsWrapper.HaveGamePieceRecord(ID))
            {
                Pieces = pieces;
            }
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