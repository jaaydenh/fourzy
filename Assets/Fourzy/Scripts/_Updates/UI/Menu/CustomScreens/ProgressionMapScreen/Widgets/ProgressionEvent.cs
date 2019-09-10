//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.ProgressionMap;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Fourzy._Updates.UI.Widgets
{
    [ExecuteInEditMode]
    public class ProgressionEvent : WidgetBase
    {
        public Camera3dItemProgressionLine linePrefab;
        public TMP_Text placeholder;
        public RectTransform content;
        public ProgressionEventType EventType;

        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.GAME), BoxGroup("PuzzlePack"), ValueDropdown("GetPacksData")]
        public string packName = "";
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.GAME), BoxGroup("PuzzlePack")]
        public TMP_Text nameLabel;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.GAME), BoxGroup("PuzzlePack")]
        public TweenBase starTween;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.GAME), BoxGroup("PuzzlePack")]
        public SliderExtended progressSlider;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.GAME), BoxGroup("PuzzlePack")]
        public RectTransform gamePieceParent;

        [ShowIfGroup("Currency/EventType", Value = ProgressionEventType.CURRENCY), BoxGroup("Currency"), InlineButton("ResetRewardID", "Reset ID")]
        public string currencyRewardID = Guid.NewGuid().ToString();
        [ShowIfGroup("Currency/EventType", Value = ProgressionEventType.CURRENCY), BoxGroup("Currency")]
        public RewardsManager.Reward reward;

        [ShowIfGroup("Reward/EventType", Value = ProgressionEventType.REWARD), BoxGroup("Reward")]
        public string otherReward;
        [ShowIfGroup("Reward/EventType", Value = ProgressionEventType.REWARD), BoxGroup("Reward")]
        public string otherRewardID;

        public List<ProgressionEvent> unlockWhenComplete;
        public AdvancedEvent onUnlock;
        public AdvancedEvent onRewarded;
        public AdvancedEvent onReset;

        private BasicPuzzlePack _puzzlePack;
        private ButtonExtended buttonExtended;
        private GamePieceView gamePieceView;

        //[HideInInspector]
        public List<Camera3dItemProgressionLine> lines;

        private bool _rewarded;
        private bool _unlocked;

        public Camera3dItemProgressionMap map { get; private set; }

        /// <summary>
        /// Editor stuff
        /// </summary>
        private bool eventUsed;

        [HideInInspector]
        public ResourceDB resouceDB;

        public BasicPuzzlePack PuzzlePack
        {
            get
            {
                if (!_puzzlePack) _puzzlePack = GameContentManager.Instance.GetExternalPuzzlePack(packName);

                return _puzzlePack;
            }
        }

        public bool wasRewarded
        {
            get
            {
                switch (EventType)
                {
                    case ProgressionEventType.GAME:
                        switch (PuzzlePack.packType)
                        {
                            case PackType.AI_PACK:
                            case PackType.BOSS_AI_PACK:
                                return PuzzlePack.complete;

                            default:
                                return PlayerPrefsWrapper.GetEventRewarded(id);
                        }

                    default:
                        return PlayerPrefsWrapper.GetEventRewarded(id);
                }
            }
        }

        public string id
        {
            get
            {
                switch (EventType)
                {
                    case ProgressionEventType.GAME:
                        return PuzzlePack.getUnlockRewardID;

                    case ProgressionEventType.CURRENCY:
                        return currencyRewardID;

                    default:
                        return otherRewardID;
                }
            }
        }

        protected override void Awake()
        {
            if (!Application.isPlaying) return;

            base.Awake();
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            if (Event.current.control)
            {
                if (Event.current.button == 1 && Camera.current != null)
                {
                    if (!eventUsed)
                    {
                        eventUsed = true;

                        Vector3 position = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
                        position.z = transform.position.z - 5f;

                        RaycastHit2D hit2d = Physics2D.Raycast(position, Camera.current.transform.forward, 10f);

                        if (hit2d)
                        {
                            if (hit2d.transform != transform)
                            {
                                ProgressionEvent other = hit2d.transform.GetComponent<ProgressionEvent>();

                                if (!other) return;

                                if (unlockWhenComplete.Contains(other))
                                    unlockWhenComplete.Remove(other);
                                else
                                    unlockWhenComplete.Add(other);

                                EditorUtility.SetDirty(this);
                            }
                        }
                    }
                }
                else
                    eventUsed = false;
            }
        }
#endif

        public override void _Update()
        {
            base._Update();

            if (wasRewarded) Rewarded(false);
            else lines.ForEach(line => line.SetColorLocked());

            if (!_unlocked && !_rewarded) return;

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    switch (PuzzlePack.packType)
                    {
                        case PackType.PUZZLE_PACK:
                            //update slider
                            progressSlider.value = (float)PuzzlePack.puzzlesComplete.Count / PuzzlePack.enabledPuzzlesData.Count;

                            if (PuzzlePack.complete) progressSlider.SetFillColor(Color.green);

                            break;
                        case PackType.AI_PACK:
                            //close eyes if complete
                            if (PuzzlePack.complete)
                                gamePieceView.Sleep();

                            break;
                    }

                    break;
            }
        }

        public virtual void Unlock(bool animate)
        {
            if (_unlocked) return;

            onUnlock.Invoke();
            _unlocked = true;

            if (wasRewarded)
            {
                switch (EventType)
                {
                    case ProgressionEventType.GAME:
                        buttonExtended.interactable = true;

                        break;

                    default:
                        buttonExtended.interactable = false;

                        break;
                }

                Rewarded(animate);
            }
            else
                buttonExtended.interactable = true;
        }

        public virtual void Rewarded(bool animate)
        {
            if (_rewarded) return;

            //unlock next events
            unlockWhenComplete.ForEach(@event => @event.Unlock(animate));
            lines.ForEach(line => line.SetColorUnlocked());

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    switch (PuzzlePack.packType)
                    {
                        case PackType.PUZZLE_PACK:
                            if (animate)
                            {
                                starTween.PlayForward(true);
                            }
                            else
                            {
                                starTween.AtProgress(1f);
                            }

                            break;
                    }

                    break;

                case ProgressionEventType.CURRENCY:
                    buttonExtended.interactable = false;

                    break;

                case ProgressionEventType.REWARD:
                    buttonExtended.interactable = false;

                    break;
            }

            _rewarded = true;
            onRewarded.Invoke();
        }

        public virtual void OnTap()
        {
            if (!buttonExtended.interactable) return;

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    switch (PuzzlePack.packType)
                    {
                        case PackType.AI_PACK:
                        case PackType.BOSS_AI_PACK:
                            menuScreen.menuController.GetScreen<VSGamePrompt>().Prompt(PuzzlePack);

                            break;

                        case PackType.PUZZLE_PACK:
                            //only open prepack prompt if there are any rewards in puzzle pack
                            if (PuzzlePack.allRewards.Count > 0)
                                menuScreen.menuController.GetScreen<PrePackPrompt>().Prompt(PuzzlePack);
                            else
                                PuzzlePack.StartNextUnsolvedPuzzle();

                            break;
                    }

                    break;

                case ProgressionEventType.CURRENCY:
                    Rewarded(true);
                    PlayerPrefsWrapper.SetEventRewarded(id, true);

                    //give reward
                    new RewardsManager.Reward[] { reward }.AssignRewards();

                    map.CheckMapComplete();

                    break;

                case ProgressionEventType.REWARD:
                    Rewarded(true);
                    PlayerPrefsWrapper.SetEventRewarded(id, true);

                    map.CheckMapComplete();

                    break;
            }
        }

        public void ResetGraphics()
        {
            /// <summary>
            /// Only for currency reward type
            /// </summary>
            switch (EventType)
            {
                case ProgressionEventType.CURRENCY:
                    //if (PlayerPrefsWrapper.GetEventRewarded(id)) Debug.Log(id);

                    PlayerPrefsWrapper.SetEventRewarded(id, false);

                    break;
            }

            if (string.IsNullOrEmpty(gameObject.scene.name)) return;

            onReset.Invoke();

            _unlocked = false;
            _rewarded = false;
        }

        protected override void OnInitialized()
        {
            if (!Application.isPlaying) return;

            base.OnInitialized();

            placeholder.gameObject.SetActive(false);

            buttonExtended = GetComponentInChildren<ButtonExtended>();
            map = GetComponentInParent<Camera3dItemProgressionMap>();

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    nameLabel.text = PuzzlePack.name;

                    if (PuzzlePack.packType == PackType.BOSS_AI_PACK || PuzzlePack.packType == PackType.AI_PACK)
                    {
                        //spawn gamepiece
                        gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(PuzzlePack.GetHerdID()).player1Prefab, gamePieceParent);

                        gamePieceView.transform.localPosition = Vector3.zero;
                        gamePieceView.transform.localScale = Vector3.one * 140f;
                        gamePieceView.StartBlinking();
                    }

                    break;

                case ProgressionEventType.CURRENCY:
                    RewardsScreenWidget rewardWidget =
                        Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), content);

                    rewardWidget.SetData(reward);
                    rewardWidget.ResetAnchors();

                    break;
            }
        }

#if UNITY_EDITOR
        private IEnumerable GetPacksData()
        {
            ValueDropdownList<string> values = new ValueDropdownList<string>();

            if (!resouceDB) resouceDB = ResourceDB.Instance;

            //get all puzzlepacks
            foreach (ResourceItem pack in resouceDB.root.GetChild(Constants.PUZZLE_PACKS_ROOT_FOLDER).GetChilds("", ResourceItem.Type.Folder))
                values.Add(pack.Name, pack.Name);

            return values;
        }
#endif

        private void ResetRewardID()
        {
            currencyRewardID = Guid.NewGuid().ToString();

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [Button("Add Line")]
        private void AddLine()
        {
            if (lines == null) lines = new List<Camera3dItemProgressionLine>();

            Camera3dItemProgressionMap map = GetComponentInParent<Camera3dItemProgressionMap>();

            if (!map) return;

            lines.Add(Instantiate(linePrefab, map.linesParent).Initialize(transform));
        }

        [Button("Remove Line")]
        private void RemoveLine()
        {
            if (lines == null || lines.Count == 0) return;

            DestroyImmediate(lines[lines.Count - 1].gameObject);
            lines.RemoveAt(lines.Count - 1);
        }

        public enum ProgressionEventType
        {
            NONE,
            GAME,
            CURRENCY,
            REWARD,
        }
    }
}