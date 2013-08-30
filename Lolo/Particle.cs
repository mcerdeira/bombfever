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
        public Rectangle hitBox;
        public Texture2D Texture { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public Color Color { get; set; }
        public float Size { get; set; }
        public int TTL { get; set; }

        public Particle(Map map, Player player, Player player2, Texture2D texture, Vector2 position, Vector2 velocity,
            float angle, float angularVelocity, Color color, float size, int ttl)
        {
            this.disabled = false;
            this.map = map;
            this.player = player;
            this.player2 = player2;            
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Angle = angle;
            AngularVelocity = angularVelocity;
            Color = color;
            Size = size;
            TTL = ttl;
        }

        public void Update()
        {
            hitBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
            TTL--;
            if(TTL <= 50)
            {
                disabled = true;
            }
            if (!disabled)
            {
                if (hitBox.Intersects(player.hitBox) && player.Status != "respawning" && player.inmunityCounter == 0)
                {
                    player.Status = "dead";
                }
                if (hitBox.Intersects(player2.hitBox) && player2.Status != "respawning" && player2.inmunityCounter == 0)
                {
                    player2.Status = "dead";
                }
            }

            for (int index = 0; index < map.tiles.Count; index++)
            {
                if (this.hitBox.Intersects(map.tiles[index].hitBox))
                {
                    if (!disabled && map.tiles[index].BreakAble)
                    {
                        map.tiles[index].Action = "dead";
                    }

                    disabled = true; // If the particle has bounced, its no longuer deathly (maybe it should dissapear too)

                    Random rnd = new Random();
                    int speedup = rnd.Next(1, 5);

                    Velocity.X *= speedup;
                    Velocity.Y *= speedup;

                    //Velocity.X *= -1;
                    Velocity.Y *= -1;

                    break;
                }
            }
            Position += Velocity;
            //Angle += AngularVelocity;
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
            spriteBatch.Draw(Texture, Position, sourceRectangle, Color, 0, origin, Size, SpriteEffects.None, 0f);           
        }
    }
}
