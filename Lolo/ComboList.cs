using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lolo
{
    class ComboList
    {
        public string Caption; // The button caption
        SpriteFont Font;
        Vector2 Position;
        Color Color;
        private int ScreenWidth;
        private List<string> Value;
        public int Status;
        public int Val;
        private Keys previousMenuKey = Keys.None;

        public ComboList(string caption, int screenwidth, SpriteFont font, Color color, List<string> value)
        {
            this.Value = value;
            this.ScreenWidth = screenwidth;
            this.Font = font;
            this.Color = color;
            this.Caption = caption;
        }

        public string getValue()
        {
            return this.Value[Val];
        }

        public float getWidth()
        {
            return Font.MeasureString(this.Value[Val]).X;
        }

        public float getHeight()
        {
            return Font.MeasureString(this.Value[Val]).Y;
        }

        public string getCaption()
        {
            return this.Value[Status];
        }

        public void SetPosition(Vector2 position)
        {
            this.Position = position;
        }

        public void Update(GameTime gametime)
        {
            bool change = false;
            if (this.Status == 1)
            {
                #warning This must use the p1 configuration or fixed keyboard (like main menu)
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    if (previousMenuKey != Keys.Left)
                    {
                        change = true;
                        this.Val--;
                        previousMenuKey = Keys.Left;
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.Right))
                {
                    if (previousMenuKey != Keys.Right)
                    {
                        change = true;
                        this.Val++;
                        previousMenuKey = Keys.Right;
                    }
                }
                else
                {
                    previousMenuKey = Keys.None;
                }
            }
            if (change)
            {
                if (this.Val < 0)
                {
                    this.Val = this.Value.Count - 1;
                }
                if (this.Val > this.Value.Count-1)
                {
                    this.Val = 0;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string value = this.Value[Val];
            float centerX = General.getScreenCenterTextX(Caption + ": " + value, ScreenWidth, Font);
            Color col;
            if (Status == 0)
            {
                col = Color;
            }
            else
            {
                col = Color.Yellow;
            }            
            spriteBatch.DrawString(Font, Caption + ": " + value, new Vector2(centerX, Position.Y), col);
        }
    }
}
