using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    class MainMenu
    {
        Texture2D Texture;
        List<Button> btns = new List<Button>();

        public MainMenu(Texture2D texture)
        {
            this.Texture = texture;
            Button btn = new Button("Start", GameState.Playing);
            btns.Add(btn);
            btn = new Button("Options", GameState.Options);
            btns.Add(btn);
            btn = new Button("Quit", GameState.Quit);
            btns.Add(btn);
        }

        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
        }
    }
}
