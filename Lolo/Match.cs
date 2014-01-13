using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lolo
{
    public class Match
    {
        private int p1Wins;
        private int p2Wins;
        private int draws;

        public Match()
        {
            this.p1Wins = 0;
            this.p2Wins = 0;
            this.draws = 0;
        }

        public int p1Score()
        {
            return this.p1Wins;
        }

        public int p2Score()
        {
            return this.p2Wins;
        }

        public int Draws()
        {
            return this.draws;
        }

        public void p1Win()
        {
            this.p1Wins++;
        }
        public void p2Win()
        {
            this.p2Wins++;
        }
        public void DrawGame()
        {
            this.draws++;
        }
    }
}
