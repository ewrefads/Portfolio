using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SharpDX.Direct3D9;


using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using SamplerState = Microsoft.Xna.Framework.Graphics.SamplerState;

namespace Chaos_in_Colosseum
{   
    /// <summary>
    /// Vores spil verden hvor alt koden bliver udført fra og styrer de forskellige GameObjects
    /// </summary>
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        //Alle vores GameObjekter samles i gaemObjects listen
        private static List<GameObject> gameObjects;

        //En liste over de GameObjecter der skal tilføjes til gameObjects listen så der undgås fejl
        private static List<GameObject> gameObjectsToAdd;

        //Array over Enemysprites som bruges til at loade enemy sprites
        private Texture2D[] defaultEnemySprites;
        
        //Array over boss1Spriites som bruges til at loade boss1 sprites
        private Texture2D[] boss1Sprites;
        
        //holder styr på vores spiller objekt

        private Player player;
        //holder styr på vores camera objekt
        private Camera camera;

        //Beskriver hvor stort spillets vindue er
        public static Vector2 screenSize;
        //beskriver hvor stort et spil område spillet foregår på
        public static Vector2 worldBounds;

        //hvor mange enemies der er spawned ind i verden
        private int enemiesSpawned = 0;

        
        private SpriteFont debugFont;

        //Tekst som viser hvilken wave vi er nået til
        private SpriteFont waveCounter;
        //int der holder styr på hvilken wave spilleren er nået til
        private int waveNumber;

        //healthbar sprites (baggrund og forgrund)
        private Texture2D standardHealthBar;
        private Texture2D standardHealthBarElement;

        private Texture2D standardCoolDownBar;

        private Texture2D pixel;

        public static List<GameObject> GetGameObjects
        {
            get { return gameObjects; }
        }

        
        public GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameObjects = new List<GameObject>();
            defaultEnemySprites = new Texture2D[9];
            boss1Sprites = new Texture2D[4];
            gameObjectsToAdd = new List<GameObject>();
            
            IsMouseVisible = true;
            
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            
        

            //sætter størrelsen af spillets vindue
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;
            //definere størrelsen i vores static variable screenSize til brug i andre klasser
            screenSize.X = _graphics.PreferredBackBufferWidth;
            screenSize.Y = _graphics.PreferredBackBufferHeight;

            _graphics.ApplyChanges();
            float speed = 10f;
            

            //laver et nyt Kamera objekt
            camera = new Camera();

            //laver en ny spiller objekt
            string[] playerAssets = { "player_facing_down1", "player_facing_down2", "player_facing_down3", "player_facing_down4" };
            player = new Player(playerAssets, new Vector2(screenSize.X / 2, screenSize.Y * 0.75f), 1000);
            

            
            //yilføjer en spiller til vores liste over GameObjects
            gameObjects.Add(player);

            base.Initialize();
            
        }




        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>("font");

            //loader content for alle GameoObjekter i verdenen
            foreach (GameObject go in gameObjects) {
                go.LoadContent(Content);
            }

            //loader textures og fonts
            standardHealthBar = Content.Load<Texture2D>("HealthBar");
            standardHealthBarElement = Content.Load<Texture2D>("HealthBarElement");
            standardCoolDownBar = Content.Load<Texture2D>("coolDownBar");
            waveCounter = Content.Load<SpriteFont>("font");
            pixel = Content.Load<Texture2D>("pixel");


            //loader enemySprites

            for(int i = 0; i < defaultEnemySprites.Length; i++)
            {
                int num = i + 1;
                defaultEnemySprites[i] = Content.Load<Texture2D>("wormSprites/worm" + num);
            }

            // Textures for Boss 1
            for (int i = 0; i < boss1Sprites.Length; i++)
            {
                int num = i + 1;
                boss1Sprites[i] = Content.Load<Texture2D>("boss" + num);
            }

            camera.LoadContent(Content, player);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //slutter applicationen hvis vi trykker Escape
            
            

            // TODO: Add your update logic here
            foreach (GameObject go in gameObjectsToAdd)
            {
                go.LoadContent(Content);
                gameObjects.Add(go);
            }
            //camera.FollowPlayer(player);
            //camera.Move(gameTime);
            gameObjectsToAdd.Clear();

            //opdatere alle GameObjekter i spillet
            foreach (GameObject go in gameObjects) {
                go.Update(gameTime);
                //checker efter collisioner mellem alle GameObjekter i spillet
                foreach (GameObject other in gameObjects)
                {
                    if (go.IsColliding(other))
                    {
                        go.OnCollision(other);
                        other.OnCollision(go);
                        //break hvis der er en collision så det samme objekt ikke kollidere flere gange
                        break;
                    }
                }
            }


            /*if (player.Position.X < player.spriteSize.X / 2 + 40)
            {
                player.Position = new Vector2(player.spriteSize.X / 2 + 40, player.Position.Y);
                player.CurrentWeapon.Position = new Vector2(player.spriteSize.X / 2 + 40 + 45, player.Position.Y + 45);
            }
            if (player.Position.X > _graphics.PreferredBackBufferWidth - player.spriteSize.X / 2 - 50)
            {
                player.Position = new Vector2(_graphics.PreferredBackBufferWidth - player.spriteSize.X / 2 - 50, player.Position.Y);
                player.CurrentWeapon.Position = new Vector2(_graphics.PreferredBackBufferWidth - player.spriteSize.X / 2 - 50 + 45, player.Position.Y + 45);
            }*/


            player.Position = new Vector2( Math.Clamp(player.Position.X,camera.rectangle.X +600, camera.rectangle.Width -1700),player.Position.Y);

            //checker hvor mange enemies der er i vores spil lige nu
            int enemyCount=0;
            foreach (GameObject item in gameObjects)
            {
                if (item is Enemy)
                {
                    enemyCount++;
                }
            }
            
            //hvis alle fjender er døde så begynder den næste wave
            Random rnd = new Random();
            if (enemyCount < 1)
            {
                WaveManager(new int[]{1,2,3}, new int[] {2 , 2,2});
                enemiesSpawned++;
            }
            

            //opdatere Camera position
            foreach (GameObject item in gameObjects)
            {
                if (item is Player)
                {
                    camera.Update((Player)item);
                }

            }

            //får keyboard state og checker om vi trykker R, hvis ja så restart spillet
            KeyboardState keyState = Keyboard.GetState();
            //restart game
            if (keyState.IsKeyDown(Keys.R))
            {
                RestartGame();
            }

            //fjerner alle GameObjects som burde fjernes
            RemoveGameObjects();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //blå baggrund
            GraphicsDevice.Clear(Color.CornflowerBlue);
            

            //vi tegner vores Camera ud fra den position Camera objektet har
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp,transformMatrix: camera.transform);


            
            _spriteBatch.Draw(camera.sprite, camera.rectangle, Color.White);

            
            //viser runde nummeret som spilleren er nået til
            _spriteBatch.DrawString(waveCounter, "WAVE: "+waveNumber, new Vector2(player.Position.X - 200, 50),Color.White,0,Vector2.Zero, 2, SpriteEffects.None,1);

            //checker hvor mange enemies der er tilbage
            int enemyCount = 0;
            foreach (GameObject item in gameObjects)
            {
                if (item is Enemy)
                {
                    enemyCount++;
                }
            }

            //viser hvor mange fjender der stadig er i live på banen
            _spriteBatch.DrawString(waveCounter, "ENEMIES LEFT: "+enemyCount, new Vector2(player.Position.X - 200,150),Color.White,0,Vector2.Zero, 2, SpriteEffects.None,1);


            //checker om spilleren er i live
            bool playerAlive = false;
            foreach (GameObject item in gameObjects)
            {
                if (item is Player)
                {
                    playerAlive = true;
                }
            }

            //Hvis spilleren er død så tegn en string der siger spilleren er død
            if (!playerAlive)
            {
                _spriteBatch.DrawString(waveCounter, "YOU ARE DEAD! RESTART(R)", new Vector2(player.Position.X - 400, 400), Color.Red, 0, Vector2.Zero, 5, SpriteEffects.None, 1);
                
            }
            
            //_spriteBatch.DrawString(debugFont, "Last attack time: " + player.LastAttackTime, Vector2.Zero, Color.Black);
            //_spriteBatch.DrawString(debugFont, "game objects: " + gameObjects.Count, new Vector2(0, 13), Color.Black);
            // TODO: Add your drawing code here
            foreach (GameObject go in gameObjects) {
                go.Draw(_spriteBatch);
                //DrawCollisionBox(go);
            }
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        /// <summary>
        /// Spawner en enemy type på en position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="type"></param>
        public void spawnEnemy(Vector2 position, int type) {

            gameObjects.Add(new Enemy(defaultEnemySprites, position, type, 1, 1, standardHealthBar, standardHealthBarElement, standardCoolDownBar));

        }

        /// <summary>
        /// tyrer hvor mange enemies der skal spawne i en ny wave
        /// </summary>
        /// <param name="types">types definere hvilken våben fjenderne spawner med</param>
        /// <param name="nums">nums definere hvor mange der spawner af hver type</param>
        /// <exception cref="Exception"></exception>
        public void WaveManager(int[] types, int[] nums) {
            waveNumber++;

            //hvis begge arrays ikke er lige lange så kan vores nested forloop ikke køre. derfor smid en fejl istedet
            if (types.Length != nums.Length) {
                throw new Exception("types and nums must be the same length");
            }
            //Spawner de forskellige typer af enemies og antallet er baseret på hvilke wave nummer vi er på
            Random rnd = new Random();
            for (int i = 0; i < types.Length; i++) {
                for (int j = 0; j < nums[i] * waveNumber; j++) {
                    //vi spawner en enemy på en tilfædlig x position
                    spawnEnemy(new Vector2(_graphics.PreferredBackBufferWidth + rnd.Next(0, 4500), screenSize.Y * 0.75f), types[i]);
                }
            }
        }
        /// <summary>
        /// Kan kaldes alle steder i projektet og instantiere et nyt GameObjekt i vores verden
        /// </summary>
        /// <param name="go"></param>
        public static void InstantiateGameObject(GameObject go){
            gameObjectsToAdd.Add(go);
        }
        /// <summary>
        /// Fjerner alle GameObjects hvor boolen shouldRemove er sandt
        /// </summary>
        private void RemoveGameObjects() {
            List<GameObject> gameObjectsToRemove = new List<GameObject>();
            foreach (GameObject go in gameObjects) {
                if (go.ShouldRemove) {
                    gameObjectsToRemove.Add(go);
                }
            }
            foreach (GameObject goTR in gameObjectsToRemove) {
                gameObjects.Remove(goTR);
            }
        }

       

        /// <summary>
        /// Restarter spillet ved at slette alle GameObjekter og resete waveNumber og spawner en ny spiller ind
        /// </summary>
        public void RestartGame()
        {

            //slet alle GaemoObjects
            foreach (GameObject gameobject in gameObjects)
            {
                gameobject.ShouldRemove = true;
            }
            gameObjects.Clear();

            //laver ny spiller
            string[] playerAssets = { "player_facing_down1", "player_facing_down2", "player_facing_down3", "player_facing_down4" };
            player = new Player(playerAssets, new Vector2(screenSize.X / 2, screenSize.Y * 0.75f), 1000);

            gameObjects.Add(player);

            //loader content for de nye objekter
            foreach (GameObject go in gameObjects)
            {
                go.LoadContent(Content);
            }

            //waven reseter
            waveNumber = 0;
        }

        /// <summary>
        /// Tegner collisionBoxe rundt om alle sprites (bruges kun til at bug fixe)
        /// </summary>
        /// <param name="go"></param>
        private void DrawCollisionBox(GameObject go)
        {
            Rectangle top = new Rectangle(go.CollisionBox.X, go.CollisionBox.Y, go.CollisionBox.Width, 1);
            Rectangle bottom = new Rectangle(go.CollisionBox.X, go.CollisionBox.Y + go.CollisionBox.Height, go.CollisionBox.Width, 1);
            Rectangle left = new Rectangle(go.CollisionBox.X, go.CollisionBox.Y, 1, go.CollisionBox.Height);
            Rectangle right = new Rectangle(go.CollisionBox.X + go.CollisionBox.Width, go.CollisionBox.Y, 1, go.CollisionBox.Height);

            _spriteBatch.Draw(pixel, top, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.Draw(pixel, bottom, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.Draw(pixel, left, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            _spriteBatch.Draw(pixel, right, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
    }
}