using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    public class Particle
    {
        private bool disabled;
        private Map map;
        private Player player;
        private Player player2;
        private int TTL_Total;
        public Rectangle hitBox;
        public Texture2D Texture { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }        
        public float Size { get; set; }
        public int TTL { get; set; }
        private Vector2 EmitterLocation;
        private bool miniExplosion = false;
        private bool Eternalfire = false;
        private bool CharExplosion = false;
        private bool PortalExplosion = false;

        public Particle(Map map, Player player, Player player2, Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, float size, int ttl, Vector2 emitterlocation, bool miniexplosion = false, 
            bool eternalfire = false, bool charexplosion = false, bool portalexplosion = false)
        {
            //this.ExplodeFX = explodefx;
            this.EmitterLocation = emitterlocation;
            this.disabled = portalexplosion ? true : false;
            this.map = map;
            this.player = player;
            this.player2 = player2;
            this.miniExplosion = miniexplosion;
            this.Eternalfire = eternalfire;
            this.CharExplosion = charexplosion;
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Angle = angle;
            this.AngularVelocity = angularVelocity;            
            this.Size = size;
            this.TTL = ttl;
            this.TTL_Total = ttl;
            this.PortalExplosion = portalexplosion;
        }

        public void Update()
        {
            TTL--;
            if (TTL <= (TTL_Total / 2) - 2)
            {
                disabled = true;
            }
            if (!disabled)
            {
                float distance = 0;
                hitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);                
                if (player.Status != "respawning" && player.inmunityCounter == 0 && hitBox.Intersects(player.hitBox))
                {
                    player.Status = "dead";
                }
                if (player2.Status != "respawning" && player2.inmunityCounter == 0 && hitBox.Intersects(player2.hitBox))
                {
                    player2.Status = "dead";
                }
                for (int index = 0; index < map.tiles.Count; index++)
                {
                    if (map.tiles[index].ID != 0 && map.tiles[index].ID != 6)
                    {
                        distance = Vector2.Distance(General.Rectangle2Vector(map.tiles[index].hitBox), this.EmitterLocation);
                        if (distance < 100 && this.hitBox.Intersects(map.tiles[index].hitBox))
                        {
                            if (!disabled && map.tiles[index].BreakAble)
                            {
                                if (map.tiles[index].inmunityCounter == 0)
                                {
                                    map.tiles[index].Action = "dead";
                                }
                            }
                            else
                            {
                                if (Math.Abs(distance) < 100)
                                {
                                    map.tiles[index].Shake();
                                }
                            }

                            disabled = true; // If the particle has bounced, its no longuer deathly (maybe it should dissapear too)

                            Random rnd = new Random();
                            int speedup = rnd.Next(1, 5);

                            Velocity.X *= speedup;
                            Velocity.Y *= speedup;

                            Velocity.Y *= -1;

                            if (!miniExplosion)
                            {
                                TTL = -1;
                            }
                            break;
                        }
                    }
                }
            }
            Position += Velocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Position.Y >= 250 && Position.Y <= 320)
            {
                // Warps
                if (Position.X < -21)
                {
                    Position.X = 796;
                }
                if (Position.X > 798)
                {
                    Position.X = -20;
                }
            }
            Rectangle sourceRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            if (disabled && !this.Eternalfire && !this.PortalExplosion)
            {
                spriteBatch.End();
                spriteBatch.Begin();
                spriteBatch.Draw(Texture, Position, sourceRectangle, Color.Gray, 0, origin, Size, SpriteEffects.None, 0f);
            }
            else
            {
                spriteBatch.End();
                if (this.Eternalfire)
                {
                    spriteBatch.Begin(0, BlendState.Additive, null, null, null);
                    spriteBatch.Draw(Texture, Position, sourceRectangle, Color.BlueViolet, 0, origin, Size, SpriteEffects.None, 0f);
                }
                else if (this.CharExplosion)
                {
                    spriteBatch.Begin(0, BlendState.Additive, null, null, null);
                    spriteBatch.Draw(Texture, Position, sourceRectangle, Color.GreenYellow, 0, origin, Size, SpriteEffects.None, 0f);
                }
                else if (this.PortalExplosion)
                {
                    spriteBatch.Begin(0, BlendState.Additive, null, null, null);
                    spriteBatch.Draw(Texture, Position, sourceRectangle, Color.Crimson, 0, origin, Size, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Begin(0, BlendState.Additive, null, null, null);
                    spriteBatch.Draw(Texture, Position, sourceRectangle, Color.OrangeRed, 0, origin, Size, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
