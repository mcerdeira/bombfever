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
    class CharacterSelection
    {
        private List<Texture2D> PlayerSelectionTextures = new List<Texture2D>();
        SpriteFont Font;
        Texture2D Texture;
        int State1;
        int State2;
        int ScreenWidth;
        int ScreenHeight;

        public CharacterSelection(Texture2D texture, SpriteFont font, List<Texture2D> playerselectiontextures, int screenheight, int screenwidth)
        {        
            this.Texture = texture;
            this.Font = font;
            this.PlayerSelectionTextures = playerselectiontextures;
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            int x = 0, y = 0, line = 0;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(Texture, dest, source, Color.White);
            x = ScreenHeight / 3;
            y = ScreenWidth  / 4;

            for (int index = 0; index < PlayerSelectionTextures.Count; index++)
            {
                width = PlayerSelectionTextures[index].Width;
                height = PlayerSelectionTextures[index].Height;
                source = new Rectangle(0, 0, width, height);
                dest = new Rectangle(x, y, width, height);
                spriteBatch.Draw(PlayerSelectionTextures[index], dest, source, Color.White);
                x += PlayerSelectionTextures[index].Width + 60;
                line++;
                if (line == 3)
                {
                    x = ScreenHeight / 3;
                    y += PlayerSelectionTextures[index].Height + 60;
                }
            }
            //spriteBatch.DrawString(Font, "P1", new Vector2(0, 0), Color.Yellow);
            //spriteBatch.DrawString(Font, "P2", new Vector2(0, 0), Color.Yellow);
        }
    }
}
