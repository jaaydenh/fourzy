using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class ActivateBombPower : IBossPower, IMove
    {
        public string Name { get { return "Activate Bomb"; } }
        public BossPowerType PowerType { get { return BossPowerType.ActivateBomb; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }
        public int HiddenBombs { get; set; }
        public BoardLocation BombLocation { get; set; }
       
        public string Notation { get { return Name; } }

        public ActivateBombPower(int HiddenBombs)
        {
            this.HiddenBombs = HiddenBombs;
        }

        public ActivateBombPower(BoardLocation BombLocation)
        {
            this.BombLocation = BombLocation;
        }

        public bool Activate(GameState State)
        {
            BoardSpace Target = State.Board.ContentsAt(BombLocation);

            List<IToken> HiddenBomb = Target.FindTokens(TokenType.HIDDEN_BOMB);
            if (HiddenBomb.Count == 0) return false;

            if (!Target.ContainsPiece && Target.TokensAllowEndHere)
            {
                CircleBombToken Bomb = new CircleBombToken();
                State.Board.AddToken(Bomb, BombLocation);
                Bomb.Space = State.Board.ContentsAt(BombLocation);
                ((HiddenBombToken)HiddenBomb.First()).Found = true;

                State.Board.RecordGameAction(new GameActionTokenDrop(Bomb, TransitionType.BOSS_POWER, BombLocation, BombLocation));

                return true;
            }

            return false;
        }

        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            List<IToken> HiddenBombs = State.Board.FindTokens(TokenType.HIDDEN_BOMB);
            if (HiddenBombs.Count == 0) return false;

            foreach (IToken Bomb in HiddenBombs)
            {
                HiddenBombToken b = (HiddenBombToken)Bomb;
                if (b.Space.TokensAllowEndHere && !b.Space.ContainsPiece && !b.Found) return true;
            }
            return false;

            //if (PossibleBombLocations.Count == 0) HideBombs(State.Board);

            //foreach (BoardLocation l in PossibleBombLocations)
            //    if (State.Board.ContentsAt(l).ContainsOnlyTerrain)
            //    {
            //        return true; 
            //    }
            //return false;            
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            List<IToken> HiddenBombs = State.Board.FindTokens(TokenType.HIDDEN_BOMB);
        
            foreach (IToken Bomb in HiddenBombs)
            {
                HiddenBombToken b = (HiddenBombToken)Bomb;
                if (b.Found) continue;
                if (b.Space.TokensAllowEndHere) Powers.Add(new ActivateBombPower(b.Space.Location));
            }

            //TurnEvaluator TE = new TurnEvaluator(State);
            //foreach (BoardLocation l in PossibleBombLocations)
            //    if (State.Board.ContentsAt(l).ContainsOnlyTerrain)
            //    {
            //        Powers.Add(new ActivateBombPower(l));
            //    }

            return Powers;
        }

    }
}