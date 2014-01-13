using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lolo
{
    class EnemyManager
    {
        private List<Texture2D> texturesList;
        private List<Enemy> enList;

        public EnemyManager(List<Texture2D> tlist)
        {
            this.texturesList = tlist;
        }

        public void CreateEnemy(Vector2 location)
        {
            Random rnd = new Random();
            int i = rnd.Next((int)EnemyTypes.Devil1, (int)EnemyTypes.Spider);
            Enemy en = new Enemy(texturesList[i], i, location);
            enList.Add(en);
        }

        public void KillEnemy(Enemy enemy)
        {
            enemy.Die();
            enList.Remove(enemy);
        }

        public void Update()
        {
            for (int index = 0; index < enList.Count; index++)
            {
                enList[index].Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int index = 0; index < enList.Count; index++)
            {
                enList[index].Draw(spriteBatch);
            }
        }
    }
}
