using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class NecromancerBoss : IBoss
    {
        public string Name { get { return "The Necrodancer"; } }
        public List<IBossPower> Powers { get; }
        const int MinStartingGhosts = 4;
        const int MaxStartingGhosts = 12;
        public bool UseCustomAI { get { return false; } }
        public bool BossGoesFirst { get { return false; } }

        public NecromancerBoss()
        {
            this.Powers = new List<IBossPower>();
            this.Powers.Add(new GhostCharmPower());
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
            List<IToken> Ghosts = State.Board.FindTokens(TokenType.GHOST);

            if (Ghosts.Count > MaxStartingGhosts)
            {
                int Over = Ghosts.Count - MaxStartingGhosts;
                for (int i = 0; i < Over; i++)
                    Ghosts.Remove(Ghosts[0]);
            }

            if (State.Board.FindTokens(TokenType.GHOST).Count < MinStartingGhosts)
            {
                int count = 100;
                while (count-- > 0 && State.Board.FindTokens(TokenType.GHOST).Count < MinStartingGhosts)
                {
                    BoardLocation Target = State.Board.Random.RandomLocation(new BoardLocation(1, 1), State.Board.Rows - 2, State.Board.Columns - 2);
                    if (State.Board.ContentsAt(Target).ContainsOnlyTerrain)
                        State.Board.AddToken(new GhostToken(), Target, AddTokenMethod.ALWAYS, true);
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
