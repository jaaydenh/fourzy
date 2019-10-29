using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DoubleBlockPower : IBossPower, IMove
    {
        public string Name { get { return "Block Move"; } }
        public BossPowerType PowerType { get { return BossPowerType.DoubleBlock; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public List<BoardLocation> BlockLocations { get; set; }


        public DoubleBlockPower()
        {
            this.BlockLocations = new List<BoardLocation>();
        }

        public DoubleBlockPower(List<BoardLocation> Locations)
        {
            this.BlockLocations = new List<BoardLocation>();
            foreach (BoardLocation l in Locations) this.BlockLocations.Add(l);
        }

        public DoubleBlockPower(GameState State, List<SimpleMove> MovesToBlock)
        {
            this.BlockLocations = new List<BoardLocation>();
            if (MovesToBlock == null)
            {
                return;
            }

            foreach (SimpleMove m in MovesToBlock)
                this.BlockLocations.Add(TurnEvaluator.FirstLocation(m, State.Board));
        }


        public bool Activate(GameState State)
        {
            //For now, do not allow blocker on a corner.
            //  Maybe if we eventually allow a corner move.

            foreach (BoardLocation l in BlockLocations)
            {
                if (State.Board.Corners.Contains(l)) return false;
            }

            List<IToken> Blockers = State.Board.FindTokens(TokenType.MOVE_BLOCKER);
            foreach (IToken b in Blockers)
            {
                State.Board.RecordGameAction(new GameActionTokenRemove(b.Space.Location, TransitionType.BOSS_POWER, b));
                State.Board.ContentsAt(b.Space.Location).RemoveTokens(TokenType.MOVE_BLOCKER);
            }
            
            foreach (BoardLocation l in BlockLocations)
            {
                BoardSpace Target = State.Board.ContentsAt(l);
                MoveBlockerToken t = new MoveBlockerToken();
                t.Space = State.Board.ContentsAt(l);
                State.Board.AddToken(t, l);
                State.Board.RecordGameAction(new GameActionTokenDrop(t, TransitionType.BOSS_POWER, l, l));
            }

            return true;
        }

        //Should always be true. Otherwise, game would be a draw.
        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        //NEED TO DO THIS DIFFERENTLY...
        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            List<BoardLocation> Locations = new List<BoardLocation>();
            foreach(BoardLocation l in State.Board.Edges)
            {
                if (State.Board.ContentsAt(l).ContainsTokenType(TokenType.BLOCKER)) continue;
                if (State.Board.ContentsAt(l).ContainsPiece && !State.Board.ContentsAt(l).TokensAllowPushing) continue;
                Locations.Add(l);
            }
              
            List<List<BoardLocation>> Combos = GetAllCombos(Locations).Where(a=>a.Count == 2).ToList();
            foreach (List<BoardLocation> c in Combos)
            {
                if (c.Count != 2) continue;
                Powers.Add(new DoubleBlockPower(c));                
            }

            return Powers;
        }

        public static List<List<T>> GetAllCombos<T>(List<T> list)
        {
            int comboCount = (int)Math.Pow(2, list.Count) - 1;
            List<List<T>> result = new List<List<T>>();
            for (int i = 1; i < comboCount + 1; i++)
            {
                // make each combo here
                result.Add(new List<T>());
                for (int j = 0; j < list.Count; j++)
                {
                    if ((i >> j) % 2 != 0)
                        result.Last().Add(list[j]);
                }
            }
            return result;
        }
    }
}