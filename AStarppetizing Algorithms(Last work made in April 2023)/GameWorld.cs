using AStarppetizing_Algorithms.Builder;
﻿using AStarppetizing_Algorithms.Components;
using AStarppetizing_Algorithms.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AStarppetizing_Algorithms
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

        // Fields for tidsbaseret score
        private float timeScore;
        private bool counting=true;

        private GameObject startPoint;
        private GameObject endPoint;
        private Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();

        private Color backgroundColor = Color.DimGray;
        private List<CodeBlock> referenceCodeBlocks = new List<CodeBlock>();


        //liste over levels
        private List<Level> levels = new List<Level>();

        //hvilke level er vi på nu
        private int currenLevel=1;
        //spiller objektet i levelet
        private Player player;



        //Gameobjects
        private List<GameObject> gameObjects = new List<GameObject>();

        private List<GameObject> newGameObjects = new List<GameObject>();

        private List<GameObject> destroyedGameObjects = new List<GameObject>();

        //UI ELEMENTS
        private List<GameObject> uiObjects = new List<GameObject>();

        private List<GameObject> newUIObjects = new List<GameObject>();

        private List<GameObject> destroyeduiObjects = new List<GameObject>();

        // Menu button list
        private List<GameObject> menuButtons = new List<GameObject>();

        // Level select button list
        private List<GameObject> levelButtons = new List<GameObject>();

        public static float DeltaTime { get; private set; }

        private CodeManager codeManager = CodeManager.Instance;
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
        public List<Collider> Colliders { get; private set; } = new List<Collider>();
        public static Vector2 ScreenSize { get => screenSize; set => screenSize = value; }

        public List<GameObject> NewGameObjects { get => newGameObjects; }
        public List<GameObject> DestroyedGameObjects { get => destroyedGameObjects; }
        public GraphicsDeviceManager Graphics { get => _graphics; }

        public List<GameObject> GameObjects { get => gameObjects; }

        public List<GameObject> DestroyeduiObjects { get => destroyeduiObjects; }
        public List<GameObject> NewUIObjects { get => newUIObjects; }

        public Color BackgroundColor { get => backgroundColor; set => backgroundColor = value; }
        public GameObject StartPoint { get; internal set; }
        public List<Level> Levels { get => levels; }
        public int CurrenLevel { get => currenLevel; }
        public Dictionary<Vector2, GameObject> Tiles { get => tiles; set => tiles = value; }
        public GameObject EndPoint { get => endPoint; set => endPoint = value; }
        internal Player Player { get => player; }

        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {



            // TODO: Add your initialization logic here
            // Aspect ratio 16:9
            Graphics.PreferredBackBufferWidth = 1800;
            Graphics.PreferredBackBufferHeight = 900;


            screenSize.X = Graphics.PreferredBackBufferWidth;
            screenSize.Y = Graphics.PreferredBackBufferHeight;

            Graphics.ApplyChanges();


            

            GameObject b;
            Button but;
            CodeBlock c;
            SpriteRenderer sr;

            //Genereing af knapper til at lave codeblocks
            int x = 0;
            int y = 0;
            foreach (CODEBLOCKTYPES type in Enum.GetValues(typeof(CODEBLOCKTYPES)))
            {
                if (type != CODEBLOCKTYPES.empty || type != CODEBLOCKTYPES.checkNeighboursEnd)
                {
                    b = UIFactory.Instance.Create(UITYPE.BUTTON);
                    c = new CodeBlock();
                    c.Type = type;
                    but = (Button)b.GetComponent<Button>();
                    bool validBlock = true;
                    switch (type)
                    {
                        case CODEBLOCKTYPES.createLists:
                            c.Method = CodeManager.Instance.CreateLists;
                            but.ButtonText = "Create Lists";
                            break;
                        case CODEBLOCKTYPES.findShortestDistance:
                            c.Method = CodeManager.Instance.FindShortestDistance;
                            but.ButtonText = "Find Shortest Distance";
                            break;
                        case CODEBLOCKTYPES.moveToClosedList:
                            c.Method = CodeManager.Instance.MoveToClosedList;
                            but.ButtonText = "Move to closedList";
                            break;
                        case CODEBLOCKTYPES.checkNeighbours:
                            c.Method = CodeManager.Instance.CheckNeighbours;
                            but.ButtonText = "Check Neighbours";
                            break;
                        case CODEBLOCKTYPES.isGoalReached:
                            c.Method = CodeManager.Instance.IsGoalReached;
                            but.ButtonText = "Is Goal Reached";
                            break;
                        case CODEBLOCKTYPES.isOnClosedList:
                            c.Method = CodeManager.Instance.IsOnClosedList;
                            but.ButtonText = "Is On Closed List";
                            break;
                        case CODEBLOCKTYPES.isOnOpenList:
                            c.Method = CodeManager.Instance.IsOnOpenList;
                            but.ButtonText = "Is On Open List";
                            break;
                        case CODEBLOCKTYPES.checkForObstacles:
                            c.Method = CodeManager.Instance.checkForObstacles;
                            but.ButtonText = "Check For Obstacles";
                            break;
                        case CODEBLOCKTYPES.setCurrentNodeAsParent:
                            c.Method = CodeManager.Instance.SetCurrentNodeAsParent;
                            but.ButtonText = "Set Current Node as Parent";
                            break;
                        case CODEBLOCKTYPES.addToOpenList:
                            c.Method = CodeManager.Instance.AddToOpenList;
                            but.ButtonText = "Add to open list";
                            break;
                        case CODEBLOCKTYPES.calculateF:
                            c.Method = CodeManager.Instance.CalculateF;
                            but.ButtonText = "Calculate F";
                            break;
                        case CODEBLOCKTYPES.calculateG:
                            c.Method = CodeManager.Instance.CalculateG;
                            but.ButtonText = "Calculate G";
                            break;
                        case CODEBLOCKTYPES.calculateH:
                            c.Method = CodeManager.Instance.CalculateH;
                            but.ButtonText = "Calculate H";
                            break;
                        case CODEBLOCKTYPES.changeParent:
                            c.Method = CodeManager.Instance.ChangeParent;
                            but.ButtonText = "Change Parent";
                            break;
                        case CODEBLOCKTYPES.isOpenListEmpty:
                            c.Method = CodeManager.Instance.IsOpenListEmpty;
                            but.ButtonText = "Is Open List Empty";
                            break;
                        default:
                            but.ButtonText = "Not Implemented";
                            validBlock = false;
                            break;
                    }



                    SpriteRenderer spr = (SpriteRenderer)b.GetComponent<SpriteRenderer>();
                    //spr.LayerDepth = 1;
                    referenceCodeBlocks.Add(c);
                    but.OnClick = c.newCodeBlock;
                    if (validBlock)
                    {
                        b.Transform.Position = new Vector2(1190 + x * (spr.Sprite.Width * spr.Scale + 30), 60 + y * (spr.Sprite.Height * spr.Scale));
                        y++;
                        if (40 + y * (spr.Sprite.Height * spr.Scale) > screenSize.Y - 10 * (spr.Sprite.Height * spr.Scale))
                        {
                            x++;
                            y = 0;
                        }
                        uiObjects.Add(b);
                    }
                }

            }

                //Knap til at køre koden med
                //CodeManager.Instance.DisplayMode = true;
                b = UIFactory.Instance.Create(UITYPE.BUTTON);
                but = (Button)b.GetComponent<Button>();
                but.ButtonText = "Run code";
                but.OnClick = CodeManager.Instance.RunCode;

                b.Transform.Position = new Vector2(ScreenSize.X * 0.65f, 700);
                uiObjects.Add(b);



                b = UIFactory.Instance.Create(UITYPE.BUTTON);
                but = (Button)b.GetComponent<Button>();
                but.ButtonText = "To main menu";
                but.OnClick = Placeholder; // Luk nuværende level ned og åben menu (Mangler metode)
                b.Transform.Position = new Vector2(screenSize.X * 0.75f, 800);
                uiObjects.Add(b);


                currenLevel = 0;

                InitializeMenu();

                //main gameplay panel
                GameObject panel1 = new GameObject();
                panel1.AddComponent(new Panel());
                SpriteRenderer panel1spr = (SpriteRenderer)panel1.AddComponent(new SpriteRenderer());
                uiObjects.Add(panel1);
                panel1.Transform.Position = new Vector2(500, 250);
                panel1spr.Scale = 4;
                panel1spr.LayerDepth = 0.8f;

                //Code blocks panel
                GameObject panel2 = new GameObject();
                panel2.AddComponent(new Panel());
                SpriteRenderer panel2spr = (SpriteRenderer)panel2.AddComponent(new SpriteRenderer());
                uiObjects.Add(panel2);
                panel2.Transform.Position = new Vector2(1250, 310);
                panel2spr.Scale = 2.5f;
                panel2spr.rotation = 1.575f;

                //blockcode panel
                GameObject panel3 = new GameObject();
                panel3.AddComponent(new Panel());
                SpriteRenderer panel3spr = (SpriteRenderer)panel3.AddComponent(new SpriteRenderer());
                uiObjects.Add(panel3);
                panel3.Transform.Position = new Vector2(1560, 310);
                panel3spr.Scale = 2.5f;
                panel3spr.rotation = 1.575f;

                //text display panel
                GameObject panel4 = new GameObject();
                panel4.AddComponent(new Panel());
                SpriteRenderer panel4spr = (SpriteRenderer)panel4.AddComponent(new SpriteRenderer());
                uiObjects.Add(panel4);
                panel4.Transform.Position = new Vector2(500, 740);
                panel4spr.Scale = 4;

                //buttons panel
                GameObject panel5 = new GameObject();
                panel5.AddComponent(new Panel());
                SpriteRenderer panel5spr = (SpriteRenderer)panel5.AddComponent(new SpriteRenderer());
                uiObjects.Add(panel5);
                panel5.Transform.Position = new Vector2(1400, 760);
                panel5spr.Scale = 2.5f;



                for (int i = 0; i < gameObjects.Count; i++)
                {
                    GameObjects[i].Awake();
                }

                for (int i = 0; i < uiObjects.Count; i++)
                {
                    uiObjects[i].Awake();
                }

                base.Initialize();
            
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            defaultFont = Content.Load<SpriteFont>("default");


            //setting up levels

            //level 1
            Level lvl1 = new Level(
                new Vector2(10, 10), 
                new Vector2[] { new Vector2(5, 5), new Vector2(5, 4),new Vector2(5, 3), new Vector2(5, 2), new Vector2(5, 6) }, 
                new Vector2[] { new Vector2(4, 4) }, 
                new INGREDIENTTYPE[] { INGREDIENTTYPE.Egg });
            levels.Add(lvl1);

            //tilføjer de tiles som er i level1 til gameobjects listen så de bliver spawnet ind
            foreach (GameObject gameObject in lvl1.tiles)
            {
                
                gameObjects.Add(gameObject);
            }

            //level 2
            Level lvl2 = new Level(new Vector2(10, 10),
                new Vector2[] { new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 2), new Vector2(4, 4), new Vector2(4, 5), new Vector2(4, 6), new Vector2(5, 6) },
                new Vector2[] { new Vector2(0, 0), new Vector2(6, 6) },
                new INGREDIENTTYPE[] { INGREDIENTTYPE.Cucumber, INGREDIENTTYPE.Tomato });
            levels.Add(lvl2);

            

            //level 3
            Level lvl3 = new Level(new Vector2(10, 10),
                new Vector2[] { new Vector2(3, 1), new Vector2(4, 0), new Vector2(5, 0), new Vector2(6, 0), new Vector2(7, 1), new Vector2(7, 2), new Vector2(7, 3), new Vector2(6, 4), new Vector2(5, 4), new Vector2(4, 4), new Vector2(7, 5), new Vector2(7, 6), new Vector2(7, 7), new Vector2(6, 8), new Vector2(5, 8), new Vector2(4, 8), new Vector2(3, 7) },
                new Vector2[] { new Vector2(5, 2), new Vector2(5, 6), new Vector2(8, 4) },
                new INGREDIENTTYPE[] { INGREDIENTTYPE.Egg, INGREDIENTTYPE.Cheese, INGREDIENTTYPE.Flour });
            levels.Add(lvl3);

            //spawns background
            /*for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    GameObject bg = new GameObject();
                    SpriteRenderer sr = (SpriteRenderer) bg.AddComponent(new SpriteRenderer());
                    sr.SetSprite("tilefloor");
                    sr.Scale = 1.5f;
                    bg.Transform.Position = levels[currenLevel].tiles[x, y].Transform.Position;
                    gameObjects.Add(bg);
                }
            }*/


            // Level select knapper
            int count = 0;
            foreach (var level in levels) //levels i collectionn
            {

                GameObject levelBtn = UIFactory.Instance.Create(UITYPE.MENUBUTTON);
                Button btn = (Button)levelBtn.GetComponent<Button>();
                btn.ButtonText = "level" + (++count);
                btn.Index = count;
                btn.isOnClick = false;
                btn.OnClickParam = new Button.buttonMethodParam(ChangeLevel);
                levelBtn.Transform.Position = new Vector2(ScreenSize.X * 0.5f, 100*count);
                uiObjects.Add(levelBtn);

                levelButtons.Add(levelBtn);

            }

            //player

            //sætter spiller objektet op i levelet
            GameObject playerchar = new GameObject();
            player = (Player)playerchar.AddComponent(new Player());
            playerchar.AddComponent(new SpriteRenderer());
            playerchar.AddComponent(new Animator());
            player.GridPosition = new Vector2(2, 2);
            player.GameObject.Transform.Position = levels[currenLevel].tiles[2, 2].Transform.Position;
            gameObjects.Add(Player.GameObject);


            defaultFont = Content.Load<SpriteFont>("default");
            defaultFont = Content.Load<SpriteFont>("spriteFont00");

            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Start();
            }

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Start();
            }

            
            //ChangeLevel();


            // TODO: use this.Content to load your game content here
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            codeManager.Update(gameTime);
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Update(gameTime);
            }
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Update(gameTime);
            }
            Cleanup();

            if (counting) // Når man er i gang med et level så tæl tid for score
            {
                timeScore += DeltaTime;
            }

            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(backgroundColor);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
            
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Draw(_spriteBatch); 
            }
            CodeManager.Instance.Draw(_spriteBatch);
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Draw(_spriteBatch);
            }

            if (counting) // Display noget tekst der viser tid når man er i gang med et level
            {
                _spriteBatch.DrawString(defaultFont,"Time: " + Convert.ToInt32(timeScore) + " sec.", new Vector2(screenSize.X*0.4f,screenSize.Y*0.03f), Color.Blue, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
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
            //tiles.Add(go.Transform.Position,go);
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

            for (int i = 0; i < DestroyeduiObjects.Count; i++)
            {


                uiObjects.Remove(DestroyeduiObjects[i]);


            }
            DestroyeduiObjects.Clear();
            NewUIObjects.Clear();
        }


        //- - - - - - - - - - - - BUTTONMETHODS - - - - - - - - - - - - 
        private void Placeholder() // Placeholder for button click event with unimplmeted method
        {
            foreach (GameObject btn in menuButtons)
            {
                // Flytter menu knapper væk fra skærmen (Husk at tilføje en metode sonm rykker dem tilbage igen når der er brug for dem)
                btn.Transform.Position += new Vector2(Graphics.PreferredBackBufferWidth*2f,0); 
            }
        }

        private void ExitProgram() // En metode som lukker programmet ned
        {
            System.Environment.Exit(0);
        }



        /// <summary>
        /// En metode for at forkorte Initialize metoden.
        /// </summary>
        private void InitializeMenu()
        {
            

            // Main menu knapper
            GameObject levelSelect = UIFactory.Instance.Create(UITYPE.MENUBUTTON);
            
            Button b = (Button)levelSelect.GetComponent<Button>();
            b.ButtonText = "Levels";
            b.OnClick += /*Insert method here*/Placeholder; // Method to open a list of levelbuttons
            levelSelect.Transform.Position = new Vector2(ScreenSize.X * 0.85f, 700);
            uiObjects.Add(levelSelect);
            menuButtons.Add(levelSelect);


            GameObject exitButton = UIFactory.Instance.Create(UITYPE.MENUBUTTON);
            b = (Button)exitButton.GetComponent<Button>();
            b.ButtonText = "Exit program";
            b.OnClick = ExitProgram; // Method to exit program
            exitButton.Transform.Position = new Vector2(ScreenSize.X * 0.85f, 800);
            uiObjects.Add(exitButton);
            menuButtons.Add(exitButton);
            

            // Level buttons

            List<GameObject> levelButtons = new List<GameObject>();
            
            //Vector2 position = new Vector2();
            
            
        }
        //- - - - - - - - - - - - END OF BUTTONMETHODS - - - - - - - - - - - - 



        /// <summary>
        /// Method til at skifte levels i programmet. Kendte bugs: Kan ikke skifte til level 1 (De andre levels virker helt fint))
        /// </summary>
        /// <param name="nmb"></param>
        public void ChangeLevel(int nmb)

        {
            if (currenLevel != 0)
            {
                // Sæt counting til false

            }


            //destorys old level
            foreach (GameObject gameObject in Levels[CurrenLevel].tiles)
            {
                destroyedGameObjects.Add(gameObject);
            }

            currenLevel = nmb;
            

            //checker om levelet kan loades
            if (currenLevel < levels.Count && currenLevel > 0) {

                //spawns new level
                foreach (GameObject gameObject in levels[currenLevel].tiles)
                {
                    Instantiate(gameObject);
                }

                // Start timer
                //method

            }
            //hvis levelet ikke kan loades så loades menuen
            else
            {
                //spawns menu


            }
        }

    }
}
