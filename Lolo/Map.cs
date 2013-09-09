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
                    neighbors = new int[] {17, 26, 33};
                    break;
                case 26:
                    neighbors = new int[] {18, 25, 27, 34};
                    break;
                case 27:
                    neighbors = new int[] {19, 26, 28, 35};
                    break;
                case 28:
                    neighbors = new int[] {20, 27, 29, 36};
                    break;
                case 29:
                    neighbors = new int[] {21, 28, 30, 37};
                    break;
                case 30:
                    neighbors = new int[] {22, 29, 31, 38};
                    break;
                case 31:
                    neighbors = new int[] {23, 30, 120, 39};
                    break;
                case 33:
                    neighbors = new int[] {25, 34, 41};
                    break;
                case 34:
                    neighbors = new int[] {26, 33, 35, 42};
                    break;
                case 35:
                    neighbors = new int[] {27, 34, 36, 43};
                    break;
                case 36:
                    neighbors = new int[] {28, 35, 37, 44};
                    break;
                case 37:
                    neighbors = new int[] {29, 36, 38, 45};
                    break;
                case 38:
                    neighbors = new int[] {30, 37, 39, 46};
                    break;
                case 39:
                    neighbors = new int[] {31, 38, 128, 47};
                    break;
                case 41: // Near warp
                    neighbors = new int[] {33, 42, 49}; 
                    break;
                case 42:
                    neighbors = new int[] {34, 41, 43, 50};
                    break;
                case 43:
                    neighbors = new int[] {35, 42, 44, 51};
                    break;
                case 44:
                    neighbors = new int[] {36, 43, 45, 52};
                    break;
                case 45:
                    neighbors = new int[] {37, 44, 46, 53};
                    break;
                case 46:
                    neighbors = new int[] {38, 45, 47, 54};
                    break;
                case 47: 
                    neighbors = new int[] {39, 46, 136, 55};
                    break;
                case 49: // Near warp
                    neighbors = new int[] {41, 50, 57}; 
                    break;
                case 50:
                    neighbors = new int[] {49, 42, 51, 58};
                    break;
                case 51:
                    neighbors = new int[] {50, 43, 52, 59};
                    break;
                case 52:
                    neighbors = new int[] {51, 44, 53, 60};
                    break;  
                case 53:
                    neighbors = new int[] {52, 45, 54, 61};
                    break;
                case 54:
                    neighbors = new int[] {53, 46, 55, 62};
                    break;  
                case 55:
                    neighbors = new int[] {54, 47, 144, 63};
                    break;  
                case 57:
                    neighbors = new int[] {49, 58, 65};
                    break;  
                case 58:
                    neighbors = new int[] {50, 57, 59, 66};
                    break;
                case 59:
                    neighbors = new int[] {51, 58, 60, 67};
                    break;  
                case 60:
                    neighbors = new int[] {52, 59, 61, 68};
                    break;
                case 61:
                    neighbors = new int[] {53, 60, 62, 69};
                    break;
                case 62:
                    neighbors = new int[] {54, 61, 63, 70};
                    break;
                case 63:
                    neighbors = new int[] {55, 62, 152, 71};
                    break;
                case 65:
                    neighbors = new int[] {57, 66, 73};
                    break;
                case 66:
                    neighbors = new int[] {58, 65, 67, 74};
                    break;
                case 67:
                    neighbors = new int[] {59, 66, 68, 75};
                    break;
                case 68:
                    neighbors = new int[] {60, 67, 69, 76};
                    break;
                case 69:
                    neighbors = new int[] {61, 68, 70, 77};
                    break;
                case 70:
                    neighbors = new int[] {62, 69, 71, 78};
                    break;
                case 71:
                    neighbors = new int[] {63, 70, 160, 79};
                    break;
                case 73:
                    neighbors = new int[] {65, 74, 81};
                    break;
                case 74:
                    neighbors = new int[] {66, 73, 75, 82};
                    break;
                case 75:
                    neighbors = new int[] {67, 74, 76, 83};
                    break;
                case 76:
                    neighbors = new int[] {68, 75, 77, 84};
                    break;
                case 77:
                    neighbors = new int[] {69, 76, 78, 85};
                    break;
                case 78:
                    neighbors = new int[] {70, 77, 79, 86};
                    break;
                case 79:
                    neighbors = new int[] {71, 78, 168, 87};
                    break;
                case 81:
                    neighbors = new int[] {73, 82};
                    break;
                case 82:
                    neighbors = new int[] {74, 81, 83};
                    break;
                case 83:
                    neighbors = new int[] {75, 82, 84};
                    break;
                case 84:
                    neighbors = new int[] {76, 83, 85};
                    break;
                case 85:
                    neighbors = new int[] {77, 84, 86};
                    break;
                case 86:
                    neighbors = new int[] {78, 85, 87};
                    break;
                case 87:
                    neighbors = new int[] {79, 86, 176};
                    break;
                case 104:
                    neighbors = new int[] {15, 105, 112};
                    break;
                case 105:
                    neighbors = new int[] {104, 106, 113};
                    break;
                case 106:
                    neighbors = new int[] {105, 107, 114};
                    break;
                case 107:
                    neighbors = new int[] {106, 108, 115};
                    break;
                case 108:
                    neighbors = new int[] {107, 109, 116};
                    break;
                case 109:
                    neighbors = new int[] {108, 110, 117};
                    break;
                case 110:
                    neighbors = new int[] {109, 118};
                    break;
                case 112:
                    neighbors = new int[] {23, 104, 113, 120};
                    break;
                case 113:
                    neighbors = new int[] {112, 105, 114, 121};
                    break;
                case 114:
                    neighbors = new int[] {113, 106, 115, 122};
                    break;
                case 115:
                    neighbors = new int[] {114, 107, 116, 123};
                    break;
                case 116:
                    neighbors = new int[] {115, 108, 117, 124};
                    break;
                case 117:
                    neighbors = new int[] {116, 109, 118, 125};
                    break;
                case 118:
                    neighbors = new int[] {117, 110, 126};
                    break;
                case 120:
                    neighbors = new int[] {31, 112, 121, 128};
                    break;
                case 121:
                    neighbors = new int[] {113, 120, 122, 129};
                    break;
                case 122:
                    neighbors = new int[] {114, 121, 123, 130};
                    break;
                case 123:
                    neighbors = new int[] {115, 122, 124, 131};
                    break;
                case 124:
                    neighbors = new int[] {116, 123, 125, 132};
                    break;
                case 125:
                    neighbors = new int[] {117, 124, 126, 133};
                    break;
                case 126:
                    neighbors = new int[] {118, 125, 134};
                    break;
                case 128:
                    neighbors = new int[] {39, 120, 129, 136};
                    break;
                case 129:
                    neighbors = new int[] {128, 121, 130, 137};
                    break;
                case 130:
                    neighbors = new int[] {129, 122, 131, 138};
                    break;
                case 131:
                    neighbors = new int[] {130, 123, 132, 139};
                    break;
                case 132:
                    neighbors = new int[] {131, 124, 133, 140};
                    break;
                case 133:
                    neighbors = new int[] {132, 125, 134, 141};
                    break;
                case 134:
                    neighbors = new int[] {133, 126, 142};
                    break;
                case 136:
                    neighbors = new int[] {47, 128, 137, 144};
                    break;
                case 137:
                    neighbors = new int[] {136, 129, 138, 145};
                    break;
                case 138:
                    neighbors = new int[] {137, 130, 139, 146};
                    break;
                case 139:
                    neighbors = new int[] {138, 131, 140, 147};
                    break;
                case 140:
                    neighbors = new int[] {139, 132, 141, 148};
                    break;
                case 141:
                    neighbors = new int[] {140, 133, 142, 149};
                    break;
                case 142:
                    neighbors = new int[] {141, 134, 150};
                    break;
                case 144:
                    neighbors = new int[] {55, 136, 145, 152};
                    break;
                case 145:
                    neighbors = new int[] {144, 137, 146, 153};
                    break;
                case 146:
                    neighbors = new int[] {145, 138, 147, 154};
                    break;
                case 147:
                    neighbors = new int[] {146, 139, 148, 155};
                    break;
                case 148:
                    neighbors = new int[] {147, 140, 149, 156};
                    break;
                case 149:
                    neighbors = new int[] {148, 141, 50, 157};
                    break;
                case 150:
                    neighbors = new int[] {149, 142, 158};
                    break;
                case 152:
                    neighbors = new int[] {63, 144, 153, 160};
                    break;
                case 153:
                    neighbors = new int[] {152, 145, 154, 161};
                    break;
                case 154:
                    neighbors = new int[] {153, 146, 155, 162};
                    break;
                case 155:
                    neighbors = new int[] {154, 147, 156, 163};
                    break;
                case 156:
                    neighbors = new int[] {155, 148, 157, 164};
                    break;
                case 157:
                    neighbors = new int[] {156, 149, 158, 165};
                    break;
                case 158:
                    neighbors = new int[] {157, 150, 166};
                    break;
                case 160:
                    neighbors = new int[] {71, 152, 161, 168};
                    break;
                case 161:
                    neighbors = new int[] {160, 153, 162, 169};
                    break;
                case 162:
                    neighbors = new int[] {161, 154, 163, 170};
                    break;
                case 163:
                    neighbors = new int[] {162, 155, 164, 171};
                    break;
                case 164:
                    neighbors = new int[] {163, 156, 165, 172};
                    break;
                case 165:
                    neighbors = new int[] {164, 157, 166, 173};
                    break;
                case 166:
                    neighbors = new int[] {165, 158, 174};
                    break;
                case 168:
                    neighbors = new int[] {79, 160, 169, 176};
                    break;
                case 169:
                    neighbors = new int[] {168, 161, 170, 177};
                    break;
                case 170:
                    neighbors = new int[] {169, 162, 171, 178};
                    break;
                case 171:
                    neighbors = new int[] {170, 163, 172, 179};
                    break;
                case 172:
                    neighbors = new int[] {171, 164, 173, 180};
                    break;
                case 173:
                    neighbors = new int[] {172, 165, 174, 181};
                    break;
                case 174:
                    neighbors = new int[] {173, 166, 182};
                    break;
                case 176:
                    neighbors = new int[] {87, 168, 177};
                    break;
                case 177:
                    neighbors = new int[] {176, 169, 178};
                    break;
                case 178:
                    neighbors = new int[] {177, 170, 179};
                    break;
                case 179:
                    neighbors = new int[] {178, 171, 180};
                    break;
                case 180:
                    neighbors = new int[] {179, 172, 181};
                    break;
                case 181:
                    neighbors = new int[] {180, 173, 182};
                    break;
                case 182:
                    neighbors = new int[] {181, 174};
                    break;
                default:
                    neighbors = new int[] {-1};
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
