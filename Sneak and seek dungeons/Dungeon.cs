using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Sneak_and_seek_dungeons.Components;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sneak_and_seek_dungeons
{
    /// <summary>
    /// De forskellige former for rum
    /// </summary>
    enum ROOMTYPE {unknown, chamber, passage, passageExitAtEnd }

    enum DUNGEONTYPE {deathTrap, lair, maze, mine, portal, stronghold, temple, tomb, treasureVault}

    enum DEATHTRAPROOMS {antechamber, guardRoom, vault, puzzleRoom, trap, spectateRoom}

    enum LAIRROOMS {armory, audienceChamber, banquetRoom, barracks, leaderBedRoom, chapel, cistern, well, guardRoom, kennel, kitchen, prison, storage, throneRoom, tortureChamber, trainingRoom, trophyRoom, latrine, bath, workshop }

    enum MAZEROOMS {conjureRoom, guardRoom, beastLair, prison, shrine, storage, trap, well, workshop }

    enum MINEROOMS {minersBarracks, supervisorBedRoom, chapel, cistern, guardRoom, kitchen, labratory, lode, supervisorsOffice, smithy, storage, vault}

    enum PORTALROOMS {antechamber, armory, audienceChamber, barracks, bedroom, chapel, cistern, classRoom, conjuringRoom, crypt, diningRoom, divinationRoom, dormitory, entryRoom, gallery, guardRoom, kitchen, labratory, library, prison, portalRoom, storage, vault, study, tortureChamber, latrine, bath, workshop}

    enum strongHoldRooms {antechamber, armory, audienceChamber, aviary, banquetRoom, barracks, bath, bedroom, chapel, cistern, diningRoom, dressingRoom, gallery, gameRoom, guardRoom, kennel, kitchen, library, lounge, pantry, sittingRoom, stable, storage, vault, study, throneRoom, waitingRoom, latrine, crypt}

    enum templeRooms {armory, audienceChamber, banquetRoom, barracks, cells, centralTemple, chapel, classRoom, conjuringRoom, crypt, largeDiningRoom, smallDiningRoom, divinationRoom, dormitory, guardRoom, kennel, kitchen, library, prison, robingRoom, stable, storage, vault, tortureChamber, trophyRoom, latrine, bath, well, workshop }

    enum tombRooms {anteChamber, chapel, crypt, divinationRoom, falseCrypt, gallery, grandCrypt, guardRoom, robingRoom, storage, tomb, workshop}

    enum treasureVaultRooms {anteChamber, armory, barracks, cistern, guardRoom, kennel, kitchen, watchRoom, prison, vault, tortureChamber, trap }

    
    /// <summary>
    /// Klasse for dungeon
    /// </summary>
    public class Dungeon
    {
        /// <summary>
        /// Sprite der bruges til gulvet
        /// </summary>
        Texture2D floorSprite = GameWorld.Instance.Content.Load<Texture2D>("newFloor");

        /// <summary>
        /// Sprite til debug tegning af rum
        /// </summary>
        private Texture2D texture = GameWorld.Instance.Content.Load<Texture2D>("Wall");

        /// <summary>
        /// Punktet hvor spilleren spawner ind i dungeonen
        /// </summary>
        public Vector2 spawnPoint;

        /// <summary>
        /// En liste med alle rum i dungeonen
        /// </summary>
        List<Rectangle> rooms = new List<Rectangle>();

        DUNGEONTYPE dungeonType;

        Dictionary<Rectangle, LAIRROOMS> lairRooms = new Dictionary<Rectangle, LAIRROOMS>();
        Dictionary<Rectangle, DEATHTRAPROOMS> deathTrapRooms = new Dictionary<Rectangle, DEATHTRAPROOMS>();
        Dictionary<Rectangle, MAZEROOMS> mazeRooms = new Dictionary<Rectangle, MAZEROOMS>();


        /// <summary>
        /// En liste med alle sektioner af passager i dungeonen
        /// </summary>
        List<Rectangle> passageSections = new List<Rectangle>();

        /// <summary>
        /// De felter hvor veje ud af et rum placeres
        /// </summary>
        Queue<GridField> exitFields = new Queue<GridField>();

        /// <summary>
        /// Rumtyper på den anden side af exitFields
        /// </summary>
        Queue<ROOMTYPE> exits = new Queue<ROOMTYPE>();

        /// <summary>
        /// Felter ved udgange ud af dungeonen
        /// </summary>
        List<GridField> dungeonExits = new List<GridField>();

        /// <summary>
        /// Liste over døre
        /// </summary>
        Dictionary<GridField, List<Vector2>> doors = new Dictionary<GridField, List<Vector2>>();

        Dictionary<GridField, List<Vector2>> secretDoors = new Dictionary<GridField, List<Vector2>>();

        Dictionary<GridField, List<Vector2>> lockedDoors = new Dictionary<GridField, List<Vector2>>();

        Dictionary<Item, List<Vector2>> keys = new Dictionary<Item, List<Vector2>>();

        /// <summary>
        /// ekstremværdierne for gridfields placering
        /// </summary>
        int minX;
        int minY;
        int maxX;
        int maxY;

        /// <summary>
        /// Det maksimalle antal fjender der kan være i en dungeon
        /// </summary>
        int maxEnemies = 100;

        int maxContainers = 50;
        /// <summary>
        /// Property til oversigten af rum i dungeonen
        /// </summary>
        public List<Rectangle> Rooms { get => rooms; set => rooms = value; }


        /// <summary>
        /// Properties for ekstremerne i dungeonen
        /// </summary>
        public int MinX { get => minX; set => minX = value; }
        public int MinY { get => minY; set => minY = value; }
        public int MaxX { get => maxX; set => maxX = value; }
        public int MaxY { get => maxY; set => maxY = value; }

        /// <summary>
        /// Property for passagesektionerne
        /// </summary>
        public List<Rectangle> PassageSections { get => passageSections; set => passageSections = value; }
        internal Dictionary<GridField, List<Vector2>> Doors { get => doors;}

        private List<LootChest> containers = new List<LootChest>();

        private Dictionary<GridField, List<Vector2>> triggerDoors = new Dictionary<GridField, List<Vector2>>();
        private Dictionary<Trigger, Vector2> triggers = new Dictionary<Trigger, Vector2>();

        /// <summary>
        /// Primær metode til at lave dungeonen. Laver det første rum og sætter proccesen til at lave de resterende i gang. Derefter retter den diverse fejl og endelig sætter den produktionen af vægge igang
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void CreateDungeon() {
            Random rnd = new Random();
            int roomType = rnd.Next(10);
            Array values = Enum.GetValues(typeof(DUNGEONTYPE));
            dungeonType = (DUNGEONTYPE)values.GetValue(rnd.Next(values.Length));
            if (dungeonType != DUNGEONTYPE.deathTrap || dungeonType != DUNGEONTYPE.lair || dungeonType != DUNGEONTYPE.maze) {
                dungeonType = DUNGEONTYPE.deathTrap;
            }
            int roomHeight = 5;
            int roomWidth = 10;
            bool exitTop = false;
            bool exitBot = false;
            bool exitLef = false;
            bool exitRig = false;
            int entrancePos = 0;
            bool tIntersection = false;
            switch (roomType) {
                case (0):
                    roomHeight = 4;
                    roomWidth = 4;
                    exitTop = true;
                    exitBot = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    entrancePos = 4;
                    break;
                case (1):
                    roomHeight = 4;
                    roomWidth = 4;
                    exitTop = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.unknown);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    entrancePos = 3;
                    break;
                case (2):
                    exitTop = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.unknown);
                    exits.Enqueue(ROOMTYPE.unknown);
                    exits.Enqueue(ROOMTYPE.unknown);
                    entrancePos = 3;
                    roomHeight = 8;
                    roomWidth = 8;
                    break;
                case (3):
                    exitTop = true;
                    exitBot = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.unknown);
                    exits.Enqueue(ROOMTYPE.unknown);
                    entrancePos = 4;
                    roomHeight = 4;
                    roomWidth = 16;
                    break;
                case (4):
                    exitTop = true;
                    exitBot = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    entrancePos = 4;
                    roomHeight = 4;
                    roomWidth = 8;
                    break;
                case (7):
                    exitTop = true;
                    exitBot = true;
                    exitLef = true;
                    exitRig = true;
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.unknown);
                    exits.Enqueue(ROOMTYPE.unknown);
                    entrancePos = 4;
                    roomHeight = 4;
                    roomWidth = 4;
                    break;
                case (8):
                    tIntersection = true;
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exitLef = true;
                    exitRig = true;
                    break;
                case (5):
                case (6):
                case (9):
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exits.Enqueue(ROOMTYPE.passage);
                    exitTop = true;
                    exitBot = true;
                    exitLef = true;
                    exitRig = true;
                    entrancePos = 4;
                    break;
            }
            int entrance = rnd.Next(entrancePos + 1);
            int gridSize = GameWorld.Instance.GridSize;
            Vector2 startPoint = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2 - (roomWidth / 2) * gridSize, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 2 - (roomHeight / 2) * gridSize);
            minX = (int)startPoint.X;
            minY = (int)startPoint.Y;
            maxX = minX + roomWidth * gridSize;
            maxY = minY + roomHeight * gridSize;
            Rectangle room = new Rectangle((int)startPoint.X, (int)startPoint.Y, roomWidth * gridSize, roomHeight * gridSize);
            Rooms.Add(room);
            spawnPoint = room.Center.ToVector2();
            for (int i = (int)startPoint.X; i < (int)startPoint.X + roomWidth * gridSize; i += gridSize) {
                for (int j = (int)startPoint.Y; j < (int)startPoint.Y + roomHeight * gridSize; j += gridSize)
                {
                    GridField gF = new GridField(new Rectangle(i, j, gridSize, gridSize), true, true, true, true);
                    if (i == (int)startPoint.X) {
                        gF.enterExitLef = false;
                        if (exitLef && j == startPoint.Y + (roomHeight / 2) * gridSize) {
                            gF.enterExitLef = true;
                            if (!exitFields.Contains(gF)) {
                                exitFields.Enqueue(gF);
                            }
                            gF.containsPassage = true;
                            
                        }
                    }
                    if (i == (int)startPoint.X + roomWidth * gridSize - gridSize)
                    {
                        gF.enterExitRig = false;
                        if (exitRig && j == startPoint.Y + (roomHeight / 2) * gridSize)
                        {
                            gF.enterExitRig = true;
                            gF.containsPassage = true;
                            if (!exitFields.Contains(gF))
                            {
                                exitFields.Enqueue(gF);
                            }
                        }
                    }
                    if (j == (int)startPoint.Y)
                    {
                        gF.enterExitTop = false;
                        if (exitTop && i == startPoint.X + (roomWidth / 2) * gridSize)
                        {
                            gF.enterExitTop = true;
                            gF.containsPassage = true;
                            if (!exitFields.Contains(gF))
                            {
                                exitFields.Enqueue(gF);
                            }
                        }
                    }
                    if (j == (int)startPoint.Y + roomHeight * gridSize - gridSize)
                    {
                        gF.enterExitBot = false;
                        gF.containsPassage = true;
                        if (exitBot && i == startPoint.X + (roomWidth / 2) * gridSize)
                        {
                            gF.enterExitBot = true;
                            if (!exitFields.Contains(gF))
                            {
                                exitFields.Enqueue(gF);
                            }
                        }
                    }
                    GameWorld.Instance.Grid.Add(new Vector2(i, j), gF);
                }
            }
            while (exits.Count > 0)
            {
                ROOMTYPE currentRoom = exits.Dequeue();
                if (currentRoom == ROOMTYPE.unknown)
                {
                    int res = rnd.Next(19);
                    if (rooms.Count > 10) {
                        res += rooms.Count - 10;
                    }
                    switch (res)
                    {
                        case (0):
                        case (1):
                            CreateRoom(gridSize, ROOMTYPE.passage, true);
                            break;
                        case (2):
                        case (3):
                        case (4):
                        case (5):
                        case (6):
                        case (7):
                            CreateSpecificPassage(gridSize, 4, ROOMTYPE.passage, true);
                            break;
                        case (8):
                        case (9):
                        case (10):
                        case (11):
                        case (12):
                        case (13):
                        case (14):
                        case (15):
                        case (16):
                        case (17):
                            CreateRoom(gridSize, ROOMTYPE.chamber, true);
                            break;
                        case (18):
                            exitFields.Dequeue();
                            break;
                        default:
                            if (dungeonExits.Count < 4) {
                                rnd.Next(20);
                                res += rooms.Count - 10;
                                GridField gF = exitFields.Dequeue();
                                if (res > 15 && exitCheck(gF, gridSize)) {
                                    dungeonExits.Add(gF);
                                }
                            }
                            break;
                        
                            //break;
                    }
                }
                else if (currentRoom == ROOMTYPE.passage)
                {
                    CreateRoom(gridSize, ROOMTYPE.passage, false);
                }
                else if (currentRoom == ROOMTYPE.passageExitAtEnd)
                {
                    CreateRoom(gridSize, ROOMTYPE.passage, false);
                }
                else if (currentRoom == ROOMTYPE.chamber)
                {
                    CreateRoom(gridSize, ROOMTYPE.chamber, false);
                }
            }
            List<GridField> newGridFields = new List<GridField>();
            List<Vector2> validGridField = new List<Vector2>();
            for (int i = minX - gridSize; i <= maxX + gridSize; i += gridSize) {
                for (int j = minY - gridSize; j <= maxY + gridSize; j += gridSize)
                {
                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j)))
                    {
                        GridField gF = new GridField(new Rectangle(i, j, gridSize, gridSize), false, false, false, false);
                        gF.emptySpace = true;
                        GameWorld.Instance.Grid.Add(new Vector2(i, j), gF);
                        newGridFields.Add(gF);
                    }
                    else {
                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) || newGridFields.Contains(GameWorld.Instance.Grid[new Vector2(i - gridSize, j)]))
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitLef = false;
                        }
                        else if (GameWorld.Instance.Grid[new Vector2(i - gridSize, j)].enterExitRig || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitLef) {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitLef = true;
                        }
                        else
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig = false;
                        }
                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) || newGridFields.Contains(GameWorld.Instance.Grid[new Vector2(i + gridSize, j)]))
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig = false;
                        }
                        else if (GameWorld.Instance.Grid[new Vector2(i + gridSize, j)].enterExitLef || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig)
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig = true;
                        }
                        else {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig = false;
                        }
                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) || newGridFields.Contains(GameWorld.Instance.Grid[new Vector2(i, j - gridSize)]))
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop = false;
                        }
                        else if (GameWorld.Instance.Grid[new Vector2(i, j - gridSize)].enterExitBot || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop)
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop = true;
                        }
                        else
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop = false;
                        }
                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) || newGridFields.Contains(GameWorld.Instance.Grid[new Vector2(i, j + gridSize)]))
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot = false;
                        }
                        else if (GameWorld.Instance.Grid[new Vector2(i, j + gridSize)].enterExitTop || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot)
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot = true;
                        }
                        else
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot = false;
                        }
                    }
                    validGridField.Add(new Vector2(i, j));
                }
            }
            if (newGridFields.Count == validGridField.Count)
            {
                GameWorld.Instance.Grid.Clear();
                GameWorld.Instance.GameObjects.Clear();
                GameWorld.Instance.Colliders.Clear();
                GameWorld.Instance.DungeonColliders.Clear();
                rooms.Clear();
                passageSections.Clear();
                dungeonExits.Clear();
                exitFields.Clear();
                CreateDungeon();
            }
            else {
                List<Vector2> removedGridFields = new List<Vector2>();
                foreach (Vector2 gF in GameWorld.Instance.Grid.Keys)
                {
                    if (!validGridField.Contains(gF))
                    {
                        removedGridFields.Add(gF);
                    }
                }
                foreach (Vector2 gF in removedGridFields)
                {
                    if (Doors.ContainsKey(GameWorld.Instance.Grid[gF]))
                    {
                        Doors.Remove(GameWorld.Instance.Grid[gF]);
                    }
                    GameWorld.Instance.Grid.Remove(gF);

                }
                List<Rectangle> validR = new List<Rectangle>();

                int newMinX = int.MaxValue;
                int newMinY = int.MaxValue;
                int newMaxX = int.MinValue;
                int newMaxY = int.MinValue;
                foreach (Vector2 v in GameWorld.Instance.Grid.Keys)
                {
                    if (v.X < newMinX)
                    {
                        newMinX = (int)v.X;
                    }
                    if (v.Y < newMinY)
                    {
                        newMinY = (int)v.Y;
                    }
                    if (v.X > newMaxX)
                    {
                        newMaxX = (int)v.X;
                    }
                    if (v.Y > newMaxY)
                    {
                        newMaxY = (int)v.Y;
                    }
                }
                minX = newMinX;
                minY = newMinY;
                maxX = newMaxX;
                maxY = newMaxY;
                bool validSpawnPoint = false;
                foreach (GridField r in GameWorld.Instance.Grid.Values)
                {
                    if (r.position.Contains(spawnPoint) && (r.enterExitBot || r.enterExitLef || r.enterExitRig || r.enterExitTop))
                    {
                        validSpawnPoint = true;
                        break;
                    }
                }

                if (!validSpawnPoint)
                {
                    List<Rectangle> testedRooms = new List<Rectangle>();
                    while (!validSpawnPoint)
                    {
                        Rectangle cRoom = rooms[rnd.Next(rooms.Count)];
                        if (testedRooms.Count == rooms.Count - 1)
                        {
                            foreach (Rectangle r in rooms)
                            {
                                if (!testedRooms.Contains(r))
                                {
                                    cRoom = r;
                                    break;
                                }
                            }
                        }
                        while (testedRooms.Contains(cRoom) && testedRooms.Count < rooms.Count)
                        {
                            cRoom = rooms[rnd.Next(rooms.Count)];
                        }
                        if (!testedRooms.Contains(cRoom))
                        {
                            testedRooms.Add(cRoom);
                        }
                        spawnPoint = cRoom.Center.ToVector2();
                        foreach (GridField r in GameWorld.Instance.Grid.Values)
                        {
                            if ((r.position.Contains(spawnPoint)) && !r.emptySpace)
                            {
                                validSpawnPoint = true;
                                break;
                            }
                            else if (testedRooms.Count >= rooms.Count && !r.emptySpace)
                            {
                                spawnPoint = r.position.Center.ToVector2();
                                validSpawnPoint = true;
                                break;
                            }
                        }
                    }
                }
                List<Rectangle> validRooms = new List<Rectangle>();
                List<Rectangle> invalidRooms = new List<Rectangle>();
                foreach (Rectangle r in rooms)
                {
                    if (GameWorld.Instance.Pathfinding(spawnPoint, r.Center.ToVector2()).Count > 0)
                    {
                        validRooms.Add(r);
                    }
                    else if ((r.X - minX) % gridSize == 0 && (r.Y - minY) % gridSize == 0)
                    {
                        invalidRooms.Add(r);
                    }
                }
                if (invalidRooms.Count > 0)
                {
                    while (invalidRooms.Count > 0)
                    {
                        Rectangle closestsValidRoom = validRooms[0];
                        Rectangle closestsInvalidRoom = invalidRooms[0];
                        float shortestDistance = Vector2.Distance(closestsValidRoom.Center.ToVector2(), closestsInvalidRoom.Center.ToVector2());
                        foreach (Rectangle iR in invalidRooms)
                        {
                            foreach (Rectangle vR in validRooms)
                            {

                                if (Vector2.Distance(iR.Center.ToVector2(), vR.Center.ToVector2()) < shortestDistance)
                                {
                                    shortestDistance = Vector2.Distance(iR.Center.ToVector2(), vR.Center.ToVector2());
                                    closestsValidRoom = vR;
                                    closestsInvalidRoom = iR;
                                }
                            }
                        }
                        int lowX = 0;
                        int lowY = 0;
                        if (closestsInvalidRoom.X < closestsValidRoom.X)
                        {
                            lowX = closestsInvalidRoom.X;
                        }
                        else
                        {
                            lowX = closestsValidRoom.X;
                        }
                        if (closestsInvalidRoom.Y < closestsValidRoom.Y)
                        {
                            lowY = closestsInvalidRoom.Y;
                        }
                        else
                        {
                            lowY = closestsValidRoom.Y;
                        }
                        int validXMiddle = ((closestsValidRoom.Width / gridSize) / 2) * gridSize;
                        int validYMiddle = ((closestsValidRoom.Height / gridSize) / 2) * gridSize;
                        List<GridField> vertical = new List<GridField>();
                        Dictionary<GridField, Vector2> verticalDir = new Dictionary<GridField, Vector2>();
                        if (lowY < closestsValidRoom.Y)
                        {
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y)))
                            {
                                verticalDir.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y)], new Vector2(0, -1));
                                vertical.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y)]);
                            }
                            else
                            {
                                GridField gF = new GridField(new Rectangle(int.MaxValue, int.MaxValue, 1, 1), false, false, false, false);
                                int shortestDist = int.MaxValue;
                                Vector2 pos = new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y);
                                foreach (GridField gFd in GameWorld.Instance.Grid.Values)
                                {
                                    if (Vector2.Distance(pos, new Vector2(gFd.position.X, gFd.position.Y)) < shortestDist || gFd.position.Contains(pos.X, pos.Y))
                                    {
                                        gF = gFd;
                                        if (gFd.position.Contains(pos.X, pos.Y))
                                        {
                                            break;
                                        }
                                    }
                                }
                                verticalDir.Add(gF, new Vector2(0, -1));
                                vertical.Add(gF);
                            }

                        }
                        else
                        {
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y + closestsValidRoom.Height)))
                            {
                                verticalDir.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y + closestsValidRoom.Height)], new Vector2(0, 1));
                                vertical.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y + closestsValidRoom.Height)]);
                            }
                            else {
                                GridField gF = new GridField(new Rectangle(int.MaxValue, int.MaxValue, 1, 1), false, false, false, false);
                                int shortestDist = int.MaxValue;
                                Vector2 pos = new Vector2(closestsValidRoom.X + validXMiddle, closestsValidRoom.Y);
                                foreach (GridField gFd in GameWorld.Instance.Grid.Values)
                                {
                                    if (Vector2.Distance(pos, new Vector2(gFd.position.X, gFd.position.Y)) < shortestDist || gFd.position.Contains(pos.X, pos.Y))
                                    {
                                        gF = gFd;
                                        if (gFd.position.Contains(pos.X, pos.Y))
                                        {
                                            break;
                                        }
                                    }
                                }
                                verticalDir.Add(gF, new Vector2(0, 1));
                                vertical.Add(gF);
                            }
                        }
                        Dictionary<GridField, Vector2> horizontalDir = new Dictionary<GridField, Vector2>();
                        List<GridField> horizontal = new List<GridField>();
                        if (lowX < closestsValidRoom.X && GameWorld.Instance.Grid.ContainsKey(new Vector2(closestsValidRoom.X, closestsValidRoom.Y + validYMiddle)))
                        {
                            horizontalDir.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X, closestsValidRoom.Y + validYMiddle)], new Vector2(-1, 0));
                            horizontal.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X, closestsValidRoom.Y + validYMiddle)]);
                        }
                        else
                        {
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(closestsValidRoom.X + closestsValidRoom.Width, closestsValidRoom.Y + validYMiddle)))
                            {
                                horizontalDir.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + closestsValidRoom.Width, closestsValidRoom.Y + validYMiddle)], new Vector2(1, 0));
                                horizontal.Add(GameWorld.Instance.Grid[new Vector2(closestsValidRoom.X + closestsValidRoom.Width, closestsValidRoom.Y + validYMiddle)]);
                            }
                            else
                            {
                                GridField gF = new GridField(new Rectangle(int.MaxValue, int.MaxValue, 1, 1), false, false, false, false);
                                int shortestDist = int.MaxValue;
                                Vector2 pos = new Vector2(closestsValidRoom.X + closestsValidRoom.Width, closestsValidRoom.Y + validYMiddle);
                                foreach (GridField gFd in GameWorld.Instance.Grid.Values)
                                {
                                    if (Vector2.Distance(pos, new Vector2(gFd.position.X, gFd.position.Y)) < shortestDist || gFd.position.Contains(pos.X, pos.Y))
                                    {
                                        gF = gFd;
                                        if (gFd.position.Contains(pos.X, pos.Y))
                                        {
                                            break;
                                        }
                                    }
                                }
                                horizontalDir.Add(gF, new Vector2(0, -1));
                                horizontal.Add(gF);
                            }
                        }

                        while (!closestsInvalidRoom.Intersects(vertical[vertical.Count - 1].position) && !closestsInvalidRoom.Intersects(horizontal[horizontal.Count - 1].position))
                        {
                            GridField latestVertical = vertical[vertical.Count - 1];
                            Vector2 newVertical = new Vector2(latestVertical.position.X + verticalDir[latestVertical].X * gridSize, latestVertical.position.Y + verticalDir[latestVertical].Y * gridSize);
                            if (GameWorld.Instance.Grid.ContainsKey(newVertical))
                            {
                                vertical.Add(GameWorld.Instance.Grid[newVertical]);
                                if (!closestsInvalidRoom.Contains(newVertical.X, newVertical.Y) && newVertical.Y > closestsInvalidRoom.Y && newVertical.Y < closestsInvalidRoom.Y + closestsInvalidRoom.Height)
                                {
                                    if (lowX < closestsValidRoom.X)
                                    {
                                        verticalDir.Add(vertical[vertical.Count - 1], new Vector2(-1, 0));
                                    }
                                    else
                                    {
                                        verticalDir.Add(vertical[vertical.Count - 1], new Vector2(1, 0));
                                    }
                                }
                                else
                                {
                                    verticalDir.Add(vertical[vertical.Count - 1], verticalDir[vertical[vertical.Count - 2]]);
                                }
                            }
                            GridField latestHorizontal = horizontal[horizontal.Count - 1];
                            Vector2 newHorizontal = new Vector2(latestHorizontal.position.X + horizontalDir[latestHorizontal].X * gridSize, latestHorizontal.position.Y + horizontalDir[latestHorizontal].Y * gridSize);
                            if (GameWorld.Instance.Grid.ContainsKey(newHorizontal))
                            {
                                horizontal.Add(GameWorld.Instance.Grid[newHorizontal]);
                                if (!closestsInvalidRoom.Contains(newHorizontal.X, newHorizontal.Y) && newHorizontal.X > closestsInvalidRoom.X && newHorizontal.X < closestsInvalidRoom.X + closestsInvalidRoom.Width)
                                {
                                    if (lowY < closestsValidRoom.Y)
                                    {
                                        horizontalDir.Add(horizontal[horizontal.Count - 1], new Vector2(0, -1));
                                    }
                                    else
                                    {
                                        horizontalDir.Add(horizontal[horizontal.Count - 1], new Vector2(0, 1));
                                    }
                                }
                                else
                                {
                                    horizontalDir.Add(horizontal[horizontal.Count - 1], horizontalDir[horizontal[horizontal.Count - 2]]);
                                }
                            }

                        }
                        List<GridField> tunnel = new List<GridField>();
                        Dictionary<GridField, Vector2> tunnelDir = new Dictionary<GridField, Vector2>();
                        if (closestsInvalidRoom.Intersects(horizontal[horizontal.Count - 1].position))
                        {
                            tunnel = horizontal;
                            tunnelDir = horizontalDir;
                        }
                        else
                        {
                            tunnel = vertical;
                            tunnelDir = verticalDir;
                        }
                        for (int i = 0; i < tunnel.Count; i++)
                        {
                            GridField current = tunnel[i];
                            if (i < tunnel.Count - 1)
                            {
                                Vector2 direction = tunnelDir[current];
                                if (direction.X == -1)
                                {
                                    current.enterExitLef = true;
                                    tunnel[i + 1].enterExitRig = true;
                                }
                                else if (direction.X == 0)
                                {
                                    if (direction.Y == -1)
                                    {
                                        current.enterExitTop = true;
                                        tunnel[i + 1].enterExitBot = true;
                                    }
                                    else
                                    {
                                        current.enterExitBot = true;
                                        tunnel[i + 1].enterExitTop = true;
                                    }
                                }
                                else
                                {
                                    current.enterExitRig = true;
                                    tunnel[i + 1].enterExitLef = true;
                                }
                            }
                        }
                        validRooms.Clear();
                        invalidRooms.Clear();
                        foreach (Rectangle r in rooms)
                        {
                            if (GameWorld.Instance.Pathfinding(spawnPoint, r.Center.ToVector2()).Count > 0)
                            {
                                validRooms.Add(r);
                            }
                            else if ((r.X - minX) % gridSize == 0 && (r.Y - minY) % gridSize == 0)
                            {
                                bool roomExists = false;
                                foreach (GridField gF in GameWorld.Instance.Grid.Values) {
                                    if (gF.position.Contains(r.Center.X, r.Center.Y)) {
                                        roomExists = true;
                                        break;
                                    }
                                }
                                if (roomExists) {
                                    invalidRooms.Add(r);
                                }
                                
                            }
                        }
                    }
                }
                int totalEnemies = 0;
                foreach (Rectangle r in rooms)
                {
                    if ((r.X - minX) % gridSize == 0 && (r.Y - minY) % gridSize == 0)
                    {
                        int roomSize = (r.Width * r.Height) / (gridSize * 2);
                        int nEnemies = rnd.Next(2) * (roomSize / 1000);
                        if (totalEnemies + nEnemies <= maxEnemies)
                        {
                            totalEnemies += nEnemies;
                        }
                        else
                        {
                            nEnemies = maxEnemies - totalEnemies;
                        }
                        List<Vector2> enemyPos = new List<Vector2>();
                        for (int i = 0; i < nEnemies; i++)
                        {
                            int rndEnemy = GameWorld.Instance.rnd.Next(1, 3);

                            GameObject enemy;
                            switch (rndEnemy)
                            {
                                case 1:
                                    enemy = EnemyFactory.Instance.Create(ENEMYTYPE.GRUNT);
                                    break;
                                case 2:
                                    enemy = EnemyFactory.Instance.Create(ENEMYTYPE.ARCHER);
                                    break;
                                default:
                                    throw new NotImplementedException("Default switch case. Tjek om rndEnemy giver den rigtige værdi indenfor de forskellige cases");
                                    
                            }
                            
                            Vector2 enemyPosition = new Vector2(rnd.Next(r.Width) + r.X, rnd.Next(r.Height) + r.Y);

                            bool enemyPosContainsPosition = false;
                            foreach (Vector2 pos in enemyPos)
                            {
                                if (pos == enemyPosition)
                                {
                                    enemyPosContainsPosition = true;
                                    break;
                                }
                            }
                            while (enemyPosContainsPosition)
                            {
                                enemyPosition = new Vector2(rnd.Next(r.Width) + r.X, rnd.Next(r.Height) + r.Y);

                                enemyPosContainsPosition = false;
                                foreach (Vector2 pos in enemyPos)
                                {
                                    if (pos == enemyPosition)
                                    {
                                        enemyPosContainsPosition = true;
                                        break;
                                    }
                                }
                            }
                            enemy.Transform.Position = enemyPosition;
                            enemyPos.Add(enemyPosition);
                            GameWorld.Instance.GameObjects.Add(enemy);
                        }
                    }
                }
                int totalContainers = 0;
                foreach (Rectangle r in rooms)
                {
                    if ((r.X - minX) % gridSize == 0 && (r.Y - minY) % gridSize == 0)
                    {
                        int numbContainers = rnd.Next(5);
                        if (totalContainers + numbContainers > maxContainers)
                        {
                            numbContainers = maxContainers - totalContainers;
                        }
                        totalContainers += numbContainers;
                        int placedContainers = 0;
                        int gridridFieldsInRoom = (r.Height / gridSize) * (r.Width / gridSize);
                        int checkedGridFields = 0;
                        for (int i = r.X; i < r.Width; i += gridSize)
                        {
                            for (int j = r.Y; j < r.Height; j += gridSize)
                            {
                                bool placeContainer = rnd.Next(gridridFieldsInRoom) + checkedGridFields >= gridridFieldsInRoom * 0.9f;
                                if (placeContainer && placedContainers < numbContainers && (GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitLef || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig || GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop))
                                {
                                    bool validPlacement = true;
                                    if (i == r.X && GameWorld.Instance.Grid[new Vector2(i, j)].enterExitLef)
                                    {
                                        validPlacement = false;
                                    }
                                    if (j == r.Y && GameWorld.Instance.Grid[new Vector2(i, j)].enterExitTop)
                                    {
                                        validPlacement = false;
                                    }
                                    if (i == r.X + r.Width - gridSize && GameWorld.Instance.Grid[new Vector2(i, j)].enterExitRig)
                                    {
                                        validPlacement = false;
                                    }
                                    if (j == r.Y + r.Height - gridSize && GameWorld.Instance.Grid[new Vector2(i, j)].enterExitBot)
                                    {
                                        validPlacement = false;
                                    }
                                    if (validPlacement)
                                    {
                                        GameObject container = ContainerFactory.Instance.Create(null);
                                        container.Transform.Position = new Vector2(i + gridSize / 2, j + gridSize / 2);
                                        GameWorld.Instance.GameObjects.Add(container);
                                        containers.Add((LootChest)container.GetComponent<LootChest>());
                                        placedContainers++;
                                    }

                                }
                                if (placedContainers == numbContainers)
                                {
                                    break;
                                }
                                checkedGridFields++;
                            }
                            if (placedContainers == numbContainers)
                            {
                                break;
                            }
                        }
                    }
                }
                if (dungeonExits.Count == 0)
                {
                    GridField exitField = null;
                    if (GameWorld.Instance.Grid.ContainsKey(new Vector2(minX, minY)))
                    {
                        exitField = GameWorld.Instance.Grid[new Vector2(minX, minY)];
                    }
                    else
                    {
                        for (int i = minY; i < maxY; i += gridSize)
                        {
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(minX, i)))
                            {
                                exitField = GameWorld.Instance.Grid[new Vector2(minX, i)];
                                break;
                            }
                        }
                    }
                }
                bool validExit = false;
                foreach (GridField exit in dungeonExits)
                {
                    List<Vector2> path = GameWorld.Instance.Pathfinding(startPoint, exit.position.Center.ToVector2());
                    if (path.Count > 0)
                    {
                        validExit = true;
                        break;
                    }
                }
                if (validExit)
                {

                }
                if (GameWorld.Instance.Grid.Count == 0)
                {
                    throw new Exception("No dungeon");
                }
                foreach (GridField g in lockedDoors.Keys) {
                    Vector2 pos = new Vector2(g.position.X, g.position.Y);

                    foreach (Vector2 v in lockedDoors[g]) {
                        if (v.X != 0)
                        {
                            if (v.X == 1)
                            {
                                
                                g.enterExitRig = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y))) {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X + gridSize, pos.Y)];
                                    neighbour.enterExitLef = false;
                                }
                                
                            }
                            else {
                                
                                g.enterExitLef = false;
                                
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X - gridSize, pos.Y)];
                                    neighbour.enterExitRig = false;
                                }
                            }
                        }
                        else {
                            if (v.Y == 1)
                            {
                                
                                g.enterExitBot = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y + gridSize)];
                                    neighbour.enterExitTop = false;
                                }
                            }
                            else
                            {
                                g.enterExitTop = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y - gridSize)];
                                    neighbour.enterExitBot = false;
                                }
                            }
                        }
                        bool keyPlaced = false;
                        int keyType = rnd.Next(2);
                        if (containers.Count > 0 && keyType == 1)
                        {
                            List<LootChest> attemptedContainers = new List<LootChest>();
                            while (!keyPlaced && attemptedContainers.Count < containers.Count)
                            {
                                int index = rnd.Next(containers.Count);
                                while (attemptedContainers.Contains(containers[index])) {
                                    index = rnd.Next(containers.Count);
                                }
                                Vector2 location = containers[index].GameObject.Transform.Position;
                                if (GameWorld.Instance.Pathfinding(spawnPoint, location).Count > 0)
                                {
                                    keyPlaced = true;
                                    GameObject key = ItemFactory.Instance.Create(ITEMTYPE.KEY);
                                    containers[index].Items.Add((Item) key.GetComponent<Item>());
                                    
                                    keys.Add((Item)key.GetComponent<Item>(), new List<Vector2>() { g.position.Center.ToVector2(), v });
                                }
                                else {
                                    attemptedContainers.Add(containers[index]);
                                }
                            }
                        }
                        if(!keyPlaced) {
                            while (!keyPlaced)
                            {
                                bool containedInRoom = false;
                                List<Rectangle> blockedRooms = new List<Rectangle>();
                                int validMinX = int.MaxValue;
                                int validMinY = int.MaxValue;
                                int validMaxX = int.MinValue;
                                int validMaxY = int.MinValue;
                                foreach (Rectangle r in Rooms) {
                                    if (GameWorld.Instance.Pathfinding(r.Center.ToVector2(), spawnPoint).Count == 0)
                                    {
                                        blockedRooms.Add(r);
                                    }
                                    else {
                                        if (r.X < validMinX) {
                                            validMinX = r.X;
                                        }
                                        if (r.Y < validMinY) {
                                            validMinY = r.Y;
                                        }
                                        if (r.X + r.Width > validMaxX) {
                                            validMaxX = r.X + r.Width;
                                        }
                                        if (r.Y + r.Height > validMaxY)
                                        {
                                            validMaxY = r.Y + r.Height;
                                        }
                                    }
                                }
                                Vector2 position = new Vector2(rnd.Next(validMinX, validMaxX), rnd.Next(validMinX, validMaxX));
                                while (!containedInRoom) {
                                    foreach (Rectangle r in Rooms) {
                                        if (r.Contains(position.X, (int)position.Y) && !blockedRooms.Contains(r)) {
                                            containedInRoom = true;
                                            break;
                                        }
                                    }
                                    if (!containedInRoom) {
                                        position = new Vector2(rnd.Next(validMinX, validMaxX), rnd.Next(validMinX, validMaxX));
                                    }
                                }
                                int attempts = 1;
                                while (GameWorld.Instance.Pathfinding(spawnPoint, position).Count == 0) {
                                    position = new Vector2(rnd.Next(validMinX, validMaxX), rnd.Next(validMinX, validMaxX));
                                    attempts++;
                                    containedInRoom = false;
                                    while (!containedInRoom)
                                    {
                                        foreach (Rectangle r in Rooms)
                                        {
                                            if (r.Contains(position.X, (int)position.Y) && !blockedRooms.Contains(r))
                                            {
                                                containedInRoom = true;
                                                break;
                                            }
                                        }
                                        if (!containedInRoom)
                                        {
                                            position = new Vector2(rnd.Next(validMinX, validMaxX), rnd.Next(validMinX, validMaxX));
                                        }
                                    }
                                }
                                foreach (GridField gF in GameWorld.Instance.Grid.Values) {
                                    if (gF.position.Contains(position.X, position.Y)) {
                                        if (keyType == 1)
                                        {
                                            GameObject container = ContainerFactory.Instance.Create(null);
                                            container.Transform.Position = gF.position.Center.ToVector2();
                                            GameWorld.Instance.GameObjects.Add(container);
                                            GameObject key = ItemFactory.Instance.Create(ITEMTYPE.KEY);
                                            LootChest chest = (LootChest)container.GetComponent<LootChest>();
                                            chest.Items.Add((Item)key.GetComponent<Item>());
                                            keys.Add((Item)key.GetComponent<Item>(), new List<Vector2>() { g.position.Center.ToVector2(), v });
                                            keyPlaced = true;
                                        }
                                        else {
                                            GameObject trigger = TriggerFactory.Instance.Create(null);
                                            trigger.Transform.Position = new Vector2(gF.position.X + gF.position.Width / 2, gF.position.Y + gF.position.Height / 2);
                                            Trigger t = (Trigger)trigger.GetComponent<Trigger>();
                                            t.TriggerType = TRIGGERTYPE.INTERACTABLE;
                                            t.ObjectType = CONNECTEDOBJECT.DOOR;
                                            SpriteRenderer sr = (SpriteRenderer)trigger.GetComponent<SpriteRenderer>();
                                            sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("PlaceHolder sprites/placeholderItem");
                                            GameWorld.Instance.GameObjects.Add(trigger);
                                            triggers.Add(t, trigger.Transform.Position);
                                            if (!triggerDoors.ContainsKey(g))
                                            {
                                                triggerDoors.Add(g, new List<Vector2>() { v });

                                            }
                                            else {
                                                triggerDoors[g].Add(v);
                                            }
                                            keyPlaced = true;
                                        }

                                        //containers.Add((LootChest)container.GetComponent<LootChest>());
                                        break;
                                    }
                                }
                            }
                        }
                        if (v.X != 0)
                        {
                            if (v.X == 1)
                            {

                                g.enterExitRig = true;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X + gridSize, pos.Y)];
                                    neighbour.enterExitLef = true;
                                }

                            }
                            else
                            {

                                g.enterExitLef = true;

                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X - gridSize, pos.Y)];
                                    neighbour.enterExitRig = true;
                                }
                            }
                        }
                        else
                        {
                            if (v.Y == 1)
                            {

                                g.enterExitBot = true;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y + gridSize)];
                                    neighbour.enterExitTop = true;
                                }
                            }
                            else
                            {
                                g.enterExitTop = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y - gridSize)];
                                    neighbour.enterExitBot = true;
                                }
                            }
                        }
                    }
                }
                foreach (GridField g in lockedDoors.Keys)
                {
                    Vector2 pos = new Vector2(g.position.X, g.position.Y);

                    foreach (Vector2 v in lockedDoors[g])
                    {
                        if (v.X != 0)
                        {
                            if (v.X == 1)
                            {

                                g.enterExitRig = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X + gridSize, pos.Y)];
                                    neighbour.enterExitLef = false;
                                }

                            }
                            else
                            {

                                g.enterExitLef = false;

                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X - gridSize, pos.Y)];
                                    neighbour.enterExitRig = false;
                                }
                            }
                        }
                        else
                        {
                            if (v.Y == 1)
                            {

                                g.enterExitBot = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y + gridSize)];
                                    neighbour.enterExitTop = false;
                                }
                            }
                            else
                            {
                                g.enterExitTop = false;
                                if (GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
                                {
                                    GridField neighbour = GameWorld.Instance.Grid[new Vector2(pos.X, pos.Y - gridSize)];
                                    neighbour.enterExitBot = false;
                                }
                            }
                        }
                    }
                }
                CreateWallsAndDoors();  
            }
        }
        /// <summary>
        /// Laver vægge efter gridfields er blevet sat op
        /// </summary>
        private void CreateWallsAndDoors()
        {
            List<Vector2> placedObjects = new List<Vector2>();
            foreach (GridField g in GameWorld.Instance.Grid.Values)
            {
                bool ignore = false;
                if (!g.enterExitBot && !g.enterExitLef && !g.enterExitRig && !g.enterExitTop)
                {
                    ignore = true;
                }
                if (!ignore && Doors.ContainsKey(g))
                {
                    foreach (Vector2 v in Doors[g])
                    {
                        GameObject door = new GameObject();
                        Texture2D[] wallArray = new Texture2D[] { GameWorld.Instance.Content.Load<Texture2D>("wall_vertical"), GameWorld.Instance.Content.Load<Texture2D>("wall") };
                        bool secretDoor = false;
                        bool lockedDoor = false;
                        if (v.X != 0)
                        {
                            door = DoorFactory.Instance.Create(DOORDIRECTION.vertical);
                            
                            if (lockedDoors.ContainsKey(g) && v.X != 0)
                            {
                                lockedDoor = true;
                            }
                            if (secretDoors.ContainsKey(g) && v.X != 0)
                            {
                                wallArray = new Texture2D[] { GameWorld.Instance.Content.Load<Texture2D>("wall_vertical"), GameWorld.Instance.Content.Load<Texture2D>("opening_vertical") };
                                SpriteRenderer sr = (SpriteRenderer)door.GetComponent<SpriteRenderer>();
                                sr.Sprite = wallArray[0];
                                sr.Color = Color.Blue;
                                secretDoor = true;
                            }
                            if (v.X == -1)
                            {
                                door.Transform.Position = new Vector2(g.position.X, g.position.Y + g.position.Height / 2); 
                            }
                            else
                            {
                                door.Transform.Position = new Vector2(g.position.X + g.position.Width, g.position.Y + g.position.Height / 2);
                            }
                        }
                        else
                        {
                            door = DoorFactory.Instance.Create(DOORDIRECTION.horizontal);
                            if (lockedDoors.ContainsKey(g) && v.Y != 0)
                            {
                                lockedDoor = true;
                            }
                            if (secretDoors.ContainsKey(g) && v.Y != 0)
                            {
                                wallArray = new Texture2D[] { GameWorld.Instance.Content.Load<Texture2D>("wall"), GameWorld.Instance.Content.Load<Texture2D>("opening") };
                                SpriteRenderer sr = (SpriteRenderer)door.GetComponent<SpriteRenderer>();
                                sr.Sprite = wallArray[0];
                                sr.Color = Color.Blue;
                                secretDoor = true;
                            }
                            
                            if (v.Y == -1)
                            {
                                door.Transform.Position = new Vector2(g.position.X + g.position.Width / 2, g.position.Y);
                            }
                            else
                            {
                                door.Transform.Position = new Vector2(g.position.X + g.position.Width / 2, g.position.Y + g.position.Height);
                            }
                        }
                        placedObjects.Add(door.Transform.Position);
                        Door d = (Door)door.GetComponent<Door>();
                        if (secretDoor)
                        {
                            d.Sprites = wallArray;
                        }
                        d.Locked = false;
                        d.Key = null;
                        if (lockedDoor && !(triggerDoors.ContainsKey(g) && triggerDoors[g].Contains(v)))
                        {
                            d.Locked = true;
                            GameObject key = ItemFactory.Instance.Create(ITEMTYPE.KEY);
                            d.Key = (Item)key.GetComponent<Item>();
                            Random rnd = new Random();
                            containers[rnd.Next(containers.Count)].Items.Add(d.Key);
                        }
                        else if (lockedDoor) {
                            d.Locked = true;
                            Trigger t = new Trigger();
                            float shortestDistance = float.MaxValue;
                            foreach (Trigger tr in triggers.Keys) {
                                float distance = Vector2.Distance(triggers[tr], door.Transform.Position);
                                if (distance < shortestDistance) {
                                    t = tr;
                                    shortestDistance = distance;
                                    break;
                                }
                            }
                            triggers.Remove(t);
                            t.ConnectedObject = d;
                        }
                        d.Neighbours[0] = g;
                        d.Neighbours[1] = GameWorld.Instance.Grid[new Vector2(g.position.X + GameWorld.Instance.GridSize * v.X, g.position.Y + GameWorld.Instance.GridSize * v.Y)];
                        
                        GameWorld.Instance.GameObjects.Add(door);
                    }
                }
                if (!g.enterExitBot && !ignore && !placedObjects.Contains(new Vector2(g.position.X + g.position.Width / 2, g.position.Y + g.position.Height)))
                {
                    GameObject wall = WallFactory.Instance.Create(WALLDIRECTION.horizontal);
                    wall.Transform.Position = new Vector2(g.position.X + g.position.Width / 2, g.position.Y + g.position.Height);
                    placedObjects.Add(new Vector2(g.position.X + g.position.Width / 2, g.position.Y + g.position.Height));
                    GameWorld.Instance.GameObjects.Add(wall);
                }
                if (!g.enterExitLef && !ignore && !placedObjects.Contains(new Vector2(g.position.X, g.position.Y + g.position.Height / 2)))
                {
                    GameObject wall = WallFactory.Instance.Create(WALLDIRECTION.vertical);
                    wall.Transform.Position = new Vector2(g.position.X, g.position.Y + g.position.Height / 2);
                    placedObjects.Add(new Vector2(g.position.X, g.position.Y + g.position.Height / 2));
                    GameWorld.Instance.GameObjects.Add(wall);
                }
                if (!g.enterExitRig && !ignore && !placedObjects.Contains(new Vector2(g.position.X + g.position.Width, g.position.Y + g.position.Height / 2)))
                {
                    GameObject wall = WallFactory.Instance.Create(WALLDIRECTION.vertical);
                    wall.Transform.Position = new Vector2(g.position.X + g.position.Width, g.position.Y + g.position.Height / 2);
                    placedObjects.Add(new Vector2(g.position.X + g.position.Width, g.position.Y + g.position.Height / 2));
                    GameWorld.Instance.GameObjects.Add(wall);
                }
                if (!g.enterExitTop && !ignore && !placedObjects.Contains(new Vector2(g.position.X + g.position.Width / 2, g.position.Y)))
                {
                    GameObject wall = WallFactory.Instance.Create(WALLDIRECTION.horizontal);
                    wall.Transform.Position = new Vector2(g.position.X + g.position.Width / 2, g.position.Y);
                    placedObjects.Add(new Vector2(g.position.X + g.position.Width, g.position.Y + g.position.Height / 2));
                    GameWorld.Instance.GameObjects.Add(wall);
                }
                
                if (!ignore)
                {
                    GameObject floor = new GameObject();
                    floor.Tag = "dungeon";
                    SpriteRenderer sr = new SpriteRenderer();
                    sr.Sprite = floorSprite;
                    sr.Scale = new Vector2(0.11f, 0.11f);
                    sr.LayerDepth -= 0.1f;
                    floor.AddComponent(sr);
                    floor.Transform.Position = new Vector2(g.position.X + g.position.Width / 2, g.position.Y + g.position.Height / 2);
                    GameWorld.Instance.GameObjects.Add(floor);
                }
            }
            if (triggers.Count > 0) {
                foreach (Trigger t in triggers.Keys) {
                    if (GameWorld.Instance.GameObjects.Contains(t.GameObject)) {
                        GameWorld.Instance.GameObjects.Remove(t.GameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Tjekker om der kan gøres plads til en ny udgang
        /// </summary>
        /// <param name="gF">dungeon exit</param>
        /// <param name="gridSize">længden og højden på et gridfelt</param>
        /// <returns></returns>
        private bool exitCheck(GridField gF, int gridSize)
        {
            Vector2 pos = new Vector2(gF.position.X, gF.position.Y);
            if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y))) {
                GridField stairs = new GridField(new Rectangle((int)pos.X - gridSize, (int)pos.Y, gridSize, gridSize), false, false, false, false);
                stairs.containsExit = true;
                GameWorld.Instance.Grid.Add(new Vector2(pos.X - gridSize, pos.Y), stairs);
                return true;
            }
            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
            {
                GridField stairs = new GridField(new Rectangle((int)pos.X + gridSize, (int)pos.Y, gridSize, gridSize), false, false, false, false);
                stairs.containsExit = true;
                GameWorld.Instance.Grid.Add(new Vector2(pos.X + gridSize, pos.Y), stairs);
                return true;
            }
            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
            {
                GridField stairs = new GridField(new Rectangle((int)pos.X, (int)pos.Y - gridSize, gridSize, gridSize), false, false, false, false);
                stairs.containsExit = true;
                GameWorld.Instance.Grid.Add(new Vector2(pos.X, pos.Y - gridSize), stairs);
                return true;
            }
            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
            {
                GridField stairs = new GridField(new Rectangle((int)pos.X, (int)pos.Y + gridSize, gridSize, gridSize), false, false, false, false);
                stairs.containsExit = true;
                GameWorld.Instance.Grid.Add(new Vector2(pos.X, pos.Y + gridSize), stairs);
                return true;
            }
            return false;

        }

        /// <summary>
        /// Laver en passage med en specifik længde
        /// </summary>
        /// <param name="gridSize">længden og højden på et gridfelt</param>
        /// <param name="length">længden på passagen i gridfields</param>
        /// <param name="type">hvilken slags passage det er</param>
        private void CreateSpecificPassage(int gridSize, int length, ROOMTYPE type, bool door)
        {
            GridField start = exitFields.Dequeue();
            Vector2 pos = Vector2.Zero;
            foreach (GridField g in GameWorld.Instance.Grid.Values) {
                if (g == start) {
                    pos = new Vector2(g.position.X, g.position.Y);
                    break;
                }
            }
            Rectangle passage = start.position;
            if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y))) {
                passage = new Rectangle((int)pos.X - gridSize * length - gridSize, (int)pos.Y - gridSize, gridSize * length, gridSize * 2);
                if (door) {
                    AddDoor(start, new Vector2(-1, 0));
                }
                
            }
            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
            {
                passage = new Rectangle((int)pos.X - gridSize, (int)pos.Y - gridSize * length - gridSize, gridSize * 2, gridSize * length);
                if (door)
                {
                    AddDoor(start, new Vector2(0, -1));
                }
            }

            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
            {
                passage = new Rectangle((int)pos.X + gridSize, (int)pos.Y - gridSize, gridSize * length, gridSize * 2);
                if (door)
                {

                    AddDoor(start, new Vector2(1, 0));

                }
            }
            else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
            {
                passage = new Rectangle((int)pos.X - gridSize, (int)pos.Y + gridSize, gridSize * 2, gridSize * length);
                if (door)
                {
                    AddDoor(start, new Vector2(0, 1));
                }
            }
            if (passage != start.position) {
                Rooms.Add(passage);
                if (passage.X < minX)
                {
                    minX = passage.X;
                }
                if (passage.Y < minY)
                {
                    minY = passage.Y;
                }
                if (passage.X + passage.Width > maxX)
                {
                    maxX = passage.X + passage.Width;
                }
                if (passage.Y + passage.Height > maxY)
                {
                    maxY = passage.Y + passage.Height;
                }
                for (int i = passage.X; i < passage.X + passage.Width; i += gridSize)
                {
                    for (int j = passage.Y; j < passage.Y + passage.Height; j += gridSize)
                    {
                        GridField gF = new GridField(new Rectangle(i, j, gridSize, gridSize), true, true, true, true);
                        bool newField = true;
                        if (GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j)))
                        {
                            GameWorld.Instance.Grid[new Vector2(i, j)] = gF;
                            newField = false;
                        }
                        if (i == passage.X)
                        {
                            gF.enterExitLef = false;
                            if (pos == new Vector2(i - gridSize, j))
                            {
                                gF.enterExitLef = true;
                            }
                            else if (type == ROOMTYPE.passage && pos.X == i + passage.Width + gridSize)
                            {
                                gF.enterExitLef = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j))) {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.Y == j + passage.Height + gridSize)
                            {
                                gF.enterExitRig = true;
                                gF.enterExitLef = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.Y == j - passage.Height - gridSize)
                            {
                                gF.enterExitRig = true;
                                gF.enterExitLef = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                        }
                        if (i == passage.X + passage.Width)
                        {
                            gF.enterExitRig = false;
                            if (pos == new Vector2(i + gridSize, j))
                            {
                                gF.enterExitRig = true;
                            }
                            else if (type == ROOMTYPE.passage && pos.X == i - passage.Width - gridSize)
                            {
                                gF.enterExitRig = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.Y == j + passage.Height + gridSize)
                            {
                                gF.enterExitRig = true;
                                gF.enterExitLef = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.Y == j - passage.Height - gridSize)
                            {
                                gF.enterExitRig = true;
                                gF.enterExitLef = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                        }
                        if (j == passage.Y)
                        {
                            gF.enterExitTop = false;
                            if (pos == new Vector2(i, j - gridSize))
                            {
                                gF.enterExitTop = true;
                            }
                            else if (type == ROOMTYPE.passage && pos.Y == j + passage.Height + gridSize)
                            {
                                gF.enterExitTop = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.X == i - passage.Width - gridSize)
                            {
                                gF.enterExitTop = true;
                                gF.enterExitBot = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.X == i + passage.Width + gridSize)
                            {
                                gF.enterExitBot = true;
                                gF.enterExitTop = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                        }
                        if (j == passage.Y + passage.Height)
                        {
                            gF.enterExitBot = false;
                            if (pos == new Vector2(i, j + gridSize))
                            {
                                gF.enterExitBot = true;
                            }
                            else if (type == ROOMTYPE.passage && pos.Y == j - passage.Height - gridSize)
                            {
                                gF.enterExitBot = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.X == i + passage.Width + gridSize)
                            {
                                gF.enterExitBot = true;
                                gF.enterExitTop = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                            else if (type == ROOMTYPE.passageExitAtEnd && pos.X == i - passage.Width - gridSize)
                            {
                                gF.enterExitTop = true;
                                gF.enterExitBot = true;
                                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                {
                                    exits.Enqueue(ROOMTYPE.passage);
                                    exitFields.Enqueue(gF);
                                }
                            }
                        }
                        if (newField)
                        {
                            GameWorld.Instance.Grid.Add(new Vector2(i, j), gF);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Laver et rum
        /// </summary>
        /// <param name="gridSize">længden og højden på et gridfelt</param>
        /// <param name="type">typen af run</param>
        private void CreateRoom(int gridSize, ROOMTYPE type, bool door)
        {
            Vector2 pos = Vector2.Zero;
            GridField start = exitFields.Dequeue();
            Random rnd = new Random();
            foreach (GridField g in GameWorld.Instance.Grid.Values)
            {
                if (g == start)
                {
                    pos = new Vector2(g.position.X, g.position.Y);
                    break;
                }
            }
            Rectangle room = start.position;
            if (type == ROOMTYPE.chamber) {
                
                int size = rnd.Next(15);
                bool large = size > 11;
                int nExits = rnd.Next(20);
                int exitsLeft = 0;
                int exitsRight = 0;
                int exitsTop = 0;
                int exitsBottom = 0;
               
                if (large)
                {
                    switch (nExits)
                    {
                        case (0):
                        case (1):
                        case (2):
                            break;
                        case (3):
                        case (4):
                        case (5):
                        case (6):
                        case (7):
                            nExits = 1;
                            break;
                        case (8):
                        case (9):
                        case (10):
                        case (11):
                        case (12):
                            nExits = 2;
                            break;
                        case (13):
                        case (14):
                        case (15):
                        case (16):
                            nExits = 3;
                            break;
                        case (17):
                            nExits = 4;
                            break;
                        case (18):
                            nExits = 5;
                            break;
                        case (19):
                            nExits = 6;
                            break;
                    }
                }
                else {
                    switch (nExits) {
                        case (0):
                        case (1):
                        case (2):
                        case (3):
                        case (4):
                            break;
                        case (5):
                        case (6):
                        case (7):
                        case (8):
                        case (9):
                        case (10):
                            nExits = 1;
                            break;
                        case (11):
                        case (12):
                        case (13):
                        case (14):
                            nExits = 2;
                            break;
                        case (15):
                        case (16):
                        case (17):
                            nExits = 3;
                            break;
                        case (18):
                        case (19):
                            nExits = 4;
                            break;
                    }
                }
                Rectangle refRectangle = new Rectangle();
                int halfWidth = 0;
                int halfHeight = 0;
                switch (size) {
                    case (0):
                    case (1):
                        refRectangle = new Rectangle(0, 0, 4 * gridSize, 4 * gridSize);
                        halfWidth = 2 * gridSize;
                        halfHeight = 2 * gridSize;
                        break;
                    case (2):
                    case (3):
                        refRectangle = new Rectangle(0, 0, 6 * gridSize, 6 * gridSize);
                        halfWidth = 3 * gridSize;
                        halfHeight = 3 * gridSize;
                        break;
                    case (4):
                    case (5):
                        refRectangle = new Rectangle(0, 0, 8 * gridSize, 8 * gridSize);
                        halfWidth = 4 * gridSize;
                        halfHeight = 4 * gridSize;
                        break;
                    case (6):
                    case (7):
                    case (8):
                        refRectangle = new Rectangle(0, 0, 4 * gridSize, 6 * gridSize);
                        halfWidth = 2 * gridSize;
                        halfHeight = 3 * gridSize;
                        break;
                    case (9):
                    case (10):
                    case (11):
                        refRectangle = new Rectangle(0, 0, 6 * gridSize, 8 * gridSize);
                        halfWidth = 3 * gridSize;
                        halfHeight = 4 * gridSize;
                        break;
                    case (12):
                    case (13):
                        refRectangle = new Rectangle(0, 0, 8 * gridSize, 10 * gridSize);
                        halfWidth = 4 * gridSize;
                        halfHeight = 5 * gridSize;
                        break;
                    case (14):
                        refRectangle = new Rectangle(0, 0, 10 * gridSize, 16 * gridSize);
                        halfWidth = 5 * gridSize;
                        halfHeight = 8 * gridSize;
                        break;
                }
                Queue<ROOMTYPE> leftExitTypes = new Queue<ROOMTYPE>();
                Queue<ROOMTYPE> rightExitTypes = new Queue<ROOMTYPE>();
                Queue<ROOMTYPE> topExitTypes = new Queue<ROOMTYPE>();
                Queue<ROOMTYPE> bottomExitTypes = new Queue<ROOMTYPE>();
                if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y)))
                {
                    room = new Rectangle((int)pos.X - refRectangle.Height, (int)pos.Y - halfWidth, refRectangle.Height, refRectangle.Width);
                    if (door)
                    {
                        AddDoor(start, new Vector2(-1, 0));
                    }
                    while (nExits > 0) {
                        int exitLoc = rnd.Next(20);
                        int exitType = rnd.Next(20);
                        ROOMTYPE exit = ROOMTYPE.unknown;
                        if (exitType < 9) {
                            exit = ROOMTYPE.passage;
                        }
                        switch (exitLoc) {
                            case (0):
                            case (1):
                            case (2):
                            case (3):
                            case (4):
                            case (5):
                            case (6):
                                exitsRight++;
                                rightExitTypes.Enqueue(exit);
                                break;
                            case (7):
                            case (8):
                            case (9):
                            case (10):
                            case (11):
                                exitsBottom++;
                                bottomExitTypes.Enqueue(exit);
                                break;
                            case (12):
                            case (13):
                            case (14):
                            case (15):
                            case (16):
                                exitsTop++;
                                topExitTypes.Enqueue(exit);
                                break;
                            case (17):
                            case (18):
                            case (19):
                                exitsLeft++;
                                leftExitTypes.Enqueue(exit);
                                break;
                        }
                        nExits--;
                    }
                }
                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
                {
                    room = new Rectangle((int)pos.X - halfWidth, (int)pos.Y - refRectangle.Height, refRectangle.Width, refRectangle.Height);
                    if (door)
                    {
                        AddDoor(start, new Vector2(0, -1));
                    }
                    while (nExits > 0) {
                        int exitLoc = rnd.Next(20);
                        switch (exitLoc)
                        {
                            case (0):
                            case (1):
                            case (2):
                            case (3):
                            case (4):
                            case (5):
                            case (6):
                                exitsBottom++;
                                break;
                            case (7):
                            case (8):
                            case (9):
                            case (10):
                            case (11):
                                exitsRight++;
                                break;
                            case (12):
                            case (13):
                            case (14):
                            case (15):
                            case (16):
                                exitsLeft++;
                                break;
                            case (17):
                            case (18):
                            case (19):
                                exitsTop++;
                                break;
                        }
                        nExits--;
                    }
                }
                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
                {
                    room = new Rectangle((int)pos.X + gridSize, (int)pos.Y - halfWidth, refRectangle.Height, refRectangle.Width);
                    if (door)
                    {
                        AddDoor(start, new Vector2(1, 0));
                    }
                    while (nExits > 0)
                    {
                        int exitLoc = rnd.Next(20);
                        switch (exitLoc)
                        {
                            case (0):
                            case (1):
                            case (2):
                            case (3):
                            case (4):
                            case (5):
                            case (6):
                                exitsLeft++;
                                break;
                            case (7):
                            case (8):
                            case (9):
                            case (10):
                            case (11):
                                exitsBottom++;
                                break;
                            case (12):
                            case (13):
                            case (14):
                            case (15):
                            case (16):
                                exitsTop++;
                                break;
                            case (17):
                            case (18):
                            case (19):
                                exitsRight++;
                                break;
                        }
                        nExits--;
                    }
                }
                else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
                {
                    room = new Rectangle((int)pos.X - halfWidth, (int)pos.Y + gridSize, refRectangle.Width, refRectangle.Height);
                    if (door)
                    {
                        AddDoor(start, new Vector2(0, 1));
                    }
                    while (nExits > 0)
                    {
                        int exitLoc = rnd.Next(20);
                        switch (exitLoc)
                        {
                            case (0):
                            case (1):
                            case (2):
                            case (3):
                            case (4):
                            case (5):
                            case (6):
                                exitsTop++;
                                break;
                            case (7):
                            case (8):
                            case (9):
                            case (10):
                            case (11):
                                exitsLeft++;
                                break;
                            case (12):
                            case (13):
                            case (14):
                            case (15):
                            case (16):
                                exitsRight++;
                                break;
                            case (17):
                            case (18):
                            case (19):
                                exitsBottom++;
                                break;
                        }
                        nExits--;
                    }
                }
                if (room != start.position) {
                    int topDoorDistance = (room.Width / ((exitsTop + 1) * gridSize));
                    int topN = 1;
                    int leftDoorDistance = room.Height / ((exitsLeft + 1) * gridSize);
                    int leftN = 1;
                    float bottomDoorDistance = room.Width / ((exitsBottom + 1) * gridSize);
                    int bottomN = 1;
                    int rightDoorDistance = room.Width / ((exitsRight + 1) * gridSize);
                    int rightN = 1;
                    Rooms.Add(room);
                    if (room.X < minX)
                    {
                        minX = room.X;
                    }
                    if (room.Y < minY)
                    {
                        minY = room.Y;
                    }
                    if (room.X + room.Width > maxX)
                    {
                        maxX = room.X + room.Width;
                    }
                    if (room.Y + room.Height > maxY)
                    {
                        maxY = room.Y + room.Height;
                    }
                    for (int i = room.X; i < room.X + room.Width; i += gridSize)
                    {
                        for (int j = room.Y; j < room.Y + room.Height; j += gridSize)
                        {
                            GridField gF = new GridField(new Rectangle(i, j, gridSize, gridSize), true, true, true, true);
                            bool newField = true;
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j)))
                            {
                                GameWorld.Instance.Grid[new Vector2(i, j)] = gF;
                                newField = false;
                            }
                            if (i == room.X)
                            {
                                gF.enterExitLef = false;
                                if (pos == new Vector2(i - gridSize, j))
                                {
                                    gF.enterExitLef = true;
                                    gF.containsPassage = true;
                                }
                                else if (exitsLeft > 0 && j - room.Y >= leftDoorDistance * gridSize * leftN)
                                {
                                    gF.enterExitLef = true;
                                    gF.containsPassage = true;
                                    leftN++;
                                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                    {
                                        int exitType = rnd.Next(20);
                                        ROOMTYPE exit = ROOMTYPE.unknown;
                                        if (exitType < 9)
                                        {
                                            exit = ROOMTYPE.passage;
                                        }
                                        exits.Enqueue(exit);
                                        exitFields.Enqueue(gF);
                                    }
                                }
                            }
                            if (i == room.X + room.Width - gridSize)
                            {
                                gF.enterExitRig = false;
                                if (pos == new Vector2(i + gridSize, j))
                                {
                                    gF.enterExitRig = true;
                                    gF.containsPassage = true;
                                }
                                else if (exitsRight > 0 && i - room.Y >= rightDoorDistance * gridSize * rightN)
                                {
                                    gF.enterExitRig = true;
                                    gF.containsPassage = true;
                                    rightN++;
                                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                    {
                                        int exitType = rnd.Next(20);
                                        ROOMTYPE exit = ROOMTYPE.unknown;
                                        if (exitType < 9)
                                        {
                                            exit = ROOMTYPE.passage;
                                        }
                                        exits.Enqueue(exit);
                                        exitFields.Enqueue(gF);
                                    }
                                }
                            }
                            if (j == room.Y)
                            {
                                gF.enterExitTop = false;
                                if (pos == new Vector2(i, j - gridSize))
                                {
                                    gF.enterExitTop = true;
                                    gF.containsPassage = true;
                                }
                                else if (exitsTop > 0 && i - room.X >= topDoorDistance * gridSize * topN)
                                {
                                    gF.enterExitTop = true;
                                    gF.containsPassage = true;
                                    topN++;
                                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                    {
                                        int exitType = rnd.Next(20);
                                        ROOMTYPE exit = ROOMTYPE.unknown;
                                        if (exitType < 9)
                                        {
                                            exit = ROOMTYPE.passage;
                                        }
                                        exits.Enqueue(exit);
                                        exitFields.Enqueue(gF);
                                    }
                                }
                            }
                            if (j == room.Y + room.Height - gridSize)
                            {
                                gF.enterExitBot = false;
                                if (pos == new Vector2(i, j + gridSize))
                                {
                                    gF.enterExitBot = true;
                                    gF.containsPassage = true;
                                }
                                else if (exitsBottom > 0 && i - room.X >= bottomDoorDistance * gridSize * bottomN)
                                {
                                    gF.enterExitBot = true;
                                    gF.containsPassage = true;
                                    if (pos.Y != j - room.Height - gridSize)
                                    {
                                        bottomN++;
                                    }
                                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                    {
                                        int exitType = rnd.Next(20);
                                        ROOMTYPE exit = ROOMTYPE.unknown;
                                        if (exitType < 9)
                                        {
                                            exit = ROOMTYPE.passage;
                                        }
                                        exits.Enqueue(exit);
                                        exitFields.Enqueue(gF);
                                    }

                                }
                            }
                            if (newField)
                            {
                                GameWorld.Instance.Grid.Add(new Vector2(i, j), gF);
                            }
                        }
                    }
                }
                
            }
            if (type == ROOMTYPE.passage) {
                int size = rnd.Next(20);
                if (PassageSections.Count > 40) {
                    size += rooms.Count - 40;
                }
                int exitsRight = 0;
                int exitsLeft = 0;
                int endExits = 2;
                Rectangle refRectangle = new Rectangle();
                bool generatingPassage = true;
                int halfHeight = 0;
                int halfWidth = 0;
                switch (size) {
                    case (0):
                    case (1):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        break;
                    case (2):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsRight = 1;
                        break;
                    case (3):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsLeft = 1;
                        break;
                    case (4):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        endExits = 1;
                        break;
                    case (5):
                    case (6):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsRight = 1;
                        break;
                    case (7):
                    case (8):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsLeft = 1;
                        break;
                    case (18):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 4 * gridSize);
                        halfHeight = 2 * gridSize;
                        halfWidth = gridSize;
                        int doornumb = rnd.Next(10);
                        endExits = 0;
                        if (doornumb == 0) {
                            endExits = 1;
                        }
                        break;
                    case (9):
                    case (10):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsLeft = 1;
                        type = ROOMTYPE.passageExitAtEnd;
                        endExits = 0;
                        break;
                    case (11):
                    case (12):
                        refRectangle = new Rectangle(0, 0, gridSize * 2, 6 * gridSize);
                        halfHeight = 3 * gridSize;
                        halfWidth = gridSize;
                        exitsRight = 1;
                        type = ROOMTYPE.passageExitAtEnd;
                        endExits = 0;
                        break;
                    case (13):
                    case (14):
                    case (15):
                    case (16):
                    case (17):
                        exits.Enqueue(ROOMTYPE.chamber);
                        exitFields.Enqueue(start);
                        generatingPassage = false;
                        break;
                    case (19):
                    default:
                        exits.Enqueue(ROOMTYPE.unknown);
                        exitFields.Enqueue(start);
                        generatingPassage = false;
                        break;
                }
                if (generatingPassage)
                {
                    int exitsTop = 0;
                    int exitsBottom = 0;
                    int exitsRig = 0;
                    int exitsLef = 0;
                    if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X - gridSize, pos.Y)))
                    {
                        room = new Rectangle((int)pos.X - refRectangle.Height, (int)pos.Y, refRectangle.Height, refRectangle.Width);
                        if (door)
                        {
                            AddDoor(start, new Vector2(-1, 0));
                        }
                        exitsTop = exitsRight;
                        exitsBottom = exitsLeft;
                        exitsRig = 0;
                        exitsLef = endExits;
                    }
                    else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y - gridSize)))
                    {
                        room = new Rectangle((int)pos.X, (int)pos.Y - refRectangle.Height, refRectangle.Width, refRectangle.Height);
                        if (door)
                        {
                            AddDoor(start, new Vector2(0, 1));
                        }
                        exitsTop = endExits;
                        exitsRig = exitsRight;
                        exitsLef = exitsLeft;
                    }
                    else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X + gridSize, pos.Y)))
                    {
                        room = new Rectangle((int)pos.X + gridSize, (int)pos.Y, refRectangle.Height, refRectangle.Width);
                        if (door)
                        {
                            AddDoor(start, new Vector2(1, 0));
                        }
                        exitsTop = exitsLeft;
                        exitsBottom = exitsRight;
                        exitsLef = 0;
                        exitsRig = endExits;
                    }
                    else if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(pos.X, pos.Y + gridSize)))
                    {
                        room = new Rectangle((int)pos.X, (int)pos.Y + gridSize, refRectangle.Width, refRectangle.Height);
                        if (door)
                        {
                            AddDoor(start, new Vector2(0, 1));
                        }
                        exitsRig = exitsLeft;
                        exitsLef = exitsTop;
                        exitsTop = 0;
                        exitsBottom = endExits;
                    }
                    PassageSections.Add(room);
                    if (room.X < minX)
                    {
                        minX = room.X;
                    }
                    if (room.Y < minY)
                    {
                        minY = room.Y;
                    }
                    if (room.X + room.Width > maxX)
                    {
                        maxX = room.X + room.Width;
                    }
                    if (room.Y + room.Height > maxY)
                    {
                        maxY = room.Y + room.Height;
                    }
                    for (int i = room.X; i < room.X + room.Width; i += gridSize)
                    {
                        for (int j = room.Y; j < room.Y + room.Height; j += gridSize)
                        {

                            GridField gF = new GridField(new Rectangle(i, j, gridSize, gridSize), true, true, true, true);
                            bool newField = true;
                            if (GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j)))
                            {
                                GameWorld.Instance.Grid[new Vector2(i, j)] = gF;
                                newField = false;
                            }
                            if (i == room.X)
                            {
                                gF.enterExitLef = false;
                                if (pos == new Vector2(i - gridSize, j))
                                {
                                    gF.enterExitLef = true;
                                }
                                else if (exitsLef > 0)
                                {
                                    
                                    if ((j == pos.Y - gridSize * 4 || j == pos.Y + gridSize * 5) && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitLef = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if (i == pos.X - room.Width && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitLef = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if ((j == pos.Y - room.Height || j == pos.Y + room.Height + gridSize) && type == ROOMTYPE.passageExitAtEnd) {
                                        gF.enterExitLef = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i - gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    gF.containsPassage = true;

                                }
                            }
                            if (i == room.X + room.Width - gridSize)
                            {
                                gF.enterExitRig = false;
                                if (pos == new Vector2(i + gridSize, j))
                                {
                                    gF.enterExitRig = true;
                                }
                                else if (exitsRig > 0) {
                                    if (j == room.Y + gridSize * 4 && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitRig = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if (exitsRig == 2 && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitRig = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if ((j == pos.Y - room.Height || j == pos.Y + room.Height + gridSize) && type == ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitRig = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i + gridSize, j)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    gF.containsPassage = true;
                                }
                                
                            }
                            if (j == room.Y)
                            {
                                gF.enterExitTop = false;
                                if (exitsTop > 0)
                                {
                                    if (i == room.X + gridSize * 4 && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitTop = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if (j == pos.Y - room.Height && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitTop = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if ((i == pos.X - room.Width || i == pos.X + room.Width + gridSize) && type == ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitTop = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j - gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    gF.containsPassage = true;
                                }
                            }
                            if (j == room.Y + room.Width - gridSize)
                            {
                                gF.enterExitBot = false;
                                gF.containsPassage = true;
                                if (exitsBottom > 0)
                                {
                                    if (i == room.X + gridSize * 4 && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitBot = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if (j == pos.Y + room.Height + gridSize && type != ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitBot = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    else if ((i == pos.X - room.Width || i == pos.X + room.Width + gridSize) && type == ROOMTYPE.passageExitAtEnd)
                                    {
                                        gF.enterExitBot = true;
                                        if (!GameWorld.Instance.Grid.ContainsKey(new Vector2(i, j + gridSize)) && !exitFields.Contains(gF))
                                        {
                                            exits.Enqueue(ROOMTYPE.passage);
                                            exitFields.Enqueue(gF);
                                        }
                                    }
                                    gF.containsPassage = true;
                                }
                            }
                            if (newField) {
                                GameWorld.Instance.Grid.Add(new Vector2(i, j), gF);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tegner omridset af et rum
        /// </summary>
        /// <param name="collisionBox">rummets rektangel</param>
        /// <param name="spriteBatch">spriteBatch der skal tegne det</param>
        public void DrawRoom(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Blue, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        /// <summary>
        /// Tegner omridset af en passagesektion
        /// </summary>
        /// <param name="collisionBox">passage sektionens rektangel</param>
        /// <param name="spriteBatch">spriteBatch der skal tegne det</param>
        public void DrawPassageSection(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

        private void AddDoor(GridField start, Vector2 direction ) { 
            Random rnd = new Random();
            if (Doors.ContainsKey(start))
            {
                Doors[start].Add(direction);
            }
            else
            {
                List<Vector2> dir = new List<Vector2>() {
                    direction
                };
                Doors.Add(start, dir);
            }
            int doorType = rnd.Next(20);
            if (doorType >= 13 && doorType != 19)
            {
                if (lockedDoors.ContainsKey(start))
                {
                    lockedDoors[start].Add(direction);
                }
                else
                {
                    List<Vector2> dir = new List<Vector2>() {
                        direction
                    };
                    lockedDoors.Add(start, dir);
                }
            }
            if (doorType > 17)
            {
                if (secretDoors.ContainsKey(start))
                {
                    secretDoors[start].Add(direction);
                }
                else
                {
                    List<Vector2> dir = new List<Vector2>() {
                        direction
                    };
                    secretDoors.Add(start, dir);
                }
            }
        }
    }
}
