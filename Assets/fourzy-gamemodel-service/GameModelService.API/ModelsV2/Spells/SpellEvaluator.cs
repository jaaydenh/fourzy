﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class SpellEvaluator
    {
        public static List<BoardLocation> GetValidSpellLocations(GameBoard Board, ISpell Spell)
        {
            List<BoardLocation> Locations = new List<BoardLocation>();
            if (!Spell.RequiresLocation) return Locations;

            switch (Spell.SpellId)
            {
                case SpellId.HEX:
                    foreach (BoardLocation l in Board.GetOpenSpaces())
                        if (!Board.ContentsAt(l).ContainsSpell) Locations.Add(l);
                    break;
            }

            return Locations;
        }

            
    }
}
