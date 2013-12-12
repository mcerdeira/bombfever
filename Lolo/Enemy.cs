using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lolo
{
    class Enemy
    {
        private Texture2D Texture;
        private int EnType;
        int Columns;
        int currentFrame;
        public int inmunityCounter = 0; // Frame duration of inmunity (after being hitted)
        private Vector2 Location;
        public Rectangle hitBox;

        public Enemy(Texture2D tex, int type, Vector2 location)
        {
            this.Texture = tex;
            this.EnType = type;
            this.Columns = tex.Width / 50; //30
            this.currentFrame = 0;
            this.Location = location;
        }

        public void Die()
        {
            
        }

        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;
            // Draw the player in the new location(x,y)
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);
            if (inmunityCounter % 4 == 0)
            {
                int hitX = (int)Location.X + 5;
                int hitY = (int)Location.Y + 10;
                hitBox = new Rectangle(hitX, hitY, 40, 40);//destinationRectangle;
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
                //spriteBatch.Draw(Texture, hitBox, hitBox, Color.Red);
            }
        }
    }
}
