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
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;
        Texture2D BtnTexture;
        List<Button> btns = new List<Button>();
        private int currButton = -1;

        public MainMenu(Texture2D texture, Texture2D btnTexture, SpriteFont font, int screenheight, int screenwidth)
        {            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.BtnTexture = btnTexture;
            Button btn = new Button("Start", btnTexture, font, Color.White, GameState.Start);
            btns.Add(btn);
            btn = new Button("Options", btnTexture, font, Color.White, GameState.Options);
            btns.Add(btn);
            btn = new Button("Quit",btnTexture, font, Color.White, GameState.Quit);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1);
        }

        public GameState GetRetState()
        {
            return btns[currButton].GetRetState();
        }

        public void ButtonFocus(int direction)
        {
            currButton += direction;
            if (currButton > btns.Count()-1)
            {
                currButton = 0;
            }
            if (currButton < 0)
            {
                currButton = btns.Count() - 1;
            }
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Status = 0;
            }
            btns[currButton].Status = 1;
        }

        public void PositionButtons()
        {
            float centerX = (ScreenWidth / 2) - ((BtnTexture.Width / 2) / 2);
            float posY = 200;

            for (int index = 0; index < btns.Count; index++)
            {
                Vector2 pos = new Vector2(centerX, posY + BtnTexture.Height);
                btns[index].SetPosition(pos);
                posY += BtnTexture.Height;
            }
        }

        public void Update(GameTime gametime)
        {
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Update(gametime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);            
            spriteBatch.Draw(Texture, dest, source, Color.White);
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }
    }
}
