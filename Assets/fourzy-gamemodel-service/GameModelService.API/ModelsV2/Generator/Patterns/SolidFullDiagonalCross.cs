﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public class SolidFullDiagonalCrossPattern : IBoardPattern
    {
        public BoardLocation Reference { get; }

        public List<BoardLocation> Locations { get; }

        public SolidFullDiagonalCrossPattern(GameBoard Board, BoardLocation Reference)
        {
            this.Reference = Reference;
            Locations = new List<BoardLocation>();

            Locations.Add(Reference);
            foreach (BoardLocation l in Reference.GetDiagonal(Board, DiagonalType.HIGHLEFT))
            {
                Locations.Add(l);
            }

            foreach (BoardLocation l in Reference.GetDiagonal(Board, DiagonalType.HIGHRIGHT))
            {
                Locations.Add(l);
            }

        }
    }
}