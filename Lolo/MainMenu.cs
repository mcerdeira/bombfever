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
        List<Button> btns = new List<Button>();
        private int currButton = -1;
        private SpriteFont Font;
        private SpriteFont TFont;
        private string GameTitle;

        public MainMenu(Texture2D texture, SpriteFont font, SpriteFont titlefont, int screenheight, int screenwidth, string gametitle)
        {            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.TFont = titlefont;
            this.Font = font;
            this.GameTitle = gametitle;
            Button btn = new Button("2P Versus", screenwidth, font, Color.Yellow, Color.White, GameState.Start2P);            
            btns.Add(btn);
            btn = new Button("1P Survival", screenwidth, font, Color.DarkGray, Color.Gray, GameState.MainMenu, false); // GameState.Start1P
            btns.Add(btn);
            btn = new Button("Co-op Survival", screenwidth, font, Color.Yellow, Color.White, GameState.StartCoOp);
            btns.Add(btn);
            btn = new Button("Options", screenwidth, font, Color.Yellow, Color.White, GameState.GotoOptions);
            btns.Add(btn);
            btn = new Button("Load level", screenwidth, font, Color.Yellow, Color.White, GameState.LoadFromFile);
            btns.Add(btn);
            btn = new Button("Credits", screenwidth, font, Color.Yellow, Color.White, GameState.Credits);
            btns.Add(btn);
            btn = new Button("Quit", screenwidth, font, Color.Yellow, Color.White, GameState.Quit);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1);
        }

        public GameState GetRetState()
        {
            return btns[currButton].GetRetState();
        }

        private void calcCurrentButton(int direction)
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
        }

        public void ButtonFocus(int direction)
        {
            calcCurrentButton(direction);

            while (!btns[currButton].Enabled)
            {
                calcCurrentButton(direction);                
            }

            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Status = 0;
            }
            btns[currButton].Status = 1;
        }

        public void PositionButtons()
        {
            float posY = 250;
            for (int index = 0; index < btns.Count; index++)
            {
                float centerX = General.getScreenCenterTextX(btns[index].getCaption(), ScreenWidth, Font);
                Vector2 pos = new Vector2(centerX, posY);
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
            float centerX = General.getScreenCenterTextX(GameTitle, ScreenWidth, TFont);
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);            
            spriteBatch.Draw(Texture, dest, source, Color.White);
            spriteBatch.DrawString(TFont, GameTitle, new Vector2(centerX, 0), Color.Yellow);
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }
    }
}
