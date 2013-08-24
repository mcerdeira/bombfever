using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    [Serializable()]
    public class Data
    {
        public string timelimit { get; set; }
        public string gametype { get; set; }
    }

    public class OptionMenu
    {
        int ScreenWidth;
        int ScreenHeight;
        Texture2D Texture;
        List<Object> btns = new List<Object>();
        private int currButton = -1;
        private SpriteFont Font;

        public OptionMenu(Texture2D texture, SpriteFont font, int screenheight, int screenwidth)
        {
            this.ScreenHeight = screenheight;
            this.ScreenWidth = screenwidth;
            this.Texture = texture;
            this.Font = font;            
            CheckBox chk = new CheckBox("Test", screenwidth, font, Color.White);
            btns.Add(chk);
            ComboList cboTime = new ComboList("Time Limit", screenwidth, font, Color.White, new List<String>(new String[] { "60", "80", "100", "120" }), "timelimit");
            btns.Add(cboTime);
            ComboList cbotype = new ComboList("Game type", screenwidth, font, Color.White, new List<String>(new String[] { "Time attack", "First hit wins"}), "gametype");
            btns.Add(cbotype);
            Button btn = new Button("Acept", screenwidth, font, Color.White, GameState.Start1P);
            btns.Add(btn);
            btn = new Button("Cancel", screenwidth, font, Color.White, GameState.GotoMainMenu);
            btns.Add(btn);
            PositionButtons();
            ButtonFocus(1);
        }

        private void saveOptions()
        {
            //Data tx = new Data();
            //tx.gametype = 
            //tx

            //// Write to XML
            //XmlSerializer writer = new XmlSerializer(typeof(Data));
            //using (FileStream file = File.OpenWrite("data.xml"))
            //{
            //    writer.Serialize(file, tx);
            //}

            //// Read from XML
            //Data rx;

            //XmlSerializer reader = new XmlSerializer(typeof(Data));
            //using (FileStream input = File.OpenRead("data.xml"))
            //{
            //    rx = reader.Deserialize(input) as Data;
            //}
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
