using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Lolo
{
    [Serializable()]
    public class GameOptions
    {
        public int p1control;
        public int p2control;
        public int gametype;
        public int timelimit;
        public int music;
        public int fx;
    }
}
