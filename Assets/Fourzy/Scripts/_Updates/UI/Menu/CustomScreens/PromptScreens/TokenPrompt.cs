//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
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
        public RectTransform tokenParent;
        public Image tileBGImage;
        public TokensDataHolder.TokenData data { get; private set; }

        private GameboardView gameboard;
        private GameBoardDefinition gameboardDefinition;
        private TokenView currentView;

        protected override void Awake()
        {
            base.Awake();

            gameboard = GetComponentInChildren<GameboardView>();
        }

        public virtual void Prompt(TokensDataHolder.TokenData data)
        {
            this.data = data;

            ClearPrevious();

            currentView = Instantiate(GameContentManager.Instance.GetTokenPrefab(data.tokenType), tokenParent).SetData(TokenFactory.Create(data.tokenType.TokenTypeToString()));
            currentView.transform.localPosition = Vector3.zero;

            tileBGImage.enabled = data.showBackgroundTile;
            tileBGImage.color = data.backgroundTileColor;

            gameboardDefinition = GameContentManager.Instance.GetMiscBoard(data.gameboardInstructionID);

            List<ThemesDataHolder.GameTheme> themes = GameContentManager.Instance.GetTokenThemes(data.tokenType);

            //badges
            if (data.isSpell)
            {
                locationBadge.SetState(false);
                priceBadge.SetValue(data.price);

                if (themes.Count > 0) priceBadge.SetColor(themes[0].themeColor);
            }
            else
            {
                List<string> locations = GameContentManager.Instance.GetTokenAreaNames(data.tokenType);

                if (locations.Count == 6) locationBadge.SetValue("All");
                else locationBadge.SetValue(string.Join(", ", locations));

                if (themes.Count > 0) locationBadge.SetColor(themes[0].themeColor);

                priceBadge.SetState(false);
            }

            extra.text = LocalizationManager.Value(data.description);

            Prompt(LocalizationManager.Value(data.name), "");

            //play instructions
            CancelRoutine("tokenInstructions");
            StartRoutine("tokenInstructions", InstructionRoutine(), gameboard.StopBoardUpdates);
        }

        public virtual void Prompt(TokenView tokenView)
        {
            data = GameContentManager.Instance.tokensDataHolder.GetTokenData(tokenView.tokenType);

            ClearPrevious();

            currentView = Instantiate(tokenView, tokenParent);
            currentView.transform.localPosition = Vector3.zero;

            tileBGImage.enabled = data.showBackgroundTile;
            tileBGImage.color = data.backgroundTileColor;

            gameboardDefinition = GameContentManager.Instance.GetMiscBoard(data.gameboardInstructionID);

            List<ThemesDataHolder.GameTheme> themes = GameContentManager.Instance.GetTokenThemes(data.tokenType);

            //badges
            if (data.isSpell)
            {
                locationBadge.SetState(false);
                priceBadge.SetValue(data.price);

                if (themes.Count > 0) priceBadge.SetColor(themes[0].themeColor);
            }
            else
            {
                List<string> locations = GameContentManager.Instance.GetTokenAreaNames(data.tokenType);

                if (locations.Count == 6) locationBadge.SetValue("All");
                else locationBadge.SetValue(string.Join(", ", locations));

                if (themes.Count > 0) locationBadge.SetColor(themes[0].themeColor);

                priceBadge.SetState(false);
            }

            extra.text = LocalizationManager.Value(data.description);

            Prompt(LocalizationManager.Value(data.name), "");

            //play instructions
            CancelRoutine("tokenInstructions");
            StartRoutine("tokenInstructions", InstructionRoutine(), () => gameboard.StopBoardUpdates());
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            CancelRoutine("tokenInstructions");
        }

        public override PromptScreen Prompt()
        {
            base.Prompt();

            //StartCoroutine(AdjustOverflow());

            return this;
        }

        //private IEnumerator AdjustOverflow()
        //{
        //    yield return new WaitForEndOfFrame();

        //    string extraText = "";
        //    int charIndex = 0;
        //    float height = 0f;
        //    bool overflow = false;

        //    foreach (TMP_LineInfo line in promptText.textInfo.lineInfo)
        //    {
        //        height += line.lineHeight;

        //        if (height > promptText.rectTransform.sizeDelta.y)
        //        {
        //            overflow = true;
        //            extraText += promptText.text.Substring(charIndex, line.characterCount);
        //        }

        //        charIndex += line.characterCount;
        //    }

        //    if (overflow)
        //    {
        //        promptText.text = promptText.text.Substring(0, promptText.text.Length - extraText.Length);
        //        promptText.alignment = TextAlignmentOptions.TopFlush;

        //        extra.text = extraText;
        //    }
        //    else
        //    {
        //        promptText.alignment = TextAlignmentOptions.TopJustified;

        //        extra.text = "";
        //    }
        //}

        private void ClearPrevious()
        {
            if (currentView)
            {
                currentView._Destroy();
            }
        }

        private IEnumerator InstructionRoutine()
        {
            //loop initial moves
            while (isOpened)
            {
                ClientFourzyGame game = new ClientFourzyGame(gameboardDefinition, UserManager.Instance.meAsPlayer, new Player(2, "Player Two") { HerdId = "1"});
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
