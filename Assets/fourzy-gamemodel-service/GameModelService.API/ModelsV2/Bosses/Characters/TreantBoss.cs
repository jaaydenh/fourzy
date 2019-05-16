using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class TreantBoss : IBoss
    {
        public string Name { get { return "Greenish Beard"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingTrees = 2;   
        const int MaxStartingTrees = 6;  

        public TreantBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new TreeCharmPower());
        }

        public List<IMove> GetPossibleActivations(GameState State)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State))
                    Activations.AddRange(p.GetPossibleActivations(State));
            }
            return Activations;
        }

        public bool StartGame(GameState State)
        {
            List<IToken> Tokens = State.Board.FindTokens(TokenType.FRUIT_TREE);

            if (Tokens.Count > MaxStartingTrees)
            {
                int Over = Tokens.Count - MaxStartingTrees;
                for (int i = 0; i < Over; i++)
                    Tokens.Remove(Tokens[0]);
            }

            if (State.Board.FindTokens(TokenType.FRUIT_TREE).Count < MinStartingTrees)
            {
                int count = 100;
                while (count-- > 0 && State.Board.FindTokens(TokenType.FRUIT_TREE).Count < MinStartingTrees)
                {
                    BoardLocation Target = State.Board.Random.RandomLocation(new BoardLocation(1, 1), State.Board.Rows - 2, State.Board.Columns - 2);
                    if (State.Board.ContentsAt(Target).ContainsOnlyTerrain)
                        State.Board.AddToken(new ArrowToken(State.Board.Random.RandomDirection()), Target, AddTokenMethod.ALWAYS,true);
                }
            }

            return true;
        }

        public bool TriggerPower(GameState State)
        {
            return true;
        }

        //Use for special conditions.

        public bool TriggerBossWin(GameState State)
        {
            return false;
        }

        public bool TriggerBossLoss(GameState State)
        {
            return false;
        }

    }
}
