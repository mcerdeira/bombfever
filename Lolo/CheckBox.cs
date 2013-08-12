using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    class CheckBox
    {
        public string Caption; // The button caption        
        SpriteFont Font;
        Vector2 Position;
        Color Color;
        private int ScreenWidth;
        private bool Value;
        public int Status;

        public CheckBox(string caption, int screenwidth, SpriteFont font, Color color, bool value = false)
        {
            this.Value = value;
            this.ScreenWidth = screenwidth;
            this.Font = font;
            this.Color = color;           
            this.Caption = caption;
        }

        public GameState GetRetState()
        {
            return GameState.None;
        }

        public bool getValue()
        {
            return this.Value;
        }

        public void clicked()
        {
            this.Value = !this.Value;
        }

        public float getWidth()
        {
            return Font.MeasureString(this.Caption).X;
        }

        public float getHeight()
        {
            return Font.MeasureString(this.Caption).Y;
        }

        public string getCaption()
        {
            return this.Caption;
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }

        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float centerX = General.getScreenCenterTextX(Caption, ScreenWidth, Font);
            Color col;
            if (Status == 0)
            {
                col = Color;
            }
            else
            {
                col = Color.LightSkyBlue;
            }
            string value = (this.Value) ? " [X]" : " [ ]";
            spriteBatch.DrawString(Font, Caption + value, new Vector2(centerX, Position.Y), col);
        }
    }
}