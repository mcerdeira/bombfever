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
        private bool hasToCorrect = false; // A flag that indicates if the direction is imposible and a correction is needed        
        private int relevantDiff = 0; // The minimum difference (between X and Y) for the player to change the current direction          
        private PlayerDirection Direction = new PlayerDirection();        
        private List<int> path = new List<int>();        
        private bool PathFound = false;
        private int PathFindDelay = 0;
        private bool runningAway = false;
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
        public int BombMax = 2;
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
            Columns = texture.Width / 50; //30
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
            Console.WriteLine("################################################ D I E D ###################################################");
            this.inmunityCounter = 170; // Lasts, more or less a bomb explosion time =)
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
                    Vector2 pos = Human.getLocation();
                    Vector2 avoidpos = AI_Avoid();                    
                    if (opponentReachable(pos))
                    {
                        AI_Attack();
                    }
                    else
                    {
                        if (avoidpos.X != 9999)
                        {
                            if (!runningAway)
                            {
                                pos = avoidpos;
                                PathFindDelay = 0;
                                runningAway = true;
                            }
                        }
                        else
                        {
                            if (runningAway)
                            {
                                PathFindDelay = 0;
                                runningAway = false;
                            }
                        }
                        if (PathFindDelay <= 0)
                        {
                            Console.WriteLine("################## START OVER ##########################");
                            path = new List<int>();                            
                            PathFound = false;
                            PathFindDelay = 1;
                            if (runningAway)
                            {
                                Console.WriteLine("Bomb!!!!!!!!!!!!!!!!!!!!");
                                AI_PathFind(pos, 0, endNode:-2); 
                            }
                            else
                            {
                                Console.WriteLine("Gonna get you...");
                                AI_PathFind(pos, 0);
                            }
                        }
                        else
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
                                    if (!runningAway)
                                    {
                                        PathFindDelay = 0;
                                    }
                                    else
                                    {
                                        // Walk inside the tile until hit a wall
                                        AI_WalkTillWall(elapsedTime);
                                    }
                                }
                            }
                            else
                            {
                                if (!runningAway)
                                {
                                    PathFindDelay = 0;
                                }
                                else
                                {
                                    // Walk inside the tile until hit a wall
                                    AI_WalkTillWall(elapsedTime);
                                }
                            }                                    
                        }
                    }
                }
                else
                {
                    moveLoop--;
                }
            }
            else
            {
                runningAway = false;
                PathFindDelay = 0;
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

            counter++;
            if (counter > 10)
            {
                Console.WriteLine("FAKE END");
                this.PathFound = true; // Fake path found, is a recursion control
            }

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
            int[] dirs = getNeighbor(initNode);
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

        public int[] getNeighbor(int node)
        {
            // Returns the neighbors, based on fixed positions, ignores known non walkables 

            int[] neighbors;
            switch (node)
            {
                case 9:
                    neighbors = new int[] { 17, 10 };
                    break;
                case 10:
                    neighbors = new int[] { 9, 11, 18 };
                    break;
                case 11:
                    neighbors = new int[] { 10, 12, 19 };
                    break;
                case 12:
                    neighbors = new int[] { 11, 13, 20 };
                    break;
                case 13:
                    neighbors = new int[] { 12, 14, 21 };
                    break;
                case 14:
                    neighbors = new int[] { 13, 15, 22 };
                    break;
                case 15:
                    neighbors = new int[] { 14, 104, 23 };
                    break;
                case 17:
                    neighbors = new int[] { 9, 18, 25 };
                    break;
                case 18:
                    neighbors = new int[] { 10, 17, 19, 26 };
                    break;
                case 19:
                    neighbors = new int[] { 11, 18, 20, 27 };
                    break;
                case 20:
                    neighbors = new int[] { 12, 19, 21, 28 };
                    break;
                case 21:
                    neighbors = new int[] { 13, 20, 22, 29 };
                    break;
                case 22:
                    neighbors = new int[] { 14, 21, 23, 30 };
                    break;
                case 23:
                    neighbors = new int[] { 15, 22, 112, 31 };
                    break;
                case 25:
                    neighbors = new int[] { 17, 26, 33 };
                    break;
                case 26:
                    neighbors = new int[] { 18, 25, 27, 34 };
                    break;
                case 27:
                    neighbors = new int[] { 19, 26, 28, 35 };
                    break;
                case 28:
                    neighbors = new int[] { 20, 27, 29, 36 };
                    break;
                case 29:
                    neighbors = new int[] { 21, 28, 30, 37 };
                    break;
                case 30:
                    neighbors = new int[] { 22, 29, 31, 38 };
                    break;
                case 31:
                    neighbors = new int[] { 23, 30, 120, 39 };
                    break;
                case 33:
                    neighbors = new int[] { 25, 34, 41 };
                    break;
                case 34:
                    neighbors = new int[] { 26, 33, 35, 42 };
                    break;
                case 35:
                    neighbors = new int[] { 27, 34, 36, 43 };
                    break;
                case 36:
                    neighbors = new int[] { 28, 35, 37, 44 };
                    break;
                case 37:
                    neighbors = new int[] { 29, 36, 38, 45 };
                    break;
                case 38:
                    neighbors = new int[] { 30, 37, 39, 46 };
                    break;
                case 39:
                    neighbors = new int[] { 31, 38, 128, 47 };
                    break;
                case 41: // Near warp
                    neighbors = new int[] { 33, 42, 49 };
                    break;
                case 42:
                    neighbors = new int[] { 34, 41, 43, 50 };
                    break;
                case 43:
                    neighbors = new int[] { 35, 42, 44, 51 };
                    break;
                case 44:
                    neighbors = new int[] { 36, 43, 45, 52 };
                    break;
                case 45:
                    neighbors = new int[] { 37, 44, 46, 53 };
                    break;
                case 46:
                    neighbors = new int[] { 38, 45, 47, 54 };
                    break;
                case 47:
                    neighbors = new int[] { 39, 46, 136, 55 };
                    break;
                case 49: // Near warp
                    neighbors = new int[] { 41, 50, 57 };
                    break;
                case 50:
                    neighbors = new int[] { 49, 42, 51, 58 };
                    break;
                case 51:
                    neighbors = new int[] { 50, 43, 52, 59 };
                    break;
                case 52:
                    neighbors = new int[] { 51, 44, 53, 60 };
                    break;
                case 53:
                    neighbors = new int[] { 52, 45, 54, 61 };
                    break;
                case 54:
                    neighbors = new int[] { 53, 46, 55, 62 };
                    break;
                case 55:
                    neighbors = new int[] { 54, 47, 144, 63 };
                    break;
                case 57:
                    neighbors = new int[] { 49, 58, 65 };
                    break;
                case 58:
                    neighbors = new int[] { 50, 57, 59, 66 };
                    break;
                case 59:
                    neighbors = new int[] { 51, 58, 60, 67 };
                    break;
                case 60:
                    neighbors = new int[] { 52, 59, 61, 68 };
                    break;
                case 61:
                    neighbors = new int[] { 53, 60, 62, 69 };
                    break;
                case 62:
                    neighbors = new int[] { 54, 61, 63, 70 };
                    break;
                case 63:
                    neighbors = new int[] { 55, 62, 152, 71 };
                    break;
                case 65:
                    neighbors = new int[] { 57, 66, 73 };
                    break;
                case 66:
                    neighbors = new int[] { 58, 65, 67, 74 };
                    break;
                case 67:
                    neighbors = new int[] { 59, 66, 68, 75 };
                    break;
                case 68:
                    neighbors = new int[] { 60, 67, 69, 76 };
                    break;
                case 69:
                    neighbors = new int[] { 61, 68, 70, 77 };
                    break;
                case 70:
                    neighbors = new int[] { 62, 69, 71, 78 };
                    break;
                case 71:
                    neighbors = new int[] { 63, 70, 160, 79 };
                    break;
                case 73:
                    neighbors = new int[] { 65, 74, 81 };
                    break;
                case 74:
                    neighbors = new int[] { 66, 73, 75, 82 };
                    break;
                case 75:
                    neighbors = new int[] { 67, 74, 76, 83 };
                    break;
                case 76:
                    neighbors = new int[] { 68, 75, 77, 84 };
                    break;
                case 77:
                    neighbors = new int[] { 69, 76, 78, 85 };
                    break;
                case 78:
                    neighbors = new int[] { 70, 77, 79, 86 };
                    break;
                case 79:
                    neighbors = new int[] { 71, 78, 168, 87 };
                    break;
                case 81:
                    neighbors = new int[] { 73, 82 };
                    break;
                case 82:
                    neighbors = new int[] { 74, 81, 83 };
                    break;
                case 83:
                    neighbors = new int[] { 75, 82, 84 };
                    break;
                case 84:
                    neighbors = new int[] { 76, 83, 85 };
                    break;
                case 85:
                    neighbors = new int[] { 77, 84, 86 };
                    break;
                case 86:
                    neighbors = new int[] { 78, 85, 87 };
                    break;
                case 87:
                    neighbors = new int[] { 79, 86, 176 };
                    break;
                case 104:
                    neighbors = new int[] { 15, 105, 112 };
                    break;
                case 105:
                    neighbors = new int[] { 104, 106, 113 };
                    break;
                case 106:
                    neighbors = new int[] { 105, 107, 114 };
                    break;
                case 107:
                    neighbors = new int[] { 106, 108, 115 };
                    break;
                case 108:
                    neighbors = new int[] { 107, 109, 116 };
                    break;
                case 109:
                    neighbors = new int[] { 108, 110, 117 };
                    break;
                case 110:
                    neighbors = new int[] { 109, 118 };
                    break;
                case 112:
                    neighbors = new int[] { 23, 104, 113, 120 };
                    break;
                case 113:
                    neighbors = new int[] { 112, 105, 114, 121 };
                    break;
                case 114:
                    neighbors = new int[] { 113, 106, 115, 122 };
                    break;
                case 115:
                    neighbors = new int[] { 114, 107, 116, 123 };
                    break;
                case 116:
                    neighbors = new int[] { 115, 108, 117, 124 };
                    break;
                case 117:
                    neighbors = new int[] { 116, 109, 118, 125 };
                    break;
                case 118:
                    neighbors = new int[] { 117, 110, 126 };
                    break;
                case 120:
                    neighbors = new int[] { 31, 112, 121, 128 };
                    break;
                case 121:
                    neighbors = new int[] { 113, 120, 122, 129 };
                    break;
                case 122:
                    neighbors = new int[] { 114, 121, 123, 130 };
                    break;
                case 123:
                    neighbors = new int[] { 115, 122, 124, 131 };
                    break;
                case 124:
                    neighbors = new int[] { 116, 123, 125, 132 };
                    break;
                case 125:
                    neighbors = new int[] { 117, 124, 126, 133 };
                    break;
                case 126:
                    neighbors = new int[] { 118, 125, 134 };
                    break;
                case 128:
                    neighbors = new int[] { 39, 120, 129, 136 };
                    break;
                case 129:
                    neighbors = new int[] { 128, 121, 130, 137 };
                    break;
                case 130:
                    neighbors = new int[] { 129, 122, 131, 138 };
                    break;
                case 131:
                    neighbors = new int[] { 130, 123, 132, 139 };
                    break;
                case 132:
                    neighbors = new int[] { 131, 124, 133, 140 };
                    break;
                case 133:
                    neighbors = new int[] { 132, 125, 134, 141 };
                    break;
                case 134:
                    neighbors = new int[] { 133, 126, 142 };
                    break;
                case 136:
                    neighbors = new int[] { 47, 128, 137, 144 };
                    break;
                case 137:
                    neighbors = new int[] { 136, 129, 138, 145 };
                    break;
                case 138:
                    neighbors = new int[] { 137, 130, 139, 146 };
                    break;
                case 139:
                    neighbors = new int[] { 138, 131, 140, 147 };
                    break;
                case 140:
                    neighbors = new int[] { 139, 132, 141, 148 };
                    break;
                case 141:
                    neighbors = new int[] { 140, 133, 142, 149 };
                    break;
                case 142:
                    neighbors = new int[] { 141, 134, 150 };
                    break;
                case 144:
                    neighbors = new int[] { 55, 136, 145, 152 };
                    break;
                case 145:
                    neighbors = new int[] { 144, 137, 146, 153 };
                    break;
                case 146:
                    neighbors = new int[] { 145, 138, 147, 154 };
                    break;
                case 147:
                    neighbors = new int[] { 146, 139, 148, 155 };
                    break;
                case 148:
                    neighbors = new int[] { 147, 140, 149, 156 };
                    break;
                case 149:
                    neighbors = new int[] { 148, 141, 50, 157 };
                    break;
                case 150:
                    neighbors = new int[] { 149, 142, 158 };
                    break;
                case 152:
                    neighbors = new int[] { 63, 144, 153, 160 };
                    break;
                case 153:
                    neighbors = new int[] { 152, 145, 154, 161 };
                    break;
                case 154:
                    neighbors = new int[] { 153, 146, 155, 162 };
                    break;
                case 155:
                    neighbors = new int[] { 154, 147, 156, 163 };
                    break;
                case 156:
                    neighbors = new int[] { 155, 148, 157, 164 };
                    break;
                case 157:
                    neighbors = new int[] { 156, 149, 158, 165 };
                    break;
                case 158:
                    neighbors = new int[] { 157, 150, 166 };
                    break;
                case 160:
                    neighbors = new int[] { 71, 152, 161, 168 };
                    break;
                case 161:
                    neighbors = new int[] { 160, 153, 162, 169 };
                    break;
                case 162:
                    neighbors = new int[] { 161, 154, 163, 170 };
                    break;
                case 163:
                    neighbors = new int[] { 162, 155, 164, 171 };
                    break;
                case 164:
                    neighbors = new int[] { 163, 156, 165, 172 };
                    break;
                case 165:
                    neighbors = new int[] { 164, 157, 166, 173 };
                    break;
                case 166:
                    neighbors = new int[] { 165, 158, 174 };
                    break;
                case 168:
                    neighbors = new int[] { 79, 160, 169, 176 };
                    break;
                case 169:
                    neighbors = new int[] { 168, 161, 170, 177 };
                    break;
                case 170:
                    neighbors = new int[] { 169, 162, 171, 178 };
                    break;
                case 171:
                    neighbors = new int[] { 170, 163, 172, 179 };
                    break;
                case 172:
                    neighbors = new int[] { 171, 164, 173, 180 };
                    break;
                case 173:
                    neighbors = new int[] { 172, 165, 174, 181 };
                    break;
                case 174:
                    neighbors = new int[] { 173, 166, 182 };
                    break;
                case 176:
                    neighbors = new int[] { 87, 168, 177 };
                    break;
                case 177:
                    neighbors = new int[] { 176, 169, 178 };
                    break;
                case 178:
                    neighbors = new int[] { 177, 170, 179 };
                    break;
                case 179:
                    neighbors = new int[] { 178, 171, 180 };
                    break;
                case 180:
                    neighbors = new int[] { 179, 172, 181 };
                    break;
                case 181:
                    neighbors = new int[] { 180, 173, 182 };
                    break;
                case 182:
                    neighbors = new int[] { 181, 174 };
                    break;
                default:
                    neighbors = new int[] { -1 };
                    break;
            }
            return neighbors;
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
            int width = Texture.Width / Columns;
            int height = Texture.Height;
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
            if (PStlye == PlayerStyle.Human)
            {                
                if (this.Status != "dead")
                {
                    this.Status = "idle";                    
                    if(!cwrap.IsKeyDown(PlayerActions.Bomb))
                    {
                        KeyControl = "";
                    }
                    
                    if (cwrap.IsKeyDown(PlayerActions.Bomb) && KeyControl != "bomb")
                    {
                        if (this.BombMan.KickingBomb(this))
                        {
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
                        directionX = 1;
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Left))
                    {
                        this.Status = "walking";
                        directionX = -1;
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Down))
                    {
                        this.Status = "walking";
                        directionY = 1;
                    }
                    
                    if(cwrap.IsKeyDown(PlayerActions.Up))
                    {
                        this.Status = "walking";
                        directionY = -1;
                    }

                    if(!cwrap.IsKeyDown(PlayerActions.Right) &&
                        !cwrap.IsKeyDown(PlayerActions.Left))
                    {                        
                        directionX = 0;
                    }

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

        private void placeBomb(bool attacking = false)
        {
            if (this.BombCount < this.BombMax)
            {
                BombMan.SpawnBomb(Location, InstanceName);
                this.BombCount++;
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
