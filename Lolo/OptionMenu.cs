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
    public class OptionMenu
    {
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;
        List<Object> btns = new List<Object>();
        private GameOptions gameOpt;
        private int currButton = -1;
        private SpriteFont Font;

        public OptionMenu(Texture2D texture, SpriteFont font, int screenheight, int screenwidth)
        {            
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.Font = font;
            ComboList cboP1Ctrl = new ComboList("P1 Control", screenwidth, font, Color.White, General.getControlTypes());
            btns.Add(cboP1Ctrl);
            ComboList cboP2Ctrl = new ComboList("P2 Control", screenwidth, font, Color.White, General.getControlTypes());
            btns.Add(cboP2Ctrl);
            ComboList cboTime = new ComboList("Time Limit", screenwidth, font, Color.White, General.getRoundTimes());
            btns.Add(cboTime);
            ComboList cbotype = new ComboList("Game type", screenwidth, font, Color.White, General.getGameTypes());
            btns.Add(cbotype);
            ComboList cbomusic = new ComboList("Music", screenwidth, font, Color.White, General.getOnOff());
            btns.Add(cbomusic);
            ComboList cboFx = new ComboList("FX", screenwidth, font, Color.White, General.getOnOff());
            btns.Add(cboFx);

            #warning Add checkboxes with item availability
            Button btn = new Button("Acept", screenwidth, font, Color.Yellow, Color.White, GameState.GotoMainMenu);
            btns.Add(btn);
            btn = new Button("Cancel", screenwidth, font, Color.Yellow, Color.White, GameState.GotoMainMenu);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1);            
        }

        private void saveOptions()
        {            
            this.gameOpt.p1control = ((ComboList)btns[0]).Val;
            this.gameOpt.p2control = ((ComboList)btns[1]).Val;
            this.gameOpt.timelimit = ((ComboList)btns[2]).Val;
            this.gameOpt.gametype = ((ComboList)btns[3]).Val;
            this.gameOpt.music = ((ComboList)btns[4]).Val;
            this.gameOpt.fx = ((ComboList)btns[5]).Val;

            // Write to XML
            XmlSerializer writer = new XmlSerializer(typeof(GameOptions));
            using (FileStream file = File.OpenWrite("gamedata.xml"))
            {
                writer.Serialize(file, this.gameOpt);
            }
        }

        public void Reset()
        {
            currButton = btns.Count;
            ButtonFocus(1);
        }

        public GameOptions loadOptions()
        {
            try
            {
                // Read from XML
                XmlSerializer reader = new XmlSerializer(typeof(GameOptions));
                using (FileStream input = File.OpenRead("gamedata.xml"))
                {
                    this.gameOpt = reader.Deserialize(input) as GameOptions;
                }
            }
            catch(FileNotFoundException)
            {
                this.gameOpt.p1control = 0;
                this.gameOpt.p2control = 0;
                this.gameOpt.timelimit = 0;
                this.gameOpt.gametype = 0;
                this.gameOpt.music = 0;
                this.gameOpt.fx = 0;
            }
            ((ComboList)btns[0]).Val = this.gameOpt.p1control;
            ((ComboList)btns[1]).Val = this.gameOpt.p2control;
            ((ComboList)btns[2]).Val = this.gameOpt.timelimit;
            ((ComboList)btns[3]).Val = this.gameOpt.gametype;
            ((ComboList)btns[4]).Val = this.gameOpt.music;
            ((ComboList)btns[5]).Val = this.gameOpt.fx;

            return this.gameOpt;
        }
        
        public GameState GetRetState()
        {
            if (btns[currButton].GetType().Name == "Button")
            {
                if (((Button)btns[currButton]).Caption == "Acept")
                {
                    saveOptions();
                }
                return ((Button)btns[currButton]).GetRetState();
            }
            else if (btns[currButton].GetType().Name == "CheckBox")
            {
                return ((CheckBox)btns[currButton]).GetRetState();             
            }
            else if (btns[currButton].GetType().Name == "ComboList")
            {
                return GameState.None;
            }

            return GameState.None;
        }

        public void CheckBoxClicked()
        {
            if (btns[currButton].GetType().Name == "CheckBox")
            {
                ((CheckBox)btns[currButton]).clicked();
            }
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
                if (btns[index].GetType().Name == "Button")
                {
                    ((Button)btns[index]).Status = 0;
                }
                else if (btns[index].GetType().Name == "CheckBox")
                {
                    ((CheckBox)btns[index]).Status = 0;
                }
                else if (btns[index].GetType().Name == "ComboList")
                {
                    ((ComboList)btns[index]).Status = 0;
                }
            }
            if (btns[currButton].GetType().Name == "Button")
            {
                ((Button)btns[currButton]).Status = 1;
            }
            else if (btns[currButton].GetType().Name == "CheckBox")
            {
                ((CheckBox)btns[currButton]).Status = 1;
            }
            else if (btns[currButton].GetType().Name == "ComboList")
            {
                ((ComboList)btns[currButton]).Status = 1;
            }
        }

        public void PositionButtons()
        {
            float posY = 0;
            float centerX = 0;

            for (int index = 0; index < btns.Count; index++)
            {
                Vector2 pos = new Vector2(centerX, posY);
                if (btns[index].GetType().Name == "Button")
                {
                    ((Button)btns[index]).SetPosition(pos);
                    posY += ((Button)btns[index]).getHeight();
                    centerX += ((Button)btns[index]).getWidth();
                }
                else if (btns[index].GetType().Name == "CheckBox")
                {
                    ((CheckBox)btns[index]).SetPosition(pos);
                    posY += ((CheckBox)btns[index]).getHeight();
                    centerX += ((CheckBox)btns[index]).getWidth();
                }
                else if (btns[index].GetType().Name == "ComboList")
                {
                    ((ComboList)btns[index]).SetPosition(pos);
                    posY += ((ComboList)btns[index]).getHeight();
                    centerX += ((ComboList)btns[index]).getWidth();
                }
            }

            posY = 0;            
            for (int index = 0; index < btns.Count; index++)
            {
                if (btns[index].GetType().Name == "Button")
                {
                    centerX = General.getScreenCenterTextX(((Button)btns[index]).getCaption(), ScreenWidth, Font);
                    Vector2 pos = new Vector2(centerX, posY);
                    ((Button)btns[index]).SetPosition(pos);
                    posY += ((Button)btns[index]).getHeight() / 2;
                }
                else if (btns[index].GetType().Name == "CheckBox")
                {
                    centerX = General.getScreenCenterTextX(((CheckBox)btns[index]).getCaption(), ScreenWidth, Font);
                    Vector2 pos = new Vector2(centerX, posY);
                    ((CheckBox)btns[index]).SetPosition(pos);
                    posY += ((CheckBox)btns[index]).getHeight() / 2;
                }
                else if (btns[index].GetType().Name == "ComboList")
                {
                    centerX = General.getScreenCenterTextX(((ComboList)btns[index]).getCaption(), ScreenWidth, Font);
                    Vector2 pos = new Vector2(centerX, posY);
                    ((ComboList)btns[index]).SetPosition(pos);
                    posY += ((ComboList)btns[index]).getHeight() / 2;
                }
            }
        }

        public void Update(GameTime gametime)
        {
            for (int index = 0; index < btns.Count; index++)
            {
                if (btns[index].GetType().Name == "Button")
                {
                    ((Button)btns[index]).Update(gametime);
                }
                else if (btns[index].GetType().Name == "CheckBox")
                {
                    ((CheckBox)btns[index]).Update(gametime);
                }
                else if (btns[index].GetType().Name == "ComboList")
                {
                    ((ComboList)btns[index]).Update(gametime);
                }
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
                if (btns[index].GetType().Name == "Button")
                {
                    ((Button)btns[index]).Draw(spriteBatch);
                }
                else if (btns[index].GetType().Name == "CheckBox")
                {
                    ((CheckBox)btns[index]).Draw(spriteBatch);
                }
                else if (btns[index].GetType().Name == "ComboList")
                {
                    ((ComboList)btns[index]).Draw(spriteBatch);
                }
            }
        }
    }
}
