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
    class Pause
    {
        int ScreenWidth;
        int ScreenHeight;
        public int Status = 0; // frame status
        private Vector2 Position;        
        private SpriteFont Font;
        private string Caption = "PAUSE";
        List<Button> btns = new List<Button>();
        private int currButton = -1;

        public Pause(int screenheight, int screenwidth, SpriteFont font, SpriteFont font2)
        {
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Font = font;
            float centerX = General.getScreenCenterTextX(Caption, screenwidth, font);
            float centerY = General.getScreenCenterTextY(Caption, screenheight, font);
            this.Position = new Vector2(centerX, centerY);
            Button btn = new Button("Continue", screenwidth, font2, Color.Yellow, Color.White, GameState.None);
            btns.Add(btn);
            btn = new Button("Quit", screenwidth, font2, Color.Yellow, Color.White, GameState.GotoMainMenu);
            btns.Add(btn);
            PositionButtons(centerY + 50);
            ButtonFocus(1);
        }

        public GameState GetRetState()
        {
            return btns[currButton].GetRetState();
        }

        public void PositionButtons(float initP)
        {
            float posY = initP;

            for (int index = 0; index < btns.Count; index++)
            {
                float centerX = General.getScreenCenterTextX(btns[index].getCaption(), ScreenWidth, Font);
                Vector2 pos = new Vector2(centerX, posY);
                btns[index].SetPosition(pos);
                posY += btns[index].getHeight() / 2;
            }
        }

        public void Reset()
        {
            currButton = btns.Count;
            ButtonFocus(1);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, Caption, Position,Color.White);
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }

        public void Update(GameTime gametime)
        {
            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Update(gametime);
            }
        }
    }
}
