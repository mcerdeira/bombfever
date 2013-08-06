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
        string Caption; // The button caption
        GameState RetState; // The returned game state
        SpriteFont Font;
        ButtonType Type;
        Texture2D Texture;
        Vector2 Position;
        Color Color;
        private int Columns;
        public int Status;

        public Button(string caption, Texture2D texture, SpriteFont font, Color color, GameState retstate, ButtonType type = ButtonType.Button)
        {            
            this.Font = font;
            this.Color = color;
            this.Texture = texture;
            this.Type = type;
            this.RetState = retstate;
            this.Caption = caption;
            this.Columns = Texture.Width / 200;
        }

        public GameState GetRetState()
        {
            return this.RetState;
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
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            int row = (int)((float)Status / (float)Columns);
            int column = Status % Columns;
            Rectangle source = new Rectangle(width * column, height * row, width, height);
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, width, height);            
            spriteBatch.Draw(Texture, dest, source, Color.White);
            float centerX = (dest.X + (width / 2)) - (Font.MeasureString(Caption).X / 2);
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
