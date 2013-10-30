﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    class CharacterSelection
    {
        private List<Texture2D> PlayerSelectionTextures = new List<Texture2D>();
        SpriteFont Font;
        Texture2D Texture;
        int State1 = 0;
        int State2 = 1;
        int ScreenWidth;
        int ScreenHeight;

        public CharacterSelection(Texture2D texture, SpriteFont font, List<Texture2D> playerselectiontextures, int screenheight, int screenwidth)
        {        
            this.Texture = texture;
            this.Font = font;
            this.PlayerSelectionTextures = playerselectiontextures;
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
        }

        public void Update(GameTime gametime)
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int width = Texture.Width;
            int height = Texture.Height;
            int x = 0, y = 0, line = 0;
            Rectangle source = new Rectangle(0, 0, width, height);
            Rectangle dest = new Rectangle(0, 0, width, height);
            spriteBatch.Draw(Texture, dest, source, Color.White);
            x = ScreenHeight / 3;
            y = ScreenWidth  / 4;

            spriteBatch.DrawString(Font, "Choose your hunter!", new Vector2(General.getScreenCenterTextX("Choose your hunter!", ScreenWidth, Font), 0), Color.White);

            for (int index = 0; index < PlayerSelectionTextures.Count; index++)
            {
                width = PlayerSelectionTextures[index].Width;
                height = PlayerSelectionTextures[index].Height;
                source = new Rectangle(0, 0, width, height);
                dest = new Rectangle(x, y, width, height);
                spriteBatch.Draw(PlayerSelectionTextures[index], dest, source, Color.White);
                x += PlayerSelectionTextures[index].Width + 60;
                line++;
                if (line == 3)
                {
                    x = ScreenHeight / 3;
                    y += PlayerSelectionTextures[index].Height + 60;
                }
            }
            spriteBatch.DrawString(Font, "P1", PosfromState(this.State1), Color.White);
            spriteBatch.DrawString(Font, "P2", PosfromState(this.State2), Color.White);
        }

        private Vector2 PosfromState(int state)
        {
            float x = 0, y = 0;
            switch (state)
            {
                case (int)PlayerTex.Knight:
                    x = 200;
                    y = 200;
                    break;
                case (int)PlayerTex.Girl:
                    x = 360;
                    y = 200;
                    break;
                case (int)PlayerTex.King:
                    x = 520;
                    y = 200;
                    break;
                case (int)PlayerTex.Man:
                    x = 200;
                    y = 360;
                    break;
                case (int)PlayerTex.Skelet:
                    x = 360;
                    y = 360;
                    break;
                case (int)PlayerTex.Sorce:
                    x = 520;
                    y = 360;
                    break;
            }
            return new Vector2(x, y);
        }
    }
}
