using UnityEngine;
using UnityEngine.UI;
using HedgehogTeam.EasyTouch;

namespace Fourzy
{
    public class View2 : UIView
    {
        public static View2 instance;

        [SerializeField]
        private Text gamePiecesTabText;

        [SerializeField]
        private Text tokensTabText;

        [SerializeField]
        private GameObject gamePiecesGrid;

        [SerializeField]
        private GamePieceUI gamePiecePrefab;

        [SerializeField]
        private GameObject tokensGrid;

        [SerializeField]
        private TokenUI tokenPrefab;

        [SerializeField]
        private ScrollRect scrollRect;

        private bool wasContentLoaded = false;

        void Start()
        {
            instance = this;
            keepHistory = true;
        }

        public override void ShowAnimated(AnimationDirection sourceDirection)
        {
            //ViewController.instance.currentActiveView = TotalView.view2;
            ViewController.instance.SetActiveView(TotalView.view2);
            base.ShowAnimated(sourceDirection);
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();

            if (!wasContentLoaded)
            {
                this.LoadTokens();
                this.LoadGamePieces();
                wasContentLoaded = true;
            }
        }

        public void LoadGamePiecesButton() 
        {
            gamePiecesGrid.SetActive(true);
            tokensGrid.SetActive(false);
            scrollRect.content = gamePiecesGrid.GetComponent<RectTransform>();
            tokensTabText.fontSize = 35;
            gamePiecesTabText.fontSize = 40;
            gamePiecesTabText.GetComponent<Outline>().enabled = true;
            tokensTabText.GetComponent<Outline>().enabled = false;
        }

        public void LoadTokensButton() 
        {
            gamePiecesGrid.SetActive(false);
            tokensGrid.SetActive(true);
            scrollRect.content = tokensGrid.GetComponent<RectTransform>();
            tokensTabText.fontSize = 40;
            gamePiecesTabText.fontSize = 35;
            gamePiecesTabText.GetComponent<Outline>().enabled = false;
            tokensTabText.GetComponent<Outline>().enabled = true;
        }

        private void LoadTokens()
        {
            Transform tokenGridTransform = tokensGrid.transform;
            var tokensData = GameContentManager.Instance.GetAllTokens();
            foreach (var token in tokensData)
            {
                TokenUI tokenUI = Instantiate(tokenPrefab);
                tokenUI.InitWithTokenData(token);
                tokenUI.transform.SetParent(tokenGridTransform, false);
            }
        }

        private void LoadGamePieces()
        {
            bool isEnabledPieceSelector = false;
            int gamePieceId = UserManager.instance.gamePieceId;
            Transform gamePieceGridTransform = gamePiecesGrid.transform;
            var gamePieceData = GameContentManager.Instance.GetAllGamePieces();
            for (int i = 0; i < gamePieceData.Length; i++)
            {
                var piece = gamePieceData[i];
                GamePieceUI gamePieceUI = Instantiate(gamePiecePrefab);
                gamePieceUI.id = piece.ID.ToString();
                gamePieceUI.name = piece.Name;
                gamePieceUI.gamePieceImage.sprite = GameContentManager.Instance.GetGamePieceSprite(piece.ID);
                gamePieceUI.isEnabledPieceSelector = isEnabledPieceSelector;

                gamePieceUI.transform.SetParent(gamePieceGridTransform, false);

                if (isEnabledPieceSelector)
                {
                    var toggle = gamePieceUI.GetComponent<Toggle>();
                    ToggleGroup tg = gamePiecesGrid.GetComponent<ToggleGroup>();
                    toggle.group = tg;

                    if (piece.ID == gamePieceId)
                    {
                        gamePieceUI.ActivateSelector();
                    }
                }
            }
        }
    }
}
