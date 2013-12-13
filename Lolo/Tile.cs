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
        public int inmunityCounter = 0; // Frame duration of inmunity (after being hitted)
        public int PortalDeactivateTime = 0;
        public Tile Partner;
        private int Life = 1;

        public Tile(Vector2 position, Texture2D texture, Player player, Player player2, bool brekable, bool walkable, Map map, int id, BombManager bombmanager)
        {            
            this.bombmanager = bombmanager;
            this.Walkable = walkable;
            this.BreakAble = brekable;
            this.Action = "";
            this.player = player;
            this.player2 = player2;
            this.ID = id;
            this.Texture = texture;
            if (this.ID < 0)
            {
                if (this.ID == -100)
                {
                    Life = 3;
                }
                else if (this.ID == -200)
                {
                    Life = 3;
                }            
            }
            else if (this.ID > 0)
            {                
                if (this.ID == 3)
                {
                    Life = 2;
                }
            }
            if (this.ID != 0)
            {                
                this.Columns = Texture.Width / 50;
            }
            this.Map = map;
            this.Position = position;            
        }

        public void setPartner(Tile partner)
        {
            this.Partner = partner;
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
            if (this.Action == "dead")
            {
                Life--;
                if (this.Life > 0)
                {
                    this.Action = "";
                    this.inmunityCounter = 170;
                }
            }

            if(Action == "dead")
            {
                if (this.ID == -100)
                {
                    Map.MakeWin("p2");    
                }
                if (this.ID == -200)
                {                    
                    Map.MakeWin("p1");
                }

                deadCounter++;
                if (deadCounter == 5)
                {
                    if (this.ID == 4)
                    {
                        // Top left
                        bombmanager.addExplossion(this.Position, 8);
                        // Top right
                        bombmanager.addExplossion(new Vector2(this.Position.X + this.hitBox.Width, this.Position.Y), 8);
                        // Bottom left
                        bombmanager.addExplossion(new Vector2(this.Position.X, this.Position.Y + this.hitBox.Height), 8);
                        // Bottom right
                        bombmanager.addExplossion(new Vector2(this.Position.X + this.hitBox.Width, this.Position.Y + this.hitBox.Height), 8);
                    }
                    Map.RemoveTile(this);
                }
            }
            else
            {
                if (!this.Walkable || this.ID == 6) // Walkable or portal
                {
                    CheckCollisions(player);
                    CheckCollisions(player2);
                    CheckCollisionsBomb();
                    if (this.inmunityCounter != 0)
                    {
                        this.inmunityCounter--;
                    }
                }
            }
        }

        public void Shake()
        {
            if (this.ID != 0 || this.ID < 0)
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
                if (this.ID == 6 && PortalDeactivateTime != 0)
                {
                    PortalDeactivateTime--;
                    return;
                }
                for (int index = 0; index < bombmanager.bombs.Count; index++)
                {
                    string flying = bombmanager.bombs[index].flying;
                    if (flying != "" && this.hitBox.Intersects(bombmanager.bombs[index].hitBox)) // I don't care stationary bombs
                    {
                        Vector2 v = General.IntersectDepthVector(bombmanager.bombs[index].hitBox, this.hitBox);
                        float absx = Math.Abs(v.X);
                        float absy = Math.Abs(v.Y);
                        bool realHit = true;
                        if (this.ID == 6)
                        {                            
                            this.Partner.PortalDeactivateTime = 50;
                            bombmanager.bombs[index].Position = this.Partner.Position;
                            bombmanager.bombs[index].Teletransported();
                            bombmanager.addExplossion(new Vector2(this.Position.X + (this.hitBox.Width / 2), this.Position.Y + (this.hitBox.Height / 2)), 8, false, true);
                            bombmanager.addExplossion(new Vector2(this.Partner.Position.X + (this.hitBox.Width / 2), this.Partner.Position.Y + (this.hitBox.Height / 2)), 8, false, true);
                        }
                        else
                        {
                            // This handles the case when a flying bomb hits a wall
                            if (!(v.X == 0 && v.Y == 0))
                            {
                                if (absx > absy) // the shallower impact is the correct one- this is on the y axis
                                {
                                    if (flying == "H")
                                    {
                                        realHit = false;
                                    }
                                }
                                else
                                {
                                    if (flying == "V")
                                    {
                                        realHit = false;
                                    }
                                }
                            }
                            if (realHit)
                            {
                                bombmanager.bombs[index].wallHitted = true;
                                if (bombmanager.bombs[index].BouncingBomb)
                                {
                                    Shake();
                                }
                            }
                        }
                        break;
                    }
                }
            }
        }

        private void CheckCollisions(Player player)
        {
            if (player.Item == ItemTypes.Ghost && this.ID != 2) // Ghost can go through portals
            {
                return;
            }
            if (this.ID != 0 && this.ID != 6)
            {
                if (hitBox.Intersects(player.hitBox))
                {
                    Vector2 v = General.IntersectDepthVector(player.hitBox, this.hitBox);
                    int xSign = Math.Sign(v.X);
                    int ySign = Math.Sign(v.Y);
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
                            /*
                            Vector2 newpos = new Vector2(player.hitBox.X, player.hitBox.Y + v.Y);
                            */
                            int autoAd = 0;
                            if (absx < 25)
                            {
                                autoAd = 5 * xSign;
                            }
                            Vector2 newpos = new Vector2(player.getLocation().X + autoAd, player.getLocation().Y + v.Y);
                            player.newPosition = newpos;
                        }
                        else // the x axis!
                        {
                            //axis = "x";
                            //if (v.X < 0)
                            //    loc = "left";
                            //else
                            //    loc = "right";
                            /*
                            Vector2 newpos = new Vector2(player.hitBox.X + v.X, player.hitBox.Y);
                            */
                            int autoAd = 0;
                            if (absy < 25)
                            {
                                autoAd = 5 * ySign;
                            }
                            Vector2 newpos = new Vector2(player.getLocation().X + v.X, player.getLocation().Y + autoAd);
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
                if (inmunityCounter % 5 == 0)
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
}
