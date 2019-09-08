using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class EarthQuakeBoss : IBoss
    {
        public string Name { get { return "EarthQuake"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingPits = 2;
        const int MaxStartingPits = 4;

        public EarthQuakeBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new EarthCharmPower());
            this.Powers.Add(new SplitTheEarthPower());
            this.Powers.Add(new FillHolePower());
        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State, IsDesparate))
                    Activations.AddRange(p.GetPossibleActivations(State, IsDesparate));
            }
            return Activations;
        }

        public bool StartGame(GameState State)
        {
            List<IToken> Tokens = State.Board.FindTokens(TokenType.FRUIT_TREE);

            if (Tokens.Count > MaxStartingPits)
            {
                int Over = Tokens.Count - MaxStartingPits;
                for (int i = 0; i < Over; i++)
                    Tokens.Remove(Tokens[0]);
            }

            if (State.Board.FindTokens(TokenType.PIT).Count < MinStartingPits)
            {
                int count = 100;
                while (count-- > 0 && State.Board.FindTokens(TokenType.PIT).Count < MinStartingPits)
                {
                    BoardLocation Target = State.Board.Random.RandomLocation(new BoardLocation(1, 1), State.Board.Rows - 2, State.Board.Columns - 2);
                    if (State.Board.ContentsAt(Target).ContainsOnlyTerrain)
                        State.Board.AddToken(new PitToken(), Target, AddTokenMethod.ALWAYS, true);
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
