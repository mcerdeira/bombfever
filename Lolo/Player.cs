using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lolo
{
    public class Player
    {
        private Vector2 RespawnLoc;
        public bool wallHitted;
        public Texture2D Texture { get; set; }        
        public int Columns { get; set; }
        private PlayerControls PCtrls;
        private int currentFrame;
        private string KeyControl;
        private string InstanceName;
        private PlayerStyle PStlye;
        private int ItemCollected = -1;
        private int[] idleFrames = new int[] { 0, 1, 2, 3 };
        private int[] walkFrames = new int[] { 4, 5, 6, 7 };
        private int[] deadFrames = new int[] { 8, 9, 10, 11 };
        public string Status; // walking, idle, dead
        public string PrevStatus;
        public Rectangle hitBox;
        public Vector2 newPosition;
        Vector2 Location;        
        Vector2 Speed = new Vector2();
        //Vector2 Acceleration = new Vector2(40, 40);
        BombManager BombMan;
        Score Score;
        public int BombCount = 0;
        public int BombMax = 3;
        int minVel = 200;
        int maxVel = 280;
        int directionX = 0;
        int directionY = 0;

        public Player(Texture2D texture, Vector2 location, ControlType ctype, BombManager BombMan, Score score, string instancename, PlayerStyle pstlye)
        {
            this.Score = score;
            this.RespawnLoc = location;
            this.Location = location;
            Speed.X = minVel;
            Speed.Y = minVel;
            Status = "idle";
            PrevStatus = "";
            Texture = texture;
            currentFrame = 0;
            Columns = texture.Width / 30;
            this.PStlye = pstlye;
            this.InstanceName = instancename;
            this.BombMan = BombMan;
            this.PCtrls = new PlayerControls(ctype);       
        }

        public void setItem(int itemstyle)
        {
            this.ItemCollected = itemstyle;            
        }

        public void Update(GameTime gametime)
        {
            int totalFrames = -1;
            int resetFrame = -1;
         
            UpdateInput((float)gametime.ElapsedGameTime.TotalSeconds);
            if (this.Status == "walking")
            {
                resetFrame = 4;
                totalFrames = 7;
            }
            else if(this.Status == "idle")
            {
                resetFrame = 0;
                totalFrames = 3;
            }
            else if(this.Status == "dead")
            {
                resetFrame = 8;
                totalFrames = 11;
            }

            if (this.Status != this.PrevStatus)
            {
                currentFrame = resetFrame;
                this.PrevStatus = this.Status;
            }
            currentFrame++;
            if (currentFrame == totalFrames)
            {
                if (this.Status == "dead")
                {
                    currentFrame = resetFrame;
                    resPawn();
                }
                else
                {
                    currentFrame = resetFrame;
                }
            }
        }

        private void resPawn()
        {
            this.Status = "respawning";
            string dest = (InstanceName == "p1") ? "p2" : "p1";
            Score.setScore(dest);            
            this.Location = this.RespawnLoc;
        }

        private void UpdateInput(float elapsedTime)
        {
            #warning add joystick support
            //GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed   
            if (this.Status != "dead")
            {
                this.Status = "idle";
                KeyboardState st = Keyboard.GetState();
                if (!st.IsKeyDown(PCtrls.Bomb))
                {
                    KeyControl = "";
                }

                if (st.IsKeyDown(PCtrls.Bomb) && KeyControl != "bomb" && this.BombCount < this.BombMax)
                {
                    KeyControl = "bomb";
                    BombMan.SpawnBomb(Location, InstanceName);
                    this.BombCount++;
                }

                if (st.IsKeyDown(PCtrls.Right))
                {
                    this.Status = "walking";
                    directionX = 1;
                }

                if (st.IsKeyDown(PCtrls.Left))
                {
                    this.Status = "walking";
                    directionX = -1;
                }

                if (st.IsKeyDown(PCtrls.Down))
                {
                    this.Status = "walking";
                    directionY = 1;
                }

                if (st.IsKeyDown(PCtrls.Up))
                {
                    this.Status = "walking";
                    directionY = -1;
                }

                if (!st.IsKeyDown(PCtrls.Right) &&
                    !st.IsKeyDown(PCtrls.Left))
                {
                    //Speed.X = 0;
                    directionX = 0;
                }

                if (!st.IsKeyDown(PCtrls.Up) &&
                    !st.IsKeyDown(PCtrls.Down))
                {
                    directionY = 0;
                }
                Location.X += (Speed.X * elapsedTime) * directionX;
                Location.Y += (Speed.Y * elapsedTime) * directionY;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (wallHitted)
            {
                Location = newPosition;
                wallHitted = false;
            }

            // Draw the player in the new location(x,y)
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);
            hitBox = destinationRectangle;
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
        }
    }
}
