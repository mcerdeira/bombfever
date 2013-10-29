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
        List<Button> btns = new List<Button>();
        Texture2D Texture;
        private string Result;
        private int score1;
        private int score2;
        private SpriteFont Font;
        private SpriteFont ChartFont;
        private int currButton = -1;
        private Score score;
        private Match Match;
        private Texture2D charts;

        public RoundResults(Texture2D texture, SpriteFont font, SpriteFont chartfont, Score score, GameState prevGameState, Match match, int screenheight, int screenwidth, Texture2D charts)
        {
            this.ChartFont = chartfont;
            this.charts = charts;
            this.Match = match;
            this.Font = font;
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.score = score;
            this.Texture = texture;
            Button btn;
            btn = new Button("Rematch", screenwidth, font, Color.Yellow, Color.White, prevGameState);
            btns.Add(btn);
            btn = new Button("Quit", screenwidth, font, Color.Yellow, Color.White, GameState.GotoMainMenu);
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
            this.Result = score.getResult(out this.score1, out this.score2, Match);
            float posY = ScreenWidth / 2;
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

        private float setText(SpriteBatch spriteBatch, SpriteFont Font, float startY, string text)
        {
            if (text != "@EMPTY")
            {
                float centerX = General.getScreenCenterTextX(text, ScreenWidth, Font);
                spriteBatch.DrawString(Font, text, new Vector2(centerX, startY), Color.White);
            }
            startY += Font.MeasureString(text).Y / 2;
            return startY;
        }

        private void DrawCharts(float Y, SpriteBatch spriteBatch)
        {
            // Chart
            int totalp1 = this.Match.p1Score();
            int totalp2 = this.Match.p2Score();
            int totaldw = this.Match.Draws();
            int total = totalp1 + totalp2 + totaldw;            
            int perp1 = totalp1 * 100 / total;
            int perp2 = totalp2 * 100 / total;
            int perpd = totaldw * 100 / total;

            Rectangle p1 = new Rectangle(ScreenWidth / 2- (60 / 2)  - 100, (int)Y - perp1, 60, perp1);
            Rectangle p2 = new Rectangle(ScreenWidth / 2 - (60 / 2) , (int)Y - perp2, 60, perp2);
            Rectangle dw = new Rectangle(ScreenWidth / 2 - (60 / 2) + 100, (int)Y - perpd, 60, perpd);            

            spriteBatch.Draw(charts, p1, Color.Crimson);
            spriteBatch.Draw(charts, p2, Color.BlueViolet);
            spriteBatch.Draw(charts, dw, Color.Teal);

            spriteBatch.DrawString(ChartFont, perp1.ToString() + "%", new Vector2(p1.X + ((p1.Width / 2) - (ChartFont.MeasureString(perp1.ToString() + "%").X / 2)), p1.Y - ChartFont.MeasureString(perp1.ToString() + "%").Y), Color.White);
            spriteBatch.DrawString(ChartFont, perp2.ToString() + "%", new Vector2(p2.X + ((p2.Width / 2) - (ChartFont.MeasureString(perp2.ToString() + "%").X / 2)), p2.Y - ChartFont.MeasureString(perp2.ToString() + "%").Y), Color.White);
            spriteBatch.DrawString(ChartFont, perpd.ToString() + "%", new Vector2(dw.X + ((dw.Width / 2) - (ChartFont.MeasureString(perpd.ToString() + "%").X / 2)), dw.Y - ChartFont.MeasureString(perpd.ToString() + "%").Y), Color.White);

            spriteBatch.DrawString(ChartFont, "P1", new Vector2(p1.X + ((p1.Width / 2) - (ChartFont.MeasureString("P1").X / 2)), p1.Y + p1.Height ), Color.White);
            spriteBatch.DrawString(ChartFont, "P2", new Vector2(p2.X + ((p2.Width / 2) - (ChartFont.MeasureString("P2").X / 2)), p2.Y + p2.Height), Color.White);
            spriteBatch.DrawString(ChartFont, "TIED", new Vector2(dw.X + ((dw.Width / 2) - (ChartFont.MeasureString("TIED").X / 2)), dw.Y + dw.Height), Color.White);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(Texture, dest, source, Color.White);
            float startY = 0;
            startY = setText(spriteBatch, Font, startY, "P1: " + this.score1.ToString());
            startY = setText(spriteBatch, Font, startY, "P2: " + this.score2.ToString());
            startY = setText(spriteBatch, Font, startY, this.Result);

            startY = setText(spriteBatch, Font, startY, "@EMPTY");            
            startY = setText(spriteBatch, ChartFont, startY, "Stats");
            startY = setText(spriteBatch, Font, startY, "@EMPTY");

            DrawCharts(startY+120, spriteBatch);

            for (int index = 0; index < btns.Count; index++)
            {
                btns[index].Draw(spriteBatch);
            }
        }
    }
}
