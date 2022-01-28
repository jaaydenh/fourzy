//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TokenPrompt : PromptScreen
    {
        [SerializeField]
        private GameObject newTokenRibbon;
        [SerializeField]
        private ButtonExtended tryItButton;
        [SerializeField]
        private ButtonExtended showTokenInstructions;

        [SerializeField]
        private TMP_Text locationLabel;
        [SerializeField]
        private Image tokenImage;
        public TokensDataHolder.TokenData data { get; private set; }

        private GameboardView gameboard;
        private GameBoardDefinition gameboardDefinition;

        protected override void Awake()
        {
            base.Awake();

            gameboard = GetComponentInChildren<GameboardView>();
        }

        public virtual void Prompt(TokensDataHolder.TokenData data, bool canTryIt, bool ribbon, bool showCheckbox)
        {
            this.data = data;

            newTokenRibbon.gameObject.SetActive(ribbon);
            tryItButton.SetActive(canTryIt);
            showTokenInstructions.SetActive(showCheckbox);

            InitPrompt(GameContentManager.Instance.GetTokenThemes(data.tokenType));
        }

        public virtual void Prompt(TokenView tokenView)
        {
            data = GameContentManager.Instance.tokensDataHolder.GetTokenData(tokenView.Token.Type);

            newTokenRibbon.gameObject.SetActive(false);
            tryItButton.SetActive(false);

            InitPrompt(GameContentManager.Instance.GetTokenThemes(data.tokenType));
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            CancelRoutine("tokenInstructions");
        }

        /// <summary>
        /// Invoked from button
        /// </summary>
        public void TryIt()
        {
            CloseSelf();

            GameContentManager.Instance.StartTryItBoard(data.tokenType);
        }

        public void ToggleInstructionCheckbox()
        {
            SettingsManager.Toggle(SettingsManager.KEY_TOKEN_INSTRUCTION);
        }

        private void InitPrompt(List<AreasDataHolder.GameArea> themes)
        {
            gameboardDefinition = GameContentManager.Instance.GetInstructionBoard(data.tokenType.ToString());
            tokenImage.sprite = data.GetTokenSprite();

            locationLabel.text = GameContentManager.Instance.GetTokenAreaNames(data.tokenType)[0];

            if (themes.Count > 0)
            {
                locationLabel.color = themes[0].areaColor;
            }

            Prompt(LocalizationManager.Value(data.name), LocalizationManager.Value(data.description));

            //play instructions
            CancelRoutine("tokenInstructions");
            StartRoutine("tokenInstructions", InstructionRoutine(), gameboard.StopBoardUpdates);
        }

        private IEnumerator InstructionRoutine()
        {
            //loop initial moves
            while (isOpened)
            {
                ClientFourzyGame game = new ClientFourzyGame(
                    gameboardDefinition,
                    UserManager.Instance.meAsPlayer,
                    new Player(2, "Player Two")
                    {
                        HerdId = InternalSettings.Current.DEFAULT_GAME_PIECE
                    });

                gameboard.Initialize(game, false);

                //wait a bit for screen to fade in
                yield return new WaitForSeconds(.8f);

                //play before actions
                yield return gameboard.OnPlayManagerReady();

                //wait a bit again
                yield return new WaitForSeconds(.3f);

                //play initial moves
                yield return gameboard.PlayInitialMoves();
            }
        }
    }
}
