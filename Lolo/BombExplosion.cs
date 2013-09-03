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
    public class BombExplosion
    {
        private Random random;
        public Vector2 EmitterLocation { get; set; }
        private List<Particle> particles;
        private Texture2D texture;
        private int TTL;
        public bool killme = false;        
        private Player player;
        private Player player2;
        private BombManager bombman;
        private Map map;

        public BombExplosion(int TTL, Map map, BombManager bombman, Player player, Player player2, Texture2D texture, Vector2 location)
        {
            this.map = map;
            this.bombman = bombman;
            this.player = player;
            this.player2 = player2;
            this.EmitterLocation = location;
            this.TTL = TTL;
            this.texture = texture;
            this.particles = new List<Particle>();
            this.random = new Random();
        }

        public void Update()
        {
            int total = 20;
            if (TTL > 0)
            {
                //if (!tmp)
                //{
                for (int i = 0; i < total; i++)
                {
                    particles.Add(GenerateNewParticle());
                }
                //}
                TTL--;
            }

            for (int particle = 0; particle < particles.Count; particle++)
            {
                particles[particle].Update();
                if (particles[particle].TTL <= 0)
                {
                    particles.RemoveAt(particle);
                    particle--;
                }                
            }
            if (particles.Count == 0)
            {
                bombman.RemoveBombExplosion(this);
                killme = true;
            }
            //if (!tmp)
            //{
            //    tmp = true;
            //}
        }

        private Particle GenerateNewParticle()
        {
            List<Color> cololist = new List<Color>();

            cololist.Add(Color.Yellow);
            cololist.Add(Color.Red);
            cololist.Add(Color.White);
            cololist.Add(Color.OrangeRed);
            cololist.Add(Color.LightYellow);
            cololist.Add(Color.DarkRed);
            cololist.Add(Color.Black);
            cololist.Add(Color.Crimson);


            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));

            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);
            Color color = cololist[random.Next(cololist.Count)];
            float size = (float)random.NextDouble();
            int ttl = 20 + random.Next(100);

            return new Particle(map, player, player2, texture, position, velocity, angle, angularVelocity, color, size, ttl);
        }

        public void Draw(SpriteBatch spriteBatch)
        {            
            for (int index = 0; index < particles.Count; index++)
            {
                particles[index].Draw(spriteBatch);
            }            
        }    
    }
}
