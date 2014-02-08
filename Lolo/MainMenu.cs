using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

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
        //private string GameTitle;
        private SoundEffect fxSelect;
        private SoundEffect fxSelected;

        public MainMenu(Texture2D texture, SpriteFont font, SpriteFont titlefont, int screenheight, int screenwidth, string gametitle, SoundEffect fxselect, SoundEffect fxselected)
        {            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.TFont = titlefont;
            this.Font = font;
            //this.GameTitle = gametitle;
            this.fxSelect = fxselect;
            this.fxSelected = fxselected;
            Button btn = new Button("2P Versus", screenwidth, font, Color.Yellow, Color.White, GameState.Start2P, false);
            btns.Add(btn);
            btn = new Button("1P Survival", screenwidth, font, Color.DarkGray, Color.Gray, GameState.MainMenu, false, false); // GameState.Start1P
            btns.Add(btn);
            btn = new Button("Co-op Survival", screenwidth, font, Color.Yellow, Color.White, GameState.StartCoOp, false);
            btns.Add(btn);
            btn = new Button("Options", screenwidth, font, Color.Yellow, Color.White, GameState.GotoOptions, false);
            btns.Add(btn);
            btn = new Button("Load level", screenwidth, font, Color.Yellow, Color.White, GameState.LoadFromFile, false);
            btns.Add(btn);
            btn = new Button("Credits", screenwidth, font, Color.Yellow, Color.White, GameState.Credits, false);
            btns.Add(btn);
            btn = new Button("Quit", screenwidth, font, Color.Yellow, Color.White, GameState.Quit, false);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1, true);
        }

        public GameState GetRetState()
        {
            fxSelected.Play();
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

        public void ButtonFocus(int direction, bool initial_focus = false)
        {
            calcCurrentButton(direction);

            while (!btns[currButton].Enabled)
            {
                calcCurrentButton(direction);                
            }

            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Status = 0;
                btns[index].SetXPosition(10);
            }
            btns[currButton].Status = 1;
            btns[currButton].SetXPosition(20);
            if(!initial_focus)
            {
                fxSelect.Play();
            }
        }

        public void PositionButtons()
        {
            float posY = 320;
            for (int index = 0; index < btns.Count; index++)
            {
                float centerX = General.getScreenCenterTextX(btns[index].getCaption(), ScreenWidth, Font);
                Vector2 pos = new Vector2(centerX, posY);
                btns[index].SetPosition(pos);
                posY += btns[index].getHeight() - (btns[index].getHeight() / 3);
            }
        }

        public void Update(GameTime gametime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            //float centerX = General.getScreenCenterTextX(GameTitle, ScreenWidth, TFont);
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, ScreenWidth, ScreenHeight);            
            spriteBatch.Draw(Texture, dest, source, Color.White);
            //spriteBatch.DrawString(TFont, GameTitle, new Vector2(centerX, 0), Color.Yellow);
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }
    }
}
