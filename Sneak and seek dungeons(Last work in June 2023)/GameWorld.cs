using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct3D9;
using Sneak_and_seek_dungeons.Components;
using Sneak_and_seek_dungeons.FactoryPattern;
using SneekAndSeekDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;

namespace Sneak_and_seek_dungeons
{
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Vector2 screenSize;
        private static GameWorld instance;
        public SpriteFont defaultFont;
        private int activeThreads;
        public Mutex m = new Mutex();
        private KeyboardState state;
        private int gridSize = 100;
        private Dungeon dun;
        private Dictionary<Vector2, GridField> grid = new Dictionary<Vector2, GridField>();
        public Random rnd = new Random();
        
        private SneekAndSeekRepository repo;

        private SCENEPROPERTY currentScene=SCENEPROPERTY.DUNGEON;

        //de værdier der ser hvilekn knap er trykket på
        private KeyboardState key1, prevKey1;

        // Public lock til modificering af enemy position og velocity

        public object syncEnemyMovement = new object();


        //pixel spriten som bruges til baggrund af pause skærmen
        private Texture2D pixel2;
        private Texture2D pausetext;


        private GameObject player;
        private Camera camera;


        //Gameobjects
        private List<GameObject> gameObjects = new List<GameObject>();

        private List<GameObject> newGameObjects = new List<GameObject>();

        private List<GameObject> destroyedGameObjects = new List<GameObject>();

        //UI ELEMENTS
        private List<GameObject> uiObjects = new List<GameObject>();

        private List<GameObject> uiObjects2 = new List<GameObject>();

        private List<GameObject> newUIObjects = new List<GameObject>();

        private List<GameObject> newUIObjects2 = new List<GameObject>();

        private List<GameObject> destroyeduiObjects = new List<GameObject>();



        public static float DeltaTime { get; private set; }
        public static GameWorld Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }
        /// <summary>
        /// liste over colliders i spillet
        /// Dungeoncolliders og currentdungeoncolliders er en seperat liste for at optimere spillet
        /// </summary>
        public List<Collider> Colliders { get; private set; } = new List<Collider>();
        public List<Collider> DungeonColliders { get; private set; } = new List<Collider>();
        public List<Collider> CurrentDungeonColliders { get; private set; } = new List<Collider>();
        public static Vector2 ScreenSize { get => screenSize; set => screenSize = value; }

        public List<GameObject> NewGameObjects { get => newGameObjects; }
        public List<GameObject> DestroyedGameObjects { get => destroyedGameObjects; }
        public GraphicsDeviceManager Graphics { get => _graphics; }

        public List<GameObject> GameObjects { get => gameObjects; set => gameObjects = value; }
        public List<GameObject> DestroyeduiObjects { get => destroyeduiObjects; }
        public List<GameObject> NewUIObjects { get => newUIObjects; }

        public List<GameObject> NewUIObjects2 { get => newUIObjects2; }

        public int GridSize { get => gridSize; set => gridSize = value; }
        internal Dictionary<Vector2, GridField> Grid { get => grid; private set => grid = value; }

        public Dungeon Dun { get => dun; set => dun = value; }
        public GameObject Player { get => player; private set => player = value; }
        internal SneekAndSeekRepository Repo { get => repo; private set => repo = value; }

        public Texture2D Sprite { get => pixel2; set => pixel2 = value; }
        public Texture2D Sprite2 { get => pausetext; set => pausetext = value; }
        internal Camera Camera { get => camera; private set => camera = value; }

        //private Enemy enemy = new Enemy();
        
        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenSize = new Vector2(800, 600);
            _graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            _graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;

            //sætter størrelsen af spillets vindue
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


            //_graphics.ToggleFullScreen();


            //definere størrelsen i vores static variable screenSize til brug i andre klasser
            screenSize.X = _graphics.PreferredBackBufferWidth;
            screenSize.Y = _graphics.PreferredBackBufferHeight;

            _graphics.ApplyChanges();
            SneekAndSeekMapper mapper = new SneekAndSeekMapper();
            SQLiteDatabaseProvider provider = new SQLiteDatabaseProvider("Data Source=sneekAndSeekSave.db; version=3;new=true");
            Repo = new SneekAndSeekRepository(mapper, provider);

            //Opsætning af dungeonen
            dun = new Dungeon();
            int j = 0;
            while (GameObjects.Count == 0)
            {
                dun.CreateDungeon();
                if (GameObjects.Count == 0)
                {
                    dun = new Dungeon();
                }
            }


            





            //Cleanup();
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObjects[i].Awake();
            }

            //currentScene = SCENEPROPERTY.CITY;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            defaultFont = Content.Load<SpriteFont>("defaultFont");
            ///Rasmus
            ///Indlæser pause skærmen's sprites
            pixel2 = Content.Load<Texture2D>("pixel2");
            pausetext = Content.Load<Texture2D>("pausetext");

            Player = new GameObject();
            bool freshStart = true;
            if (Repo.ExecuteReader("SELECT * FROM playerInventory").Count > 0) {
                freshStart = false;
            }

            //tilføjer spiller objektet
            Player p = (Player) Player.AddComponent(new Player(freshStart));
            Player.AddComponent(new SpriteRenderer());
            Player.Transform.Position = dun.spawnPoint;
            Collider col = (Collider) Player.AddComponent(new Collider());
            Colliders.Add(col);
            Camera = (Camera) Player.AddComponent(new Camera());
            gameObjects.Add(Player);

            //creates city gameobjects
            City.Instance.SetupCity();

            if (currentScene == SCENEPROPERTY.DUNGEON)
            {
                SceneManager.Instance.DungeonLoadedBefore = true;
            }
            if (currentScene == SCENEPROPERTY.CITY)
            {
                SceneManager.Instance.CityLoadedBefore = true;
            }

            /*
            // Tilføjelse af Enemy test objekter

            GameObject enemyTest1 = EnemyFactory.Instance.Create(ENEMYTYPE.GRUNT);
            gameObjects.Add(enemyTest1);
            enemyTest1.Transform.Position = new Vector2(Graphics.PreferredBackBufferWidth / 2 + 100, Graphics.PreferredBackBufferHeight / 2);


            GameObject enemyTest2=EnemyFactory.Instance.Create(ENEMYTYPE.GRUNT);
            gameObjects.Add(enemyTest2);
            enemyTest2.Transform.Position = new Vector2(Graphics.PreferredBackBufferWidth / 2 - 100, Graphics.PreferredBackBufferHeight / 2);
            */


            // TODO: use this.Content to load your game content here
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Start();
            }

            GameObject buttonObject1 = UIFactory.Instance.Create(UITYPE.BUTTON);
            GameObject buttonObject2 = UIFactory.Instance.Create(UITYPE.BUTTON);
            GameObject buttonObject3 = UIFactory.Instance.Create(UITYPE.BUTTON);
            //buttonObject.Start();
            //buttonObject2.Start();
            //buttonObject3.Start();
            newUIObjects2.Add(buttonObject1);
            newUIObjects2.Add(buttonObject2);
            newUIObjects2.Add(buttonObject3);

            Button exitButton = (Button)buttonObject1.GetComponent<Button>();
            Button resumeButton = (Button)buttonObject2.GetComponent<Button>();
            Button saveButton = (Button)buttonObject3.GetComponent<Button>();
            exitButton.Offset = new Vector2(-300f, - 0f);
            resumeButton.Offset = new Vector2(0, - 0f);
            saveButton.Offset = new Vector2(+300f, - 0f);
            exitButton.ButtonText = "Exit";
            resumeButton.ButtonText = "Resume";
            saveButton.ButtonText = "Save";
            exitButton.OnClick = Exit;
            resumeButton.OnClick = Global.Double;
            saveButton.OnClick = Repo.SaveGame;

            buttonObject1.Transform.Position = new Vector2(Player.Transform.Position.X + exitButton.Offset.X, Player.Transform.Position.Y + exitButton.Offset.Y);
            buttonObject2.Transform.Position = new Vector2(Player.Transform.Position.X + resumeButton.Offset.X, Player.Transform.Position.Y + resumeButton.Offset.Y);
            buttonObject3.Transform.Position = new Vector2(Player.Transform.Position.X + saveButton.Offset.X, Player.Transform.Position.Y + saveButton.Offset.Y);



        }

        protected override void Update(GameTime gameTime)
        {
            ///Rasmus
            ///I dette næste lille stykke er den primære del der pauser spillet. Det er en bool der flippes mellem at være true og false
            ///key1 og prevKey1 er funktioner som bruges med deres GetState for at tjekke om den valgte knap, P, stadigt er holdt inde eller ej
            ///uden disse ville boolen blive flippet hver gang update bliver kaldt. Men med dem kan man holde knappen nede så længe man vil, og den vil kun aktivere 1 gang
            key1 = Keyboard.GetState();

            if (key1.IsKeyDown(Keys.Escape) && prevKey1.IsKeyUp(Keys.Escape))
            {
                Global.pause(); ;
                Global.EnemyStop();
            }
            prevKey1 = key1;
                
            IEnumerable<Enemy> enemies = gameObjects.OfType<Enemy>();
            ///Rasmus
            ///i den kommende if sætning tjekkes om spillet kører eller om det er pauset. Er det pauset, så køres det kode som trækker spillet ikke.
            if (!Global.paused)
            {
                

                //IEnumerable<Enemy> enemies = gameObjects.OfType<Enemy>();
                //foreach (Enemy enemy in enemies)
                //{
                //    enemy.StartMovement();
                //}

                

                // TODO: Add your update logic here
                DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObjects[i].Update();
                }
                for (int i = 0; i < uiObjects.Count; i++)
                {
                    uiObjects[i].Update();
                }
                Cleanup();
                ChangeScenes();
                UnloadDungeonFromDistance();
            }
            else
            {
                

                for (int i = 0; i < uiObjects2.Count; i++)
                {
                    uiObjects2[i].Update();
                }

                ///Rasmus
                /// her nede prøvede vi en del til at stoppe enemies med at flytte sig når spillet var pauset, men fandt så en pænt nem løsning som var set højere oppe i koden

                //Enemy.StopMovement();
                //IEnumerable<Enemy> enemies = gameObjects.OfType<Enemy>();
                //foreach(GameObject gameObject in GameObjects)
                //{
                //    if(gameObject is Enemy)
                //    {
                //        Enemy enemy = (Enemy)gameObject;
                //        if (enemy != null)
                //        {
                //            enemy.StopMovement();
                //        }

                //    }
                //}


                //Enemy.Instance.StopMovement();
            }

            CurrentDungeonColliders.Clear();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            if (currentScene==SCENEPROPERTY.DUNGEON) {
                _spriteBatch.Begin(samplerState: Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, transformMatrix: camera.transform, sortMode: SpriteSortMode.FrontToBack);
            }
            else
            {
                _spriteBatch.Begin(samplerState: Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);


            }

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Draw(_spriteBatch);
            }
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Draw(_spriteBatch);
            }
            

            /*foreach (GridField g in grid.Values) {
                g.DrawRectangle(g.position, _spriteBatch);
            }*/

            ///Rasmus
            ///I den kommende if sætning tjekker den om spillet er pauset, og hvis det er, så tegned pauseskærmen
            if (Global.paused)
            {
                
                _spriteBatch.Draw(pixel2, new Rectangle(Convert.ToInt32(Player.Transform.Position.X)-1125, Convert.ToInt32(Player.Transform.Position.Y)-750, 2250, 1500), null, Color.LightSteelBlue, 0, Vector2.Zero, SpriteEffects.None, 0.7f);
                _spriteBatch.Draw(pausetext, new Rectangle(Convert.ToInt32(Player.Transform.Position.X) - 500, Convert.ToInt32(Player.Transform.Position.Y) - 600, 1000, 150), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.8f);
                for (int i = 0; i < uiObjects2.Count; i++)
                {
                    uiObjects2[i].Draw(_spriteBatch);
                }
            }




            foreach (Rectangle r in dun.Rooms) {
                dun.DrawRoom(r, _spriteBatch);
            }
            foreach (Rectangle r in dun.PassageSections)
            {
                dun.DrawPassageSection(r, _spriteBatch);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Opdatere hvad for nogle dungeon vægge som er relevante at udregne kollisioner for
        /// Hvis væggene er for langt væk er de irrelevante og bliver ikke tilføjet til listen
        /// </summary>
        public void UpdateDungeonColliders(Vector2 point)
        {
            

            foreach (Collider collider in DungeonColliders)
            {
                if (Vector2.Distance(collider.GameObject.Transform.Position,Player.Transform.Position) < 50 && collider.ActiveCollider)
                {
                    CurrentDungeonColliders.Add(collider);
                } 
            }


        }

        public void ChangeScenes()
        {
            if (SceneManager.Instance.scene!=currentScene)
            {
                currentScene = SceneManager.Instance.scene;

                if (currentScene == SCENEPROPERTY.DUNGEON)
                {
                    SceneManager.Instance.UnloadCity();
                    SceneManager.Instance.LoadDungeon();
                }
                else if (currentScene == SCENEPROPERTY.CITY)
                {
                    SceneManager.Instance.UnloadDungeon();
                    SceneManager.Instance.LoadCity();
                }

            }
        }

        public Component FindObjectOfType<T>() where T : Component
        {
            foreach (GameObject gameObject in GameObjects)
            {
                Component c = gameObject.GetComponent<T>();

                if (c != null)
                {
                    return c;
                }
            }

            return null;


        }

        public void Instantiate(GameObject go)
        {
            NewGameObjects.Add(go);
        }

        private void Cleanup()
        {
            //cleanup gameobjects
            for (int i = 0; i < NewGameObjects.Count; i++)
            {
                GameObjects.Add(NewGameObjects[i]);
                NewGameObjects[i].Awake();
                NewGameObjects[i].Start();



            }

            for (int i = 0; i < DestroyedGameObjects.Count; i++)
            {


                GameObjects.Remove(DestroyedGameObjects[i]);


            }
            DestroyedGameObjects.Clear();
            NewGameObjects.Clear();


            //cleanup UI
            for (int i = 0; i < NewUIObjects.Count; i++)
            {
                uiObjects.Add(NewUIObjects[i]);
                NewUIObjects[i].Awake();
                NewUIObjects[i].Start();



            }
            for (int i = 0; i < NewUIObjects2.Count; i++)
            {
                uiObjects2.Add(NewUIObjects2[i]);
                NewUIObjects2[i].Awake();
                NewUIObjects2[i].Start();



            }

            for (int i = 0; i < DestroyeduiObjects.Count; i++)
            {


                uiObjects.Remove(DestroyeduiObjects[i]);


            }
            DestroyeduiObjects.Clear();
            NewUIObjects.Clear();
            NewUIObjects2.Clear();
        }
        
        /// <summary>
        /// Ser om der er en rute mellem to punkter ved hjælp af A* algoritmen.
        /// </summary>
        /// <param name="startPoint">Start punktet</param>
        /// <param name="endPoint">Punktet der skal findes vej til</param>
        /// <returns>ruten mellem de to punkter hvis den findes. Ellers en tom liste</returns>
        public List<Vector2> Pathfinding(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2 start = startPoint;
            Vector2 end = endPoint;
            bool foundStartPoint = false;
            bool foundEndPoint = false;
            foreach (GridField g in Grid.Values) {
                if (g.position.Contains(startPoint)) {
                    start = g.position.Center.ToVector2();
                    foundStartPoint = true;
                }
                if (g.position.Contains(endPoint)) {
                    end = g.position.Center.ToVector2();
                    foundEndPoint = true;
                }
                if (foundStartPoint && foundEndPoint) {
                    break;
                }
            }
            Dictionary<Vector2, Vector2> parents = new Dictionary<Vector2, Vector2>();
            Dictionary<Vector2, int> openList = new Dictionary<Vector2, int>
            {
                { start, 0 }
            };
            Dictionary<Vector2, int> openListF = new Dictionary<Vector2, int>
            {
                { start, 0 }
            };
            List<Vector2> closedList = new List<Vector2>();

            while (!closedList.Contains(end) && openList.Count > 0)
            {
                int shortest = int.MaxValue;
                Vector2 cTile = Vector2.Zero;
                foreach (Vector2 c in openListF.Keys)
                {
                    if (openListF[c] <= shortest)
                    {
                        shortest = openListF[c];
                        cTile = c;
                    }
                }
                closedList.Add(cTile);
                openList.Remove(cTile);
                openListF.Remove(cTile);
                for (int i = (int)cTile.X - gridSize; i <= (int)cTile.X + gridSize; i += gridSize)
                {
                    for (int j = (int)cTile.Y - gridSize; j <= (int)cTile.Y + gridSize; j += gridSize)
                    {
                        if (i >= dun.MinX && j >= dun.MinY && i <= dun.MaxX && j <= dun.MaxY)
                        {
                            Vector2 cNeighbour = new Vector2(i, j);
                            GridField cNT = null;
                            foreach (GridField g in Grid.Values) {
                                if (g.position.Contains(cNeighbour.X, cNeighbour.Y)) {
                                    cNT = g;
                                    break;
                                }
                            }
                            if (!closedList.Contains(cNeighbour))
                            {
                                bool blocked = false;
                                int g;
                                if ((i == cTile.X - gridSize || i == cTile.X + GridSize) && (j == cTile.Y - GridSize || j == cTile.Y + GridSize))
                                {
                                    blocked = true;
                                    g = 14;
                                    Vector2 p = new Vector2(cTile.X, j);
                                    Vector2 c = new Vector2(i, cTile.Y);
                                    GridField t = null;
                                    GridField k = null;
                                    bool foundT = false;
                                    bool foundK = false;
                                    foreach (GridField gF in Grid.Values)
                                    {
                                        if (gF.position.Contains(p))
                                        {
                                            t = gF;
                                            foundT = true;
                                        }
                                        if (gF.position.Contains(c))
                                        {
                                            k = gF;
                                            foundK = true;
                                        }
                                        if (foundT && foundK)
                                        {
                                            break;
                                        }
                                    }
                                    if ((p.X == cTile.X - gridSize && !t.enterExitRig) || (p.X == cTile.X + gridSize && !t.enterExitLef))
                                    {
                                        blocked = true;
                                    }
                                    if ((c.Y == cTile.Y - gridSize && !k.enterExitBot) || (c.Y == cTile.Y + gridSize && !k.enterExitTop))
                                    {
                                        blocked = true;
                                    }
                                }
                                else
                                {
                                    g = 10;
                                    if ((i == cTile.X - gridSize && !cNT.enterExitRig) || (i == cTile.X + gridSize && !cNT.enterExitLef) || (j == cTile.Y - gridSize && !cNT.enterExitBot) || (j == cTile.Y + gridSize && !cNT.enterExitTop)) {
                                        blocked = true;
                                        g = int.MaxValue;
                                    }
                                    
                                }
                                
                                int h = Heuristic(cNeighbour, end);
                                int f = g + h;
                                if (openList.ContainsKey(cNeighbour) && openList[cNeighbour] > g && !blocked)
                                {
                                    openList[cNeighbour] = g;
                                    openListF[cNeighbour] = f;
                                    parents[cNeighbour] = cTile;

                                }
                                else if (!openList.ContainsKey(cNeighbour) && !blocked)
                                {
                                    openList.Add(cNeighbour, g);
                                    openListF.Add(cNeighbour, f);
                                    parents.Add(cNeighbour, cTile);
                                }
                            }
                        }

                    }
                }
            }
            List<Vector2> path = new List<Vector2>();
            if (closedList.Contains(end))
            {
                path.Add(endPoint);
                path.Add(end);
                if (end != start){
                    Vector2 cTile = parents[end];
                    while (cTile != start)
                    {
                        path.Add(cTile);
                        cTile = parents[cTile];
                    }
                }
                
            }
            path.Reverse();
            return path;

        }

        private int Heuristic(Vector2 cNeighbour, Vector2 end)
        {
            int dx = (int)Math.Abs(cNeighbour.X - end.X);
            int dy = (int)Math.Abs(cNeighbour.Y - end.Y);
            return dy + dx;
        }

        public void AlertEnemiesInRadius(Vector2 position, float radius)
        {
            List<Enemy> enemies = new List<Enemy>();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.Tag=="Enemy")
                {
                    Enemy enemy = (Enemy)gameObject.GetComponent<Enemy>();
                    enemies.Add(enemy);
                }
            }

            foreach (Enemy enemy in enemies)
            {
                float dist = Vector2.Distance(enemy.GameObject.Transform.Position,position);
                if (dist<radius && enemy.Behaviour != ENEMYBEHAVIOUR.ALERT)
                {
                    enemy.Suspicion += radius / 5;
                    
                }
            }

        }

        
        public void UnloadDungeonFromDistance()
        {
            foreach (GameObject go in gameObjects)
            {
                if (go.Tag=="dungeon"||go.Tag=="Enemy")
                {
                    float dist = Vector2.Distance(player.Transform.Position,go.Transform.Position);
                    if (dist > 1300)
                    {
                        go.Enabled = false;
                    }
                    else
                    {
                        go.Enabled = true;
                    }
                }
            }
        }
        
    }
}