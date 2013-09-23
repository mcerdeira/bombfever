using System;
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
    public class Tile
    {
        public bool BreakAble;
        public bool Walkable;
        public string Action;
        public int Status = 0; // frame status
        public int deadCounter = 0;
        public int ID;
        private int Index;
        public Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        public Rectangle hitBox;
        private Player player;        
        private Player player2;
        private Map Map;
        private int ShakeCount = 0;
        private int shakeY = 0;
        private int shakeX = 0;
        private BombManager bombmanager;

        public Tile(Vector2 position, ContentManager Content, Player player, Player player2, bool brekable, bool walkable, Map map, int id, BombManager bombmanager)
        {
            this.bombmanager = bombmanager;
            this.Walkable = walkable;
            this.BreakAble = brekable;
            this.Action = "";
            this.player = player;
            this.player2 = player2;
            this.ID = id;
            if (this.ID < 0)
            {
                if (this.ID == -100)
                {
                    Texture = Content.Load<Texture2D>("1pflag");
                }
                else if (this.ID == -200)
                {
                    Texture = Content.Load<Texture2D>("2pflag");
                }            
            }
            else if (this.ID > 0)
            {
                Texture = Content.Load<Texture2D>(this.ID.ToString());
            }
            if (this.ID != 0)
            {                
                this.Columns = Texture.Width / 50;
                this.Map = map;
            }        
            this.Position = position;                        
        }

        public void Update()
        {
            if(this.ID == 0)
            {
                return;
            }

            if (ShakeCount > 0)
            {
                if (ShakeCount % 2 == 0)
                {
                    shakeY *= -1;
                }
                else
                {
                    shakeX *= -1;
                }
                ShakeCount--;
                if (ShakeCount == 0)
                {
                    shakeY = 0;
                    shakeX = 0;
                }
            }
            // Decide sprite things, based on id
            switch (this.ID)
            {
                case 0:
                    // Space
                    break;
                default:
                    break;
            }
            if(!this.Walkable)
            {
                CheckCollisions(player);
                CheckCollisions(player2);
                CheckCollisionsBomb();
            }
            if(Action == "dead")
            {
                //Status = 1;
                deadCounter++;
                if (deadCounter == 5)
                {
                    Map.RemoveTile(this);
                }
            }
        }

        public void Shake()
        {
            if (this.ID != 0)
            {
                if (this.ShakeCount == 0)
                {
                    this.ShakeCount = 50;
                    this.shakeY = 3;
                    this.shakeX = 3;
                }
            }
        }

        private void CheckCollisionsBomb()
        {
            if (this.ID != 0)
            {
                for (int index = 0; index < bombmanager.bombs.Count; index++)
                {
                    if (this.hitBox.Intersects(bombmanager.bombs[index].hitBox))
                    {
                        bombmanager.bombs[index].wallHitted = true;
                        break;
                    }
                }
            }
        }

        private void CheckCollisions(Player player)
        {
            if (this.ID != 0)
            {
                if (hitBox.Intersects(player.hitBox))
                {
                    Vector2 v = General.IntersectDepthVector(player.hitBox, this.hitBox);
                    float absx = Math.Abs(v.X);
                    float absy = Math.Abs(v.Y);
                    //string loc ="";
                    //string axis ="";

                    // if a collision has happened		
                    if (!(v.X == 0 && v.Y == 0))
                    {
                        if (absx > absy) // the shallower impact is the correct one- this is on the y axis
                        {
                            //axis = "y";
                            //if (v.Y < 0)
                            //    loc = "top";
                            //else
                            //    loc = "bottom";
                            Vector2 newpos = new Vector2(player.hitBox.X, player.hitBox.Y + v.Y);
                            player.newPosition = newpos;
                        }
                        else // the x axis!
                        {
                            //axis = "x";
                            //if (v.X < 0)
                            //    loc = "left";
                            //else
                            //    loc = "right";
                            Vector2 newpos = new Vector2(player.hitBox.X + v.X, player.hitBox.Y);
                            player.newPosition = newpos;
                        }
                        player.wallHitted = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.ID != 0)
            {
                int width = Texture.Width / Columns;
                int height = Texture.Height;
                int row = (int)((float)Status / (float)Columns);
                int column = Status % Columns;
                Rectangle source = new Rectangle(width * column, height * row, width, height);
                Rectangle dest = new Rectangle((int)Position.X + shakeX, (int)Position.Y + shakeY, width, height);
                hitBox = dest;
                spriteBatch.Draw(Texture, dest, source, Color.White);
            }
        }
    }
}
