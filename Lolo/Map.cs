using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

/*
 * 0 Empty space
 * 1 Brick (breakable 1 hit)  
 * 2 Iron (unbreakable)
 * 3 Stone (breakable 1 hits)
 * 4 Mud (breakable 1 hit, regenerates)
 * 5 Wood breakable 1 hit, catches fire and propagates to other wood bricks next to it)
 * 6 Empty space
 * 7 Empty space
 * 8 Empty space
 * 9 Brick (breakable 1 hit) 
 * P Main player
 * E Enemy
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
        public List<Item> items = new List<Item>();
        private ContentManager Content;

        public void Update()
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                tiles[index].Update();
            }

            for (int index = 0; index < items.Count; index++)
            {
                items[index].Update();
            }
        }

        public void RemoveTile(Tile tile)
        {
            if (tile.BreakAble)
            {
                // Decide, randomly if there is any item hidden inside
                Random rdn = new Random();
                int v;
                if (tile.ID == 1)
                {
                    v = rdn.Next(0, 50); // If it is a regular brick, chances are lower
                }
                else
                {
                    v = rdn.Next(0, 8); // If it is a special brick, chances are higher
                }                
                if (v == 1)
                {                    
                    // An item is hidden inside, yay!!                    
                    Item itm = new Item(Content, new Vector2(tile.hitBox.X, tile.hitBox.Y));
                    items.Add(itm);
                }
            }
            //TODO, previous to remove the item, an animation must occur

            tiles.Remove(tile);
        }

        public void GenerateLevel(ContentManager content, Player player, Player player2, string LevelFile = "")
        {
            this.Content = content;
            Random rdn = new Random();
            int row = 0;
            int col = 0;
            int v = 0;
            bool walkable = false;
            if (LevelFile == "")
            {
                for (int r = 0; r < 12; r++)
                {
                    col = 0;
                    for (int c = 0; c < 17; c++)
                    {
                        walkable = false;
                        if ((r == 0 && c == 0) || // Vertice, room for player to initially move
                            (r == 1 && c == 0) ||
                            (r == 0 && c == 1) ||

                            (r == 10 && c == 15) ||
                            (r == 11 && c == 15) ||
                            (r == 11 && c == 14) ||

                            (r == 6 && c == 7) || // Center, a prize item
                            (r == 6 && c == 8)
                            )
                        {
                            v = 0; // 4 vertices empty
                        }
                        else if ((c == 1 && r % 2 != 0) ||
                                 (c == 14 && r % 2 != 0) ||
                                 (c == 3 && r % 2 == 0) ||
                                 (c == 12 && r % 2 == 0)
                                )
                        {
                            v = 2; // Iron (fixed positions)                        
                        }
                        else
                        {
                            v = rdn.Next(-20, 20); // Now lets do some random...  
                        }
                        if (v > 9)
                        {
                            v = 0; // But, 0 has a little more chances
                        }
                        if (v < 0 || v == 6 || v == 7 || v == 8 || v == 9)
                        {
                            v = 1; // and regular brucks has even more chances!
                        }

                        if (v == 0)
                        {
                            // If 0, then is a walkable block, but lets put some random to decide if regular empty space or what
                            walkable = true;
                        }
                        if (v != 0)
                        {
                            Vector2 pos = new Vector2(col, row);
                            Tile t = new Tile(pos, content, player, player2, (v != 2), walkable, this, v);
                            tiles.Add(t);
                        }
                        col += 50;
                    }
                    row += 50;
                }
            }
            else
            {
                string line;
                bool comentary = false ;
                System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + "\\" + LevelFile);
                while ((line = file.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        if (line.StartsWith(@"/*"))
                        {
                            comentary = true;
                        }
                        if (!comentary)
                        {
                            col = 0;
                            for (int c = 0; c < 16; c++)
                            {
                                walkable = false;
                                v = Int32.Parse(line.Substring(c, 1));
                                if (v == 0)
                                {
                                    // If 0, then is a walkable block, but lets put some random to decide if regular empty space or what
                                    walkable = true;
                                }
                                if (v != 0)
                                {
                                    Vector2 pos = new Vector2(col, row);
                                    Tile t = new Tile(pos, content, player, player2, (v != 2), walkable, this, v);
                                    tiles.Add(t);
                                }
                                col += 50;
                            }
                            row += 50;
                        }
                        if (line.EndsWith(@"*/"))
                        {
                            comentary = false;
                        }
                    }
                }
                file.Close();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                tiles[index].Draw(spriteBatch);
            }
            for (int index = 0; index < items.Count; index++)
            {
                items[index].Draw(spriteBatch);
            }
        }
    }
}
