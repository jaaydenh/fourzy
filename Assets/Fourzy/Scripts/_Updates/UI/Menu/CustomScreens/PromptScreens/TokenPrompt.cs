//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using mixpanel;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TokenPrompt : PromptScreen
    {
        public TMP_Text extra;
        public Image tokenImage;
        public TextMeshProUGUI areaText;
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

            var props = new Value();
            props["Token Type"] = data.name;
            Mixpanel.Track("Open Token Detail", props);

            tokenImage.sprite = GameContentManager.Instance.tokensDataHolder.GetTokenSprite(data);
            gameboardDefinition = GameContentManager.Instance.GetMiscBoard(data.gameboardInstructionID);

            string text = "";
            string areaString = "";
            string extraText = "";
            int charsCount = 70;
            
            string[] areas = GameContentManager.Instance.GetTokenThemes(data.tokenType).AddElementToEnd("\n");
            string[] _description = data.description.Split(' ');

            // foreach (string area in areas)
            //     if (text.Length + area.Length < charsCount)
            //         text += area + " ";
            //     else
            //         extraText += area + " ";

            if (areas.Length > 0) {
                for (int i = 0; i < 1; i++)
                {
                    if (i < areas.Length - 2) {
                        areaString += areas[i];
                    } else {
                        areaString += areas[i];
                    }
                }
            }

            areaText.text = areaString;

            // text += data.description;

            foreach (string word in _description)
                if (text.Length + word.Length < charsCount && extraText.Length == 0)
                    text += word + " ";
                else
                    extraText += word + " ";

            extra.text = extraText;
            Prompt(data.name, text, accept: null);

            //play instructions
            CancelRoutine("tokenInstructions");
            StartRoutine("tokenInstructions", InstructionRoutine(), () => gameboard.StopBoardUpdates());
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            CancelRoutine("tokenInstructions");
        }

        private IEnumerator InstructionRoutine()
        {
            //loop initial moves
            while (isOpened)
            {
                ClientFourzyGame game = new ClientFourzyGame(gameboardDefinition, UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                gameboard.Initialize(game);
                gameboard.PlayMoves(game.InitialTurns);

                yield return new WaitWhile(() => gameboard.IsRoutineActive("playMoves"));
                yield return new WaitForSeconds(2f);
            }
        }
    }
}
