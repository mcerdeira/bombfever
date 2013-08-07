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
    public class BombManager
    {
        private List<Bomb> bombs = new List<Bomb>();
        private List<BombExplosion> bombex = new List<BombExplosion>();
        private ContentManager content;
        private Map map;
        private Player Player;
        private Player Player2;

        public BombManager(ContentManager content) // Add enemy
        {
            this.content = content;
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
