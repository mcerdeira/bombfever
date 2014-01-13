using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Lolo
{
    public class ControlWrapper
    {
        private ControlType CtrlType;

        public ControlWrapper(ControlType ctpye)
        {
            this.CtrlType = ctpye;
        }

        public bool IsKeyDown(PlayerActions action)
        {
            if (CtrlType == ControlType.KeyBoard1)
            {
                Microsoft.Xna.Framework.Input.KeyboardState st = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                switch (action)
                {
                    case PlayerActions.Up:
                        return st.IsKeyDown(Keys.Up);                        
                    case PlayerActions.Down:
                        return st.IsKeyDown(Keys.Down);
                    case PlayerActions.Right:
                        return st.IsKeyDown(Keys.Right);                        
                    case PlayerActions.Left:
                        return st.IsKeyDown(Keys.Left);                        
                    case PlayerActions.Bomb:
                        return st.IsKeyDown(Keys.RightShift);
                    case PlayerActions.Select:
                        return st.IsKeyDown(Keys.Enter);
                    default:
                        return false;
                }
            }
            else if (CtrlType == ControlType.KeyBoard2)
            {
                Microsoft.Xna.Framework.Input.KeyboardState st = Microsoft.Xna.Framework.Input.Keyboard.GetState();
                switch (action)
                {
                    case PlayerActions.Up:
                        return st.IsKeyDown(Keys.W);                       
                    case PlayerActions.Down:
                        return st.IsKeyDown(Keys.S);                       
                    case PlayerActions.Right:
                        return st.IsKeyDown(Keys.D);
                    case PlayerActions.Left:
                        return st.IsKeyDown(Keys.A);
                    case PlayerActions.Bomb:
                        return st.IsKeyDown(Keys.Space);
                    case PlayerActions.Select:
                        return st.IsKeyDown(Keys.Enter);
                    default:
                        return false;
                }
            }
            else if (CtrlType == ControlType.JoyStick1 || (CtrlType == ControlType.JoyStick2))
            {
                GamePadState st;
                if (CtrlType == ControlType.JoyStick1)
                {
                    st = GamePad.GetState(PlayerIndex.One);
                }
                else
                {
                    st = GamePad.GetState(PlayerIndex.Two);
                }
                Vector2 direction = st.ThumbSticks.Left;
                switch (action)
                {
                    case PlayerActions.Up:
                        return direction.Y > 0;
                    case PlayerActions.Down:
                        return direction.Y <0;
                    case PlayerActions.Right:
                        return direction.X > 0;
                    case PlayerActions.Left:
                        return direction.X < 0;
                    case PlayerActions.Bomb:
                        return (st.Buttons.A == ButtonState.Pressed);
                    case PlayerActions.Select:
                        return (st.Buttons.B == ButtonState.Pressed); 
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
