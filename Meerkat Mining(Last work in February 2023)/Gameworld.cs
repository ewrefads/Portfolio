using Meerkat_Mining.Builderpattern;
using Meerkat_Mining.Components;
using Meerkat_Mining.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Meerkat_Mining
{
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Vector2 screenSize;
        private static GameWorld instance;
        public SpriteFont defaultFont;

        private List<GameObject> gameObjects = new List<GameObject>();

        private List<GameObject> newGameObjects = new List<GameObject>();

        private List<GameObject> destroyedGameObjects = new List<GameObject>();

        private List<GameObject> uiObjects = new List<GameObject>();

        private List<GameObject> newuiObjects = new List<GameObject>();

        private List<GameObject> destroyeduiObjects = new List<GameObject>();


        public Vector2[,] grid;
        public GameObject[,] blocks;

        public Camera camera;

        // Liste af GameObject colliders. 
        public List<Collider> Colliders { get; private set; } = new List<Collider>();


        public static float DeltaTime { get; private set; }
        
        public static Vector2 ScreenSize { get; private set; }

        public static GameWorld Instance {
            get
            {
                if (instance == null)
                {
                    instance = new GameWorld();
                }
                return instance;
            }
        }

        public List<GameObject> NewuiObjects { get => newuiObjects;}
        public List<GameObject> DestroyeduiObjects { get => destroyeduiObjects;}
        public List<GameObject> NewGameObjects { get => newGameObjects;}
        public List<GameObject> DestroyedGameObjects { get => destroyedGameObjects;}

        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            

            //_graphics.IsFullScreen = true;

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Director director = new Director(new PlayerBuilder());
            gameObjects.Add(director.Construct());


            Director director2 = new Director(new DrillBuilder());
            gameObjects.Add(director2.Construct());

            Player p = (Player)FindObjectOfType<Player>();
            p.drill = FindObjectOfType<Drill>().GameObject;


            Drill d = (Drill)FindObjectOfType<Drill>();
            d.player = p;


            ScreenSize = new Vector2(800,600);
            _graphics.PreferredBackBufferWidth = (int)ScreenSize.X;
            _graphics.PreferredBackBufferHeight = (int)ScreenSize.Y;

            //sætter størrelsen af spillets vindue
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;


            _graphics.ToggleFullScreen();


            //definere størrelsen i vores static variable screenSize til brug i andre klasser
            screenSize.X = _graphics.PreferredBackBufferWidth;
            screenSize.Y = _graphics.PreferredBackBufferHeight;

            _graphics.ApplyChanges();

            GameObject s = new ShopFactory().Create(null);
            Shop shop = (Shop)s.GetComponent<Shop>();
            shop.Initiate(p);
            gameObjects.Add(s);

            //TerrainGenerator.Instance.sizeW = 5;
            //TerrainGenerator.Instance.sizeH = 5;

            /*blocks = TerrainGenerator.Instance.Create(5, 5);
            grid = new Vector2[blocks.Length,blocks.Length];*/



            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Awake();
            }

            camera = new Camera();

            base.Initialize();


        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load lyde til spil
            GameAudio.LoadAudio(Content);


            // Terrain opbygning

            blocks = TerrainGenerator.Instance.Create(90, 70);
            grid = new Vector2[blocks.GetLength(0), blocks.GetLength(1)];
            
            //tilføjer alle blocks i vores terrain til 2D block arrayet


            for (int a = 0; a < 90; a++)

            {
                for (int b = 0; b < 70; b++)
                {
                    gameObjects.Add(blocks[a, b]);
                    grid[a,b] = blocks[a, b].Transform.Position;
                   
                }
            }

            for (int i = 0; i < 90; i++)
            {
                for (int j = 0; j < 70; j++)
                {
                    
                    GameObject go = BlockFactory.Instance.Create(BLOCKTYPE.STONE);

                    if (j==0)
                    {
                        go = BlockFactory.Instance.Create(BLOCKTYPE.GRASS);
                    }

                    go.RemoveComponent((go.GetComponent<Block>() as Block));
                    go.Transform.Position = grid[i,j];
                    (go.GetComponent<SpriteRenderer>() as SpriteRenderer).LayerDepth = 0;
                    (go.GetComponent<SpriteRenderer>() as SpriteRenderer).Color = Color.Tan ;
                    gameObjects.Add(go);
                }
            }

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Start();
            }

            defaultFont = Content.Load<SpriteFont>("Standard");

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            // TODO: Add your update logic here
            camera.Update(FindObjectOfType<Player>().GameObject);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Update();
            }

            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Update();
            }

            Cleanup();

            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.transform, sortMode: SpriteSortMode.FrontToBack) ;

            // TODO: Add your drawing code here
            for (int i = 0; i < gameObjects.Count; i++)
            {
                gameObjects[i].Draw(_spriteBatch);
            }

            _spriteBatch.End();

            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

            // TODO: Add your drawing code here
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public Component FindObjectOfType<T>() where T : Component
        {
            foreach (GameObject gameObject in gameObjects)
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
            NewuiObjects.Add(go);
        }

        public void Destroy(GameObject go)
        {
            DestroyedGameObjects.Add(go);
        }

        private void Cleanup()
        {
            for (int i = 0; i < NewGameObjects.Count; i++)
            {
                gameObjects.Add(NewGameObjects[i]);
                NewGameObjects[i].Awake();
                NewGameObjects[i].Start();

                

            }

            for (int i = 0; i < DestroyedGameObjects.Count; i++)
            {

                
                gameObjects.Remove(DestroyedGameObjects[i]);

                
            }
            DestroyedGameObjects.Clear();
            NewGameObjects.Clear();
            for (int i = 0; i < newuiObjects.Count; i++)
            {
                uiObjects.Add(newuiObjects[i]);
                newuiObjects[i].Awake();
                newuiObjects[i].Start();



            }

            for (int i = 0; i < destroyeduiObjects.Count; i++)
            {


                uiObjects.Remove(destroyeduiObjects[i]);


            }
            destroyeduiObjects.Clear();
            newuiObjects.Clear();
        }

    }
}