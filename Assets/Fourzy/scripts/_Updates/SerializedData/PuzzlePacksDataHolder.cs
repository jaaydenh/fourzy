//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using StackableDecorator;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using MessageType = StackableDecorator.MessageType;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultPuzzlePackDataHolder", menuName = "Create PuzzlePack Data Holder")]
    public class PuzzlePacksDataHolder : ScriptableObject
    {
        public Material originalFontMaterial;
        [List]
        public PuzzlePackCollection puzzlePacks;

        private Dictionary<Color, Material> puzzlePacksFontMaterials;

        public int totalPuzzlesCompleteCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.puzzlesCompleteCount);

        public int totalPuzzlesCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.puzzlesEnabled.Count);

        public PuzzleData random
        {
            get
            {
                PuzzlePack puzzlePack = puzzlePacks.list.Where(_puzzlePack => _puzzlePack.puzzlesEnabled.Count > 0).Random();

                if (puzzlePack == null) return null;

                PuzzleBoard puzzle = puzzlePack.puzzlesEnabled.Random();

                if (puzzle == null) return null;

                return puzzle.ToPuzzleData(puzzlePack.packID, puzzlePack.puzzles.list.IndexOf(puzzle));
            }
        }

        /// <summary>
        /// Runtime only
        /// </summary>
        public Material GetPuzzlePackFontMaterial(Color outlineColor)
        {
            if (!originalFontMaterial) return null;

            if (puzzlePacksFontMaterials == null)
                puzzlePacksFontMaterials = new Dictionary<Color, Material>();

            if (puzzlePacksFontMaterials.ContainsKey(outlineColor)) return puzzlePacksFontMaterials[outlineColor];

            Material newMaterial = new Material(originalFontMaterial);
            newMaterial.SetColor("_OutlineColor", outlineColor);
            newMaterial.hideFlags = HideFlags.HideAndDontSave;

            puzzlePacksFontMaterials.Add(outlineColor, newMaterial);

            return newMaterial;
        }

        [System.Serializable]
        public class PuzzlePackCollection
        {
            public List<PuzzlePack> list;
        }

        [System.Serializable]
        public class PuzzlePack
        {
            /// <summary>
            /// This is editor only field
            /// </summary>
            [HideInInspector]
            public string _name;
            public string name;
            public string packID;
            public Color outlineColor;
            public Sprite packBG;
            public UnlockRequirementsEnum unlockRequirement;
            /// <summary>
            /// Requirement quantity
            /// </summary>
            [StackableField]
            [ShowIf("#Check")]
            public int quantity;
            [List]
            public BoardsCollection puzzles;

            public List<PuzzleBoard> puzzlesEnabled => puzzles.list.Where(puzzle => puzzle.enabled).ToList();

            public List<PuzzleBoard> puzzlesComplete => puzzlesEnabled.Where(puzzle => PlayerPrefsWrapper.IsPuzzleChallengeComplete(puzzle.id)).ToList();

            public int puzzlesCompleteCount => puzzlesComplete.Count();

            public bool complete => puzzlesCompleteCount == puzzlesEnabled.Count;
            
            public PuzzleData nextUnsolvedPuzzleData
            {
                get
                {
                    List<PuzzleBoard> _enabled = puzzlesEnabled;

                    if (_enabled.Count == 0) return null;

                    if (puzzlesCompleteCount == 0)
                        return _enabled[0].ToPuzzleData(packID, 0);
                    else
                        for (int puzzleIndex = 0; puzzleIndex < _enabled.Count; puzzleIndex++)
                            if (puzzleIndex > 0 && PlayerPrefsWrapper.IsPuzzleChallengeComplete(_enabled[puzzleIndex - 1].id)
                                && !PlayerPrefsWrapper.IsPuzzleChallengeComplete(_enabled[puzzleIndex].id))
                                return _enabled[puzzleIndex].ToPuzzleData(packID, puzzleIndex);
                    
                    return _enabled[0].ToPuzzleData(packID, 0);
                }
            }

            public ClientFourzyPuzzle nextUnsolvedPuzzle
            {
                get
                {
                    PuzzleData next = nextUnsolvedPuzzleData;

                    if (next != null)
                    {
                        ClientFourzyPuzzle game = new ClientFourzyPuzzle(next);

                        //options
                        game.puzzlePack = this;

                        return game;
                    }

                    return null;
                }
            }

            public ClientFourzyPuzzle Next(ClientFourzyPuzzle current)
            {
                ClientFourzyPuzzle game = null;
                List<PuzzleBoard> _enabled = puzzlesEnabled;

                if (current._data.PackLevel == puzzles.list.Count - 1)
                    game = new ClientFourzyPuzzle(_enabled[0].ToPuzzleData(packID, 0));
                else
                {
                    int _index = puzzles.list.FindIndex(current._data.PackLevel + 1, puzzle => puzzle.enabled);

                    if (_index > -1)
                        game = new ClientFourzyPuzzle(puzzles.list[_index].ToPuzzleData(packID, _index));
                    else
                        game = new ClientFourzyPuzzle(_enabled[0].ToPuzzleData(packID, 0));
                }

                game.puzzlePack = this;

                return game;
            }

            public bool Check()
            {
                _name = $"{name}, Requirement: {unlockRequirement.ToString()}, Boards: {puzzles.list.Count}";

                return unlockRequirement != UnlockRequirementsEnum.NONE;
            }
        }

        [System.Serializable]
        public class BoardsCollection
        {
            public List<PuzzleBoard> list;
        }

        /// <summary>
        /// Editor helper
        /// </summary>
        [System.Serializable]
        public class PuzzleBoard
        {
            /// <summary>
            /// This is editor only field
            /// </summary>
            [HideInInspector]
            public string _name;
            [StackableField]
            [ShowIf("#FileCheck")]
            public TextAsset file;
            public string id;
            public string name;
            public bool enabled;
            public PuzzleGoalType goal;
            public int moveLimit;
            public string instructions;
            public string profileName;
            public string herdID;

            private AIPlayersDataHolder aiPlayers;

            public PuzzleData ToPuzzleData()
            {
                if (!file) return null;

                GameBoardDefinition gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(file.text);

                PuzzleData _data = new PuzzleData();

                _data.ID = id;
                _data.Name = name;
                _data.Enabled = enabled;
                _data.GoalType = goal;
                _data.MoveLimit = moveLimit;
                _data.PuzzlePlayer = new Player(2, profileName);
                _data.PuzzlePlayer.HerdId = herdID;
                _data.Instructions = instructions;

                _data.InitialGameBoard = gameboard.ToGameBoardData();
                //initial moves


                return _data;
            }

            public PuzzleData ToPuzzleData(string packId, int packLevel)
            {
                PuzzleData _data = ToPuzzleData();

                if (_data == null) return null;

                _data.PackID = packId;
                _data.PackLevel = packLevel;

                return _data;
            }

            public bool FileCheck()
            {
                if (!file)
                {
                    _name = "No file.";
                    return true;
                }

                GameBoardDefinition gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(file.text);

                if (gameboard != null)
                    _name = $"Name: {gameboard.BoardName}, ID: {gameboard.ID}";
                else
                    _name = "Wrong file.";

                return true;
            }
        }

        public enum UnlockRequirementsEnum
        {
            NONE = 0,
            STARS = 1,
            COINS = 2,
            GEMS = 3,
        }
    }
}
