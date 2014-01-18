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
        private int ScreenWidth;
        public int Status;
        public bool Enabled;

        public Button(string caption, int screenwidth, SpriteFont font, Color maincolor, Color color, GameState retstate, bool enabled = true)
        {
            this.Enabled = enabled;
            this.ScreenWidth = screenwidth;
            this.Font = font;
            this.Color = color;
            this.MainColor = maincolor;
            this.RetState = retstate;
            this.Caption = caption;
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

        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //float centerX = 0;// General.getScreenCenterTextX(Caption, ScreenWidth, Font);
            Color col;
            if (Status == 0)
            {
                col = Color;
            }
            else
            {
                col = MainColor;
            }
            spriteBatch.DrawString(Font, Caption, new Vector2(Position.X, Position.Y), col);
        }
    }
}
