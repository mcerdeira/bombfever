using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    class Button
    {
        string Caption; // The button caption
        GameState RetState; // The returned game state

        public Button(string caption, GameState retstate)
        {
            this.RetState = retstate;
            this.Caption = caption;
        }

        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
