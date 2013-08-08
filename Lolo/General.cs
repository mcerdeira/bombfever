using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

namespace Lolo
{
    enum GameState
    {
        MainMenu,     // Displaying main menu
        Options,      // Displaying game options
        Start1P,      // (Command) Starts the game 1P vs CPU
        Start2P,      // (Command) Starts the game 1P vs 2P
        Playing1P,    // Displaying in-game
        Playing2P,    // Displaying in-game
        LoadFromFile, // Load a level from a txt file
        Credits,      // Shows the game credits
        Quit,          // (Command) Quit the game 
        GoAndLoadFile  // (Command) Starts the game 1P vs CPU        
    }

    enum ButtonType
    {
        Button,
        CheckBox
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

    public class PlayerControls
    {
        public Keys Up;
        public Keys Down;
        public Keys Left;
        public Keys Right;
        public Keys Bomb;        

        public PlayerControls(ControlType ctpye)
        {
            if (ctpye == ControlType.KeyBoard1)
            {
                Up = Keys.Up;
                Down = Keys.Down;
                Left = Keys.Left;
                Right = Keys.Right;
                Bomb = Keys.RightShift;
            }
            else if (ctpye == ControlType.KeyBoard2)
            {
                Up = Keys.W;
                Down = Keys.S;
                Left = Keys.A;
                Right = Keys.D;
                Bomb = Keys.Space;
            }
            #warning Add here the Joystick wrapper
        }
    }

    class General
    {
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
    }
}
