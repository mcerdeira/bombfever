using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lolo
{
    public class Score
    {
        private int startCount = 0;
        private SpriteFont Font;
        private int scoreP1 = 0;
        private int scoreP2 = 0;
        private float currentTime;        
        private int ScreenHeight;
        private int ScreenWidth;
            
        public Score(int screenheight, int screenwidth, SpriteFont font, float totaltime)
        {
            this.currentTime = totaltime;            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Font = font;
        }

        public string getResult(out int scorep1, out int scorep2, Match match)
        {
            string r;
            if (scoreP1 > scoreP2)
            {
                r = "P1 Wins!";
                match.p1Win();
            }
            else if (scoreP1 < scoreP2)
            {
                r = "P2 Wins!";
                match.p2Win();
            }
            else
            {
                r = "Tied Match";
                match.DrawGame();
            }
            scorep1 = this.scoreP1;
            scorep2 = this.scoreP2;
            return r;
        }

        public float Update(GameTime gametime)
        {
            if (startCount == 0)
            {
                startCount = 60;
                currentTime--;
            }
            else
            {
                startCount--;
            }
            return currentTime;
        }

        public void setScore(string player)
        {
            if (player == "p1")
            {
                scoreP1++;
            }
            else
            {
                scoreP2++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Timer
            float centerX = General.getScreenCenterTextX(currentTime.ToString(), ScreenWidth, Font);
            spriteBatch.DrawString(Font, currentTime.ToString(), new Vector2(centerX, 0), Color.White);
            // Kills
            spriteBatch.DrawString(Font, scoreP1.ToString(), new Vector2(0 + 5, 0), Color.White);
            spriteBatch.DrawString(Font, scoreP2.ToString(), new Vector2(ScreenWidth - Font.MeasureString(scoreP2.ToString()).X - 5, 0), Color.White);
        }
    }
}
