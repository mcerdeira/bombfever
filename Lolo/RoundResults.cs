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
    class RoundResults
    {
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;
        Texture2D BtnTexture;
        List<Button> btns = new List<Button>();
        private string Result;
        private int score1;
        private int score2;
        private SpriteFont Font;
        private int currButton = -1;
        private Score score;

        public RoundResults(Texture2D texture, Texture2D btnTexture, SpriteFont font, Score score, GameState prevGameState, int screenheight, int screenwidth)
        {
            this.Font = font;
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.BtnTexture = btnTexture;
            this.score = score;            
            Button btn;
            btn = new Button("Rematch", btnTexture, font, Color.White, prevGameState);
            btns.Add(btn);            
            btn = new Button("Quit", btnTexture, font, Color.White, GameState.MainMenu);
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
            this.Result = score.getResult(out this.score1, out this.score2);
            float centerX = (ScreenWidth / 2) - ((BtnTexture.Width / 2) / 2);
            float posY = Font.MeasureString(this.Result).Y + Font.MeasureString(this.score1.ToString()).Y + Font.MeasureString(this.score2.ToString()).Y / 2;
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

        private float setText(SpriteBatch spriteBatch, float startY, string text)
        {            
            float centerX= General.getScreenCenterTextX(text, ScreenWidth, Font);
            spriteBatch.DrawString(Font, text, new Vector2(centerX, startY), Color.White);
            startY += Font.MeasureString(text).Y / 2;
            return startY;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(Texture, dest, source, Color.White);
            float startY = 100;
            startY = setText(spriteBatch, startY, "P1: " + this.score1.ToString());
            startY = setText(spriteBatch, startY, "P2: " + this.score2.ToString());
            startY = setText(spriteBatch, startY, this.Result);

            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }
    }
}
