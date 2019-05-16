using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockMovePower : IBossPower, IMove
    {
        public string Name { get { return "Block Move"; } }
        public BossPowerType PowerType { get { return BossPowerType.ArrowCharm; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation BlockLocation { get; set; }

        public BlockMovePower()
        {
            this.BlockLocation = new BoardLocation(0, 0);
        }

        public BlockMovePower(BoardLocation Location)
        {
            this.BlockLocation = Location;
        }

        public BlockMovePower(GameState State, SimpleMove MoveToBlock)
        {
            this.BlockLocation = new BoardLocation(MoveToBlock, State.Board);
        }


        public bool Activate(GameState State)
        {
            List<IToken> Blockers = State.Board.FindTokens(TokenType.MOVE_BLOCKER);
            foreach (BoardLocation l in State.Board.Edges)
            {
                State.Board.ContentsAt(l).RemoveTokens(TokenType.MOVE_BLOCKER);
                State.Board.RecordGameAction(new GameActionTokenRemove(l, TransitionType.BOSS_POWER,null));
            }
            State.Board.AddToken(new MoveBlockerToken(), BlockLocation);
            State.Board.RecordGameAction(new GameActionBossPower(this));

            return true;
        }

        public bool IsAvailable(GameState State)
        {
            return true;
        }

        public List<IMove> GetPossibleActivations(GameState State)
        {
            List<IMove> Powers = new List<IMove>();
            foreach (BoardLocation l in State.Board.Edges)
            {
                if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                    Powers.Add(new BlockMovePower(l));
            }

            return Powers;
        }

    }
}