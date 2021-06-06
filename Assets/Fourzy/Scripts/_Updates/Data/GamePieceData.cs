//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using FourzyGameModel.Model;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    [System.Serializable]
    public class GamePieceData
    {
        public string name;
        public GamePieceView player1Prefab;
        public GamePieceView player2Prefab;
        public string Id;
        public bool enabled;

        [BoxGroup("Misc data")]
        public Sprite profilePicture;
        [BoxGroup("Misc data")]
        public Vector2 profilePictureOffset;
        [BoxGroup("Misc data")]
        public Color outlineColor = Color.blue;
        [BoxGroup("Misc data")]
        public Color borderColor = Color.green;

        [InfoBox("Current amount of pieces (pulled from server)")]
        [NonSerialized, ShowInInspector]
        public int Pieces;
        [InfoBox("Actual value pulled from server")]
        [NonSerialized, ShowInInspector]
        public int Magic;
        [InfoBox("Actual value pulled from server")]
        [NonSerialized, ShowInInspector]
        public int PiecesToUnlock;

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

        public GamePieceState State
        {
            get
            {
                int piecesCount = Pieces;

                if (piecesCount == 0)
                {
                    return GamePieceState.NotFound;
                }
                else if (piecesCount < PiecesToUnlock)
                {
                    return GamePieceState.FoundAndLocked;
                }
                else
                {
                    return GamePieceState.FoundAndUnlocked;
                }
            }
        }

        public void AddPieces(int quantity)
        {

        }
    }

    public enum GamePieceState
    {
        FoundAndUnlocked,
        FoundAndLocked,
        NotFound
    }
}
