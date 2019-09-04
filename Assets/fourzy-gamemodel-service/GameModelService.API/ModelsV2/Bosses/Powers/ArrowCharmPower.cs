using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ArrowCharmPower : IBossPower, IMove
    {
        public string Name { get { return "Arrow Charm"; } }
        public BossPowerType PowerType { get { return BossPowerType.ArrowCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER;  } }

        public string Notation { get { return Name;  } }

        public BoardLocation ArrowLocation { get; set; }
        public Rotation RotationDirection { get; set; }

        public ArrowCharmPower()
        {
            this.ArrowLocation = new BoardLocation(0,0);
            this.RotationDirection = Rotation.NONE;
        }

        public ArrowCharmPower(BoardLocation Location, Rotation RotationDirection)
        {
            this.ArrowLocation = Location;
            this.RotationDirection = RotationDirection;
        }

        public bool Activate(GameState State)
        {
            List<IToken> Arrows = State.Board.ContentsAt(ArrowLocation).FindTokens(TokenType.ARROW);
            foreach (IToken t in Arrows)
            {
                ArrowToken Arrow = (ArrowToken)t;
                Direction OrigOrientation = Arrow.Orientation;
                Arrow.Orientation = BoardLocation.Rotate(Arrow.Orientation, RotationDirection);
                Arrow.Space.Parent.RecordGameAction(new GameActionTokenRotation(Arrow, TransitionType.BOSS_POWER, RotationDirection, OrigOrientation, Arrow.Orientation));
            }
            return true;
        }
        
        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            List<IToken> Arrows = State.Board.FindTokens(TokenType.ARROW);
            foreach (IToken t in Arrows)
            {
                ArrowToken Arrow = (ArrowToken)t;
                Powers.Add(new ArrowCharmPower(Arrow.Space.Location, Rotation.CLOCKWISE));
                Powers.Add(new ArrowCharmPower(Arrow.Space.Location, Rotation.COUNTER_CLOCKWISE));
            }

            return Powers;
        }

    }
}