using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class PlantTreePower : IBossPower, IMove
    {
        public string Name { get { return "Plant A Tree"; } }
        public BossPowerType PowerType { get { return BossPowerType.PlantTree; } }
        public MoveType MoveType { get { return MoveType.BOSSPOWER; } }

        public string Notation { get { return Name; } }

        public BoardLocation GrowLocation { get; set; }

        public PlantTreePower()
        {
            this.GrowLocation = new BoardLocation(0, 0);
        }

        public PlantTreePower(BoardLocation Location)
        {
            this.GrowLocation = Location;
        }

        public bool Activate(GameState State)
        {
            if (State.Board.ContentsAt(GrowLocation).ContainsPiece) return false;
            if (!State.Board.ContentsAt(GrowLocation).TokensAllowEndHere) return false;

            State.Board.AddToken(new FruitTreeToken(), GrowLocation);
            State.Board.RecordGameAction(new GameActionBossPower(this));

            BossAIHelper.GetBoss(State).UseSpecialAbility();
            return true;
        }

        public bool IsAvailable(GameState State, bool IsDesparate =false)
        {
            if (BossAIHelper.GetBoss(State).SpecialAbilityCount > 0 && IsDesparate) return true;
            return false;

        }

        public List<IMove> GetPossibleActivations(GameState State, bool IsDesparate = false)
        {
            List<IMove> Powers = new List<IMove>();

            //Instead of adding a tree anywhere, limit to the same row and column as an existing tree

            foreach (FruitTreeToken Tree in State.Board.FindTokens(TokenType.FRUIT_TREE))
            {
                foreach (Direction d in TokenConstants.GetDirections())
                {
                    foreach (BoardLocation l in Tree.Space.Location.Look(State.Board,d))
                    {
                        if (State.Board.ContentsAt(l).ContainsTokenType(TokenType.FRUIT)) continue;
                        if (State.Board.ContentsAt(l).ContainsPiece) continue;
                        if (!State.Board.ContentsAt(l).TokensAllowEndHere) continue;

                        Powers.Add(new PlantTreePower(l));
                    }
                }
            }
            
            //foreach (BoardSpace s in State.Board.Contents)
            //{
            //    if (s.ContainsPiece) continue;
            //    if (!s.TokensAllowEndHere) continue;

            //    Powers.Add(new PlantTreePower(s.Location));
            //}

            return Powers;
        }

    }
}