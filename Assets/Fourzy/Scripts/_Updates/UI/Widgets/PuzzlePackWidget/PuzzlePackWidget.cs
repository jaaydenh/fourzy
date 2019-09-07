//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(ButtonExtended))]
    public class PuzzlePackWidget : WidgetBase
    {
        public Image bgImage;
        public RectTransform gamepieceParent;

        public PuzzlePacksDataHolder.PuzzlePack puzzlePack { get; private set; }
        public ButtonExtended button { get; private set; }

        private int completeAnimation = Animator.StringToHash("PuzzlePackCompleteAnimation");
        private Animator animator;

        public void SetData(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            SetDataMinimal(puzzlePack);

            switch (puzzlePack.unlockRequirement)
            {
                case UnlockRequirementsEnum.NONE:
                    DisplayProgression(puzzlePack);
                    break;

                case UnlockRequirementsEnum.STARS:
                    if (puzzlePack.puzzlePacksHolder.totalPuzzlesCompleteCount >= puzzlePack.quantity)
                        DisplayProgression(puzzlePack);
                    else
                        button.GetBadge("starsLocked").badge.SetValue(puzzlePack.quantity);

                    break;

                case UnlockRequirementsEnum.COINS:
                    if (PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                        DisplayProgression(puzzlePack);
                    else
                    {
                        //can be unlocked
                        button.interactable = true;
                        button.GetBadge("coinsLocked").badge.SetValue(puzzlePack.quantity);
                    }

                    break;

                case UnlockRequirementsEnum.GEMS:
                    if (PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                        DisplayProgression(puzzlePack);
                    else
                    {
                        //can be unlocked
                        button.interactable = true;
                        button.GetBadge("gemsLocked").badge.SetValue(puzzlePack.quantity);
                    }

                    break;
            }
        }

        /// <summary>
        /// Similar to SetData, but shows less of it
        /// </summary>
        /// <param name="puzzlePack"></param>
        public void SetDataMinimal(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            this.puzzlePack = puzzlePack;

            bgImage.sprite = puzzlePack.packBG;

            button.GetLabel().label.fontSharedMaterial = puzzlePack.labelMaterial;
            button.SetLabel(puzzlePack.name);

            button.GetBadge("starsLocked").badge.SetState(false);
            button.GetBadge("coinsLocked").badge.SetState(false);
            button.GetBadge("gemsLocked").badge.SetState(false);

            switch (puzzlePack.packType)
            {
                case PackType.AI_PACK:
                case PackType.BOSS_AI_PACK:
                    //add gamepiece
                    GamePieceView gamePiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(puzzlePack.enabledPuzzlesData[0].PuzzlePlayer.HerdId).player1Prefab, gamepieceParent);

                    gamePiece.transform.localPosition = Vector3.zero;
                    gamePiece.transform.localScale = Vector3.one * 90f;
                    gamePiece.StartBlinking();

                    break;
            }
        }

        public void OpenPuzzlePack()
        {
            //try to unlock
            switch (puzzlePack.unlockRequirement)
            {
                case UnlockRequirementsEnum.COINS:
                case UnlockRequirementsEnum.GEMS:
                    if (!PlayerPrefsWrapper.PuzzlePackUnlocked(puzzlePack.name))
                    {
                        //show unlock popup
                        menuScreen.menuController.GetScreen<UnlockPuzzlePackPrompScreen>().Prompt(puzzlePack);
                        return;
                    }
                    break;
            }

            switch (puzzlePack.packType)
            {
                case PackType.AI_PACK:
                case PackType.BOSS_AI_PACK:
                    menuScreen.menuController.GetScreen<VSGamePrompt>().Prompt(puzzlePack);

                    break;

                case PackType.PUZZLE_PACK:
                    //only open prepack prompt if there are any rewards in puzzle pack
                    if (puzzlePack.allRewards.Count > 0)
                        menuScreen.menuController.GetScreen<PrePackPrompt>().Prompt(puzzlePack);
                    else
                        puzzlePack.StartNextUnsolvedPuzzle();

                    break;
            }
        }

        public void PlayCompleteAnimation()
        {
            Debug.Log($"Puzzle pack {puzzlePack.name}:ID-{puzzlePack.packID} complete.");

            StartCoroutine(CompleteRoutine());
        }

        private void DisplayProgression(PuzzlePacksDataHolder.PuzzlePack puzzlePack)
        {
            button.interactable = true;

            switch (puzzlePack.packType)
            {
                //only puzzle pack have progress for now
                case PackType.PUZZLE_PACK:
                    button.GetBadge("completeCounter").badge.SetValue($"{puzzlePack.puzzlesComplete.Count} / {puzzlePack.enabledPuzzlesData.Count}");

                    break;
            }

            //finished
            button.GetBadge("finished").badge.SetState(puzzlePack.complete);

            //is opened
            button.GetBadge("new").badge.SetState(!PlayerPrefsWrapper.PuzzlePackOpened(puzzlePack.packID));
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
            animator = GetComponent<Animator>();
        }

        private IEnumerator CompleteRoutine()
        {
            animator.Play(completeAnimation);

            yield break;
        }
    }
}
