using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace Lolo
{
    public class Player
    {
        // <AI Variables>
        private int moveLoop = 0; // Movement loop (the player does not move every frame)
        private Player Human; // A reference to the human player
        private Map Map; // A reference to the current map        
        private bool hasToCorrect = false; // A flag that indicates if the direction is imposible and a correction is needed        
        private int relevantDiff = 0; // The minimum difference (between X and Y) for the player to change the current direction          
        private PlayerDirection Direction = new PlayerDirection();        
        private List<int> path = new List<int>();
        private bool PathFound = false;
        private int PathFindDelay = 0;
        private bool runningAway = false;
        private Thread PathF_T;
        private AI_States AI_State = AI_States.None;
        // </AI Variables>
        private string lastDirection = "";
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
        public int BombMax = 2;
        int minVel = 200;
        int maxVel = 280;
        int directionX = 0;
        int directionY = 0;        
        private List<SoundEffect> sndFXList;

        public Player(Texture2D texture, Vector2 location, ControlType ctype, BombManager BombMan, Score score, string instancename, PlayerStyle pstlye, List<SoundEffect> sndfxlist)
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
            Columns = texture.Width / 50; //30
            this.PStlye = pstlye;
            this.InstanceName = instancename;
            this.BombMan = BombMan;
            if(this.PStlye == PlayerStyle.Human)
            {
                cwrap = new ControlWrapper(ctype);
            }
            this.sndFXList = sndfxlist;
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
            Console.WriteLine("################################################ D I E D ###################################################");
            this.inmunityCounter = 170; // Lasts, more or less a bomb explosion time =)
            this.Status = "respawning";
            string dest = (InstanceName == "p1") ? "p2" : "p1";
            Score.setScore(dest);            
            this.Location = this.RespawnLoc;
            sndFXList[(int)PlayerSndFXs.Die].Play();
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
                Vector2 pos = Human.getLocation();
                Vector2 avoidpos = AI_Avoid();                    
                if (opponentReachable(pos))
                {
                    AI_Attack();
                }
                AI_HandleState(elapsedTime, pos);
                //if (avoidpos.X != 9999) // TODO: Handle run Away from bombs
                //{
                //    pos = avoidpos;                    
                //}                

            }
        }

        private void AI_DoPath(float elapsedTime)
        {
            if (path.Count > 0)
            {
                if (Map.tiles[path[0]].Position == findMyCell())
                {
                    path.Remove(path[0]);
                }
                if (path.Count > 0)
                {
                    AI_TryWalk(Map.tiles[path[0]].Position, elapsedTime);
                }
                else
                {
                    AI_State = AI_States.None;
                }
            }
            else
            {
                AI_State = AI_States.None;
            }
        }

        private void AI_HandleState(float elapsedTime, Vector2 pos)
        {
            Console.WriteLine(AI_State);
            switch(AI_State)
            {
                case AI_States.None:
                    PathF_T = new Thread(() => AI_PathFind(pos, 0));
                    PathF_T.Start();
                    AI_State = AI_States.Finding_Path;
                    break;
                case AI_States.Walking_Path:
                    AI_DoPath(elapsedTime);
                    break;
                case AI_States.Finding_Path:
                    if (!PathF_T.IsAlive)
                    {
                        PathF_T.Abort();
                        AI_State = AI_States.Walking_Path;
                    }
                    break;          
            }
        }

        private Vector2 AI_Avoid()
        {
            Vector2 bomb;
            if (this.inmunityCounter > 10)
            {
                return new Vector2(9999, 9999);
            }
            bomb = BombMan.getNearestBomb(this.Location);
            if(bomb.X == 9999)
            {
                return bomb;
            }
            else
            {   
                float distance = Vector2.Distance(Location, bomb);
                if (distance < 200)
                {
                    return new Vector2(normaLize(bomb.X), normaLize(bomb.Y));
                }
                else
                {                                        
                    return new Vector2(9999, 9999);
                }
            }
        }

        private int AI_PathFind(Vector2 target, int counter, int initNode = -1, int endNode = -1)
        {
            // If endNode == -2, means that we are scaping, so, there is not a REAL end node

            //counter++;
            //if (counter > 10)
            //{
            //    Console.WriteLine("FAKE END");
            //    this.PathFound = true; // Fake path found, is a recursion control
            //}

            if (this.PathFound)
            {
                return 0;
            }
            if (initNode == -1)
            {
                Vector2 myCell = findMyCell();
                initNode = IndexFromCell(myCell);
                Console.WriteLine("I'm at " + initNode.ToString());
            }
            if (endNode == -1)
            {                
                endNode = IndexFromCell(new Vector2(normaLize(target.X), normaLize(target.Y)));
                Console.WriteLine("Wanna Find " + endNode.ToString());
            }

            // Sorts the neighbor list by the lowest Distance to the target
            int[] dirs = General.getNeighbor(initNode);
            List<Tile> tmp = new List<Tile>();
            List<Tile> tilelist = new List<Tile>();
            for (int i = 0; i< dirs.Count(); i++)
            {
                tmp.Add(Map.tiles[dirs[i]]);
            }
            if (endNode == -2)
            {                
                tilelist = tmp.OrderByDescending(e => Vector2.Distance(e.Position, target)).ToList();
            }
            else
            {
                tilelist = tmp.OrderBy(e => Vector2.Distance(e.Position, target)).ToList();
            }

            foreach (Tile t in tilelist)
            {
                int i = IndexFromCell(t.Position);
                if(this.PathFound)
                {
                    return 0;
                }
                if (dirs.Contains(endNode))
                {
                    // We find the final node!
                    Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<found!>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>><");
                    path.Add(endNode);                    
                    this.PathFound = true;
                    return 0;
                }
                else
                {
                    if (!path.Contains(i) && (Map.tiles[i].Walkable || Map.tiles[i].BreakAble))
                    {
                        if (!(endNode == -2) || Map.tiles[i].Walkable) // If scaping, only search open tiles
                        {
                            path.Add(i);
                            Console.WriteLine(i + " --> " + endNode.ToString());
                            int r = AI_PathFind(target, counter, i, endNode);
                        }
                    }
                }
            }
            #warning Analize the dead ends to remove dead nodes from the path
            Console.WriteLine("dead end");
            return 1;
        }

        public int IndexFromCell(Vector2 cell)
        {            
            List<Tile> results = Map.tiles.FindAll(
                delegate(Tile til)
                {
                    return til.Position.X == cell.X && til.Position.Y == cell.Y;
                }
            );
            return Map.tiles.IndexOf(results[0]);
        }

        private Vector2 findMyCell()
        {
            return new Vector2(normaLize(this.Location.X), normaLize(this.Location.Y));
        }

        private float normaLize(float n)
        {
            // This is SO awfull...
            if(n > 0 && n < 50)
            {
                n = 0;
            }
            else if (n > 50 && n < 100)
            {
                n = 50;
            }
            else if (n > 100 && n < 150)
            {
                n = 100;
            }
            else if (n > 150 && n < 200)
            {
                n = 150;
            }
            else if (n > 200 && n < 250)
            {
                n = 200;
            }
            else if (n > 250 && n < 300)
            {
                n = 250;
            }
            else if (n > 300 && n < 350)
            {
                n = 300;
            }
            else if (n > 350 && n < 400)
            {
                n = 350;
            }
            else if (n > 400 && n < 450)
            {
                n = 400;
            }
            else if (n > 450 && n < 500)
            {
                n = 450;
            }
            else if (n > 500 && n < 550)
            {
                n = 500;
            }
            else if (n > 550 && n < 600)
            {
                n = 550;
            }
            else if (n > 600 && n < 650)
            {
                n = 600;
            }
            else if (n > 650 && n < 700)
            {
                n = 650;
            }
            else if (n > 700 && n < 750)
            {
                n = 700;
            }
            else if (n > 750)
            {
                n = 750;
            }        
            return n;
        }

        private void AI_WalkTillWall(float elapsedTime)
        {
            Vector2 desiredLoc = new Vector2();
            string currentDir = "";
            Console.WriteLine("Walking " + Direction.SecondaryDirection + " till wall...");
            desiredLoc.Y = Location.Y;
            desiredLoc.X = Location.X;
            currentDir = Direction.SecondaryDirection;
            switch (currentDir)
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
            AI_chekWalkable(desiredLoc, currentDir); // Check if I can go, to the 2nd...

            if (!this.hasToCorrect) // If you get here, and couldn't go, you are f*****!
            {
                Location = desiredLoc;
                Direction.LastDirection = currentDir;
            }

            if (!this.hasToCorrect) // If you get here, and couldn't go, you are f*****!
            {
                Location = desiredLoc;
                Direction.LastDirection = currentDir;
            }
        }


        private void AI_TryWalk(Vector2 pos, float elapsedTime)
        {
            //Console.WriteLine("-----------------------------------------------------------");
            string currentDir = "";
            List<string> tries = new List<string>();
            this.Status = "walking";
            setDirection(pos, elapsedTime); // See where I wanna go
            Vector2 desiredLoc = new Vector2();
            desiredLoc.Y = Location.Y;
            desiredLoc.X = Location.X;
            currentDir = Direction.MainDirection;
            switch (currentDir)
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
            AI_chekWalkable(desiredLoc, currentDir); // Check if I can go, to the main direction
            //tries.Add(currentDir);
            //Console.WriteLine("1° " + currentDir);

            if (this.hasToCorrect)
            {
                //Console.WriteLine("can't");
                desiredLoc.Y = Location.Y;
                desiredLoc.X = Location.X;
                currentDir = Direction.SecondaryDirection;
                switch (currentDir)
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
                AI_chekWalkable(desiredLoc, currentDir); // Check if I can go, to the 2nd...
            }

            if (!this.hasToCorrect) // If you get here, and couldn't go, you are f*****!
            {
                Location = desiredLoc;
                Direction.LastDirection = currentDir;
            }
        }

        private void AI_chekWalkable(Vector2 desiredLoc, string direction)
        {
            int width = this.hitBox.Width;
            int height = this.hitBox.Height;
            Rectangle future_pos = new Rectangle((int)desiredLoc.X, (int)desiredLoc.Y, width, height);
            this.hasToCorrect = false;
            for (int index = 0; index < Map.tiles.Count; index++)
            {
                if (!(Map.tiles[index].ID == 0) && Map.tiles[index].hitBox.Intersects(future_pos))
                {
                    if (Map.tiles[index].BreakAble)
                    {
                        if (Map.tiles[index].Walkable)
                        {
                            this.hasToCorrect = false;
                            return;
                        }
                        else
                        {
                            if (this.BombCount < this.BombMax)
                            {
                                if (!runningAway)
                                {
                                    Console.WriteLine("$$$$$$$$$$$$$ BOMB $$$$$$$$$$$$$");
                                    placeBomb();
                                }
                                else
                                {
                                    this.hasToCorrect = true; // I set this to true, so the player thinks it cannot go anywhere 
                                }
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
            placeBomb(true);
        }

        private void setDirection(Vector2 pos, float elapsedTime)
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
            if (distance < 500 || ((Math.Abs(Math.Abs(XDiff) - Math.Abs(YDiff)) > relevantDiff))) // Difference is relevant (or first time)
            {
                Direction.Y += (Speed.Y * elapsedTime) * directionY;
                Direction.X += (Speed.X * elapsedTime) * directionX;

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
                switch (Direction.LastDirection)
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
                Direction.Y += (Speed.Y * elapsedTime) * directionY;
                Direction.X += (Speed.X * elapsedTime) * directionX;
                Direction.MainDirection = Direction.LastDirection;
            }            
        }

        private void UpdateInput(float elapsedTime)
        {
            string KeyH = "";
            string KeyV = "";

            if (PStlye == PlayerStyle.Human)
            {                
                if (this.Status != "dead")
                {
                    directionX = 0;
                    directionY = 0;
                    this.Status = "idle";                    
                    if(!cwrap.IsKeyDown(PlayerActions.Bomb))
                    {
                        KeyControl = "";
                    }
                    
                    if (cwrap.IsKeyDown(PlayerActions.Bomb) && KeyControl != "bomb")
                    {
                        if (this.BombMan.KickingBomb(this))
                        {
                            sndFXList[(int)PlayerSndFXs.KickBomb].Play();
                            KeyControl = "bomb";
                        }
                        else
                        {
                            if (this.BombCount < this.BombMax)
                            {
                                KeyControl = "bomb";
                                placeBomb();
                            }
                        }
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Right))
                    {
                        this.Status = "walking";
                        KeyH = "R";
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Left))
                    {
                        this.Status = "walking";
                        KeyH = "L";
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Down))
                    {
                        this.Status = "walking";
                        KeyV = "D";
                    }

                    if (cwrap.IsKeyDown(PlayerActions.Up))
                    {
                        this.Status = "walking";
                        KeyV = "U";
                    }

                    // If one of the key sets is empty
                    if (KeyH == "" || KeyV == "")
                    {
                        if (KeyH == "R")
                        {
                            lastDirection = "R";
                            directionX = 1;
                        }
                        else if(KeyH == "L")
                        {
                            lastDirection = "L";
                            directionX = -1;
                        }

                        if (KeyV == "U")
                        {
                            lastDirection = "U";
                            directionY = -1;
                        }
                        else if (KeyV == "D")
                        {
                            lastDirection = "D";
                            directionY = 1;
                        }                        
                    }

                    // If both are useds
                    if (KeyH != "" && KeyV != "")
                    {
                        if (KeyH != lastDirection)
                        {
                            if (KeyH == "R")
                            {                                
                                directionX = 1;
                            }
                            else if (KeyH == "L")
                            {                                
                                directionX = -1;
                            }
                        }
                        else
                        {
                            if (KeyV == "U")
                            {                                
                                directionY = -1;
                            }
                            else if (KeyV == "D")
                            {                                
                                directionY = 1;
                            }           
                        }
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

        private void placeBomb(bool attacking = false)
        {
            if (this.BombCount < this.BombMax)
            {
                BombMan.SpawnBomb(Location, InstanceName);
                this.BombCount++;
                this.sndFXList[(int)PlayerSndFXs.PlaceBomb].Play();
            }
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

                int hitX = (int)Location.X + 5;
                int hitY = (int)Location.Y + 10;
                hitBox = new Rectangle(hitX, hitY, 40, 40);//destinationRectangle;
                spriteBatch.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);

                spriteBatch.Draw(Texture, hitBox, hitBox, Color.Red);
            }
        }
    }
}
