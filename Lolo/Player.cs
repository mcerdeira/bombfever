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
        public bool wall;
        public Texture2D Texture { get; set; }        
        public int Columns { get; set; }
        private int currentFrame;
        private string KeyControl; 
        private int[] idleFrames = new int[] { 0, 1, 2, 3 };
        private int[] walkFrames = new int[] { 4, 5, 6, 7 };
        public string Status;        
        public string PrevStatus;
        public Rectangle hitBox;
        Vector2 Location;
        Vector2 preLocation;
        Vector2 Speed = new Vector2();
        Vector2 Acceleration = new Vector2(40, 40);
        BombManager BombMan;
        public int BombCount = 0;
        public int BombMax = 3;
        int minVel = 100;
        int maxVel = 280;
        int directionX = 0;
        int directionY = 0;

        public Player(Texture2D texture, Vector2 location, BombManager BombMan)
        {            
            Speed.X = minVel;
            Speed.Y = minVel;
            Status = "idle";
            PrevStatus = "";
            Texture = texture;
            currentFrame = 0;
            Columns = texture.Width / 30;
            this.BombMan = BombMan;
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
            if (this.Status != this.PrevStatus)
            {
                currentFrame = resetFrame;
                this.PrevStatus = this.Status;
            }
            currentFrame++;
            if (currentFrame == totalFrames)
            {
                currentFrame = resetFrame;
            }
        }

        private void UpdateInput(float elapsedTime)
        {
            Console.WriteLine(KeyControl);
            this.Status = "idle";
            KeyboardState st = Keyboard.GetState();
            if (!st.IsKeyDown(Keys.Space))
            {
                KeyControl = "";
            }

            if (st.IsKeyDown(Keys.Space) && KeyControl != "space" &&  this.BombCount < this.BombMax)
            {
                KeyControl = "space";
                BombMan.SpawnBomb(Location, "player", this);
                this.BombCount++;
            }

            if (st.IsKeyDown(Keys.Right))
            {
                this.Status = "walking";
                if (Speed.X < maxVel)
                {
                    Speed.X += Acceleration.X;                
                }
                directionX = 1;               
            }

            if (st.IsKeyDown(Keys.Left))
            {
                this.Status = "walking";
                if (Speed.X < maxVel)
                {
                    Speed.X += Acceleration.X;
                }
                directionX = -1;
            }

            if (st.IsKeyDown(Keys.Down))
            {
                this.Status = "walking";
                if (Speed.Y < maxVel)
                {
                    Speed.Y += Acceleration.Y;
                }
                directionY = 1;         
            }

            if (st.IsKeyDown(Keys.Up))
            {
                this.Status = "walking";
                if (Speed.Y < maxVel)
                {
                    Speed.Y += Acceleration.Y;
                }
                directionY = -1;
            }

            if (!st.IsKeyDown(Keys.Right) &&
                !st.IsKeyDown(Keys.Left) && Speed.X > minVel)
            {
                Speed.X = 0;
                directionX = 0;
            }

            if (!st.IsKeyDown(Keys.Up) &&
                !st.IsKeyDown(Keys.Down) && Speed.Y > minVel)
            {
                Speed.Y = 0;
                directionY = 0;
            }

            Location.X += (Speed.X * elapsedTime) * directionX;
            Location.Y += (Speed.Y * elapsedTime) * directionY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {   
            // Draw the player in the new location(x,y)
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);
            hitBox = destinationRectangle;
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            if (wall)
            {  
                // After drawing, if a collision exists, I revert the movement
                wall = false;
                Location.X = preLocation.X;
                Location.Y = preLocation.Y;
            }
            else
            {
                // After drawing, if no collision exists, y save the pre-location (safe location)
                preLocation.X = Location.X;
                preLocation.Y = Location.Y;
            }
        }
    }
}
