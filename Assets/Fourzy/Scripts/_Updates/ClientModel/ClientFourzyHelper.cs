//@vadym udod

using FourzyGameModel.Model;
using System.Collections.Generic;


namespace Fourzy._Updates.ClientModel
{
    public static class ClientFourzyHelper
    {
        public static List<BoardSpace> BoardContent(IClientFourzy game)
        {
            List<BoardSpace> result = new List<BoardSpace>();

            for (int row = 0; row < game.Rows; row++)
            {
                for (int col = 0; col < game.Columns; col++)
                {
                    result.Add(game._State.Board.ContentsAt(row, col));
                }
            }

            return result;
        }

        public static Piece ActivePlayerPiece(IClientFourzy game)
        {
            Player activePlayer = game.activePlayer;

            return new Piece(
                    game._State.ActivePlayerId,
                    int.Parse(string.IsNullOrEmpty(activePlayer.HerdId) ?
                        InternalSettings.Current.DEFAULT_GAME_PIECE :
                        activePlayer.HerdId));
        }

        public static Piece PlayerPiece(IClientFourzy game)
        {
            Player _player = game.me;

            return new Piece(
                _player.PlayerId,
                int.Parse(string.IsNullOrEmpty(_player.HerdId) ?
                    InternalSettings.Current.DEFAULT_GAME_PIECE :
                    _player.HerdId));
        }

        public static Piece OpponentPiece(IClientFourzy game)
        {
            Player _player = game.opponent;

            return new Piece(
                _player.PlayerId,
                int.Parse(string.IsNullOrEmpty(_player.HerdId) ?
                    InternalSettings.Current.DEFAULT_GAME_PIECE :
                    _player.HerdId));
        }

        public static void SetInitialTime(IClientFourzy game, float value)
        {
            game.initializedTime = value;
        }

        public static void AddPlayerMagic(IClientFourzy game, int playerId, int value)
        {
            game.magic[playerId] += value;

            game.onMagic?.Invoke(playerId, game.magic[playerId]);
        }

        public static void OnVictory(IClientFourzy game)
        {
            if (game.puzzleData)
            {
                if (game.IsWinner())
                {
                    if (game.puzzleData.pack)
                    {
                        bool _complete = game.puzzleData.pack.complete;

                        PlayerPrefsWrapper.SetPuzzleChallengeComplete(game.puzzleData.ID, true);

                        if (game.puzzleData.pack.complete && !_complete)
                        {
                            AnalyticsManager.Instance.LogEvent(
                                AnalyticsManager.AnalyticsEvents.eventComplete,
                                values: new KeyValuePair<string, object>(
                                    AnalyticsManager.EVENT_ID_KEY, game.puzzleData.pack.packId));
                        }
                    }
                    else
                    {
                        PlayerPrefsWrapper.SetFastPuzzleComplete(game.puzzleData.ID, true);

                        //send new statistics to playfab
                        GameManager.UpdateFastPuzzlesStat(GameContentManager.Instance.finishedFastPuzzlesCount);
                    }

                    //assign rewards if any
                    game.puzzleData.AssignPuzzleRewards();
                }
            }
        }

        public static void OnDraw(IClientFourzy game)
        {
            game.draw = true;
        }

        public static void CheckLost(IClientFourzy game)
        {
            if (game.isOver)
            {
                if (game.IsWinner())
                {
                    game.LoseStreak = 0;
                }
            }
        }

        public static void RemoveMember(IClientFourzy game)
        {
            game.myMembers.RemoveAt(game.myMembers.Count - 1);
        }

        public static void AddMembers(IClientFourzy game, int count)
        {
            int playerID = game.me.PlayerId;

            List<Creature> addition = new List<Creature>();
            for (int index = 0; index < count; index++)
            {
                addition.Add(new Creature(game.playerPiece.HerdId));
            }

            game._State.Herds[playerID].Members.AddRange(addition);
            game._State.Players[playerID].HerdCount = game._State.Herds[playerID].Members.Count;
        }

        public static void AssignPrefabs(IClientFourzy game, bool force = false)
        {
            string herdId;

            if (game.playerOnePrefabData == null || force)
            {
                herdId = InternalSettings.Current.DEFAULT_GAME_PIECE;
                if (game._State.Players[1].HerdId != null)
                {
                    herdId = game._State.Players[1].HerdId;
                }

                if (game._State.Players.ContainsKey(1) && !string.IsNullOrEmpty(game._State.Players[1].HerdId))
                {
                    game.playerOnePrefabData =
                        GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(herdId);
                }
                else
                {
                    game.playerOnePrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                        UserManager.Instance.gamePieceID);
                }
            }

            if (game.playerTwoPrefabData == null || force)
            {
                herdId = InternalSettings.Current.DEFAULT_GAME_PIECE;
                if (game._State.Players[2].HerdId != null)
                {
                    herdId = game._State.Players[2].HerdId;
                }

                if (game._State.Players.ContainsKey(2) && !string.IsNullOrEmpty(game._State.Players[2].HerdId))
                {
                    game.playerTwoPrefabData =
                        GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(herdId);
                }
                else
                {
                    game.playerTwoPrefabData = GameContentManager.Instance.piecesDataHolder.GetGamePiecePrefabData(
                        UserManager.Instance.gamePieceID);
                }
            }
        }

        public static Player Other(IClientFourzy game, Player player)
        {
            return game._State.Players[1].PlayerId == player.PlayerId ?
                game._State.Players[2] :
                game._State.Players[1];
        }
    }
}