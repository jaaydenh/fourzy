using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class MadBomberBoss : IBoss
    {
        public string Name { get { return "Dr Madd"; } }
        public List<IBossPower> Powers { get; }
        public const int NumberOfBombsToHide = 8;
        public bool UseCustomAI { get { return false; } }


        public MadBomberBoss()
        {
            this.Powers = new List<IBossPower>();
            //this.Powers.Add(new ThrowBombPower());
            this.Powers.Add(new TriggerAllBombsPower());
            this.Powers.Add(new ActivateBombPower(NumberOfBombsToHide));
        }

     

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            int PossibleBombLocations = State.Board.FindTokens(TokenType.HIDDEN_BOMB).Count;
            //if (PossibleBombLocations == 0) HideBombs(State.Board);

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
            int PossibleBombLocations = State.Board.FindTokens(TokenType.HIDDEN_BOMB).Count;
            while (PossibleBombLocations < MadBomberBoss.NumberOfBombsToHide)
            {
                BoardLocation l = State.Board.Random.RandomLocationNoCorner();
                if (State.Board.ContentsAt(l).ContainsOnlyTerrain) State.Board.AddToken(new HiddenBombToken(), l);

                PossibleBombLocations = State.Board.FindTokens(TokenType.HIDDEN_BOMB).Count;
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
               
        public bool TriggerBossWin(GameState State)
        {
            return false;
        }

        public bool TriggerBossLoss(GameState State)
        {
            List<IMove> Activations = new List<IMove>();
            foreach (IBossPower p in Powers)
            {
                if (p.IsAvailable(State, true)) return false;
            
            }

            return true;
        }

    }
}
