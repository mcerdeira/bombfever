using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace Lolo
{
    class LevelLoader
    {
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;
        Texture2D BtnTexture;
        List<Button> btns = new List<Button>();
        private int currButton = -1;

        public LevelLoader(Texture2D texture, Texture2D btnTexture, SpriteFont font, int screenheight, int screenwidth)
        {
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.BtnTexture = btnTexture;
            Button btn;

            string[] filePaths = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.lvl");
            foreach (string f in filePaths)
            {                
                btn = new Button(Path.GetFileName(f), btnTexture, font, Color.White, GameState.MainMenu);
                btns.Add(btn);
            }
            btn = new Button("Cancel", btnTexture, font, Color.White, GameState.MainMenu);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1);
        }

        public GameState GetRetState()
        {
            return btns[currButton].GetRetState();
        }

        public string getCaption()
        {
            return btns[currButton].getCaption();
        }

        public void ButtonFocus(int direction)
        {
            currButton += direction;
            if (currButton > btns.Count() - 1)
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
            float posY = 100;

            for (int index = 0; index < btns.Count; index++)
            {
                Vector2 pos = new Vector2(centerX, posY + BtnTexture.Height);
                btns[index].SetPosition(pos);
                posY += btns[index].getHeight() / 2;
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
