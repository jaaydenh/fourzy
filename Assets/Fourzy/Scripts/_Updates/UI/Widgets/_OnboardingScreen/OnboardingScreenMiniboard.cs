//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using FourzyGameModel.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenMiniboard : WidgetBase
    {
        [SerializeField]
        private GameboardView board;

        private GameBoardDefinition gameboardDefinition;
        private List<string> boards;

        public OnboardingScreenMiniboard SetBoards(List<string> boards)
        {
            this.boards = boards;

            return this;
        }

        public override void Show(float time)
        {
            base.Show(time);

            CancelRoutine("tokenInstructions");
            StartRoutine("tokenInstructions", BoardRoutine(), board.StopBoardUpdates);
        }

        public override void Hide(float time)
        {
            base.Hide(time);

            CancelRoutine("tokenInstructions");
        }

        private IEnumerator BoardRoutine()
        {
            //loop initial moves
            int boardIndex = 0;
            while (true)
            {
                gameboardDefinition = GameContentManager.Instance.GetMiscBoard(boards[boardIndex % boards.Count]);

                ClientFourzyGame game = new ClientFourzyGame(
                    gameboardDefinition,
                    UserManager.Instance.meAsPlayer,
                    new Player(2, "Player Two")
                    {
                        HerdId = InternalSettings.Current.DEFAULT_GAME_PIECE
                    });

                board.Initialize(game, false);
                board.FadeGamepieces(1f, .3f);
                board.FadeTokens(1f, .3f);

                //wait a bit for screen to fade in
                yield return new WaitForSeconds(.8f);

                //play before actions
                yield return board.OnPlayManagerReady();

                //wait a bit again
                yield return new WaitForSeconds(1f);

                //play initial moves
                yield return board.PlayInitialMoves();

                //repeat after X seconds
                yield return new WaitForSeconds(1.6f);
                board.FadeGamepieces(0f, .3f);
                board.FadeTokens(0f, .3f);

                yield return new WaitForSeconds(.8f);
                boardIndex++;
            }
        }
    }
}