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
    public class Bomb
    {
        public int Status = 0; // frame status
        private Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        Rectangle hitBox;
        private Player player;
        private Enemy enemy;
        private string Owner;
        private int LifeLoop = 100;
        BombManager BombMan;

        public Bomb(Vector2 position, string Owner, BombManager BombMan, ContentManager Content, Player player = null, Enemy enemy = null)
        {
            this.BombMan = BombMan;
            this.player = player;
            this.enemy = enemy;
            this.Owner = Owner;
            Texture = Content.Load<Texture2D>("bomb");
            this.Position = position;
            this.Columns = Texture.Width / 50;
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

        public void Update()
        {
            if (LifeLoop <= 50 && hitBox.Intersects(player.hitBox))
            {
                //player.wall = true;
            }
            LifeLoop--;

            if (LifeLoop == 0)
            {
                if (Owner == "player")
                {
                    player.BombCount--;
                }
                else
                {
                    //enemy.BombCount--;
                }
                BombMan.RemoveBomb(this, Position, player, enemy);
            }
        }
    }
}
