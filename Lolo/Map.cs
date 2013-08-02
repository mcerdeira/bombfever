using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

/*
 * 0 Empty space
 * 1 Brick (breakable 1 hit)  
 * 2 Iron (unbreakable)
 * 3 Stone (breakable 2 hits)
 * 4 Mud (breakable 1 hit, regenerates)
 * 5 Wood breakable 1 hit, catches fire and propagates to other wood bricks next to it)
 * 6 Empty space
 * 7 Empty space
 * 8 Empty space
 * 9 Brick (breakable 1 hit) 
 * P Main player
 * E Enemy
 * I Item (hidden into Brick)
 * A Pit
 * K Portal (teletransportation)
 * C Ice (slipery floor)
 * M Moving floor
*/

namespace Lolo
{
    public class Map
    {        
        public List<Tile> tiles = new List<Tile>();

        public void Update()
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                tiles[index].Update();
            }
        }

        public void GenerateLevel(ContentManager content, Player player)
        {
            Random rdn = new Random();           
            int row = 0;
            int v = 0;
            for (int r= 0; r< 20; r++)
            {
                int col = 0;
                for (int c = 0; c < 20; c++)
                {
                    if ((r == 0 && c == 0) ||
                        (r == 0 && c == 19) ||
                        (r == 19 && c == 0) ||
                        (r == 19 && c == 19)
                        )
                    {
                        v = 0; // 4 vertices empty
                    }
                    else if((r==10 && c == 10) ||
                             (c == 1 && r % 2 == 0) ||
                             (c == 18 && r % 2 == 0) ||
                             (c == 3 && r % 2 != 0) ||
                             (c == 16 && r % 2 != 0)
                            )
                    { 
                        v = 2; // Iron
                    }
                    else
                    {
                        v = rdn.Next(0, 20);
                    }                    
                    if(v==6 || v==7 || v==8 || v>9)
                    {
                        v = 0;
                    }
                    if (v == 9)
                    {
                        v = 1;
                    }
                    if (v != 0)
                    {
                        Vector2 pos = new Vector2(col, row);
                        Tile t = new Tile(pos, content,player,v);
                        tiles.Add(t);                        
                    }
                    col += 50;
                }
                row += 50;
            }            
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                tiles[index].Draw(spriteBatch);
            }
        }
    }
}
