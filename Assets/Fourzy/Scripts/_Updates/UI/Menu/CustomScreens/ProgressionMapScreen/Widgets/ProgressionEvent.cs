﻿//@vadym udod

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
        public List<RectTransform> sizeIgnore;

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
        public string otherRewardID;

        public List<ProgressionEvent> unlockWhenComplete;
        public AdvancedEvent onUnlock;
        public AdvancedEvent onRewarded;
        public AdvancedEvent onReset;

        //[HideInInspector]
        public List<Camera3dItemProgressionLine> lines;

        private BasicPuzzlePack _puzzlePack;
        private GamePieceView gamePieceView;
        private Animator animator;
        private Vector2 _size;

        private bool completeDisplayed = false;

        public bool _rewarded { get; private set; }
        public bool _unlocked { get; private set; }
        public Camera3dItemProgressionMap map { get; private set; }

        public Vector2 size
        {
            get
            {
                if (_size == Vector2.zero)
                {
                    FindSize(rectTransform);
                }

                return _size;
            }
        }

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
                        //switch (PuzzlePack.packType)
                        //{
                        //    case PackType.AI_PACK:
                        //    case PackType.BOSS_AI_PACK:
                        return PuzzlePack.complete;

                    //    default:
                    //        return PlayerPrefsWrapper.GetRewardRewarded(id);
                    //}

                    default:
                        return PlayerPrefsWrapper.GetRewardRewarded(id);
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
                                {
                                    unlockWhenComplete.Remove(other);
                                }
                                else
                                {
                                    unlockWhenComplete.Add(other);
                                }

                                EditorUtility.SetDirty(this);
                            }
                        }
                    }
                }
                else
                {
                    eventUsed = false;
                }
            }
        }
#endif

        public override void _Update()
        {
            base._Update();

            if (wasRewarded)
            {
                Rewarded(false);
            }
            else
            {
                lines.ForEach(line => line.SetColorLocked());
            }

            //if (!_unlocked && !_rewarded) return;

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    switch (PuzzlePack.packType)
                    {
                        case PackType.PUZZLE_PACK:
                            //update slider
                            progressSlider.value =
                                (float)PuzzlePack.puzzlesComplete.Count / PuzzlePack.enabledPuzzlesData.Count;

                            if (PuzzlePack.complete)
                            {
                                progressSlider.SetFillColor(Color.green);
                                starTween.AtProgress(1f);
                            }
                            else
                            {
                                progressSlider.SetFillColor(Color.red);
                                starTween.AtProgress(0f);
                            }

                            break;

                        case PackType.AI_PACK:
                            if (PuzzlePack.complete)
                            {
                                gamePieceView.Sleep();
                            }
                            else
                            {
                                gamePieceView.WakeUp();
                            }

                            break;
                    }

                    break;
            }

            if (gameObject.activeInHierarchy && menuScreen.isOpened && !completeDisplayed && _rewarded)
            {
                completeDisplayed = true;

                StartRoutine("rewardRoutine", RewardAnimationRoutine());
            }
        }

        public virtual void Unlock(bool animate)
        {
            if (_unlocked) return;

            onUnlock.Invoke();
            _unlocked = true;

            if (wasRewarded)
            {
                Rewarded(animate);
            }

            switch (EventType)
            {
                case ProgressionEventType.CURRENCY:
                case ProgressionEventType.REWARD:
                    if (!wasRewarded)
                    {
                        Rewarded(true);
                        PlayerPrefsWrapper.SetRewardRewarded(id, true);
                        if (EventType == ProgressionEventType.CURRENCY)
                        {
                            new RewardsManager.Reward[] { reward }.AssignRewards();
                        }
                    }

                    switch (otherRewardID)
                    {
                        case Constants.GAME_MODE_FAST_PUZZLES:
                        case Constants.GAME_MODE_GAUNTLET_GAME:
                            button.interactable = true;

                            break;
                    }

                    break;

                case ProgressionEventType.GAME:
                    button.interactable = true;

                    break;
            }
        }

        public virtual void Rewarded(bool animate)
        {
            if (_rewarded) return;

            //unlock next events
            unlockWhenComplete.ForEach(@event =>
            {
                if (@event.gameObject.activeInHierarchy)
                {
                    @event.Unlock(animate);
                }
            });
            lines.ForEach(line =>
            {
                if (line.gameObject.activeInHierarchy)
                {
                    line.SetColorUnlocked();
                }
            });

            _rewarded = true;
            onRewarded.Invoke();
        }

        public void OnTap()
        {
            if (!button.interactable) return;

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    switch (PuzzlePack.packType)
                    {
                        case PackType.AI_PACK:
                        case PackType.BOSS_AI_PACK:
                            menuScreen.menuController.GetOrAddScreen<VSGamePrompt>().Prompt(PuzzlePack);

                            break;

                        case PackType.PUZZLE_PACK:
                            //only open prepack prompt if there are any rewards in puzzle pack
                            //if (PuzzlePack.allRewards.Count > 0)
                            menuScreen.menuController.GetOrAddScreen<PrePackPrompt>().Prompt(PuzzlePack);
                            //else
                            //    PuzzlePack.StartNextUnsolvedPuzzle();

                            break;
                    }

                    AnalyticsManager.Instance.LogEvent(
                        AnalyticsManager.AnalyticsEvents.eventOpened,
                        values: new KeyValuePair<string, object>(AnalyticsManager.EVENT_ID_KEY, PuzzlePack.packId));

                    break;

                case ProgressionEventType.REWARD:
                    switch (otherRewardID)
                    {
                        case Constants.GAME_MODE_FAST_PUZZLES:
                            menuScreen.menuController.GetOrAddScreen<PromptScreen>().Prompt("Puzzles Ladder",
                                "Compete to solve the most puzzles and climb to the top of the leaderboard!",
                                "PLAY",
                                null,
                                () =>
                                {
                                    menuScreen.menuController.BackToRoot();
                                    menuScreen.menuController.OpenScreen<FastPuzzlesScreen>();
                                },
                                null);

                            break;

                        case Constants.GAME_MODE_GAUNTLET_GAME:
                            menuScreen.menuController.GetOrAddScreen<GauntletIntroScreen>()._Prompt();

                            break;
                    }

                    break;
            }
        }

        public ProgressionEvent ResetEvent()
        {
            switch (EventType)
            {
                case ProgressionEventType.CURRENCY:
                    PlayerPrefsWrapper.SetRewardRewarded(id, false);

                    break;
            }

            if (string.IsNullOrEmpty(gameObject.scene.name)) return null;

            button.interactable = false;
            onReset.Invoke();

            _unlocked = false;
            _rewarded = false;

            return this;
        }

        protected override void OnInitialized()
        {
            if (!Application.isPlaying) return;

            base.OnInitialized();

            placeholder.gameObject.SetActive(false);

            map = GetComponentInParent<Camera3dItemProgressionMap>();
            animator = GetComponent<Animator>();
            completeDisplayed = wasRewarded;

            switch (EventType)
            {
                case ProgressionEventType.GAME:
                    nameLabel.text = PuzzlePack.name;

                    if (PuzzlePack.packType == PackType.BOSS_AI_PACK || PuzzlePack.packType == PackType.AI_PACK)
                    {
                        //spawn gamepiece
                        gamePieceView = Instantiate(
                            GameContentManager.Instance.piecesDataHolder.GetGamePieceData(
                                PuzzlePack.GetHerdId()).player1Prefab, 
                            gamePieceParent);

                        gamePieceView.transform.localPosition = Vector3.zero;
                        gamePieceView.transform.localScale = Vector3.one * 140f;
                        gamePieceView.StartBlinking();
                    }

                    break;

                case ProgressionEventType.CURRENCY:
                    RewardsScreenWidget rewardWidget =
                        Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), content);

                    rewardWidget.SetData(reward);
                    rewardWidget.transform.localScale = Vector3.one * 1.8f;
                    rewardWidget.ResetAnchors();

                    break;
            }

            FindSize(rectTransform);
        }

        private void FindSize(RectTransform obj)
        {
            if (obj == null || sizeIgnore.Contains(obj)) return;

            if (_size.x < obj.sizeDelta.x) _size.x = obj.sizeDelta.x;
            if (_size.y < obj.sizeDelta.y) _size.y = obj.sizeDelta.y;

            //find chunker
            for (int i = 0; i < obj.childCount; i++)
            {
                FindSize(obj.GetChild(i).GetComponent<RectTransform>());
            }
        }

        private IEnumerator RewardAnimationRoutine()
        {
            yield return new WaitForSeconds(1f);

            //animate
            switch (EventType)
            {
                case ProgressionEventType.CURRENCY:
                    animator.SetTrigger("bounce");
                    Vector2 anchors = map.GetEventCameraRelativePosition(this);
                    PersistantOverlayScreen.instance.AnimateReward(false, reward.rewardType, reward.quantity, anchors);

                    break;
            }
        }

#if UNITY_EDITOR
        private IEnumerable GetPacksData()
        {
            ValueDropdownList<string> values = new ValueDropdownList<string>();

            if (!resouceDB) resouceDB = ResourceDB.Instance;

            //get all puzzlepacks
            foreach (ResourceItem pack in resouceDB.root
                .GetChild(Constants.PUZZLE_PACKS_ROOT_FOLDER)
                .GetChilds("", ResourceItem.Type.Folder))
            {
                values.Add(pack.Name, pack.Name);
            }

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