﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class BlockMovePower : IBossPower, IMove
    {
        public string Name { get { return "Block Move"; } }
        public BossPowerType PowerType { get { return BossPowerType.DoubleBlock; } }
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
            this.BlockLocation = TurnEvaluator.FirstLocation(MoveToBlock, State.Board);
        }


        public bool Activate(GameState State)
        {
            //For now, do not allow blocker on a corner.
            //  Maybe if we eventually allow a corner move.
            if (State.Board.Corners.Contains(BlockLocation)) return false;

            BoardSpace Target = State.Board.ContentsAt(BlockLocation);

            //if (Target.ContainsPiece) return false;
            //if (!Target.ContainsOnlyTerrain) return false;

            List<IToken> Blockers = State.Board.FindTokens(TokenType.MOVE_BLOCKER);
            foreach (IToken b in Blockers)
            {
                State.Board.RecordGameAction(new GameActionTokenRemove(b.Space.Location, TransitionType.BOSS_POWER, b));
                State.Board.ContentsAt(b.Space.Location).RemoveTokens(TokenType.MOVE_BLOCKER);
            }

            //foreach (BoardLocation l in State.Board.Edges)
            //{
            //    if (State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
            //    {
            //        MoveBlockerToken RemoveToken = (MoveBlockerToken)State.Board.ContentsAt(l).FindTokens(TokenType.MOVE_BLOCKER).First();

            //        State.Board.RecordGameAction(new GameActionTokenRemove(l, TransitionType.BOSS_POWER, RemoveToken));
            //        State.Board.ContentsAt(l).RemoveTokens(TokenType.MOVE_BLOCKER);
            //    }
            //}
            MoveBlockerToken t = new MoveBlockerToken();
            t.Space = State.Board.ContentsAt(BlockLocation);
            State.Board.AddToken(t, BlockLocation);
            State.Board.RecordGameAction(new GameActionBossPower(this));
            State.Board.RecordGameAction(new GameActionTokenDrop(t, TransitionType.BOSS_POWER, BlockLocation, BlockLocation));

            return true;
        }

        //Should always be true. Otherwise, game would be a draw.
        public bool IsAvailable(GameState State, bool IsDesparate = false)
        {
            return true;
        }

        //NEED TO DO THIS DIFFERENTLY...  I DON'T THINK THIS WORKS TOO WELL.
        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();
            Player Boss = BossAIHelper.GetBoss(State);
            int Count = State.Board.FindPieces(Boss.PlayerId).Count;
            bool Found = false;
            if (Count < State.Board.Rows)
                switch (Count % 4)
                {
                    case 0:
                        for (int c = 1; c < State.Board.Columns - 2; c++)
                        {
                            BoardLocation l = new BoardLocation(0, c);
                            if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                            {
                                Powers.Add(new BlockMovePower(l));
                                Found = true;
                            }

                        }
                        break;
                    case 1:
                        for (int c = 1; c < State.Board.Columns - 2; c++)
                        {
                            BoardLocation l = new BoardLocation(State.Board.Rows - 1, c);
                            if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                            {
                                Powers.Add(new BlockMovePower(l));
                                Found = true;
                            }

                        }
                        break;
                    case 2:
                        for (int r = 1; r < State.Board.Rows - 2; r++)
                        {
                            BoardLocation l = new BoardLocation(r, 0);
                            if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                            {
                                Powers.Add(new BlockMovePower(l));
                                Found = true;
                            }

                        }
                        break;
                    case 3:
                        for (int r = 1; r < State.Board.Rows - 2; r++)
                        {
                            BoardLocation l = new BoardLocation(r, State.Board.Columns - 1);
                            if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                            {
                                Powers.Add(new BlockMovePower(l));
                                Found = true;
                            }

                        }
                        break;
                }
            if (!Found)
            {
                foreach (BoardLocation l in State.Board.Edges)
                {
                    if (!State.Board.ContentsAt(l).ContainsTokenType(TokenType.MOVE_BLOCKER))
                        Powers.Add(new BlockMovePower(l));
                }
            }

            return Powers;
        }

    }
}