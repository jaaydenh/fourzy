//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class ProgressionRewardWidget : MonoBehaviour
    {
        public Image image;

        [SerializeField]
        private AnimationCurve forwardCurve;
        [SerializeField]
        private AnimationCurve backCurve;
        [SerializeField]
        private AnimationCurve progressAnimation;

        private ScaleTween tween;

        public float AnimationTime => tween ? tween.playbackTime : 0f;

        private void Awake()
        {
            tween = GetComponent<ScaleTween>();
        }

        public void PlayForward()
        {
            tween.from = Vector3.zero;
            tween.to = Vector3.one;
            tween.curve = forwardCurve;
            tween.playbackTime = .5f;

            tween.PlayForward(true);
        }

        public void PlayBackward()
        {
            tween.from = Vector3.zero;
            tween.to = Vector3.one;
            tween.curve = backCurve;
            tween.playbackTime = .5f;

            tween.PlayForward(true);
        }

        public void AnimateProgress()
        {
            tween.from = Vector3.one;
            tween.to = Vector3.one * 2f;
            tween.curve = progressAnimation;
            tween.playbackTime = .34f;

            tween.PlayForward(true);
        }

        public void ShowGamepiece(string pieceID)
        {
            image.color = Color.clear;

            GamePieceView _gamepiece = Instantiate(
                GameContentManager.Instance.piecesDataHolder.GetGamePieceData(pieceID).player1Prefab,
                transform);
            _gamepiece.transform.localScale = Vector3.one * .5f;
            _gamepiece.StartBlinking();
        }

        public void ShowToken(TokenType type)
        {
            image.sprite = GameContentManager.Instance
                        .GetTokenData(type)
                        .GetTokenSprite();
        }

        public void ShowArea(Area area)
        {
            image.color = Color.clear;

            PracticeScreenAreaSelectWidget areaWidget = Instantiate(GameContentManager.GetPrefab<PracticeScreenAreaSelectWidget>("AREA_SELECT_WIDGET_SMALL"), transform);
            areaWidget.SetData(area, false);
            areaWidget.rectTransform.sizeDelta = Vector2.one;
            areaWidget.SetLocalPosition(Vector2.zero);

            Destroy(areaWidget.button);
        }
    }
}
