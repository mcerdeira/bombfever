using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

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
        private Map map;
        private Texture2D Texture;
        private Vector2 Position;
        public int Status = 0; // frame status
        private int Columns;
        Rectangle hitBox;
        private int Style;
        private Player player;
        private Player player2;
        private bool collected = false;

        public Item(ContentManager content, Vector2 position, Player player, Player player2, Map map)
        {
            Random rnd = new Random();
            this.Style = rnd.Next(0, 6);
            #warning This will be content.Load<Texture2D>("item" + this.Style + this.Style.ToString());
            this.Texture = content.Load<Texture2D>("item"); 
            this.Position = position;
            this.Columns = Texture.Width / 50;
            this.player = player;
            this.player2 = player2;
            this.map = map;
        }

        public void Update()
        {            
            if (!collected)
            {
                // As the check order matters, to make it more fair, the check order is random 50/50
                Random rnd = new Random();
                int i = rnd.Next(0, 2);
                if (i == 0)
                {
                    CheckCollisions(player);
                    CheckCollisions(player2);
                }
                else
                {
                    CheckCollisions(player2);
                    CheckCollisions(player);
                }
            }
        }

        private void CheckCollisions(Player player)
        {
            if (hitBox.Intersects(player.hitBox))
            {
                collected = true;
                player.setItem(this.Style);
                map.RemoveItem(this);
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
