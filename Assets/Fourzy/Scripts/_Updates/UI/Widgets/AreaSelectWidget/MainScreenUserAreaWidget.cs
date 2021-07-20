//@vadym udod

using Fourzy._Updates.Mechanics._GamePiece;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class MainScreenUserAreaWidget : WidgetBase
    {
        [SerializeField]
        private RectTransform pieceParent;

        [SerializeField]
        private TMP_Text playerNameLabel;
        [SerializeField]
        private TMP_Text pieceNameLabel;
        [SerializeField]
        private TMP_Text ratingLabel;

        [SerializeField]
        private TMP_Text winsLabel;
        [SerializeField]
        private TMP_Text losesLabel;
        [SerializeField]
        private TMP_Text drawsLabel;

        private GamePieceView currentGamepiece;

        protected void Start()
        {
            UserManager.onRatingUpdate += OnRatingUpate;
            UserManager.onDisplayNameChanged += OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID += OnUpdateUserGamePieceID;
            UserManager.onWinsUpdate += OnWinsUpdate;
            UserManager.onLosesUpdate += OnLosesUpdate;
            UserManager.onDrawsUpdate += OnDrawsUpdate;
        }

        protected void OnDestroy()
        {
            UserManager.onRatingUpdate -= OnRatingUpate;
            UserManager.onDisplayNameChanged -= OnUpdateUserInfo;
            UserManager.OnUpdateUserGamePieceID -= OnUpdateUserGamePieceID;
        }

        private void OnUpdateUserInfo()
        {
            playerNameLabel.text = UserManager.Instance.userName;
        }

        private void OnRatingUpate(int rating)
        {
            if (UserManager.Instance.lastCachedRating == -1)
            {

                ratingLabel.text = $"{LocalizationManager.Value("rating")}: ...";
            }
            else
            {
                ratingLabel.text = $"{LocalizationManager.Value("rating")}: {UserManager.Instance.lastCachedRatingFiltered}";
            }
        }

        private void OnUpdateUserGamePieceID(string gamePieceID)
        {
            if (currentGamepiece)
            {
                Destroy(currentGamepiece.gameObject);
            }

            GamePieceData _data = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(gamePieceID);
            currentGamepiece = Instantiate(_data.player1Prefab, pieceParent);

            currentGamepiece.transform.localPosition = Vector3.zero;
            currentGamepiece.StartBlinking();

            pieceNameLabel.text = _data.name;
        }

        private void OnWinsUpdate(int wins)
        {
            winsLabel.text = wins + "";
        }

        private void OnLosesUpdate(int loses)
        {
            losesLabel.text = loses + "";
        }

        private void OnDrawsUpdate(int draw)
        {
            drawsLabel.text = draw + "";
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UserManager user = UserManager.Instance;
            OnUpdateUserGamePieceID(user.gamePieceID);
            OnUpdateUserInfo();

            OnRatingUpate(-1);
            OnWinsUpdate(UserManager.Instance.playfabWinsCount);
            OnLosesUpdate(UserManager.Instance.playfabLosesCount);
            OnDrawsUpdate(UserManager.Instance.playfabDrawsCount);
        }
    }
}
