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
        ButtonType Type;        
        Vector2 Position;
        Color Color;
        private int ScreenWidth;
        public int Status;

        public Button(string caption, int screenwidth, SpriteFont font, Color color, GameState retstate, ButtonType type = ButtonType.Button)
        {
            this.ScreenWidth = screenwidth;
            this.Font = font;
            this.Color = color;            
            this.Type = type;
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
            spriteBatch.DrawString(Font, Caption, new Vector2(centerX, Position.Y), col);
        }
    }
}
