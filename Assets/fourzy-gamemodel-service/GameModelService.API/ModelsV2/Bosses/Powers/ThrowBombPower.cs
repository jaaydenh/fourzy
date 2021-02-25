using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ThrowBombPower : IBossPower, IMove
    {
        public string Name { get { return "Throw Bomb"; } }
        public BossPowerType PowerType { get { return BossPowerType.ThrowBomb; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }
        public const int MaxBombs = 2;
        public Direction ThrowDirection { get; set; }
        public int ThrowLocation { get; set; }

        public string Notation { get { return Name; } }

        public ThrowBombPower(Direction Direction = Direction.NONE, int Location = -1)
        {
            this.ThrowDirection = Direction;
            this.ThrowLocation = Location;
        }

        public bool Activate(GameState State)
        {
            int bomb_count = 0;
            bomb_count += State.Board.FindTokens(TokenType.CIRCLE_BOMB).Count;
            bomb_count += State.Board.FindTokens(TokenType.CROSS_BOMB).Count;
            //need to include other bombs here.

            if (bomb_count > MaxBombs) return false;

            TurnEvaluator TE = new TurnEvaluator(State);
            List<BoardLocation> ThrowPath = TE.TraceMove(new SimpleMove(new Piece(0), ThrowDirection, ThrowLocation));

            //State.Board.RecordGameAction(new GameActionBossPower(this));
            int pathid = 0;
            CircleBombToken Bomb = new CircleBombToken();
            BoardLocation FirstLocation = new BoardLocation(0, 0);
            BoardLocation LastLocation = new BoardLocation(0, 0);
            foreach (BoardLocation l in ThrowPath)
            {

                if (pathid == 0)
                {
                    FirstLocation = l;
                    //State.Board.AddToken(Bomb, l);
                    //Bomb.Space = State.Board.ContentsAt(l);
                    //State.Board.RecordGameAction(new GameActionTokenDrop(Bomb, TransitionType.BOSS_POWER, l, l));
                }
                else if (pathid == ThrowPath.Count - 1)
                {
                    State.Board.AddToken(Bomb, l);
                    Bomb.Space = State.Board.ContentsAt(l);
                    State.Board.RecordGameAction(new GameActionTokenDrop(Bomb, TransitionType.BOSS_POWER, FirstLocation, l));
                }
                else
                {
                    //State.Board.ContentsAt(l).RemoveOneToken(Bomb.Type);
                    //State.Board.AddToken(Bomb, l);
                    //Bomb.Space = State.Board.ContentsAt(l);
                    //State.Board.RecordGameAction(new GameActionTokenMovement(Bomb, TransitionType.BOSS_POWER, LastLocation, l));
                }
                LastLocation = l;
                pathid++;
            }

            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            //if (!IsDesparate) return false;
            int bomb_count = 0;
            bomb_count += State.Board.FindTokens(TokenType.CIRCLE_BOMB).Count;
            bomb_count += State.Board.FindTokens(TokenType.CROSS_BOMB).Count;
            //need to include other bombs here.

            if (bomb_count > MaxBombs) return false;
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            TurnEvaluator TE = new TurnEvaluator(State);
            List<SimpleMove> PossibleMoves = new List<SimpleMove>();

            if (IsDesparate) PossibleMoves = TE.GetAvailableSimpleMoves();
            else
            {
                PossibleMoves = TE.GetAvailableSimpleMoves(Direction.LEFT);
                PossibleMoves.AddRange(TE.GetAvailableSimpleMoves(Direction.RIGHT));
            }
            List<IMove> Powers = new List<IMove>();
            foreach (SimpleMove m in PossibleMoves)
            {
                Powers.Add(new ThrowBombPower(m.Direction, m.Location));
            }

            //foreach (BoardSpace s in State.Board.Contents)
            //{
            //    if (s.ContainsPiece) continue;
            //    if (!s.TokensAllowEndHere) continue;
            //    if (s.ContainsTokenType(TokenType.CIRCLE_BOMB)) continue;

            //    Powers.Add(new ThrowBombPower(s.Location));
            //}

            return Powers;
        }

    }
}