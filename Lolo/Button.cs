using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    class Button
    {
        public string Caption; // The button caption
        GameState RetState; // The returned game state
        SpriteFont Font;               
        Vector2 Position;
        Color Color;
        Color MainColor;
        public bool Centered;
        private int ScreenWidth;
        public int Status;
        public bool Enabled;

        public Button(string caption, int screenwidth, SpriteFont font, Color maincolor, Color color, GameState retstate, bool centered, bool enabled = true)
        {
            this.Enabled = enabled;
            this.ScreenWidth = screenwidth;
            this.Font = font;
            this.Color = color;
            this.MainColor = maincolor;
            this.RetState = retstate;
            this.Caption = caption;
            this.Centered = centered;
        }

        public GameState GetRetState()
        {
            return this.RetState;
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

        public void SetXPosition(float x)
        {
            this.Position.X = x;
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float positionX;
            if (this.Centered)
            {
                positionX = General.getScreenCenterTextX(Caption, ScreenWidth, Font);
            }
            else
            {
                positionX = Position.X;
            }
            Color col;
            if (Status == 0)
            {
                col = Color;
            }
            else
            {
                col = MainColor;
            }
            spriteBatch.DrawString(Font, Caption, new Vector2(positionX, Position.Y), col);
        }
    }
}
