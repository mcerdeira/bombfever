using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Lolo
{
    public class BombManager
    {
        public List<Bomb> bombs = new List<Bomb>();
        private List<BombExplosion> bombex = new List<BombExplosion>();
        private ContentManager content;
        private Map map;
        private Player Player;
        private Player Player2;        

        public BombManager(ContentManager content)
        {            
            this.content = content;
        }

        public void addExplossion(Vector2 position, int particles = 20)
        {
            BombExplosion ex = new BombExplosion(10, map, this, Player, Player2, content.Load<Texture2D>("particle"), position, particles);
            bombex.Add(ex);
        }

        public bool KickingBomb(Player player)
        {
            bool retVal = false;
            for (int index = 0; index < bombs.Count; index++)
            {
                if (player.hitBox.Intersects(bombs[index].hitBox))
                {
                    Vector2 v = General.IntersectDepthVector(player.hitBox, bombs[index].hitBox);
                    float absx = Math.Abs(v.X);
                    float absy = Math.Abs(v.Y);
                    string direction = "";
                    if (!(v.X == 0 && v.Y == 0))
                    {
                        if (absx > absy) // the shallower impact is the correct one- this is on the y axis
                        {
                            if (v.Y < 0)
                            {
                                direction = "top";
                            }
                            else
                            {
                                direction = "bottom";
                            }
                        }
                        else // the x axis!
                        {
                            if (v.X < 0)
                            {
                                direction = "left";
                            }
                            else
                            {
                                direction = "right";
                            }
                        }
                    }
                    bombs[index].Kicked(direction);
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }

        public Vector2 getNearestBomb(Vector2 pos)
        {            
            Vector2 min = new Vector2(9999, 9999);
            float minDist = 999999;
            float dist = 0;
            for (int index = 0; index < bombs.Count; index++)
            {
                dist = Vector2.Distance(pos, bombs[index].Position);
                if (dist < minDist)
                {
                    minDist = dist;
                    min = bombs[index].Position;                    
                }
            }
            for (int index = 0; index < bombex.Count; index++)
            {
                dist = Vector2.Distance(pos, bombex[index].EmitterLocation);
                if (dist < minDist)
                {
                    minDist = dist;
                    min = bombex[index].EmitterLocation;
                }
            }
            return min;
        }

        public void UpdateMap(Map map, Player player, Player player2)
        {
            this.Player = player;
            this.Player2 = player2;
            this.map = map;
        }

        public void Update()
        {
            for (int index = 0; index < bombs.Count; index++)
            {
                bombs[index].Update();
            }
            for (int i = 0; i < bombex.Count; i++)
            {
                bombex[i].Update();
            }
        }

        public void RemoveBombExplosion(BombExplosion be)
        {
            bombex.Remove(be);
            be = null;
        }

        public void RemoveBomb(Bomb bomb, Vector2 position)
        {
            bombs.Remove(bomb);
            bomb = null;
            BombExplosion ex = new BombExplosion(60, map, this, Player, Player2, content.Load<Texture2D>("particle"), position);
            bombex.Add(ex);
        }

        public void SpawnBomb(Vector2 position, string owner)
        {
            Bomb b = new Bomb(position, owner, this, content, Player, Player2);
            bombs.Add(b);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < bombs.Count; index++)
            {
                bombs[index].Draw(spriteBatch);
            }
            for (int i = 0; i < bombex.Count; i++)
            {
                bombex[i].Draw(spriteBatch);
            }
        }
    }
}
