using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
/*
 * Implements map layout as seen in ScreenMap.xlsx!Basic Layout 
 */

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
        private Player player;
        private Player player2;
        private BombManager bombmanager;

        public Map(Player player, Player player2, BombManager bombmanager)
        {
            this.bombmanager = bombmanager;
            this.player = player;
            this.player2 = player2;
        }

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

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void RemoveTile(Tile tile)
        {
            if (tile.BreakAble && tile.ID != -100 && tile.ID != -200)
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
                    Item itm = new Item(Content, new Vector2(tile.hitBox.X, tile.hitBox.Y), player, player2, this);
                    items.Add(itm);
                }
            }
            //TODO, previous to remove the item, an animation must occur

            tiles.Remove(tile);
        }

        public int[] getNeighbor(int node)
        {
            // Returns the neighbors, based on fixed positions, ignores known non walkables 

            int[] neighbors;
            switch (node)
            {
                case 9:
                    neighbors = new int[]{17, 10};                    
                    break;
                case 10:
                    neighbors = new int[] {9, 11, 18};
                    break;  
                case 11:
                    neighbors = new int[] { 10, 12, 19 };
                    break;
                case 12:
                    neighbors = new int[] { 11, 13, 20 };
                    break;
                case 13:
                    neighbors = new int[] { 12, 14, 21 };
                    break;
                case 14:
                    neighbors = new int[] { 13, 15, 22 };
                    break;
                case 15:
                    neighbors = new int[] { 14, 104, 23 };
                    break;
                case 17:
                    neighbors = new int[] {9, 18, 25 };
                    break;
                case 18:
                    neighbors = new int[] {10, 17, 19, 26 };
                    break;
                case 19:
                    neighbors = new int[] {11, 18, 20, 27 };
                    break;
                case 20:
                    neighbors = new int[] {12, 19, 21, 28 };
                    break;
                case 21:
                    neighbors = new int[] { 13, 20, 22, 29 };
                    break;
                case 22:
                    neighbors = new int[] { 14, 21, 23, 30 };
                    break;
                case 23:
                    neighbors = new int[] { 15, 22, 112, 31 };
                    break;
                case 25:
                    neighbors = new int[] {};
                    break;
                case 26:
                    neighbors = new int[] { };
                    break;
                case 27:
                    neighbors = new int[] { };
                    break;
                case 28:
                    neighbors = new int[] { };
                    break;
                case 29:
                    neighbors = new int[] { };
                    break;
                case 30:
                    neighbors = new int[] { };
                    break;
                case 31:
                    neighbors = new int[] { };
                    break;
                case 33:
                    neighbors = new int[] { };
                    break;
                case 34:
                    neighbors = new int[] { };
                    break;
                case 35:
                    neighbors = new int[] { };
                    break;
                case 36:
                    neighbors = new int[] { };
                    break;
                case 37:
                    neighbors = new int[] { };
                    break;
                case 38:
                    neighbors = new int[] { };
                    break;
                case 39:
                    neighbors = new int[] { };
                    break;
                case 41:
                    neighbors = new int[] { };
                    break;
                case 42:
                    neighbors = new int[] { };
                    break;
                case 43:
                    neighbors = new int[] { };
                    break;
                case 44:
                    neighbors = new int[] { };
                    break;
                case 45:
                    neighbors = new int[] { };
                    break;
                case 46:
                    neighbors = new int[] { };
                    break;
                case 47: neighbors = new int[] { };
                    break;
                case 49:
                    neighbors = new int[] { };
                    break;
                case 50:
                    neighbors = new int[] { };
                    break;
                case 51:
                    neighbors = new int[] { };
                    break;
                case 52:
                    neighbors = new int[] { };
                    break;  
                case 53:
                    neighbors = new int[] { };
                    break;
                case 54:
                    neighbors = new int[] { };
                    break;  
                case 55:
                    neighbors = new int[] { };
                    break;  
                case 57:
                    neighbors = new int[] { };
                    break;  
                case 58:
                    neighbors = new int[] { };
                    break;
                case 59:
                    neighbors = new int[] { };
                    break;  
                case 60:
                    neighbors = new int[] { };
                    break;
                case 61:
                    neighbors = new int[] { };
                    break;
                case 62:
                    neighbors = new int[] { };
                    break;
                case 63:
                    neighbors = new int[] { };
                    break;
                case 65:
                    neighbors = new int[] { };
                    break;
                case 66:
                    neighbors = new int[] { };
                    break;
                case 67:
                    neighbors = new int[] { };
                    break;
                case 68:
                    neighbors = new int[] { };
                    break;
                case 69:
                    neighbors = new int[] { };
                    break;
                case 70:
                    neighbors = new int[] { };
                    break;
                case 71:
                    neighbors = new int[] { };
                    break;
                case 73:
                    neighbors = new int[] { };
                    break;
                case 74:
                    neighbors = new int[] { };
                    break;
                case 75:
                    neighbors = new int[] { };
                    break;
                case 76:
                    neighbors = new int[] { };
                    break;
                case 77:
                    neighbors = new int[] { };
                    break;
                case 78:
                    neighbors = new int[] { };
                    break;
                case 79:
                    neighbors = new int[] { };
                    break;
                case 81:
                    neighbors = new int[] { };
                    break;
                case 82:
                    neighbors = new int[] { };
                    break;
                case 83:
                    neighbors = new int[] { };
                    break;
                case 84:
                    neighbors = new int[] { };
                    break;
                case 85:
                    neighbors = new int[] { };
                    break;
                case 86:
                    neighbors = new int[] { };
                    break;
                case 87:
                    neighbors = new int[] { };
                    break;
                default:
                    neighbors = new int[] { };
                    break;
            }
            return neighbors;
        }

        public void GenerateLevel(ContentManager content, string LevelFile = "")
        {
            this.Content = content;
            Random rdn = new Random();
            int row = 0;
            int col = 0;
            int v = 0;
            bool walkable = false;
            int[,] arrtiles = new int[16, 12];
            int[] colsMap = new int[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            int[] rowsMap = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };

            if (LevelFile == "")
            {
                for (int r = 0; r < 12; r++)
                {
                    col = 0;
                    for (int c = 0; c < 8; c++)
                    {
                        walkable = false;
                        if ((r == 1 && c == 1) || // Vertice, room for player to initially move
                            (r == 1 && c == 2) ||
                            (r == 2 && c == 1) ||

                            (r == 9 && c == 14) ||
                            (r == 10 && c == 14) ||
                            (r == 10 && c == 13) 
                            )
                        {
                            v = 0; // 4 vertices empty
                        }
                        else if (r== 0 ||
                                r == 11 ||
                                c == 0 ||
                                c == 15||
                                (c == 4 && r % 2 != 0) ||
                                 (c == 13 && r % 2 != 0) ||
                                 (c == 2 && r % 2 == 0) ||
                                 (c == 11 && r % 2 == 0)
                                )
                        {
                            v = 2; // Iron (fixed positions)                                                
                        }
                        else if ((r == 6 && c == 7) || (r == 6 && c == 8))
                        {
                            // Center, flags items  
                            if (c == 7)
                            {
                                v = -100;
                            }
                            else
                            {
                                v = -200;
                            }                            
                        }
                        else
                        {
                            v = rdn.Next(-20, 20); // Now lets do some random...  
                            arrtiles[c, r] = v; // Save the random, for mirroring
                        }

                        if (c == 0 && r == 5 || c == 0 && r == 6 ||
                                c == 15 && r == 5 || c == 15 && r == 6) 
                        {
                            v = 0; // Warps
                        }

                        if (v > 9)
                        {
                            v = 0; // But, 0 has a little more chances
                        }
                        if (v < 0 || v == 6 || v == 7 || v == 8 || v == 9)
                        {
                            if (v != -200 && v != -100)
                            {
                                v = 1; // and regular bricks has even more chances!
                            }
                        }

                        if (v == 0)
                        {
                            // If 0, then is a walkable block, but lets put some random to decide if regular empty space or what
                            walkable = true;
                        }
                        Vector2 pos = new Vector2(col, row);
                        Tile t = new Tile(pos, content, player, player2, (v != 2), walkable, this, v, bombmanager);
                        tiles.Add(t);                                                       
                        col += 50;
                    }
                    row += 50;
                }

                // Mirroring
                row = 0;
                for (int r = 0; r < 12; r++)
                {
                    col = 400;
                    for (int c = 8; c < 16; c++)
                    {
                        walkable = false;
                        if ((r == 1 && c == 1) || // Vertice, room for player to initially move
                            (r == 1 && c == 2) ||
                            (r == 2 && c == 1) ||

                            (r == 9 && c == 14) ||
                            (r == 10 && c == 14) ||
                            (r == 10 && c == 13)
                            )
                        {
                            v = 0; // 4 vertices empty
                        }
                        else if (r == 0 ||
                                r == 11 ||
                                c == 0 ||
                                c == 15 ||
                                (c == 4 && r % 2 != 0) ||
                                 (c == 13 && r % 2 != 0) ||
                                 (c == 2 && r % 2 == 0) ||
                                 (c == 11 && r % 2 == 0)
                                )
                        {
                            v = 2; // Iron (fixed positions)                                                
                        }
                        else if ((r == 6 && c == 7) || (r == 6 && c == 8))
                        {
                            // Center, flags items  
                            if (c == 7)
                            {
                                v = -100;
                            }
                            else
                            {
                                v = -200;
                            }   
                        }
                        else
                        {
                            v = arrtiles[colsMap[c], rowsMap[r]];
                        }

                        if (c == 0 && r == 5 || c == 0 && r == 6 ||
                                c == 15 && r == 5 || c == 15 && r == 6)
                        {
                            v = 0; // Warps
                        }

                        if (v > 9)
                        {
                            v = 0; // But, 0 has a little more chances
                        }
                        if (v < 0 || v == 6 || v == 7 || v == 8 || v == 9)
                        {
                            if (v != -200 && v != -100)
                            {
                                v = 1; // and regular bricks has even more chances!
                            }
                        }

                        if (v == 0)
                        {
                            // If 0, then is a walkable block, but lets put some random to decide if regular empty space or what
                            walkable = true;
                        }
                        Vector2 pos = new Vector2(col, row);
                        Tile t = new Tile(pos, content, player, player2, (v != 2), walkable, this, v, bombmanager);
                        tiles.Add(t);                        
                        col += 50;
                    }
                    row += 50;
                }
            }
            else
            {
                string line;
                bool comentary = false ;
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(Directory.GetCurrentDirectory() + "\\Levels\\" + LevelFile);
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
                                    if (line.Substring(c, 1) == "@")
                                    {
                                        v = rdn.Next(0, 5); // Now lets do some random...
                                    }
                                    else
                                    {
                                        v = Int32.Parse(line.Substring(c, 1));
                                    }
                                    if (v == 0)
                                    {
                                        // If 0, then is a walkable block, but lets put some random to decide if regular empty space or what
                                        walkable = true;
                                    }
                                    if (v != 0)
                                    {
                                        Vector2 pos = new Vector2(col, row);
                                        Tile t = new Tile(pos, content, player, player2, (v != 2), walkable, this, v, bombmanager);
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
                }catch(Exception exc){
                    // If an error ocurred loading from file, lets just leave what we can load
                }
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
