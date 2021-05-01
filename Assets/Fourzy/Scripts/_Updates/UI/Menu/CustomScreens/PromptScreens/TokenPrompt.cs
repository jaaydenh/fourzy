//@vadym udod

using Fourzy._Updates.ClientModel;
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
        public Badge locationBadge;
        public Badge priceBadge;
        public TMP_Text extra;
        public Image tokenImage;
        public TokensDataHolder.TokenData data { get; private set; }

        private GameboardView gameboard;
        private GameBoardDefinition gameboardDefinition;

        protected override void Awake()
        {
            base.Awake();

            gameboard = GetComponentInChildren<GameboardView>();
        }

        public virtual void Prompt(TokensDataHolder.TokenData data)
        {
            this.data = data;

            gameboardDefinition = GameContentManager.Instance.GetMiscBoard(data.gameboardInstructionID);

            InitPrompt(GameContentManager.Instance.GetTokenThemes(data.tokenType));
        }

        public virtual void Prompt(TokenView tokenView)
        {
            data = GameContentManager.Instance.tokensDataHolder.GetTokenData(tokenView.tokenType);

            InitPrompt(GameContentManager.Instance.GetTokenThemes(data.tokenType));
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            CancelRoutine("tokenInstructions");
        }

        private void InitPrompt(List<AreasDataHolder.GameArea> themes)
        {
            gameboardDefinition = GameContentManager.Instance.GetMiscBoard(data.gameboardInstructionID);
            tokenImage.sprite = data.GetTokenSprite();

            //badges
            if (data.isSpell)
            {
                locationBadge.SetState(false);
                priceBadge.SetValue(data.price);

                if (themes.Count > 0)
                {
                    priceBadge.SetColor(themes[0].areaColor);
                }
            }
            else
            {
                List<string> locations = GameContentManager.Instance.GetTokenAreaNames(data.tokenType);

                if (locations.Count == 6)
                {
                    locationBadge.SetValue("All");
                }
                else
                {
                    locationBadge.SetValue(string.Join(", ", locations));
                }

                if (themes.Count > 0)
                {
                    locationBadge.SetColor(themes[0].areaColor);
                }

                priceBadge.SetState(false);
            }

            extra.text = LocalizationManager.Value(data.description);

            Prompt(LocalizationManager.Value(data.name), "");

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
