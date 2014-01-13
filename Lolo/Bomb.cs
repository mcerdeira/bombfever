using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Lolo
{
    public class Bomb
    {
        public int Status = 0; // frame status
        public int frCounter = 0; // Frame counter (it keeps the animation rate)
        public Vector2 Position;
        private Texture2D Texture;
        private Texture2D Texture2;
        private int Columns;
        public string Owner;
        public Rectangle hitBox;
        public Rectangle bombHitBox;
        private Player player;
        private Player player2;
        private int KickedLoop = 0;
        private int xMove = 0;
        private int yMove = 0;
        private int LifeLoop = 100;
        public bool wallHitted = false;
        public bool EternalFire = false;
        public bool BouncingBomb = false;
        public string flying = "";
        int bounceSnd = 0;
        BombManager BombMan;
        List<SoundEffect> SndfxBouncingBomb;
        SoundEffect sndfxPortal;

        public Bomb(Vector2 position, string owner, BombManager BombMan, Texture2D texture, Texture2D texture2, Player player, Player player2, bool eternalFire, bool bouncing, List<SoundEffect> sndfxbouncingbomb, SoundEffect sndportal)
        {
            this.Owner = owner;
            this.BombMan = BombMan;
            this.player = player;
            this.player2 = player2;
            this.Texture = texture;
            this.Texture2 = texture2;
            this.Position = position;
            this.Columns = Texture.Width / 50; //30            
            this.EternalFire = eternalFire;
            this.BouncingBomb = bouncing;
            this.sndfxPortal = sndportal;
            this.SndfxBouncingBomb = sndfxbouncingbomb;
            if (bouncing)
            {
                LifeLoop = 200;
            }
        }

        public void Teletransported()
        {
            this.sndfxPortal.Play();
            this.xMove *= -1;
            this.yMove *= -1;
        }

        public void Kicked(string direction)
        {
            this.KickedLoop = 5;
            this.LifeLoop += 5; // A little extra time if it gets kicked
            yMove = 0;
            xMove = 0;
            switch (direction)
            {
                case "top":
                    this.flying = "V";
                    yMove = 10;
                    break;
                case "bottom":
                    this.flying = "V";
                    yMove = -10;
                    break;
                case "left":
                    this.flying = "H";
                    xMove = 10;
                    break;
                case "right":
                    this.flying = "H";
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
            int hitBoxSize = 35;
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

            int hitX = (int)Position.X + (width / 2) - (hitBoxSize / 2);
            int hitY = (int)Position.Y + (height / 2) - (hitBoxSize / 2);            
            hitBox = new Rectangle(hitX, hitY, hitBoxSize, hitBoxSize);

            bombHitBox = new Rectangle((int)Position.X, (int)Position.Y, 50, 50);

            if (LifeLoop < 90)
            {
                // Twinkling effect
                int freq = 0;
                if (LifeLoop < 90)
                {
                    freq = 10;
                }
                if (LifeLoop < 50)
                {
                    freq = 5;
                }
                if (LifeLoop < 20)
                {
                    freq = 2;
                }

                if (LifeLoop % freq != 0)
                {
                    spriteBatch.Draw(Texture, dest, source, Color.White);
                }
                else
                {
                    spriteBatch.Draw(Texture2, dest, source, Color.White);
                }
            }
            else
            {
                spriteBatch.Draw(Texture, dest, source, Color.White);
            }
            //This was for debugging
            //spriteBatch.Draw(Texture, bombHitBox, source, Color.Red);
        }

        private void CheckCollisions(Player player)
        {
            if (hitBox.Intersects(player.hitBox))
            {
                player.Pause(true, true);
            }
        }

        private void CheckCollisionsBomb()
        {
            if (this.flying != "")
            {
                for (int index = 0; index < BombMan.bombs.Count; index++)
                {
                    if (BombMan.bombs[index] != this)
                    {
                        string _flying = BombMan.bombs[index].flying;
                        if (this.bombHitBox.Intersects(BombMan.bombs[index].bombHitBox)) // I don't care stationary bombs
                        {                            
                            Vector2 v = General.IntersectDepthVector(BombMan.bombs[index].bombHitBox, this.bombHitBox);
                            float absx = Math.Abs(v.X);
                            float absy = Math.Abs(v.Y);                            

                            // This handles the case when a flying bomb hits a wall
                            if (!(v.X == 0 && v.Y == 0))
                            {                                
                                BombMan.bombs[index].wallHitted = true;
                                this.wallHitted = true;
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void Update()
        {
            // This two if are for freezing bombs when player is paused
            if (this.Owner == "p1")
            {
                if (this.player.PausedLoop != 0)
                {
                    return;
                }
            }
            if (this.Owner == "p2")
            {
                if (this.player2.PausedLoop != 0)
                {
                    return;
                }
            }

            if (flying != "") // Flying bombs makes players stay still
            {
                if (KickedLoop > 0)
                {
                    KickedLoop--;
                }
                if (KickedLoop == 0)
                {
                    CheckCollisions(player);
                    CheckCollisions(player2);
                }
            }

            CheckCollisionsBomb();
            
            LifeLoop--;

            if (wallHitted)
            {                
                if (this.BouncingBomb)
                {
                    if (xMove > 0 || xMove < 0)
                    {
                        xMove *= -1;
                    }
                    if (yMove > 0 || yMove < 0)
                    {
                        yMove *= -1;
                    }                    
                    SndfxBouncingBomb[bounceSnd].Play();
                    bounceSnd++;
                    if (bounceSnd > SndfxBouncingBomb.Count - 1)
                    {
                        bounceSnd = 0;
                    }
                    wallHitted = false;
                }
                else
                {
                    this.flying = "";
                    xMove = 0;
                    yMove = 0;
                    wallHitted = false;
                }
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
