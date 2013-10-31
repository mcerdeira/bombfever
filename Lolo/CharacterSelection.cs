using System;
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
        public PlayerTex State1;
        public PlayerTex State2;
        bool Select1 = false;
        bool Select2 = false;
        int ScreenWidth;
        int ScreenHeight;
        int frameCounter = 0;

        public CharacterSelection(Texture2D texture, SpriteFont font, List<Texture2D> playerselectiontextures, int screenheight, int screenwidth)
        {        
            this.Texture = texture;
            this.Font = font;
            this.PlayerSelectionTextures = playerselectiontextures;
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            State1 = PlayerTex.Knight;
            State2 = PlayerTex.Girl;
        }

        public bool Update(GameTime gametime)
        {
            return (Select1 && Select2);
        }

        public void ButtonFocus(int direction, string player)
        {
            if (direction != 3)
            {
                if (player == "p1")
                {
                    if (!this.Select1)
                    {
                        this.State1 = CalcPosition(direction, this.State1, this.State2);
                    }
                }
                else
                {
                    if (!this.Select2)
                    {
                        this.State2 = CalcPosition(direction, this.State2, this.State1);
                    }
                }
            }
            else
            {
                if (player == "p1")
                {
                    this.Select1 = !this.Select1;
                }
                else
                {
                    this.Select2 = !this.Select2;
                }
            }
        }

        private PlayerTex CalcPosition(int direction, PlayerTex cur, PlayerTex forbid)
        {
            PlayerTex future = PlayerTex.PlaceHolder;
            switch (direction)
            {
                case 1: // Down
                case -1: // Up
                    switch(cur)
                    {
                        case PlayerTex.Knight:
                            future = PlayerTex.Man;
                            break;
                        case PlayerTex.Girl:
                            future = PlayerTex.Skelet;
                            break;
                        case PlayerTex.King:
                            future = PlayerTex.Sorce;
                            break;
                        case PlayerTex.Man:
                            future = PlayerTex.Knight;
                            break;
                        case PlayerTex.Skelet:
                            future = PlayerTex.Girl;
                            break;
                        case PlayerTex.Sorce:
                            future = PlayerTex.King;
                            break;
                    }
                    break;
                case 2: // Right
                    switch(cur)
                    {
                        case PlayerTex.Knight:
                            future = PlayerTex.Girl;
                            break;
                        case PlayerTex.Girl:
                            future = PlayerTex.King;
                            break;
                        case PlayerTex.King:
                            future = PlayerTex.Knight;
                            break;
                        case PlayerTex.Man:
                            future = PlayerTex.Skelet;
                            break;
                        case PlayerTex.Skelet:
                            future = PlayerTex.Sorce;
                            break;
                        case PlayerTex.Sorce:
                            future = PlayerTex.Man;
                            break;
                    }
                    break;
                case -2: // Left
                    switch(cur)
                    {
                        case PlayerTex.Knight:
                            future = PlayerTex.King;
                            break;
                        case PlayerTex.Girl:
                            future = PlayerTex.Knight;
                            break;
                        case PlayerTex.King:
                            future = PlayerTex.Girl;
                            break;
                        case PlayerTex.Man:
                            future = PlayerTex.Sorce;
                            break;
                        case PlayerTex.Skelet:
                            future = PlayerTex.Man;
                            break;
                        case PlayerTex.Sorce:
                            future = PlayerTex.Skelet;
                            break;
                    }
                    break;
            }
            if (future != forbid)
            {
                return future;
            }
            else
            {
                return cur;
            }
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
            string p1 = "";
            string p2 = "";
            if (frameCounter > 25)
            {
                frameCounter = 0;
            }
            else if (frameCounter > 20)
            {
                p1 = "";
                p2 = "";
            }
            else if (frameCounter >= 0)
            {
                p1 = "[P1]";
                p2 = "[P2]";
            }
            frameCounter++;
            if (this.Select1)
            {
                p1 = "[P1]";                
            }
            if (this.Select2)
            {                
                p2 = "[P2]";
            }
            spriteBatch.DrawString(Font, p1, PosfromState(this.State1), this.Select1 ? Color.Yellow : Color.White);
            spriteBatch.DrawString(Font, p2, PosfromState(this.State2), this.Select2 ? Color.Yellow : Color.White);
        }

        private Vector2 PosfromState(PlayerTex state)
        {
            float x = 0, y = 0;
            switch (state)
            {
                case PlayerTex.Knight:
                    x = 200;
                    y = 200;
                    break;
                case PlayerTex.Girl:
                    x = 360;
                    y = 200;
                    break;
                case PlayerTex.King:
                    x = 520;
                    y = 200;
                    break;
                case PlayerTex.Man:
                    x = 200;
                    y = 360;
                    break;
                case PlayerTex.Skelet:
                    x = 360;
                    y = 360;
                    break;
                case PlayerTex.Sorce:
                    x = 520;
                    y = 360;
                    break;
            }
            return new Vector2(x, y-50);
        }
    }
}
