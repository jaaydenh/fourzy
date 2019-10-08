using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class DirectionMasterBoss : IBoss
    {
        public string Name { get { return "The Direction Master"; } } 
        public List<IBossPower> Powers { get; }
        const int MinStartingArrows = 6;   //too few arrows and boss is too easy
        const int MaxStartingArrows = 12;  //too many arrows and boss has too many combinations to think about.
        public bool UseCustomAI { get { return false; } }

        public DirectionMasterBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new ArrowCharmPower());
            this.Powers.Add(new GlobalArrowChangePower());
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
            List<IToken> Arrows = State.Board.FindTokens(TokenType.ARROW);

            if (Arrows.Count > MaxStartingArrows)
            {
                int Over = Arrows.Count - MaxStartingArrows;
                for (int i = 0; i < Over; i++)
                    Arrows.Remove(Arrows[0]);
            }

            if (State.Board.FindTokens(TokenType.ARROW).Count < MinStartingArrows)
            {
                int count = 100;
                while (count-->0 && State.Board.FindTokens(TokenType.ARROW).Count < MinStartingArrows)
                {
                    BoardLocation Target = State.Board.Random.RandomLocation(new BoardLocation(1, 1), State.Board.Rows - 2, State.Board.Columns - 2);
                    if (State.Board.ContentsAt(Target).ContainsOnlyTerrain)
                        State.Board.AddToken(new ArrowToken(State.Board.Random.RandomDirection()), Target);
                }
            }
            return true;
        }

        public PlayerTurn GetTurn(GameState State)
        {
            return null; 
        }

        public bool TriggerPower(GameState State)
        {
            //loop through powers and if any trigger, return true;
            return true;
        }


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
