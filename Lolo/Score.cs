﻿using System;
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
        private float currentTime = 80f;
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
            float centerX = (ScreenWidth / 2) - (Font.MeasureString(currentTime.ToString()).X / 2);            
            spriteBatch.DrawString(Font, currentTime.ToString(), new Vector2(centerX, -40), Color.White);
            // Kills
            spriteBatch.DrawString(Font, scoreP1.ToString(), new Vector2(0, -40), Color.White);
            spriteBatch.DrawString(Font, scoreP2.ToString(), new Vector2(ScreenWidth - Font.MeasureString(scoreP2.ToString()).X, -40), Color.White);
        }
    }
}