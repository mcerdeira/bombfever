using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Audio;
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
        private SoundEffect sndItemPick;
        private bool Survival;

        public Map(Player player, Player player2, BombManager bombmanager, List<Texture2D> itemtextures, List<Texture2D> tiletextures, Score score, SoundEffect snditempick, bool survival)
        {            
            this.Score = score;
            this.ItemTextures = itemtextures;
            this.TileTextures = tiletextures;
            this.bombmanager = bombmanager;
            this.player = player;
            this.player2 = player2;
            this.sndItemPick = snditempick;
            this.Survival = survival;
        }
     
        public void MakeWin(string player)
        {
            this.player.Pause(true);
            this.player2.Pause(true);
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
                    v = rdn.Next(0, 15); // If it is a regular brick, chances are lower
                }
                else if (tile.ID == 5)
                {
                    v = 1; // If it is wood brick, allways an item
                }
                else
                {
                    v = rdn.Next(0, 8); // If it is a special brick, chances are higher
                }

                if (v == 1)
                {                    
                    // An item is hidden inside, yay!!
                    int style = rdn.Next(0, (int)ItemTypes.Count);
                    Item itm = new Item(ItemTextures[style], tile.Position, player, player2, this, style, this.sndItemPick);
                    items.Add(itm);
                }
            }
            /*
            This need for the empty tile is a problem related with the NPC, for now, removed...
            tile.ID = 0;
            tile.Walkable = true;
            tile.BreakAble = true;
            */
            tiles.Remove(tile); // Remove is replaced by turning the tile into an empty tile
        }

        public void GenerateLevel(string LevelFile)
        {            
            Random rdn = new Random();
            int row = 0;
            int col = 0;
            int v = 0;
            bool walkable = false;
            int[,] arrtiles = new int[16, 12];
            Tile[,] arrportals = new Tile[16, 12];
            int[] colsMap = new int[] { 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            int[] rowsMap = new int[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
            int tntCounter = 0;

            //TODO: If it is survival, then it must position enemies and the exit door. Also central tomb tiles must not appear

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
                            // Now lets do some random... 
                            v = rdn.Next(-20, 20);
                            if (v == 6)
                            {
                                // Portals tiles requires that Rows must be uneven and Cols must be even
                                if (r % 2 == 0 || c % 2 != 0)
                                {
                                    v = rdn.Next(1, 6);
                                }
                            }
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
                        if (v < 0 || v == 7 || v == 8 || v == 9)
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
                        if (v == 0 || v == 6)
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
                            Tile t = new Tile(pos, TileTextures[tile_index], player, player2, (v != 2), walkable, this, v, bombmanager);
                            tiles.Add(t);
                            if (v == 6)
                            {
                                arrportals[c, r] = t;
                            }
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
                        if (v < 0 || v == 7 || v == 8 || v == 9)
                        {
                            if (v != -200 && v != -100)
                            {
                                v = 1; // and regular bricks has even more chances!
                            }
                        }
                        if (v == 0 || v == 6)
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
                            if (v == 6)
                            {
                                t.setPartner(arrportals[colsMap[c], rowsMap[r]]);
                                arrportals[colsMap[c], rowsMap[r]].setPartner(t);
                            }
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
                                    if (v == 0 || v == 6)
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
