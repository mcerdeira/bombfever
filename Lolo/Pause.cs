using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Lolo
{
    class Pause
    {
        public int Status = 0; // frame status
        private Vector2 Position;        
        private SpriteFont Font;
        private string Caption = "PAUSE";

        public Pause(int screenheight, int screenwidth, SpriteFont font)
        {
            this.Font = font;
            float centerX = (screenwidth / 2) - (Font.MeasureString(Caption).X / 2);
            float centerY = (screenheight / 2) - (Font.MeasureString(Caption).Y / 2);
            this.Position = new Vector2(centerX, centerY);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Caption, Position,Color.White);
        }

        public void Update()
        {
        }
    }
}
