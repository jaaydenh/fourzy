//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameWinLoseScreen : MenuScreen
    {
        public Color winColor;
        public Color loseColor;
        public TMP_Text stateLabel;
        public TMP_Text infoLabel;
        public TMP_Text tapToContinueLabel;
        public RectTransform piecesParent;

        public GameObject buttonsRow;
        public ButtonExtended nextGameButton;
        public ButtonExtended rematchButton;
        public ButtonExtended exitButton;

        public AlphaTween tapToContinue;

        private IClientFourzy game;
        private PromptScreen waitingScreen;
        private TurnBaseScreen turnBasedTab;
        private bool showButtonRow = false;
        private int botRematchesLeft;

        private List<GamePieceView> gamePieces = new List<GamePieceView>();

        protected override void Awake()
        {
            base.Awake();

            FourzyPhotonManager.onRoomPropertiesUpdate += OnRoomPropertiesUpdate;
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onRoomPropertiesUpdate -= OnRoomPropertiesUpdate;
        }

        public void Open(IClientFourzy game)
        {
            menuController.OpenScreen(this);

            this.game = game;
            string bgTapText = "";
            infoLabel.text = "";

            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>" +
                            $"{LocalizationManager.Value("draw")}</color>";
                        bgTapText = LocalizationManager.Value("continue");
                    }
                    else
                    {
                        if (game.IsWinner() && 
                            (game.myMembers.Count > 0 || game.puzzleData.pack.complete))
                        {
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                $"{LocalizationManager.Value("won")}</color>";
                            bgTapText = LocalizationManager.Value("continue");
                        }
                        else
                        {
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>" +
                                $"{LocalizationManager.Value("lost")}</color>";

                            if (game.myMembers.Count > 0)
                            {
                                bgTapText = LocalizationManager.Value("rematch");
                            }
                            else
                            {
                                bgTapText = LocalizationManager.Value("back_to_menu");
                            }
                        }
                    }

                    break;

                case GameMode.VERSUS:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>" +
                            $"{LocalizationManager.Value("draw")}</color>";
                    }
                    else
                    {
                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.LOCAL_GAME:
                                if (game.IsWinner())
                                {
                                    stateLabel.text = $"{LocalizationManager.Value("player_one")} <color=#" +
                                        $"{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                        $"{LocalizationManager.Value("won")}</color>";
                                }
                                else
                                {
                                    stateLabel.text = $"{LocalizationManager.Value("player_two")} <color=#" +
                                        $"{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                        $"{LocalizationManager.Value("won")}</color>";
                                }

                                break;

                            case GameTypeLocal.REALTIME_BOT_GAME:
                            case GameTypeLocal.REALTIME_LOBBY_GAME:
                            case GameTypeLocal.REALTIME_QUICKMATCH:
                                if (game.IsWinner())
                                {
                                    stateLabel.text = $"{game.me.DisplayName} <color=#" +
                                        $"{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                        $"{LocalizationManager.Value("won")}</color>";
                                }
                                else
                                {
                                    stateLabel.text = $"{game.opponent.DisplayName} <color=#" +
                                        $"{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                        $"{LocalizationManager.Value("won")}</color>";
                                }

                                break;
                        }
                    }

                    break;

                default:
                    if (game.draw)
                    {
                        stateLabel.text = $"<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>" +
                            $"{LocalizationManager.Value("draw")}</color>";
                    }
                    else
                    {
                        if (game.IsWinner())
                        {
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(winColor)}>" +
                                $"{LocalizationManager.Value("won")}</color>";
                        }
                        else
                        {
                            stateLabel.text = $"You<color=#{ColorUtility.ToHtmlStringRGB(loseColor)}>" +
                                $"{LocalizationManager.Value("lost")}</color>";
                        }
                    }

                    break;
            }

            tapToContinue.AtProgress(0f);
            if (RewardsScreen.WillDisplayRewardsScreen(game) || 
                (game.puzzleData && game.puzzleData.pack))
            {
                //display 'tap to continue'
                if (string.IsNullOrEmpty(bgTapText))
                {
                    switch (Application.platform)
                    {
                        case RuntimePlatform.IPhonePlayer:
                        case RuntimePlatform.Android:
                            bgTapText = LocalizationManager.Value("tap_to_continue");

                            break;

                        default:
                            bgTapText = LocalizationManager.Value("tap_to_continue");

                            break;

                    }
                }

                tapToContinueLabel.text = bgTapText;
                tapToContinue.PlayForward(true);
                SetButtonRowState(false);

                showButtonRow = true;
            }
            else
            {
                SetButtonRowState(true);
            }

            //gamepieces
            foreach (GamePieceView gamepiece in gamePieces)
            {
                Destroy(gamepiece.gameObject);
            }
            gamePieces.Clear();

            //get winning gamepieces
            List<GamePieceView> winningGamepieces = GamePlayManager.Instance.board.GetWinningPieces();

            //move them to ui layer
            for (int index = 0; index < winningGamepieces.Count; index++)
            {
                GamePieceView gamepieceView = Instantiate(winningGamepieces[index], piecesParent);

                gamepieceView.transform.position = winningGamepieces[index].transform.position;
                gamepieceView.transform.localScale = (GameManager.Instance.Landscape ? 89f : 75f) * 
                    Vector3.one;
                gamepieceView.PlayWinAnimation(index * .15f + .15f);

                gamePieces.Add(gamepieceView);
            }
        }

        public void SetInfoLabel(string text)
        {
            infoLabel.text = text;
        }

        public void CloseIfOpened()
        {
            if (isCurrent)
            {
                menuController.CloseCurrentScreen(true);
            }
        }

        public override void Open()
        {
            base.Open();

            if (showButtonRow)
            {
                tapToContinue.AtProgress(0f);
                SetButtonRowState(true);

                showButtonRow = false;
            }
        }

        public void OnGameStarted()
        {
        }

        public void Rematch()
        {
            switch (game._Type)
            {
                case GameType.REALTIME:
                    if (!waitingScreen)
                    {
                        //send event
                        AnalyticsManager.Instance.LogEvent(
                            "requestRealtimeRematch",
                            AnalyticsManager.AnalyticsProvider.ALL,
                            new KeyValuePair<string, object>("isWinner", game.IsWinner()),
                            new KeyValuePair<string, object>(
                                "isBotOpponent",
                                GameManager.Instance.ExpectedGameType == GameTypeLocal.REALTIME_BOT_GAME),
                            new KeyValuePair<string, object>("isPrivate", false));

                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.REALTIME_LOBBY_GAME:
                            case GameTypeLocal.REALTIME_QUICKMATCH:
                                FourzyPhotonManager.SetClientRematchReady();

                                //send rematch request
                                var result = PhotonNetwork.RaiseEvent(
                                    Constants.REMATCH_REQUEST,
                                    null,
                                    new Photon.Realtime.RaiseEventOptions()
                                    {
                                        Flags = new Photon.Realtime.WebFlags(Photon.Realtime.WebFlags.HttpForwardConst)
                                        {
                                            HttpForward = true
                                        }
                                    },
                                    SendOptions.SendReliable);

                                break;

                            case GameTypeLocal.REALTIME_BOT_GAME:
                                StartRoutine(
                                    "botRematch",
                                    InternalSettings.Current.BOT_SETTINGS.randomRematchAcceptTime,
                                    () =>
                                    {
                                        if (botRematchesLeft > 0 && waitingScreen)
                                        {
                                            waitingScreen.CloseSelf();
                                            waitingScreen = null;

                                            GamePlayManager.Instance.LoadGame(null);

                                            botRematchesLeft -= 1;
                                        }
                                        else
                                        {
                                            GamePlayManager.Instance.BackButtonOnClick();
                                        }
                                    },
                                    null);

                                break;
                        }

                        GamePlayManager.Instance.rematchRequested = true;

                        //open waiting prompt
                        waitingScreen = menuController.GetOrAddScreen<PromptScreen>()
                            .Prompt(
                                "Rematch Requested",
                                "Waiting for other player to accept request.",
                                "Leave Game",
                                null,
                                () => GamePlayManager.Instance.BackButtonOnClick(), null)
                            .CloseOnAccept();
                    }

                    break;

                default:
                    if (isCurrent)
                    {
                        menuController.CloseCurrentScreen(true);
                    }

                    GamePlayManager.Instance.Rematch(true);

                    break;
            }
        }

        public void NextGame()
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    turnBasedTab.OpenNext();

                    break;

                default:
                    GamePlayManager.Instance.LoadGame(game.Next());

                    break;
            }

            if (isCurrent) menuController.CloseCurrentScreen(true);
        }

        public void OnExitTap()
        {
            GamePlayManager.Instance.BackButtonOnClick();
        }

        public void OnBGTap()
        {
            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    menuController.CloseCurrentScreen();

                    if (game.IsWinner())
                    {
                        if (game.puzzleData.pack.complete)
                            menuController.GetOrAddScreen<GauntletWinPrompt>()._Prompt(game);
                        else
                        {
                            if (game.myMembers.Count == 0)
                                OnGauntletLost();
                            else
                                menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                    game.puzzleData.pack, 
                                    () => GamePlayManager.Instance.gameplayScreen.OnBack());
                        }
                    }
                    else
                    {
                        if (game.myMembers.Count == 0)
                        {
                            OnGauntletLost();
                        }
                        else
                        {
                            Rematch();
                        }
                    }

                    break;

                case GameMode.AI_PACK:
                case GameMode.BOSS_AI_PACK:
                    if (!game.draw && game.IsWinner())
                    {
                        if (game.puzzleData.lastInPack)
                        {
                            //force update map
                            GameManager.Instance.currentMap.UpdateWidgets();

                            //open screen for next event
                            BasicPuzzlePack nextPack = GameManager.Instance.currentMap
                                .GetNextPack(game.puzzleData.pack.packId);

                            if (nextPack == null)
                            {
                                GamePlayManager.Instance.BackButtonOnClick();
                                return;
                            }
                            else
                            {
                                nextPack.StartNextUnsolvedPuzzle();
                            }

                            if (nextPack)
                            {
                                switch (nextPack.packType)
                                {
                                    case PackType.AI_PACK:
                                    case PackType.BOSS_AI_PACK:
                                        menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                            nextPack,
                                            () => GamePlayManager.Instance.BackButtonOnClick(),
                                            () => menuController.CloseCurrentScreen());

                                        break;

                                    case PackType.PUZZLE_PACK:
                                        menuController.GetOrAddScreen<PrePackPrompt>().Prompt(
                                            nextPack,
                                            () => GamePlayManager.Instance.BackButtonOnClick(),
                                            () => menuController.CloseCurrentScreen());

                                        break;
                                }
                            }
                            else
                            {
                                GamePlayManager.Instance.BackButtonOnClick();
                            }
                        }
                        else
                        {
                            menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                game.puzzleData.pack,
                                () => GamePlayManager.Instance.BackButtonOnClick());
                        }
                    }
                    else
                    {
                        Rematch();
                    }

                    break;

                default:
                    if (rematchButton.gameObject.activeInHierarchy)
                    {
                        switch (game._Type)
                        {
                            case GameType.REALTIME:
                                //GamePlayManager.Instance.BackButtonOnClick();

                                break;

                            default:
                                Rematch();

                                break;
                        }
                    }
                    else if (nextGameButton.gameObject.activeInHierarchy)
                    {
                        NextGame();
                    }
                    else
                    {
                        if (game.puzzleData && !game.puzzleData.lastInPack)
                        {
                            menuController.GetOrAddScreen<VSGamePrompt>().Prompt(
                                game.puzzleData.pack,
                                () => GamePlayManager.Instance.gameplayScreen.OnBack());
                        }
                        else
                        {
                            GamePlayManager.Instance.BackButtonOnClick();
                        }
                    }

                    break;
            }
        }

        private void OnGauntletLost()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.Android:
                    menuController.GetOrAddScreen<GauntletLostPrompt>()._Prompt(game);

                    break;

                default:
                    menuController
                        .GetOrAddScreen<PromptScreen>()
                        .Prompt("Your journey has ended", "You have no more Fourzies to play.",
                            "Retry",
                            "Quit",
                            () =>
                            {
                                game.puzzleData.pack.ResetPlayerPrefs();
                                game.AddMembers(InternalSettings.Current.GAUNTLET_DEFAULT_MOVES_COUNT);

                                menuController.GetOrAddScreen<VSGamePrompt>().Prompt(game.puzzleData.pack);
                            },
                            () => GamePlayManager.Instance.BackButtonOnClick());

                    break;
            }
        }

        private void SetButtonRowState(bool value)
        {
            buttonsRow.SetActive(value);
            if (value)
            {
                switch (game._Type)
                {
                    case GameType.PRESENTATION:
                        rematchButton.SetActive(false);
                        break;

                    case GameType.TURN_BASED:
                        nextGameButton.SetActive(turnBasedTab.nextChallenge != null);
                        break;

                    case GameType.REALTIME:
                        if (GameManager.Instance.ExpectedGameType == GameTypeLocal.REALTIME_LOBBY_GAME) {
                            rematchButton.SetActive(true);
                        } else {
                            rematchButton.SetActive(false);
                        }
                        exitButton.SetActive(true);
                        break;

                    case GameType.TRY_TOKEN:
                        rematchButton.SetActive(false);
                        exitButton.SetActive(true);
                        break;

                    case GameType.PASSANDPLAY:
                        rematchButton.SetActive(true);
                        exitButton.SetActive(true);
                        break;

                    case GameType.ONBOARDING:
                        switch (game._Mode)
                        {
                            case GameMode.VERSUS:
                                exitButton.SetActive(true);

                                break;
                        }

                        break;

                    default:
                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.REALTIME_BOT_GAME:
                                switch (GameManager.Instance.botGameType)
                                {
                                    case GameManager.BotGameType.REGULAR:
                                        rematchButton.SetActive(true);

                                        break;

                                    default:
                                        rematchButton.SetActive(false);

                                        break;
                                }
                                break;
                            default:
                                nextGameButton.SetActive(false);
                                exitButton.SetActive(false);
                                rematchButton.SetActive(false); 

                                break;
                        }
                        break;
                }
            }
            else
            {
                nextGameButton.SetActive(false);
                rematchButton.SetActive(false);
                exitButton.SetActive(false);
            }
        }

        private void OnRoomPropertiesUpdate(Hashtable values)
        {
            if (FourzyPhotonManager.CheckPlayersRematchReady() && 
                waitingScreen && 
                waitingScreen.isOpened)
            {
                waitingScreen.CloseSelf();
            }

            waitingScreen = null;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            turnBasedTab = menuController.GetOrAddScreen<TurnBaseScreen>();
            botRematchesLeft = InternalSettings.Current.BOT_SETTINGS.randomRematchTimes;
        }
    }
}
