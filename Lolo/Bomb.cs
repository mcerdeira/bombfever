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
        public Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        private string Owner;
        Rectangle hitBox;
        private Player player;
        private Player player2;
        private int LifeLoop = 100;
        BombManager BombMan;

        public Bomb(Vector2 position, string owner, BombManager BombMan, ContentManager Content, Player player, Player player2)
        {
            this.Owner = owner;
            this.BombMan = BombMan;
            this.player = player;
            this.player2 = player2;                      
            Texture = Content.Load<Texture2D>("bomb");
            Vector2 pos = new Vector2(position.X - ((Texture.Width - player.hitBox.Width) / 2), position.Y - ((Texture.Height - player.hitBox.Height) / 2));
            this.Position = pos;
            this.Columns = Texture.Width / 50;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            int row = (int)((float)Status / (float)Columns);
            int column = Status % Columns;
            // Warps
            if (Position.X < -21)
            {
                Position.X = 796;
            }
            if (Position.X > 798)
            {
                Position.X = -20;
            }           
            Rectangle source = new Rectangle(width * column, height * row, width, height);
            Rectangle dest = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            hitBox = dest;
            spriteBatch.Draw(Texture, dest, source, Color.White);
        }

        private void CheckCollisions(Player player)
        {
            if (LifeLoop <= 50 && hitBox.Intersects(player.hitBox))
            {
                Vector2 v = General.IntersectDepthVector(player.hitBox, this.hitBox);
                float absx = Math.Abs(v.X);
                float absy = Math.Abs(v.Y);
                // if a collision has happened		
                if (!(v.X == 0 && v.Y == 0))
                {
                    if (absx > absy) // the shallower impact is the correct one- this is on the y axis
                    {
                        Vector2 newpos = new Vector2(player.hitBox.X, player.hitBox.Y + v.Y);
                        player.newPosition = newpos;
                    }
                    else // the x axis!
                    {
                        Vector2 newpos = new Vector2(player.hitBox.X + v.X, player.hitBox.Y);
                        player.newPosition = newpos;
                    }
                    player.wallHitted = true;
                }
            }
        }

        public void Update()
        {
            CheckCollisions(player);
            CheckCollisions(player2);
            LifeLoop--;

            if (LifeLoop == 0)
            {
                if (Owner == "p1")
                {
                    player.BombCount--;
                    Vector2 pos = new Vector2(hitBox.Center.X, hitBox.Center.Y);
                    BombMan.RemoveBomb(this, pos);
                }
                else
                {
                    player2.BombCount--;
                    Vector2 pos = new Vector2(hitBox.Center.X, hitBox.Center.Y);
                    BombMan.RemoveBomb(this, pos);
                }
            }
        }
    }
}
