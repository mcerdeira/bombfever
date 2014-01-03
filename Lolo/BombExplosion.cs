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
        private int totalParticles;
        private bool miniExplosion = false;
        private bool Eternalfire = false;
        private bool CharExplosion = false;
        private bool PortalExplosion = false;
        private string Owner = "";
        private List<Tile> subSetMap;

        public BombExplosion(int TTL, Map map, BombManager bombman, Player player, Player player2, Texture2D texture, Vector2 location, string owner, int totalParticles = 20, bool miniexplosion = false, bool eternalfire = false, bool charExplosion = false, bool portalexplosion = false)
        {
            this.miniExplosion = miniexplosion;
            this.totalParticles = totalParticles;
            this.map = map;
            this.bombman = bombman;
            this.player = player;
            this.player2 = player2;
            this.EmitterLocation = location;
            this.TTL = TTL;
            this.texture = texture;
            this.particles = new List<Particle>();
            this.random = new Random();
            this.Eternalfire = eternalfire;
            this.CharExplosion = charExplosion;
            this.Owner = owner;
            if (this.Eternalfire)
            {
                this.totalParticles = 40;
            }
            this.PortalExplosion = portalexplosion;

            this.subSetMap = map.tiles.Where(x => (Vector2.Distance(x.Position, location) < 80 && x.ID != 0 && x.ID != 6)).ToList();
        }

        public void Update()
        {
            // This two if are fir freezing bombs when player is paused
            if (this.Owner == "p1")
            {
                int pl = this.player.PausedLoop;
                if (pl != 0)
                {
                    if(pl <= 350)
                    {
                        return;
                    }
                    else
                    {
                        if (pl % 2 == 0)
                        {
                            return;
                        }
                    }                    
                }
            }
            if (this.Owner == "p2")
            {
                int pl = this.player2.PausedLoop;
                if (pl != 0)
                {
                    if (pl <= 350)
                    {
                        return;
                    }
                    else
                    {
                        if (pl % 2 == 0)
                        {
                            return;
                        }
                    }
                }
            }


            if (TTL > 0)
            {
                for (int i = 0; i < totalParticles; i++)
                {
                    particles.Add(GenerateNewParticle());
                }
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
        }

        private Particle GenerateNewParticle()
        {
            Vector2 position = EmitterLocation;
            Vector2 velocity = new Vector2(
                                    1f * (float)(random.NextDouble() * 2 - 1),
                                    1f * (float)(random.NextDouble() * 2 - 1));

            float angle = 0;
            float angularVelocity = 0.1f * (float)(random.NextDouble() * 2 - 1);            
            float size = (float)random.NextDouble();
            int ttl = totalParticles + random.Next(100);

            return new Particle(map, player, player2, texture, position, velocity, angle, angularVelocity, size, ttl, EmitterLocation, this.miniExplosion, this.Eternalfire, this.CharExplosion, this.PortalExplosion, this.subSetMap);
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
