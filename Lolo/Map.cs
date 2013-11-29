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

namespace Lolo
{
    public class Map
    {        
        public List<Tile> tiles = new List<Tile>();
        public List<Item> items = new List<Item>();        
        private Player player;
        private Player player2;
        private BombManager bombmanager;
        private Score Score;
        private List<Texture2D> ItemTextures = new List<Texture2D>();
        private List<Texture2D> TileTextures = new List<Texture2D>();

        public Map(Player player, Player player2, BombManager bombmanager, List<Texture2D> itemtextures, List<Texture2D> tiletextures, Score score)
        {            
            this.Score = score;
            this.ItemTextures = itemtextures;
            this.TileTextures = tiletextures;
            this.bombmanager = bombmanager;
            this.player = player;
            this.player2 = player2;
        }

        public void GenPortal(int tile1)
        {
            int tile2 = getOppositeTile(tile1);
            tiles[tile1].GenPortal(TileTextures[7]);
            tiles[tile2].GenPortal(TileTextures[7]);
        }

        public int getOppositeTile(int tile)
        {
            int result = -1;
            switch (tile)
            {
                case 9:
                    result = 110;
                    break;
                case 10:
                    result = 109;
                    break;
                case 11:
                    result = 108;
                    break;
                case 12:
                    result = 107;
                    break;
                case 13:
                    result = 106;
                    break;
                case 14:
                    result = 105;
                    break;
                case 15:
                    result = 104;
                    break;
                case 17:
                    result = 118;
                    break;
                case 18:
                    result = 117;
                    break;
                case 19:
                    result = 116;
                    break;
                case 20:
                    result = 115;
                    break;
                case 21:
                    result = 114;
                    break;
                case 22:
                    result = 113;
                    break;
                case 23:
                    result = 112;
                    break;
                case 25:
                    result = 126;
                    break;
                case 26:
                    result = 125;
                    break;
                case 27:
                    result = 124;
                    break;
                case 28:
                    result = 123;
                    break;
                case 29:
                    result = 122;
                    break;
                case 30:
                    result = 121;
                    break;
                case 31:
                    result = 120;
                    break;
                case 33:
                    result = 134;
                    break;
                case 34:
                    result = 133;
                    break;
                case 35:
                    result = 132;
                    break;
                case 36:
                    result = 131;
                    break;
                case 37:
                    result = 130;
                    break;
                case 38:
                    result = 129;
                    break;
                case 39:
                    result = 128;
                    break;
                case 40: // Teleporter
                    result = 143;
                    break;
                case 41:
                    result = 142;
                    break;
                case 42:
                    result = 141;
                    break;
                case 43:
                    result = 140;
                    break;
                case 44:
                    result = 139;
                    break;
                case 45:
                    result = 138;
                    break;
                case 46:
                    result = 137;
                    break;
                case 47:
                    result = 136;
                    break;
                case 48: // Teleporter
                    result = 151;
                    break;
                case 49:
                    result = 150;
                    break;
                case 50:
                    result = 149;
                    break;
                case 51:
                    result = 148;
                    break;
                case 52:
                    result = 147;
                    break;
                case 53:
                    result = 146;
                    break;
                case 54:
                    result = 145;
                    break;
                case 55:
                    result = 144;
                    break;
                case 57:
                    result = 158;
                    break;
                case 58:
                    result = 157;
                    break;
                case 59:
                    result = 156;
                    break;
                case 60:
                    result = 155;
                    break;
                case 61:
                    result = 154;
                    break;
                case 62:
                    result = 153;
                    break;
                case 63:
                    result = 152;
                    break;
                case 65:
                    result = 166;
                    break;
                case 66:
                    result = 165;
                    break;
                case 67:
                    result = 164;
                    break;
                case 68:
                    result = 163;
                    break;
                case 69:
                    result = 162;
                    break;
                case 70:
                    result = 161;
                    break;
                case 71:
                    result = 160;
                    break;
                case 73:
                    result = 174;
                    break;
                case 74:
                    result = 173;
                    break;
                case 75:
                    result = 172;
                    break;
                case 76:
                    result = 171;
                    break;
                case 77:
                    result = 170;
                    break;
                case 78:
                    result = 169;
                    break;
                case 79:
                    result = 168;
                    break;
                case 81:
                    result = 182;
                    break;
                case 82:
                    result = 181;
                    break;
                case 83:
                    result = 180;
                    break;
                case 84:
                    result = 179;
                    break;
                case 85:
                    result = 178;
                    break;
                case 86:
                    result = 177;
                    break;
                case 87:
                    result = 176;
                    break;
                case 104:
                    result = 15;
                    break;
                case 105:
                    result = 14;
                    break;
                case 106:
                    result = 13;
                    break;
                case 107:
                    result = 12;
                    break;
                case 108:
                    result = 11;
                    break;
                case 109:
                    result = 10;
                    break;
                case 110:
                    result = 9;
                    break;
                case 112:
                    result = 23;
                    break;
                case 113:
                    result = 22;
                    break;
                case 114:
                    result = 21;
                    break;
                case 115:
                    result = 20;
                    break;
                case 116:
                    result = 19;
                    break;
                case 117:
                    result = 18;
                    break;
                case 118:
                    result = 17;
                    break;
                case 120:
                    result = 31;
                    break;
                case 121:
                    result = 30;
                    break;
                case 122:
                    result = 29;
                    break;
                case 123:
                    result = 28;
                    break;
                case 124:
                    result = 27;
                    break;
                case 125:
                    result = 26;
                    break;
                case 126:
                    result = 25;
                    break;
                case 128:
                    result = 39;
                    break;
                case 129:
                    result = 38;
                    break;
                case 130:
                    result = 37;
                    break;
                case 131:
                    result = 36;
                    break;
                case 132:
                    result = 35;
                    break;
                case 133:
                    result = 34;
                    break;
                case 134:
                    result = 33;
                    break;
                case 136:
                    result = 47;
                    break;
                case 137:
                    result = 46;
                    break;
                case 138:
                    result = 45;
                    break;
                case 139:
                    result = 44;
                    break;
                case 140:
                    result = 43;
                    break;
                case 141:
                    result = 42;
                    break;
                case 142:
                    result = 41;
                    break;
                case 143: // Teleporter
                    result = 40;
                    break;
                case 144:
                    result = 55;
                    break;
                case 145:
                    result = 54;
                    break;
                case 146:
                    result = 53;
                    break;
                case 147:
                    result = 52;
                    break;
                case 148:
                    result = 51;
                    break;
                case 149:
                    result = 50;
                    break;
                case 150:
                    result = 49;
                    break;
                case 151: // Teleporter
                    result = 48;
                    break;
                case 152:
                    result = 63;
                    break;
                case 153:
                    result = 62;
                    break;
                case 154:
                    result = 61;
                    break;
                case 155:
                    result = 60;
                    break;
                case 156:
                    result = 59;
                    break;
                case 157:
                    result = 58;
                    break;
                case 158:
                    result = 57;
                    break;
                case 160:
                    result = 71;
                    break;
                case 161:
                    result = 70;
                    break;
                case 162:
                    result = 69;
                    break;
                case 163:
                    result = 68;
                    break;
                case 164:
                    result = 67;
                    break;
                case 165:
                    result = 66;
                    break;
                case 166:
                    result = 65;
                    break;
                case 168:
                    result = 79;
                    break;
                case 169:
                    result = 78;
                    break;
                case 170:
                    result = 77;
                    break;
                case 171:
                    result = 76;
                    break;
                case 172:
                    result = 75;
                    break;
                case 173:
                    result = 74;
                    break;
                case 174:
                    result = 73;
                    break;
                case 176:
                    result = 87;
                    break;
                case 177:
                    result = 86;
                    break;
                case 178:
                    result = 85;
                    break;
                case 179:
                    result = 84;
                    break;
                case 180:
                    result = 83;
                    break;
                case 181:
                    result = 82;
                    break;
                case 182:
                    result = 81;
                    break;
            }
            return result;
        }

        public void MakeWin(string player)
        {
            this.player.Pause();
            this.player2.Pause();
            this.Score.MakeWin(player);
        }

        public void ExtraTime()
        {
            this.Score.ExtraTime();
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
                    v = rdn.Next(0, 20); // If it is a regular brick, chances are lower
                }
                else if (tile.ID == 5)
                {
                    v = rdn.Next(0, 2); // If it is wood, chances are really high
                }
                else
                {
                    v = rdn.Next(0, 8); // If it is a special brick, chances are higher
                }                
                if (v == 1)
                {                    
                    // An item is hidden inside, yay!!
                    int style = (int)ItemTypes.Portal;// rdn.Next(0, (int)ItemTypes.Count);
                    Item itm = new Item(ItemTextures[style], tile.Position, player, player2, this, style);
                    items.Add(itm);
                }
            }
            /*
            This need for the empty tile is a problem related with the NPC, for now, removed...
            */
            tile.ID = 0;
            tile.Walkable = true;
            tile.BreakAble = true;
            //tiles.Remove(tile); // Remove is replaced by turning the tile into an empty tile
        }

        public void GenerateLevel(string LevelFile = "")
        {            
            Random rdn = new Random();
            int row = 0;
            int col = 0;
            int v = 0;
            bool walkable = false;
            int[,] arrtiles = new int[16, 12];
            int[] colsMap = new int[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            int[] rowsMap = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            int tntCounter = 0;

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
                                if (tntCounter > 0)
                                {
                                    v = 4;
                                    arrtiles[c, r] = v; // Save the random, for mirroring                                    
                                }
                                else
                                {
                                    v = 1; // and regular bricks has even more chances!
                                }                                
                            }
                        }
                        if (v == 0)
                        {                            
                            walkable = true;
                        }
                        if (v == 4)
                        {
                            if(tntCounter == 0)
                            {
                                tntCounter = 6;
                            }
                            else
                            {
                                tntCounter--;
                            }
                        }
                        if (v != 0)
                        {
                            Vector2 pos = new Vector2(col, row);
                            int tile_index = 0;
                            if (v == -100)
                            {
                                tile_index = (int)TileIndexes.p1flag;
                            }
                            else if (v == -200)
                            {
                                tile_index = (int)TileIndexes.p2flag;
                            }
                            else
                            {
                                tile_index = v - 1;
                            }
                            Tile t = new Tile(pos,TileTextures[tile_index], player, player2, (v != 2), walkable, this, v, bombmanager);
                            tiles.Add(t);
                        }
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
                            walkable = true;
                        }
                        if (v != 0)
                        {
                            Vector2 pos = new Vector2(col, row);
                            int tile_index = 0;
                            if (v == -100)
                            {
                                tile_index = (int)TileIndexes.p1flag;
                            }
                            else if (v == -200)
                            {
                                tile_index = (int)TileIndexes.p2flag;
                            }
                            else
                            {
                                tile_index = v - 1;
                            }
                            Tile t = new Tile(pos, TileTextures[tile_index], player, player2, (v != 2), walkable, this, v, bombmanager);
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
                                    Vector2 pos = new Vector2(col, row);
                                    int tile_index = 0;
                                    if (v == -100)
                                    {
                                        tile_index = (int)TileIndexes.p1flag;
                                    }
                                    else if (v == -200)
                                    {
                                        tile_index = (int)TileIndexes.p2flag;
                                    }
                                    else
                                    {
                                        tile_index = v - 1;
                                    }
                                    Tile t = new Tile(pos, TileTextures[tile_index], player, player2, (v != 2), walkable, this, v, bombmanager);
                                    tiles.Add(t);
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
                }catch(Exception){
                    // If an error ocurred loading from file, lets just leave what we can load
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < tiles.Count; index++)
            {
                if (tiles[index].ID != 0)
                {
                    tiles[index].Draw(spriteBatch);
                }
            }
            for (int index = 0; index < items.Count; index++)
            {
                items[index].Draw(spriteBatch);
            }
        }
    }
}
