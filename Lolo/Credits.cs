using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    public class Credits
    {
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;                     
        private SpriteFont Font;
        private List<string> Credit;

        public Credits(Texture2D texture, SpriteFont font, List<string> credits, int screenheight, int screenwidth)
        {
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.Font = font;
            this.Credit = credits;
        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(Texture, dest, source, Color.White);
            int y = 0;
            float centerX;
            foreach (string s in this.Credit)
            {
                centerX = General.getScreenCenterTextX(s, ScreenWidth, Font);
                spriteBatch.DrawString(Font, s, new Vector2(centerX, y), Color.White);
                y += (int)Font.MeasureString(s).Y;
            }            
        }
    }
}
