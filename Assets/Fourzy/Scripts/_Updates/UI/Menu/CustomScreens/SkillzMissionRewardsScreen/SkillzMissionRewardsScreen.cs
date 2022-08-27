//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzMissionRewardsScreen : MenuScreen
    {
        [SerializeField]
        private SkillzMissionRewardsFoldout foldoutPrefab;
        [SerializeField]
        private RectTransform foldoutsParent;

        private Dictionary<SkillzGamepieceGroup, (SkillzMissionRewardsFoldout foldout, IEnumerable<GamePieceWidgetSmall> pieces)> foldouts;

        public List<GamePieceWidgetSmall> GamePieces { get; private set; } = new List<GamePieceWidgetSmall>();

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                SkillzGameController.Instance.Test();
            }
        }
#endif

        public override void Open()
        {
            UpdatePieces();

            base.Open();

            foreach (GamePieceWidgetSmall widget in GamePieces)
            {
                //highlight selected one
                widget.SetSelectedState(widget.Data.Id == UserManager.Instance.gamePieceId);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            CreateGamePieces();
            CreateFoldouts();
        }

        private void CreateGamePieces()
        {
            foreach (GamePieceData prefabData in GameContentManager.Instance.SkillzMissionRewardsGamePieces())
            {
                GamePieceWidgetSmall widget = GameContentManager.InstantiatePrefab<GamePieceWidgetSmall>(
                    "GAME_PIECE_SMALL",
                    transform);
                widget.SetData(prefabData);
                widget.SetOnTap(OnWidgetTap);

                widget.UpdateLabels = false;

                GamePieces.Add(widget);
                widgets.Add(widget);
            }
        }

        private void CreateFoldouts()
        {
            foldouts = new Dictionary<SkillzGamepieceGroup, (SkillzMissionRewardsFoldout foldout, IEnumerable<GamePieceWidgetSmall> pieces)>();

            foreach (SkillzGamepieceGroup group in GameContentManager.Instance.skillzMissionRewardsDataHolder.Groups)
            {
                SkillzMissionRewardsFoldout foldout = Instantiate(foldoutPrefab, foldoutsParent);
                IEnumerable<GamePieceWidgetSmall> pieces = GamePieces.Where(gamepiece => group.pieces.Contains(gamepiece.Data.Id));

                foldouts.Add(group, (foldout, pieces));

                //Position gamepieces.
                foldout.AddObjects(pieces.Select(gamepiece => gamepiece.rectTransform).ToArray());
                foldout.ToggleFoldout();
            }

            //Disable foldout prefab.
            foldoutPrefab.SetActive(false);
        }

        private void UpdatePieces()
        {
            foreach (var foldoutInfo in foldouts)
            {
                // Update gamepieces 
                if ((foldoutInfo.Key.cashGames && SkillzGameController.Instance.PlayerData_CashGamesPlayed >= foldoutInfo.Key.gameCount) ||
                    (!foldoutInfo.Key.cashGames && SkillzGameController.Instance.PlayerData_GamesPlayed >= foldoutInfo.Key.gameCount))
                {
                    foreach (GamePieceWidgetSmall gamepiece in foldoutInfo.Value.pieces)
                    {
                        gamepiece.Data.Pieces = gamepiece.Data.PiecesToUnlock;
                    }
                }

                int value = foldoutInfo.Key.cashGames ? SkillzGameController.Instance.PlayerData_CashGamesPlayed : SkillzGameController.Instance.PlayerData_GamesPlayed;

                foldoutInfo.Value.foldout.SetLabelText($"{LocalizationManager.Value(foldoutInfo.Key.categoryId)}({value}/{foldoutInfo.Key.gameCount})");
            }
        }

        private void OnWidgetTap(GamePieceWidgetSmall widget)
        {
            menuController.GetOrAddScreen<UpgradeGamePiecePromptScreen>().Prompt(widget.Data);
        }
    }
}
