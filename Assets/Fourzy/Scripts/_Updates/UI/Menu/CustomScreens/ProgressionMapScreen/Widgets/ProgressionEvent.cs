//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.ProgressionMap;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class ProgressionEvent : WidgetBase
    {
        public TMP_Text placeholderLabel;
        public RectTransform content;
        public ProgressionEventType EventType;

        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.PUZZLE_PACK), BoxGroup("PuzzlePack"), ValueDropdown("GetPacksData")]
        public string packName = "";
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.PUZZLE_PACK), BoxGroup("PuzzlePack")]
        public TMP_Text nameLabel;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.PUZZLE_PACK), BoxGroup("PuzzlePack")]
        public TweenBase starTween;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.PUZZLE_PACK), BoxGroup("PuzzlePack")]
        public SliderExtended progressSlider;
        [ShowIfGroup("PuzzlePack/EventType", Value = ProgressionEventType.PUZZLE_PACK), BoxGroup("PuzzlePack")]
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
        public List<Camera3dItemProgressionLine> enableLinesWhenComplete;
        public AdvancedEvent onUnlock;
        public AdvancedEvent onRewarded;
        public AdvancedEvent onReset;

        private BasicPuzzlePack _puzzlePack;
        private ButtonExtended buttonExtended;
        private GamePieceView gamePieceView;

        private bool _rewarded;
        private bool _unlocked;

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
                    case ProgressionEventType.PUZZLE_PACK:
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
                    case ProgressionEventType.PUZZLE_PACK:
                        return PuzzlePack.getUnlockRewardID;

                    case ProgressionEventType.CURRENCY:
                        return currencyRewardID;

                    default:
                        return otherRewardID;
                }
            }
        }

        protected void Start()
        {
            switch (EventType)
            {
                case ProgressionEventType.PUZZLE_PACK:
                    //if (IsPuzzlePack)
                    //{
                    nameLabel.text = PuzzlePack.name;
                    //}

                    if (PuzzlePack.packType == PackType.BOSS_AI_PACK || PuzzlePack.packType == PackType.AI_PACK)
                    {
                        //spawn gamepiece
                        gamePieceView = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(PuzzlePack.enabledPuzzlesData[0].PuzzlePlayer.HerdId).player1Prefab, gamePieceParent);

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

        public override void _Update()
        {
            base._Update();

            if (wasRewarded) Rewarded(false);
            else enableLinesWhenComplete.ForEach(line => line.SetColorLocked());

            if (!_unlocked && !_rewarded) return;

            switch (EventType)
            {
                case ProgressionEventType.PUZZLE_PACK:
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
                    case ProgressionEventType.PUZZLE_PACK:
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
            enableLinesWhenComplete.ForEach(line => line.SetColorUnlocked());

            switch (EventType)
            {
                case ProgressionEventType.PUZZLE_PACK:
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

                            //case PuzzlePacksDataHolder.PackType.AI_PACK:
                            //    print(PuzzlePack.name + " rewarded");

                            //    break;
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
                case ProgressionEventType.PUZZLE_PACK:
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

                    break;

                case ProgressionEventType.REWARD:
                    Rewarded(true);
                    PlayerPrefsWrapper.SetEventRewarded(id, true);

                    break;
            }
        }

        public void ResetGraphics()
        {
            onReset.Invoke();

            _unlocked = false;
            _rewarded = false;

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
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            buttonExtended = GetComponentInChildren<ButtonExtended>();
            placeholderLabel.gameObject.SetActive(false);

            //
        }

        //editor stuff
        //private IEnumerable GetPacksData()
        //{
        //    ValueDropdownList<int> values = new ValueDropdownList<int>();

        //    if (!packsData) return values;

        //    for (int packIndex = 0; packIndex < packsData.puzzlePacks.list.Count; packIndex++)
        //        values.Add(packsData.puzzlePacks.list[packIndex].name, packIndex);

        //    return values;
        //}
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

        [Button("Reset Anchor")]
        private void ResetAnchorsToCurrentPosition()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();

            if (!rectTransform) return;

            Rect parentRect = rectTransform.parent.GetComponent<RectTransform>().rect;
            rectTransform.anchorMin = rectTransform.anchorMax =
                new Vector2(rectTransform.localPosition.x / parentRect.width, (rectTransform.localPosition.y + parentRect.height) / parentRect.height);
            rectTransform.anchoredPosition = Vector2.zero;
        }

        public enum ProgressionEventType
        {
            NONE,
            PUZZLE_PACK,
            CURRENCY,
            REWARD,
        }
    }
}