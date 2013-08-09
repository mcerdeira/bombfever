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
    class Score
    {
        private int startCount = 0;
        private SpriteFont Font;
        private int scoreP1;
        private int scoreP2;
        private float currentTime = 60f;
        private int ScreenHeight;
        private int ScreenWidth;
            
        public Score(int screenheight, int screenwidth, SpriteFont font)
        {
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Font = font;
        }

        public float Update(GameTime gametime)
        {
            if (gametime.TotalGameTime.Seconds > this.startCount)  
            {  
                this.startCount = gametime.TotalGameTime.Seconds;  
                currentTime --;              
            }
            return currentTime;
        }

        public void setScore(string player)
        {
            if (player == "player1")
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
            float centerX = (ScreenWidth / 2) - (Font.MeasureString(currentTime.ToString()).X / 2);
            float centerY = (ScreenHeight / 2) - (Font.MeasureString(currentTime.ToString()).Y / 2);                   
            spriteBatch.DrawString(Font, currentTime.ToString(), new Vector2(centerX, -40), Color.White);
        }
    }
}
