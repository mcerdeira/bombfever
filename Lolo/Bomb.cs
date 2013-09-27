﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Lolo
{
    public class Bomb
    {
        public int Status = 0; // frame status
        public int frCounter = 0; // Frame counter (it keeps the animation rate)
        public Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        private string Owner;
        public Rectangle hitBox;
        private Player player;
        private Player player2;
        private int xMove = 0;
        private int yMove = 0;
        private int LifeLoop = 100;
        public bool wallHitted = false;        
        BombManager BombMan;

        public Bomb(Vector2 position, string owner, BombManager BombMan, ContentManager Content, Player player, Player player2)
        {
            this.Owner = owner;
            this.BombMan = BombMan;
            this.player = player;
            this.player2 = player2;
            Texture = Content.Load<Texture2D>("bomb");
            //Vector2 pos = new Vector2(position.X - ((Texture.Width - player.hitBox.Width) / 2), position.Y - ((Texture.Height - player.hitBox.Height) / 2));
            this.Position = position;
            this.Columns = Texture.Width / 50; //30
        }

        public void Kicked(string direction)
        {
            this.LifeLoop += 5; // A little extra time if it gets kicked
            yMove = 0;
            xMove = 0;
            switch (direction)
            {
                case "top":
                    yMove = 10;
                    break;
                case "bottom":
                    yMove = -10;
                    break;
                case "left":
                    xMove = 10;
                    break;
                case "right":
                    xMove = -10;
                    break;
            }
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
            //hitBox = dest;

            int hitX = (int)Position.X + 10;
            int hitY = (int)Position.Y + 10;
            hitBox = new Rectangle(hitX, hitY, 30, 30);

            spriteBatch.Draw(Texture, dest, source, Color.White);

            //This was for debugging
            //spriteBatch.Draw(Texture, hitBox, hitBox, Color.Red);
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
            #warning See what happens with no bomb colitions
            //CheckCollisions(player);
            //CheckCollisions(player2);
            LifeLoop--;

            if (wallHitted)
            {
                xMove = 0;
                yMove = 0;
                wallHitted = false;
            }
            Position.X += xMove;
            Position.Y += yMove;
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
            frCounter++;
            if (frCounter == 10)
            {
                frCounter = 0;
                Status++;
            }
            if (Status > 2)
            {
                Status = 0;
            }            
        }
    }
}
