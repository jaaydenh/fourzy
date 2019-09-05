//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using StackableDecorator;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultPuzzlePackDataHolder", menuName = "Create PuzzlePack Data Holder")]
    public class PuzzlePacksDataHolder : ScriptableObject
    {
        public Material originalFontMaterial;
        [List]
        public PuzzlePackCollection puzzlePacks;

        private Dictionary<Color, Material> puzzlePacksFontMaterials;

        public int totalPuzzlesCompleteCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.puzzlesComplete.Count);

        public int totalPuzzlesCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.puzzlesEnabled.Count);
        private static GamePiecesDataHolder piecesData;

        public PuzzleData random
        {
            get
            {
                PuzzlePack puzzlePack = puzzlePacks.list.Where(_puzzlePack => _puzzlePack.puzzlesEnabled.Count > 0).Random();

                if (puzzlePack == null) return null;

                PuzzleBoard puzzle = puzzlePack.puzzlesEnabled.Random();

                if (puzzle == null) return null;

                return puzzle.puzzleData;
            }
        }

        public void Initialize()
        {
            puzzlePacks.list.ForEach(puzzlePack => { puzzlePack.puzzles.list.ForEach(puzzle => { puzzle.Initialize(puzzlePack, puzzlePack.puzzles.list.IndexOf(puzzle)); }); });
        }

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

        /// <summary>
        /// Runtime only
        /// </summary>
        public void ResetPuzzlesPlayerPrefs()
        {
            puzzlePacks.list.ForEach(puzzlePack =>
            {
                puzzlePack.puzzles.list.ForEach(puzzleBoard =>
                {
                    PlayerPrefsWrapper.SetPuzzleChallengeComplete(puzzleBoard.puzzleData.ID, false);
                    PlayerPrefsWrapper.SetGameRewarded(puzzleBoard.puzzleData.ID, false);
                });
            });
        }

        /// <summary>
        /// Reset unlocked/opened states
        /// </summary>
        public void ResetPuzzlePacksPlayerPrefs()
        {
            puzzlePacks.list.ForEach(puzzlePack =>
            {
                puzzlePack.puzzles.list.ForEach(puzzleBoard =>
                {
                    PlayerPrefsWrapper.SetPuzzlePackUnlocked(puzzlePack.packID, false);
                    PlayerPrefsWrapper.SetPuzzlePackOpened(puzzlePack.packID, false);
                });
            });
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
            public PackType packType = PackType.PUZZLE_PACK;
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
            [List]
            public RewardsCollection rewards;

            /// <summary>
            /// Runtime only
            /// </summary>
            public List<PuzzleBoard> puzzlesEnabled => puzzles.list.Where(puzzle => puzzle.puzzleData.Enabled && puzzle._enabled).ToList();

            /// <summary>
            /// Runtime only
            /// </summary>
            public List<PuzzleBoard> puzzlesComplete => puzzlesEnabled.Where(puzzle => PlayerPrefsWrapper.IsPuzzleChallengeComplete(puzzle.puzzleData.ID)).ToList();

            public bool complete => puzzlesComplete.Count == puzzlesEnabled.Count;

            public bool justFinished { get; set; }

            /// <summary>
            /// Runtime only
            /// </summary>
            public ClientPuzzleData nextUnsolvedData
            {
                get
                {
                    List<PuzzleBoard> _enabled = puzzlesEnabled;

                    if (_enabled.Count == 0) return null;

                    for (int puzzleIndex = 0; puzzleIndex < _enabled.Count; puzzleIndex++)
                    {
                        if (puzzleIndex == 0 && !PlayerPrefsWrapper.IsPuzzleChallengeComplete(_enabled[puzzleIndex].puzzleData.ID))
                            return _enabled[puzzleIndex].puzzleData;
                        else if (puzzleIndex > 0 && PlayerPrefsWrapper.IsPuzzleChallengeComplete(_enabled[puzzleIndex - 1].puzzleData.ID)
                            && !PlayerPrefsWrapper.IsPuzzleChallengeComplete(_enabled[puzzleIndex].puzzleData.ID))
                            return _enabled[puzzleIndex].puzzleData;
                    }

                    return _enabled[0].puzzleData;
                }
            }

            public IClientFourzy nextUnsolved
            {
                get
                {
                    ClientPuzzleData data = nextUnsolvedData;

                    if (data != null)
                    {
                        switch (data.packType)
                        {
                            case PackType.PUZZLE_PACK: return new ClientFourzyPuzzle(data);

                            default: return ClientFourzyGame.FromPuzzleData(data);
                        }
                    }

                    return null;
                }
            }

            /// <summary>
            /// Runtime only
            /// </summary>
            public IClientFourzy Next(IClientFourzy current)
            {
                ClientPuzzleData puzzleData = null;
                List<PuzzleBoard> _enabled = puzzlesEnabled;

                if (current.puzzleData.PackLevel == puzzles.list.Count - 1)
                    puzzleData = _enabled[0].puzzleData;
                else
                {
                    int _index = puzzles.list.FindIndex(current.puzzleData.PackLevel + 1, puzzle => puzzle.puzzleData.Enabled);

                    if (_index > -1)
                        puzzleData = puzzles.list[_index].puzzleData;
                    else
                        puzzleData = _enabled[0].puzzleData;
                }

                switch (puzzleData.packType)
                {
                    case PackType.PUZZLE_PACK: return new ClientFourzyPuzzle(puzzleData);

                    default: return ClientFourzyGame.FromPuzzleData(puzzleData);
                }
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

        [System.Serializable]
        public class RewardsCollection
        {
            public List<RewardsManager.Reward> list;
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
            [Tooltip("Enabled must be checked in both BoardFile and here for board to be available in puzzlePack")]
            public bool _enabled = true;
            public PuzzleGoalType goal;
            [StackableField]
            [ShowIf("#AIPackTypeCheck")]
            public AIProfile aiProfile;
            [StackableField]
            [ShowIf("#BossPackTypeCheck")]
            public BossType aiBoss;
            [StackableField]
            [ShowIf("#PuzzkePackTypeCheck")]
            public int moveLimit;
            [StackableField]
            [ShowIf("#PuzzkePackTypeCheck")]
            public string instructions;

            [StackableField]
            [ShowIf("#OpponentProfileCheck")]
            public string profileName;
            [StackableField]
            [ShowIf("#OpponentProfileCheck")]
            public int herdID = 1;

            public GameBoardDefinition boardDefinition { get; private set; }
            public ClientPuzzleData puzzleData { get; private set; }

            private string _fileName;
            private PuzzlePacksDataHolder dataHolder;
            private bool _lastEnabled = false;

            public void Initialize(PuzzlePack pack, int packLevel)
            {
                if (!file) return;

                GameBoardDefinition gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(file.text);

                puzzleData = new ClientPuzzleData();

                puzzleData.PackID = pack.packID;
                puzzleData.PackLevel = packLevel;

                puzzleData.packType = pack.packType;
                puzzleData.aiProfile = aiProfile;
                puzzleData.aiBoss = aiBoss;

                puzzleData.ID = gameboard.ID;
                puzzleData.Name = gameboard.BoardName;
                puzzleData.Enabled = gameboard.Enabled;
                puzzleData.GoalType = goal;
                puzzleData.MoveLimit = moveLimit;
                puzzleData.PuzzlePlayer = new Player(2, profileName);
                puzzleData.PuzzlePlayer.HerdId = herdID + "";
                puzzleData.Instructions = instructions;

                puzzleData.gameBoardDefinition = gameboard;
                puzzleData.InitialGameBoard = gameboard.ToGameBoardData();

                //initial moves
            }

            public bool FileCheck()
            {
                if (!file)
                {
                    _name = "No file.";
                    _fileName = "";
                    return true;
                }

                if (_fileName != file.name || _lastEnabled != _enabled)
                {
                    _fileName = file.name;
                    _lastEnabled = _enabled;

                    GameBoardDefinition gameboard = null;

                    try
                    {
                        gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(file.text);
                        _name = $"Name: {gameboard.BoardName}, Enabled: {gameboard.Enabled && _enabled}";
                    }
                    catch (JsonReaderException)
                    {
                        _name = "Wrong file";
                    }
                }

                return true;
            }

            public bool OpponentProfileCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    if (!dataHolder) dataHolder = AssetDatabase.LoadAssetAtPath<PuzzlePacksDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultPuzzlePackDataHolder")[0]));
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType != PackType.PUZZLE_PACK || aiBoss != BossType.None || aiProfile != AIProfile.Player;
                }
            }

            public bool PuzzkePackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    if (!dataHolder) dataHolder = AssetDatabase.LoadAssetAtPath<PuzzlePacksDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultPuzzlePackDataHolder")[0]));
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.PUZZLE_PACK;
                }
            }

            public bool BossPackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    if (!dataHolder) dataHolder = AssetDatabase.LoadAssetAtPath<PuzzlePacksDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultPuzzlePackDataHolder")[0]));
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.BOSS_AI_PACK;
                }
            }

            public bool AIPackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    if (!dataHolder) dataHolder = AssetDatabase.LoadAssetAtPath<PuzzlePacksDataHolder>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("DefaultPuzzlePackDataHolder")[0]));
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.AI_PACK;
                }
            }
        }

        public enum UnlockRequirementsEnum
        {
            NONE = 0,
            STARS = 1,
            COINS = 2,
            GEMS = 3,
        }

        public enum PackType
        {
            PUZZLE_PACK,
            AI_PACK,
            BOSS_AI_PACK,
        }
    }
}
