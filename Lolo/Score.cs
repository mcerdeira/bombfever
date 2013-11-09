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
        private string GameType;
        private bool WinByTile;
            
        public Score(int screenheight, int screenwidth, SpriteFont font, float totaltime, string gametype)
        {
            this.WinByTile = false;
            this.GameType = gametype;
            this.currentTime = totaltime;            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Font = font;
        }

        public void ExtraTime()
        {
            currentTime += 10;
        }

        public void MakeWin(string player)
        {
            if (currentTime > 0 && !WinByTile)
            {
                WinByTile = true;
                if (player == "p1")
                {
                    scoreP1 = 1;
                    scoreP2 = 0;
                }
                else
                {
                    scoreP1 = 0;
                    scoreP2 = 1;
                }
                currentTime = 0;
            }
        }

        public string getResult(out int scorep1, out int scorep2, Match match)
        {
            string r;
            string tilemsg = "";            
            if (WinByTile)
            {
                tilemsg = " [By blowing up the tile]";
            }
            if (scoreP1 > scoreP2)
            {
                if ((scoreP1 - scoreP2) >= 10)
                {
                    r = "P1 Just kicked your ass!";
                }
                else
                {
                    r = "P1 Wins!" + tilemsg;
                }                
                match.p1Win();
            }
            else if (scoreP1 < scoreP2)
            {
                if ((scoreP2 - scoreP1) >= 10)
                {
                    r = "P2 Just kicked your ass!";
                }
                else
                {
                    r = "P2 Wins!" + tilemsg;
                }
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

        public void SwitchScores()
        {
            int temp;
            temp = scoreP1;
            scoreP1 = scoreP2;
            scoreP2 = temp;
        }

        public void setScore(string player)
        {
            if (this.GameType == "First hit wins")
            {
                currentTime = 0;
            }
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
