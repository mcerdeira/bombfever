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
    public class Enemy
    {
        public Texture2D Texture { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        private int currentFrame;
        private int totalFrames;
        private int[] idleFrames = new int[] { 0, 1, 2, 3 };
        private int[] walkFrames = new int[] { 4, 5, 6, 7 };
        Vector2 Location;

        public Enemy(Texture2D texture, Vector2 location)
        {
            Texture = texture;
            currentFrame = 0;
            totalFrames = 7;
        }

        public void Update()
        {
            UpdateInput();
            currentFrame++;
            if (currentFrame == totalFrames)
                currentFrame = 0;
        }

        private void UpdateInput()
        {
            // TODO: Update location here
            // based on AI, pathfinding, etc
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = (int)((float)currentFrame / (float)Columns);
            int column = currentFrame % Columns;
            Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
            Rectangle destinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);            
            spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);            
        }
    }
}
