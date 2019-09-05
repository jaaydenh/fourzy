//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Rewards;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RewardsScreenGamePieceWidget : RewardsScreenWidget
    {
        public GamePieceView gamePiece { get; private set; }
        public RewardsManager.GamePieceReward reward;

        public override void SetData(RewardsManager.Reward data)
        {
            if (data.rewardType != CurrencyType.GAME_PIECE) return;

            reward = data as RewardsManager.GamePieceReward;

            base.SetData(data);

            GamePieceWidgetSmall widgetSmall = GameContentManager.InstantiatePrefab<GamePieceWidgetSmall>(GameContentManager.PrefabType.GAME_PIECE_SMALL, transform);
            widgetSmall.rectTransform.anchorMin = widgetSmall.rectTransform.anchorMax = Vector2.one * .5f;
            widgetSmall.SetData(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(reward.gamePieceID));
            
            widgetSmall.transform.localPosition = Vector3.zero;
            widgetSmall.transform.localScale = Vector3.one * .55f;
        }
    }
}
