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
    public class Player
    {
        // <AI Variables>
        private int moveLoop = 0; // Movement loop (the player does not move every frame)
        private Player Human; // A reference to the human player
        private Map Map; // A reference to the current map
        private int runAwayDelay = 0; // A little delay to keep running away after the bomb explodes
        private bool hasToCorrect = false; // A flag that indicates if the direction is imposible and a correction is needed        
        private PlayerDirection Direction = new PlayerDirection(); // Current direction
        private PlayerDirection prevDirection = new PlayerDirection(); // Previous direction
        private int relevantDiff = 0; // The minimum difference (between X and Y) for the player to change the current direction    
        private bool runningAway = false;
        private string maintainDirection = "";
        // </AI Variables>
        private Vector2 RespawnLoc; // Location the respawn will point to
        public int inmunityCounter = 0; // Frame duration of inmunity (after being hitted)
        public bool wallHitted; // Player hitted a wall Flag        
        public Texture2D Texture { get; set; }
        public int Columns { get; set; }        
        private int currentFrame;
        private string KeyControl;
        private string InstanceName;
        private PlayerStyle PStlye;
        private ControlWrapper cwrap;
        private int ItemCollected = -1;
        private int[] idleFrames = new int[] { 0, 1, 2, 3 };
        private int[] walkFrames = new int[] { 4, 5, 6, 7 };
        private int[] deadFrames = new int[] { 8, 9, 10, 11 };
        public string Status; // walking, idle, dead
        public string PrevStatus;
        public Rectangle hitBox;
        public Vector2 newPosition;
        Vector2 Location;        
        Vector2 Speed = new Vector2();
        //Vector2 Acceleration = new Vector2(40, 40);
        BombManager BombMan;
        Score Score;
        public int BombCount = 0;
        public int BombMax = 3;
        int minVel = 200;
        int maxVel = 280;
        int directionX = 0;
        int directionY = 0;

        public Player(Texture2D texture, Vector2 location, ControlType ctype, BombManager BombMan, Score score, string instancename, PlayerStyle pstlye)
        {            
            this.Score = score;
            this.RespawnLoc = location;
            this.Location = location;
            Speed.X = minVel;
            Speed.Y = minVel;
            Status = "idle";
            PrevStatus = "";
            Texture = texture;
            currentFrame = 0;
            Columns = texture.Width / 30;
            this.PStlye = pstlye;
            this.InstanceName = instancename;
            this.BombMan = BombMan;
            if(this.PStlye == PlayerStyle.Human)
            {
                cwrap = new ControlWrapper(ctype);
            }
        }

        public void setItem(int itemstyle)
        {
            this.ItemCollected = itemstyle;            
        }

        public void Update(GameTime gametime)
        {
            int totalFrames = -1;
            int resetFrame = -1;
         
            UpdateInput((float)gametime.ElapsedGameTime.TotalSeconds);
            if (this.Status == "walking")
            {
                resetFrame = 4;
                totalFrames = 7;
            }
            else if(this.Status == "idle")
            {
                resetFrame = 0;
                totalFrames = 3;
            }
            else if(this.Status == "dead")
            {
                resetFrame = 8;
                totalFrames = 11;
            }

            if (this.Status != this.PrevStatus)
            {
                currentFrame = resetFrame;
                this.PrevStatus = this.Status;
            }
            currentFrame++;
            if (currentFrame == totalFrames)
            {
                if (this.Status == "dead")
                {
                    currentFrame = resetFrame;
                    resPawn();
                }
                else
                {
                    currentFrame = resetFrame;
                }
            }
            if (this.inmunityCounter != 0)
            {
                this.inmunityCounter--;
            }
        }

        private void resPawn()
        {
            this.inmunityCounter = 170; // Lasts, more or less a bomb explosion time
            this.Status = "respawning";
            string dest = (InstanceName == "p1") ? "p2" : "p1";
            Score.setScore(dest);            
            this.Location = this.RespawnLoc;
        }

        // AI Stuff

        public void InitAI(Player human, Map map)
        {
            this.Human = human;
            this.Map = map;
            this.relevantDiff = 50;
        }

        private bool opponentReachable(Vector2 pos)
        {
            Vector2 result = new Vector2();
            result = pos - Location;

            if (Math.Abs(result.X) < 30 && Math.Abs(result.Y) < 30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AI_Plan(float elapsedTime)
        {
            if (this.Status != "dead")
            {
                this.Status = "idle";
                if (moveLoop == 0)
                {                    
                    moveLoop = 2;
                    Vector2 pos = getOpponentPosition();
                    Vector2 avoidpos = AI_Avoid();
                    if (avoidpos.X == 9999)
                    {
                        maintainDirection = "";
                        if (runAwayDelay > 0)
                        {
                            //Console.WriteLine("Evade Bomb " + DateTime.Now.ToString());
                            AI_TryWalk(avoidpos, elapsedTime, -1);
                            runAwayDelay--;
                        }
                        else
                        {
                            if (opponentReachable(pos))
                            {
                                AI_Attack();
                            }
                            else
                            {
                                //Console.WriteLine("Seek Character " + DateTime.Now.ToString());
                                AI_TryWalk(pos, elapsedTime);
                            }
                        }
                    }
                    else
                    {                        
                        //Console.WriteLine("I see a bomb! " + DateTime.Now.ToString());
                        AI_TryWalk(avoidpos, elapsedTime, -1);
                        runAwayDelay = 15;
                    }
                }
                else
                {
                    moveLoop--;
                }
            }
        }

        private Vector2 AI_Avoid()
        {
            this.runningAway = false;
            Vector2 bomb = BombMan.getNearestBomb(this.Location);
            if(bomb.X == 9999)
            {                
                return bomb;
            }
            else
            {   
                float distance = Vector2.Distance(Location, bomb);
                if (distance < 200)
                {
                    this.runningAway = true;
                    return bomb;
                }
                else
                {                                        
                    return new Vector2(9999, 9999);
                }
            }
        }

        private void AI_TryWalk(Vector2 pos, float elapsedTime, int runAway = 1)
        {
            Console.WriteLine("-----------------------------------------------------------");

            List<string> tries = new List<string>();
            this.Status = "walking";
            setDirection(pos, elapsedTime, runAway); // See where I wanna go
            Vector2 desiredLoc = new Vector2();
                
            if (maintainDirection == "")
            {
                switch (getRealDirecion(Direction.MainDirection, runAway))
                {
                    case "R":
                    case "L":
                        desiredLoc.X = Direction.X;
                        desiredLoc.Y = Location.Y;
                        break;
                    case "D":
                    case "U":
                        desiredLoc.X = Location.X;
                        desiredLoc.Y = Direction.Y;
                        break;
                }

                AI_chekWalkable(desiredLoc); // Check if I can go, to the main direction
                tries.Add(Direction.MainDirection);
                Console.WriteLine("1° " + getRealDirecion(Direction.MainDirection, runAway));

                if (this.hasToCorrect) // If I can't...
                {
                    switch (prevDirection.MainDirection)
                    {
                        case "R":
                            desiredLoc.X += (Speed.X * elapsedTime) * 1 * runAway;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "L":
                            desiredLoc.X += (Speed.X * elapsedTime) * -1 * runAway;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "D":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * 1 * runAway;
                            break;
                        case "U":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * -1 * runAway;
                            break;
                    }
                    AI_chekWalkable(desiredLoc); // Check if I can go, to the 2nd
                    tries.Add(getRealDirecion(prevDirection.MainDirection, runAway));
                    Console.WriteLine("2° " + getRealDirecion(prevDirection.MainDirection, runAway));
                }
                else
                {
                    maintainDirection = "";
                    prevDirection.MainDirection = Direction.MainDirection;
                    prevDirection.SecondaryDirection = Direction.SecondaryDirection;
                    prevDirection.X = Direction.X;
                    prevDirection.Y = Direction.Y;
                }

                if (this.hasToCorrect)
                {
                    desiredLoc.Y = Location.Y;
                    desiredLoc.X = Location.X;
                    // I can't remember why I did this runAway reset, so I commented it...
                    //if (runAway == -1)
                    //{
                    //    runAway = 1;
                    //}
                    switch (prevDirection.SecondaryDirection)
                    {
                        case "R":
                            desiredLoc.X += (Speed.X * elapsedTime) * 1 * runAway;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "L":
                            desiredLoc.X += (Speed.X * elapsedTime) * -1 * runAway;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "D":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * 1 * runAway;
                            break;
                        case "U":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * -1 * runAway;
                            break;
                    }
                    AI_chekWalkable(desiredLoc); // Check if I can go, to the 3°...
                    tries.Add(getRealDirecion(prevDirection.SecondaryDirection, runAway));
                    Console.WriteLine("3° " + getRealDirecion(prevDirection.SecondaryDirection, runAway));
                }
                else
                {
                    maintainDirection = "";
                }
            }

            if (this.hasToCorrect || maintainDirection != "")
            {
                string dir = "";
                if(maintainDirection == "")
                {
                    dir = getNotUsed(tries);
                }
                else
                {
                    dir = maintainDirection;
                }
                while (dir != "")
                {
                    desiredLoc.Y = Location.Y;
                    desiredLoc.X = Location.X;
                    tries.Add(dir);
                    switch (dir)
                    {
                        case "R":
                            desiredLoc.X += (Speed.X * elapsedTime) * 1;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "L":
                            desiredLoc.X += (Speed.X * elapsedTime) * -1;
                            desiredLoc.Y = Location.Y;
                            break;
                        case "D":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * 1;
                            break;
                        case "U":
                            desiredLoc.X = Location.X;
                            desiredLoc.Y += (Speed.Y * elapsedTime) * -1;
                            break;
                    }
                    AI_chekWalkable(desiredLoc); // Let's try any that we didn't try earlier... 
                    Console.WriteLine("N° " + dir);
                    if (!this.hasToCorrect)
                    {
                        maintainDirection = dir;
                        dir = "";
                    }
                    else
                    {
                        if(maintainDirection =="")
                        {
                            dir = getNotUsed(tries);
                        }
                        else
                        {
                            break;
                        }
                    }                    
                }
            }
            if (!this.hasToCorrect) // If you get here, and couldn't go, you are f*****!
            {
                Location = desiredLoc;
            }
            

            Console.WriteLine("-----------------------------------------------------------");
        }

        private string getNotUsed(List<string> directions)
        {
            if (!directions.Contains("U"))
            {
                return "U";
            }
            if (!directions.Contains("D"))
            {
                return "D";
            }
            if (!directions.Contains("L"))
            {
                return "L";
            }
            if (!directions.Contains("R"))
            {
                return "R";
            }
            return ""; // All directions used... =(
        }

        private string getRealDirecion(string direction, int mult)
        {
            string r = "";
            if(mult == 1)
            {
                return direction;
            }

            if(direction == "R")
            {
                r = "L";
            }
            if(direction == "L")
            {
                r = "R";
            }
            if(direction == "D")
            {
                r = "U";
            }
            if(direction == "U")
            {
                r = "D";
            }
            return r;
        }

        private void AI_chekWalkable(Vector2 desiredLoc)
        {            
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            Rectangle future_pos = new Rectangle((int)desiredLoc.X, (int)desiredLoc.Y, width, height);
            this.hasToCorrect = false;
            for (int index = 0; index < Map.tiles.Count; index++)
            {
                if (Map.tiles[index].hitBox.Intersects(future_pos))
                {                    
                    if (Map.tiles[index].BreakAble)
                    {
                        if (this.BombCount < this.BombMax)
                        {
                            if (!runningAway)
                            {
                                placeBomb();
                            }
                            else
                            {
                                this.hasToCorrect = true; // I set this to true, so the player thinks it cannot go anywhere 
                            }
                        }
                    }
                    else
                    {
                        this.hasToCorrect = true;
                    }                    
                    return;
                }
            }
        }

        private void AI_Attack()
        {
            placeBomb();
        }

        Vector2 FindInterceptingPoint()
        {
            Vector2 v, d, t; 
            v = this.Speed;
            d = Human.getLocation() - this.Location; // range to close            
            t = Vector2.Divide(d, v.X);
            return Human.getLocation() + (Speed * t); // target point
        }

        private void setDirection(Vector2 pos, float elapsedTime, int runAway)
        {            
            Direction.X = Location.X;
            Direction.Y = Location.Y;
            float YDiff = pos.Y - this.Location.Y;
            float XDiff = pos.X - this.Location.X;
            float distance;
            distance = Vector2.Distance(Location, pos);

            if (YDiff < 0)
            {
                directionY = -1; // UP
            }
            else
            {
                directionY = 1; // DOWN
            }
            if (XDiff < 0)
            {
                directionX = -1; // LEFT 
            }
            else
            {
                directionX = 1; // RIGHT
            }
            if (distance < 500 || ((Math.Abs( Math.Abs(XDiff) - Math.Abs(YDiff)) > relevantDiff) || prevDirection.MainDirection == "")) // Difference is relevant (or first time)
            {
                Direction.Y += (Speed.Y * elapsedTime) * directionY * runAway;
                Direction.X += (Speed.X * elapsedTime) * directionX * runAway;

                if (Math.Abs(XDiff) > Math.Abs(YDiff))
                {                    
                    Direction.MainDirection = (directionX == 1) ? "R" : "L";
                    Direction.SecondaryDirection = (directionY == 1) ? "D" : "U";
                }
                else
                {                    
                    Direction.MainDirection = (directionY == 1) ? "D" : "U";
                    Direction.SecondaryDirection = (directionX == 1) ? "R" : "L";
                }                
            }
            else // Difference is not relevant, keep the previous direction
            {
                switch (prevDirection.MainDirection)
                {
                    case "U":
                        directionY = -1; // UP                                                
                        break;
                    case "D":
                        directionY = 1; // DOWN                                                
                        break;
                    case "L":
                        directionX = -1; // LEFT                         
                        break;
                    case "R":
                        directionX = 1; // RIGHT           
                        break;
                }
                Direction.Y += (Speed.Y * elapsedTime) * directionY * runAway;
                Direction.X += (Speed.X * elapsedTime) * directionX * runAway;
                Direction.MainDirection = prevDirection.MainDirection;
            }            
        }

        private Vector2 getOpponentPosition()
        {
            return FindInterceptingPoint();//;Human.getLocation();
        }

        private void UpdateInput(float elapsedTime)
        {
            if (PStlye == PlayerStyle.Human)
            {                
                if (this.Status != "dead")
                {
                    this.Status = "idle";
                    
                    //if (!st.IsKeyDown(PCtrls.Bomb))
                    if(!cwrap.IsKeyDown(PlayerActions.Bomb))
                    {
                        KeyControl = "";
                    }

                    //if (st.IsKeyDown(PCtrls.Bomb) && KeyControl != "bomb" && this.BombCount < this.BombMax)
                    if (cwrap.IsKeyDown(PlayerActions.Bomb) && KeyControl != "bomb" && this.BombCount < this.BombMax)
                    {
                        KeyControl = "bomb";
                        placeBomb();
                    }

                    //if (st.IsKeyDown(PCtrls.Right))
                    if(cwrap.IsKeyDown(PlayerActions.Right))
                    {
                        this.Status = "walking";
                        directionX = 1;
                    }

                    //if (st.IsKeyDown(PCtrls.Left))
                    if(cwrap.IsKeyDown(PlayerActions.Left))
                    {
                        this.Status = "walking";
                        directionX = -1;
                    }

                    //if (st.IsKeyDown(PCtrls.Down))
                    if(cwrap.IsKeyDown(PlayerActions.Down))
                    {
                        this.Status = "walking";
                        directionY = 1;
                    }

                    //if (st.IsKeyDown(PCtrls.Up))
                    if(cwrap.IsKeyDown(PlayerActions.Up))
                    {
                        this.Status = "walking";
                        directionY = -1;
                    }

                    //if (!st.IsKeyDown(PCtrls.Right) &&
                    //    !st.IsKeyDown(PCtrls.Left))
                    if(!cwrap.IsKeyDown(PlayerActions.Right) &&
                        !cwrap.IsKeyDown(PlayerActions.Left))
                    {
                        //Speed.X = 0;
                        directionX = 0;
                    }

                    //if (!st.IsKeyDown(PCtrls.Up) &&
                    //    !st.IsKeyDown(PCtrls.Down))
                    if (!cwrap.IsKeyDown(PlayerActions.Up) &&
                        !cwrap.IsKeyDown(PlayerActions.Down))
                    {
                        directionY = 0;
                    }
                    Location.X += (Speed.X * elapsedTime) * directionX;
                    Location.Y += (Speed.Y * elapsedTime) * directionY;
                }
            }
            else
            {
                AI_Plan(elapsedTime);
            }

            // Warps
            if (Location.X < -21)
            {
                Location.X = 796;
            }
            if (Location.X > 798)
            {
                Location.X = -20;
            }
        }

        public Vector2 getLocation()
        {
            return Location;
        }

        private void placeBomb()
        {                       
            BombMan.SpawnBomb(Location, InstanceName);
            this.BombCount++; 
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (wallHitted)
            {
                Location = newPosition;
                wallHitted = false;
            }

            if (inmunityCounter % 2 == 0)
            {
                // Draw the player in the new location(x,y)
                int width = Texture.Width / Columns;
                int height = Texture.Height;
                int row = (int)((float)currentFrame / (float)Columns);
                int column = currentFrame % Columns;
                Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
                Rectangle destinationRectangle = new Rectangle((int)Location.X, (int)Location.Y, width, height);
                hitBox = destinationRectangle;
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            }
        }
    }
}
