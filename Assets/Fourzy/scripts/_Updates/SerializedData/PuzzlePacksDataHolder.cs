//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using StackableDecorator;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CreateAssetMenu(fileName = "DefaultPuzzlePackDataHolder", menuName = "Create PuzzlePack Data Holder")]
    public class PuzzlePacksDataHolder : ScriptableObject
    {
        [InfoBox("$GetMessage", VisibleIf = "GetMessageLength")]
        public Material originalFontMaterial;
        [BoxGroup("Puzzle Packs Icons")]
        public Sprite puzzlePackProgressionIconEmpty, puzzlePackProgressionIconSet;
        [BoxGroup("AI Packs Icons")]
        public Sprite aiPackProgressionIconEmpty, aiPackProgressionIconSet;
        [BoxGroup("Boss Packs Icons")]
        public Sprite bossPackProgressionIconEmpty, bossPackProgressionIconSet;

        [List]
        public PuzzlePackCollection puzzlePacks;

        private Dictionary<Color, Material> puzzlePacksFontMaterials;

        public int totalPuzzlesCompleteCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.puzzlesComplete.Count);

        public int totalPuzzlesCount => puzzlePacks.list.Sum(puzzlePack => puzzlePack.enabledPuzzlesData.Count);

        public ClientPuzzleData random
        {
            get
            {
                PuzzlePack puzzlePack = puzzlePacks.list.Where(_puzzlePack => _puzzlePack.enabledPuzzlesData.Count > 0).Random();

                if (puzzlePack == null) return null;

                ClientPuzzleData _data = puzzlePack.enabledPuzzlesData.Random();

                if (_data == null) return null;

                return _data;
            }
        }

        public void Initialize() => puzzlePacks.list.ForEach(puzzlePack => puzzlePack.Initialize(this));

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
        public void ResetPlayerPrefs()
        {
            puzzlePacks.list.ForEach(puzzlePack => puzzlePack.ResetPlayerPrefs());
        }

        public string GetMessage()
        {
            if (puzzlePacks.list.Count == 0) return "";

            List<PuzzlePack> duplicateIDs = puzzlePacks.list.GroupBy(x => x.packID).Where(g => g.Count() > 1).Select(y => y.First()).ToList();
            return duplicateIDs.Count == 0 ? "" : "Duplicate IDs: " + string.Join(",", duplicateIDs.Select(puzzlePack => (puzzlePack.name + " - " + puzzlePack.packID)));
        }

        public bool GetMessageLength() => GetMessage().Length > 0;

        [System.Serializable]
        public class PuzzlePackCollection
        {
            public List<PuzzlePack> list;
        }

        [System.Serializable]
        public class PuzzlePack : BasicPuzzlePack
        {
            /// <summary>
            /// This is editor only field
            /// </summary>
            [HideInInspector]
            public string _name;
            public Color outlineColor;
            public Sprite packBG;
            [StackableField, StackableDecorator.ShowIf("#OpponentTypeCheck")]
            public Color aiColor;
            [StackableField, StackableDecorator.ShowIf("#OpponentTypeCheck")]
            public Color playerColor;
            public bool overrideProgressionIcons;
            [StackableField, StackableDecorator.ShowIf("$overrideProgressionIcons")]
            public Sprite progressionIconEmpty, progressionIconSet;
            /// <summary>
            /// Requirement quantity
            /// </summary>
            [StackableField]
            [StackableDecorator.ShowIf("#Check")]
            public int quantity;
            [List]
            public BoardsCollection puzzles;

            public PuzzlePacksDataHolder puzzlePacksHolder { get; private set; }
            public Material labelMaterial { get; private set; }

            public Sprite _progressionIconEmpty
            {
                get
                {
                    if (overrideProgressionIcons)
                        return progressionIconEmpty;
                    else
                    {
                        switch (packType)
                        {
                            case PackType.PUZZLE_PACK:
                                return puzzlePacksHolder.puzzlePackProgressionIconEmpty;

                            case PackType.AI_PACK:
                                return puzzlePacksHolder.aiPackProgressionIconEmpty;

                            default:
                                return puzzlePacksHolder.bossPackProgressionIconEmpty;
                        }
                    }
                }
            }

            public Sprite _progressionIconSet
            {
                get
                {
                    if (overrideProgressionIcons)
                        return progressionIconEmpty;
                    else
                    {
                        switch (packType)
                        {
                            case PackType.PUZZLE_PACK:
                                return puzzlePacksHolder.puzzlePackProgressionIconSet;

                            case PackType.AI_PACK:
                                return puzzlePacksHolder.aiPackProgressionIconSet;

                            default:
                                return puzzlePacksHolder.bossPackProgressionIconSet;
                        }
                    }
                }
            }

            public void Initialize(PuzzlePacksDataHolder puzzlePacksHolder)
            {
                base.Initialize();

                this.puzzlePacksHolder = puzzlePacksHolder;
                labelMaterial = puzzlePacksHolder.GetPuzzlePackFontMaterial(outlineColor);

                for (int puzzleIndex = 0; puzzleIndex < puzzles.list.Count; puzzleIndex++)
                {
                    ClientPuzzleData _data = puzzles.list[puzzleIndex].GetPuzzleData(this, puzzleIndex);

                    if (_data != null)
                    {
                        puzzlesData.Add(_data);

                        if (!allRewards.ContainsKey(puzzleIndex)) allRewards.Add(puzzleIndex, new List<RewardsManager.Reward>());
                        allRewards[puzzleIndex].AddRange(_data.rewards);

                        if (_data.Enabled)
                        {
                            enabledPuzzlesData.Add(_data);

                            if (_data.rewards.Length > 0)
                                rewardPuzzles.Add(_data);
                        }
                    }
                }
            }

            /// <summary>
            /// Editor stuff
            /// </summary>
            /// <returns></returns>
            public bool Check()
            {
                _name = $"{name}, R: {unlockRequirement.ToString()}, B: {puzzles.list.Count}, ID: {packID}, Type: {packType}";

                return unlockRequirement != UnlockRequirementsEnum.NONE;
            }

            public bool OpponentTypeCheck() => packType != PackType.PUZZLE_PACK;

            public bool AITypeCheck() => packType == PackType.AI_PACK;
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

        [System.Serializable]
        public class SpellsCollection
        {
            public List<SpellId> list;
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
            [StackableField, StackableDecorator.ShowIf("#FileCheck")]
            public TextAsset file;
            [Tooltip("Enabled must be checked in both BoardFile and here for board to be available in puzzlePack")]
            public bool _enabled = true;
            public PuzzleGoalType goal;
            [List]
            public RewardsCollection rewards;
            [StackableField, StackableDecorator.ShowIf("#RewardsCheck")]
            public bool overridePorgressionIcons;
            [StackableField, StackableDecorator.ShowIf("#RewardsCheckExtra")]
            public Sprite progressionIconEmpty, progressionIconSet;

            [Space(10f), StackableField, StackableDecorator.ShowIf("#AIPackTypeCheck")]
            public AIProfile aiProfile;
            [StackableField, StackableDecorator.ShowIf("#BossPackTypeCheck")]
            public BossType aiBoss;
            [StackableField, StackableDecorator.ShowIf("#PuzzlePackTypeCheck")]
            public int moveLimit;
            [StackableField, StackableDecorator.ShowIf("#PuzzlePackTypeCheck")]
            public bool overrideInstructions;
            [StackableField, StackableDecorator.ShowIf("#PuzzlePackTypeCheckCheckOverride")]
            public string instructions;

            [StackableField, StackableDecorator.ShowIf("#OpponentProfileCheck")]
            public bool overrideAIProfile;
            [StackableField, StackableDecorator.ShowIf("#AIPackTypeCheckOverride")]
            public string profileName;
            [StackableField, StackableDecorator.ShowIf("#OpponentProfileCheckOverride")]
            public int herdID = 1;
            [StackableField, StackableDecorator.ShowIf("#OpponentProfileCheck")]
            public PlayerEnum firstTurn = PlayerEnum.ONE;
            [List, StackableDecorator.ShowIf("#OpponentProfileCheck")]
            public SpellsCollection availableSpells;

            private string _fileName;
            private PuzzlePacksDataHolder dataHolder;
            private bool _lastEnabled = false;

            public ClientPuzzleData GetPuzzleData(PuzzlePack pack, int packLevel)
            {
                if (!file) return null;

                GameBoardDefinition gameboard = JsonConvert.DeserializeObject<GameBoardDefinition>(file.text);

                ClientPuzzleData puzzleData = new ClientPuzzleData();

                puzzleData.PackID = pack.packID;
                puzzleData.PackLevel = packLevel;

                puzzleData.pack = pack;
                puzzleData.aiProfile = aiProfile;
                puzzleData.aiBoss = aiBoss;
                puzzleData.firstTurn = (int)firstTurn;

                puzzleData.ID = pack.puzzlePacksHolder.name + "_" + pack.packID + "_" + gameboard.BoardName + "_" + gameboard.ID;
                puzzleData.Name = gameboard.BoardName;
                puzzleData.Enabled = gameboard.Enabled && _enabled;
                puzzleData.GoalType = goal;
                puzzleData.MoveLimit = moveLimit;
                puzzleData.PuzzlePlayer =
                    new Player(2, overrideAIProfile ? profileName : pack.aiPlayerName) { HerdId = overrideAIProfile ? herdID + "" : pack.herdID };

                switch (pack.packType)
                {
                    case PackType.BOSS_AI_PACK:
                        puzzleData.PuzzlePlayer.BossType = aiBoss;
                        puzzleData.PuzzlePlayer.Profile = AIProfile.BossAI;

                        break;

                    case PackType.AI_PACK:
                        puzzleData.PuzzlePlayer.Profile = aiProfile;

                        break;

                    case PackType.PUZZLE_PACK:
                        puzzleData.PuzzlePlayer.Profile = AIProfile.PuzzleAI;

                        break;
                }

                puzzleData.GetInstructions();
                puzzleData.availableSpells = availableSpells.list.ToArray();
                puzzleData.rewards = rewards.list.ToArray();
                puzzleData.progressionIconEmpty = overridePorgressionIcons ? progressionIconEmpty : pack._progressionIconEmpty;
                puzzleData.progressionIconSet = overridePorgressionIcons ? progressionIconSet : pack._progressionIconSet;

                puzzleData.gameBoardDefinition = gameboard;
                puzzleData.InitialGameBoard = gameboard.ToGameBoardData();

                return puzzleData;
            }

            private bool RewardsCheck() => rewards.list.Count > 0;

            private bool RewardsCheckExtra() => rewards.list.Count > 0 && overridePorgressionIcons;

            private bool FileCheck()
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
                        _name = $"{gameboard.BoardName}, On: {gameboard.Enabled && _enabled}, R: {rewards.list.Count}";
                    }
                    catch (JsonReaderException)
                    {
                        _name = "Wrong file";
                    }
                }

                return true;
            }

            private bool OpponentProfileCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    GetDataHolder();
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType != PackType.PUZZLE_PACK && (aiBoss != BossType.None || aiProfile != AIProfile.Player);
                }
            }

            private bool PuzzlePackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    GetDataHolder();
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.PUZZLE_PACK;
                }
            }

            private bool BossPackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    GetDataHolder();
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.BOSS_AI_PACK;
                }
            }

            private bool AIPackTypeCheck()
            {
                if (Application.isPlaying)
                    return false;
                else
                {
#if UNITY_EDITOR
                    GetDataHolder();
#else
                    return false;
#endif
                    return dataHolder.puzzlePacks.list.Find(pack => pack.puzzles.list.Contains(this)).packType == PackType.AI_PACK;
                }
            }

            private bool AIPackTypeCheckOverride() => AIPackTypeCheck() && overrideAIProfile;

            private bool OpponentProfileCheckOverride() => OpponentProfileCheck() && overrideAIProfile;

            public bool PuzzlePackTypeCheckCheckOverride() => PuzzlePackTypeCheck() && overrideInstructions;

            private void GetDataHolder()
            {
#if UNITY_EDITOR
                if (!dataHolder)
                {
                    foreach (UnityEngine.Object obj in Selection.objects)
                    {
                        Type objType = obj.GetType();

                        if (objType == typeof(PuzzlePacksDataHolder) || objType.IsSubclassOf(typeof(PuzzlePacksDataHolder))) dataHolder = obj as PuzzlePacksDataHolder;
                    }
                }
#endif
            }
        }
    }
}
