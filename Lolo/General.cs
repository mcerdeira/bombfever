using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lolo
{
    public enum GameState
    {        
        None,
        MainMenu,      // Displaying main menu
        GotoMainMenu,  // (Command) Call the main menu display
        Options,       // Displaying game options
        GotoOptions,   // (Command) Call the options menu
        Start1P,       // (Command) Starts the game 1P vs CPU
        Start2P,       // (Command) Starts the game 1P vs 2P
        Playing1P,     // Displaying in-game
        Playing2P,     // Displaying in-game
        LoadFromFile,  // Load a level from a txt file
        Credits,       // Shows the game credits
        Quit,          // (Command) Quit the game 
        GoAndLoadFile, // (Command) Starts the game 1P vs CPU        
        RoundResults   // Displaying round results
    }

    public enum ControlType
    {
        KeyBoard1,
        KeyBoard2,
        JoyStick1,
        JoyStick2
    }
    
    public enum PlayerStyle
    {
        Human,
        Machine
    }

    public enum PlayerActions
    { 
        Up,
        Down,
        Right,
        Left,
        Bomb,
        Select,
        None
    }

    public enum ItemTypes
    {        
        Inmortal,
        Shield,
        Speed_Shoe,
        Freeze,
        Sudden_Death,
        Extra_Time,
        Extra_Bomb,
        Triggered_Bombs,
        Bouncing_Bombs,
    }

    class General
    {
        public static List<string> getControlTypes()
        {
            return new List<String>(new String[] { "Keyboard", "Joystick" });
        }

        public static List<string> getRoundTimes()
        {
            return new List<String>(new String[] { "60", "80", "100", "120" });
        }

        public static List<string> getGameTypes()
        {
            return new List<String>(new String[] { "Time attack", "First hit wins" });
        }

        public static Vector2 Rectangle2Vector(Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y);
        }

        public static Vector2 IntersectDepthVector(Rectangle main, Rectangle r)
        {
          // Calculate centers.
          Vector2 centerA = new Vector2(main.Left + main.Width/2, main.Top + main.Height/2);
          Vector2 centerB = new Vector2(r.Left + r.Width/2, r.Top + r.Height/2);

          // Calculate current and minimum-non-intersecting distances between centers.
          float distanceX = centerA.X - centerB.X;
          float distanceY = centerA.Y - centerB.Y;
          float minDistanceX = (main.Width / 2) + (r.Width / 2);
          float minDistanceY = (main.Height / 2) + (r.Height / 2);

          // If we are not intersecting at all, return (0, 0).
          if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            return new Vector2(0,0);

          // Calculate and return intersection depths.
          float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
          float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
          return new Vector2(depthX, depthY);
        }

        public static float getScreenCenterTextX(string text, int screenwidth, SpriteFont Font)
        {
            return (screenwidth / 2) - (Font.MeasureString(text.Trim()).X / 2);
        }

        public static float getScreenCenterTextY(string text, int screenheight, SpriteFont Font)
        {
            return (screenheight / 2) - (Font.MeasureString(text.Trim()).Y / 2);
        }

        Vector2 FindInterceptingPoint(Vector2 Loc1, Vector2 Loc2, Vector2 Speed)
        {
            Vector2 v, d, t;
            v = Speed;
            d = Loc1 - Loc2; // range to close            
            t = Vector2.Divide(d, v.X);
            return Loc1 + (Speed * t); // target point
        }
    }
}
