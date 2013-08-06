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

/*
 * 0 Four bombs
 * 1 Extended explossion
 * 2 Triggered bombs
 * 3 Kills oponent
 * 4 Inmunity (lasts 1 hit)
 * 5 Make all bombs to explode
 */

namespace Lolo
{
    public class Item
    {
        private Texture2D Texture;
        private Vector2 Position;
        public int Status = 0; // frame status
        private int Columns;
        Rectangle hitBox;

        public Item(Texture2D texture, Vector2 position)
        {
            this.Texture = texture;
            this.Position = position;
            this.Columns = Texture.Width / 50;
        }

        public void Update()
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
            hitBox = dest;
            spriteBatch.Draw(Texture, dest, source, Color.White);
        }
    }
}
