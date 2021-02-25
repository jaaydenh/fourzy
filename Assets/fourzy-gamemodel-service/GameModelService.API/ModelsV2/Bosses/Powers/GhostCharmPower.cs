using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class GhostCharmPower : IBossPower, IMove
    {
        public string Name { get { return "Ghost Charm"; } }
        public BossPowerType PowerType { get { return BossPowerType.GhostCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation GhostLocation { get; set; }
        public Direction MoveDirection { get; set; }

        public GhostCharmPower()
        {
            this.GhostLocation = new BoardLocation(0, 0);
            this.MoveDirection = Direction.NONE;
        }

        public GhostCharmPower(BoardLocation Location, Direction MoveDirection)
        {
            this.GhostLocation = Location;
            this.MoveDirection = MoveDirection;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Ghosts = State.Board.ContentsAt(GhostLocation).FindTokens(TokenType.GHOST);
            foreach (IToken t in Ghosts)
            {
                GhostToken Ghost = (GhostToken)t;

                foreach(BoardLocation Target in Ghost.Space.Location.Look(State.Board, MoveDirection))
                {
                    if (!Target.OnBoard(State.Board)) continue;
                    if (State.Board.ContentsAt(Target).ContainsPiece) continue;
                    if (!State.Board.ContentsAt(Target).TokensAllowEndHere) continue;

                    State.Board.ContentsAt(Ghost.Space.Location).RemoveTokens(TokenType.GHOST);
                    State.Board.ContentsAt(Target).AddToken(new GhostToken());
                    State.RecordGameAction(new GameActionTokenMovement(Ghost, TransitionType.BOSS_POWER, Ghost.Space.Location, Target));
                    return true;
                }
            }
            return false;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            List<IToken> Ghosts = State.Board.FindTokens(TokenType.GHOST);

            if (IsDesparate)
            {

          
            foreach (IToken t in Ghosts)
            {
                GhostToken Ghost = (GhostToken)t;
                foreach (Direction d in TokenConstants.GetDirections())
                {
                    foreach (BoardLocation Target in Ghost.Space.Location.Look(State.Board, d))
                    {
                        if (!Target.OnBoard(State.Board)) continue;
                        if (State.Board.ContentsAt(Target).ContainsPiece) continue;
                        if (!State.Board.ContentsAt(Target).TokensAllowEndHere) continue;

                        Powers.Add(new GhostCharmPower(Ghost.Space.Location, d));
                        break;
                    }
                }

            }
            }
            else
            {
                int RandomGhost = State.Random.RandomInteger(0, Ghosts.Count-1);

                GhostToken Ghost = (GhostToken)Ghosts[RandomGhost];
                foreach (Direction d in TokenConstants.GetDirections() )
                {
                    foreach (BoardLocation Target in Ghost.Space.Location.Look(State.Board, d))
                    {
                        if (!Target.OnBoard(State.Board)) continue;
                        if (State.Board.ContentsAt(Target).ContainsPiece) continue;
                        if (!State.Board.ContentsAt(Target).TokensAllowEndHere) continue;

                        Powers.Add(new GhostCharmPower(Ghost.Space.Location, d));
                        break;
                    }
                }
            }

            return Powers;
        }

    }
}