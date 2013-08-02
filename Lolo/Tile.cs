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
    public class Tile
    {
        public int Status = 0; // frame status
        public int ID;
        private Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        public Rectangle hitBox;
        private Player player;
        private Enemy Enemy;

        public Tile(Vector2 position, ContentManager Content, Player player, int id)
        {
            this.player = player;
            this.ID = id;
            Texture = Content.Load<Texture2D>(this.ID.ToString());            
            this.Position = position;
            this.Columns = Texture.Width / 50;
        }

        public void Update()
        {
            switch (this.ID)
            {
                case 0:
                    // Space
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            if(hitBox.Intersects(player.hitBox))
            {                
                player.wall = true;
            }
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
