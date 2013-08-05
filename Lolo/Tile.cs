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
    public class Tile
    {
        public bool BreakAble;
        public string Action;
        public int Status = 0; // frame status
        public int ID;
        private Vector2 Position;
        private Texture2D Texture;
        private int Columns;
        public Rectangle hitBox;
        private Player player;
        private Enemy Enemy;
        private Map Map;

        public Tile(Vector2 position, ContentManager Content, Player player, bool brekable, Map map, int id)
        {
            this.BreakAble = brekable;
            this.Action = "";
            this.player = player;
            this.ID = id;
            Texture = Content.Load<Texture2D>(this.ID.ToString());            
            this.Position = position;
            this.Columns = Texture.Width / 50;
            this.Map = map;
        }

        public void Update()
        {            
            switch (this.ID)
            {
                case 0:
                    // Space
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                default:
                    break;
            }
            if(hitBox.Intersects(player.hitBox))
            {
                //if (player.hitBox.Right > hitBox.Left && player.hitBox.Right < hitBox.Right)
                //{
                //    player.wallX = "left";
                //    Console.Write("left");
                //}
                //else if (player.hitBox.Left < hitBox.Right && player.hitBox.Left > hitBox.Left)
                //{
                //    player.wallX = "right";
                //    Console.Write("right");
                //}


                //if (player.hitBox.Top < hitBox.Bottom && player.hitBox.Top > hitBox.Top)
                //{                    
                //    player.wallY = "top";
                //    Console.Write("top");
                //}
                //else if (player.hitBox.Bottom > hitBox.Top && player.hitBox.Bottom < hitBox.Bottom)
                //{
                //    player.wallY = "bottom";
                //    Console.Write("bottom");
                //}

                Vector2 v = General.IntersectDepthVector(player.hitBox,this.hitBox);

	            float absx = Math.Abs(v.X);
	            float absy = Math.Abs(v.Y);
                string loc ="";
                string axis ="";
		
                    // if a collision has happened		
	            if (! (v.X ==0 && v.Y ==0) ) {
		            if (absx > absy) // the shallower impact is the correct one- this is on the y axis
		            {
			            axis = "y";
			            if (v.Y < 0)
				            loc = "top";
			            else
				            loc = "bottom";

                        Vector2 newpos = new Vector2(player.hitBox.X, player.hitBox.Y+v.Y);
                        player.newPosition = newpos;
		            }
		            else // the x axis!
		            {
			            axis = "x";
			            if (v.X < 0)
				            loc = "left";
			            else
				            loc = "right";

                        Vector2 newpos = new Vector2(player.hitBox.X + v.X, player.hitBox.Y);
                        player.newPosition = newpos;
		            }
                    player.wallHitted = true;                   		            
	            }
                
            }
            if(Action == "dead")
            {
                Map.RemoveTile(this);
            }
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
    }
}
