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
        private string Direction = ""; // Current direction
        private string prevDirection = ""; // Previous direction
        private int relevantDiff = 0; // The minimum difference (between X and Y) for the player to change the current direction
        // </AI Variables>
        private Vector2 RespawnLoc; // Location the respawn will point to
        public int inmunityCounter = 0; // Frame duration of inmunity (after being hitted)
        public bool wallHitted; // Player hitted a wall Flag        
        public Texture2D Texture { get; set; }
        public int Columns { get; set; }
        private PlayerControls PCtrls;        
        private int currentFrame;
        private string KeyControl;
        private string InstanceName;
        private PlayerStyle PStlye;
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
            this.PCtrls = new PlayerControls(ctype);
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
            this.inmunityCounter = 150;
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
            this.relevantDiff = Map.tiles[0].hitBox.Width;
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
                if (moveLoop == 0 || this.hasToCorrect)
                {
                    moveLoop = 2;
                    Vector2 pos = getOpponentPosition();
                    Vector2 avoidpos = AI_Avoid();
                    if (avoidpos.X == 9999)
                    {
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
                        #warning Will be nicer to wait hidden, until the bomb explodes
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
                    //Console.WriteLine("Evade");
                    return bomb;
                }
                else
                {
                    //Console.WriteLine("Dont evade");
                    return new Vector2(9999, 9999);
                }
            }
        }

        private void AI_TryWalk(Vector2 pos, float elapsedTime, int runAway = 1)
        {
            this.Status = "walking";            
            Vector2 desiredLoc;
            if (this.hasToCorrect)
            {
                desiredLoc = workAround(elapsedTime, Direction);
            }
            else
            {
                desiredLoc = setDirection(pos, elapsedTime, runAway); // Go and check for an alternative
            }
            AI_chekWalkable(desiredLoc, elapsedTime);
        }

        private void AI_chekWalkable(Vector2 desiredLoc, float elapsedTime)
        {
            bool free = true; // No tile, can walk
            int width = Texture.Width / Columns;
            int height = Texture.Height;
            Rectangle future_pos = new Rectangle((int)desiredLoc.X, (int)desiredLoc.Y, width, height);
            for (int index = 0; index < Map.tiles.Count; index++)
            {
                if (Map.tiles[index].hitBox.Intersects(future_pos))
                {
                    free = false;
                    if (Map.tiles[index].BreakAble)
                    {
                        if (this.BombCount < this.BombMax)
                        {
                            placeBomb();
                        }
                        return;
                    }
                }
            }
            if (free)
            {
                Console.WriteLine("try to go " + Direction + " and could");
                prevDirection = Direction;                     
                this.hasToCorrect = false;
                Location = desiredLoc;
            }
            else
            {
                Console.WriteLine("try to go " + Direction + " and could NOT");
                prevDirection = Direction;
                this.hasToCorrect = true;              
            }
        }

        private Vector2 workAround(float elapsedTime, string direction)
        {
            #warning See if we can choose alternate direction based on player position
            Vector2 desiredDirection = Location;
            switch (direction)
            {
                case "R":
                    directionY = -1; // UP
                    Direction = "U";
                    desiredDirection.Y += (Speed.Y * elapsedTime) * directionY;
                    break;
                case "L":
                    directionY = 1; // DOWN
                    Direction = "D";
                    desiredDirection.Y += (Speed.Y * elapsedTime) * directionY;
                    break;
                case "U":
                    directionX = -1; // LEFT
                    Direction = "L";
                    desiredDirection.X += (Speed.X * elapsedTime) * directionX;
                    break;
                case "D":
                    directionX = 1; // RIGHT 
                    Direction = "R";
                    desiredDirection.X += (Speed.X * elapsedTime) * directionX;
                    break;
            }
            return desiredDirection;
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

        private Vector2 setDirection(Vector2 pos, float elapsedTime, int runAway)
        {
            Vector2 desiredDirection = Location;
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
            if (distance < 500 || ((Math.Abs( Math.Abs(XDiff) - Math.Abs(YDiff)) > relevantDiff) || prevDirection == "")) // Difference is relevant (or first time)
            {
                Console.WriteLine("Direction by enemy " + Direction + "(difference");

                if (Math.Abs(XDiff) > Math.Abs(YDiff))
                {
                    desiredDirection.X += (Speed.X * elapsedTime) * directionX * runAway;
                    Direction = (directionX == 1) ? "R" : "L";
                }
                else
                {
                    desiredDirection.Y += (Speed.Y * elapsedTime) * directionY * runAway;
                    Direction = (directionY == 1) ? "D" : "U";
                }
            }
            else // Difference is not relevant, keep the previous direction
            {
                switch (prevDirection)
                {
                    case "U":
                        directionY = -1; // UP                        
                        desiredDirection.Y += (Speed.Y * elapsedTime) * directionY * runAway;
                        break;
                    case "D":
                        directionY = 1; // DOWN                        
                        desiredDirection.Y += (Speed.Y * elapsedTime) * directionY * runAway;
                        break;
                    case "L":
                        directionX = -1; // LEFT 
                        desiredDirection.X += (Speed.X * elapsedTime) * directionX * runAway;
                        break;
                    case "R":
                        directionX = 1; // RIGHT                         
                        desiredDirection.X += (Speed.X * elapsedTime) * directionX * runAway;
                        break;
                }
                Console.WriteLine("Direction by enemy " + Direction + "(previous");
            }           
            return desiredDirection;
        }

        private Vector2 getOpponentPosition()
        {
            return FindInterceptingPoint();//;Human.getLocation();
        }

        private void UpdateInput(float elapsedTime)
        {
            if (PStlye == PlayerStyle.Human)
            {
                #warning add joystick support
                //GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed   
                if (this.Status != "dead")
                {
                    this.Status = "idle";
                    KeyboardState st = Keyboard.GetState();
                    if (!st.IsKeyDown(PCtrls.Bomb))
                    {
                        KeyControl = "";
                    }

                    if (st.IsKeyDown(PCtrls.Bomb) && KeyControl != "bomb" && this.BombCount < this.BombMax)
                    {
                        KeyControl = "bomb";
                        placeBomb();
                    }

                    if (st.IsKeyDown(PCtrls.Right))
                    {
                        this.Status = "walking";
                        directionX = 1;
                    }

                    if (st.IsKeyDown(PCtrls.Left))
                    {
                        this.Status = "walking";
                        directionX = -1;
                    }

                    if (st.IsKeyDown(PCtrls.Down))
                    {
                        this.Status = "walking";
                        directionY = 1;
                    }

                    if (st.IsKeyDown(PCtrls.Up))
                    {
                        this.Status = "walking";
                        directionY = -1;
                    }

                    if (!st.IsKeyDown(PCtrls.Right) &&
                        !st.IsKeyDown(PCtrls.Left))
                    {
                        //Speed.X = 0;
                        directionX = 0;
                    }

                    if (!st.IsKeyDown(PCtrls.Up) &&
                        !st.IsKeyDown(PCtrls.Down))
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
