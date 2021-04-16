//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using FourzyGameModel.Model;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [System.Serializable]
    public class GamePieceData
    {
        public static System.Action<GamePieceData> onUpgrade;

        public string name;
        public GamePieceView player1Prefab;
        public GamePieceView player2Prefab;
        public string Id;
        public bool enabled;
        public int startingMagic = 100;
        public int piecesToUnlock = 40;

        [InfoBox("How many pieces unlocked by default")]
        public int pieces = 0;

        [BoxGroup("Misc data")]
        public Sprite profilePicture;
        [BoxGroup("Misc data")]
        public Vector2 profilePictureOffset;
        [BoxGroup("Misc data")]
        public Color outlineColor = Color.blue;
        [BoxGroup("Misc data")]
        public Color borderColor = Color.green;

        public int Pieces
        {
            get => PlayerPrefsWrapper.GetGamePiecePieces(Id);
            set => PlayerPrefsWrapper.GamePieceUpdatePiecesCount(Id, value);
        }

        public int Champions
        {
            get => PlayerPrefsWrapper.GetGamePieceChampions(Id);
            set => PlayerPrefsWrapper.GamePieceUpdateChampionsCount(Id, value);
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
                //int piecesCount = Pieces;

                //if (piecesCount < piecesToUnlock) return 0;

                //for (int count = 0; count < 5; count++)
                //    if (count < piecesProgression.list.Count)
                //    {
                //        if (piecesProgression.list[count] > piecesCount)
                //        {
                //            if (count > 0) starsCount = count;

                //            break;
                //        }
                //    }
                //    else
                //        return count;

                return starsCount;
            }
        }

        public int GetCurrentTierProgression
        {
            get
            {
                //if (Pieces < piecesToUnlock)
                //    return piecesToUnlock;
                //else
                //    return piecesProgression.list[Champions];

                return 80;
            }
        }

        public GamePieceState State
        {
            get
            {
                int piecesCount = Pieces;

                if (piecesCount == 0)
                {
                    return GamePieceState.NotFound;
                }
                else if (piecesCount < piecesToUnlock)
                {
                    return GamePieceState.FoundAndLocked;
                }
                else
                {
                    return GamePieceState.FoundAndUnlocked;
                }
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
            if (!PlayerPrefsWrapper.HaveGamePieceRecord(Id))
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