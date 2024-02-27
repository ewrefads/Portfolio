using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;
//using SamplerState = SharpDX.Direct3D9.SamplerState;

namespace TrainTD
{
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;


        //Alle vores GameObjekter samles i gaemObjects listen
        private static List<GameObject> gameObjects;

        //En liste over de GameObjecter der skal tilføjes til gameObjects listen så der undgås fejl
        private static List<GameObject> gameObjectsToAdd;

        //Beskriver hvor stort spillets vindue er
        public static Vector2 screenSize;
        //beskriver hvor stort et spil område spillet foregår på
        public static Vector2 worldBounds;

        //default spritefonten som bliver brugt i spillet
        private SpriteFont main;

        //kameraets som styrer positionen og zoom af hvor spilleren ser (blev ikke implimentered)
        private Camera camera;

        KeyboardState pad1, prevPad1;

        //en pixel sprite som bruges til debugging
        private Texture2D pixel;

        //baggrunden af banen
        private Texture2D background;

        //de forskellige hustyper
        private Texture2D bank;
        private Texture2D sheriff;
        private Texture2D toilet;
        private Texture2D barn;
        private Texture2D house;
        private Texture2D house2;
        private Texture2D house3;
        private Texture2D church;

        // Sprite for In-game info (Kul, Liv, Metal & Runde)
        private Texture2D gameInfo;

        //tog spriten
        private Texture2D locomotiveSprite;
        private SoundEffectInstance trainSound;
        private Texture2D coalCartSprite;
        private Texture2D[] smokeSprites;

        //forskellige tower sprites
        private Texture2D[] towerSprites;

        //button sprites som alle knapper bruger
        private Texture2D buttonSprite;
        private Texture2D passengerCartSprite;


        //pause spriten som vises når man pauser
        private Texture2D pause;

        //spriten for togskinnerne 
        private Texture2D straightTrackSprite;

        //menuen i starten af spillet
        private Texture2D titleScreen;

        //styrer hvilken fase spillerens tur er i
        private static int currentPhase;
        //checker hovrnår fasen ændre sig 
        private static bool phaseChanged;

        //samling af alle uielementer som knapper
        private static List<UIElement> uiElements;
        //ui elementer som venter på at blive spawned ind
        private static List<UIElement> uiElementsToAdd;

        //spiller objekterne (2 byer)
        private static Player[] players;
        //styrer hvilken spillers tur det er
        private static int currentPlayer;

        //liste over de forskellige tårne i spillet
        private List<Tower> towers;
        //checker om vi er forbi menuen
        private bool inGame;


        private static List<Track> tracks;

        public static List<GameObject> GetGameObjects
        {
            get { return gameObjects; }
        }

        // currentPhase property
        public static int CurrentPhase { get => currentPhase;}
        public static bool PhaseChanged { get => phaseChanged; set => phaseChanged = value; }

        public GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            gameObjects = new List<GameObject>();
            gameObjectsToAdd = new List<GameObject>();
            uiElements = new List<UIElement>();
            uiElementsToAdd = new List<UIElement>();
            towers = new List<Tower>();
            towerSprites = new Texture2D[2];
            players = new Player[2];
            tracks = new List<Track>();
            camera = new Camera();
            inGame = false;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //sætter størrelsen af spillets vindue
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            //definere størrelsen i vores static variable screenSize til brug i andre klasser
            screenSize.X = _graphics.PreferredBackBufferWidth;
            screenSize.Y = _graphics.PreferredBackBufferHeight;

            _graphics.ApplyChanges();

            worldBounds = new Vector2(-1000, 1000);

            currentPhase = 0;
            phaseChanged = false;
            currentPlayer = 0;
            String[] townSprites = { "smoke1", "smoke2", "smoke3", "smoke4", "smoke5", "smoke6", "smoke7" };
            players[0] = new Player(Vector2.Zero, townSprites, 1);
            players[1] = new Player(Vector2.Zero, townSprites, 1);

            gameObjects.Add(players[0]);


            //string[] towerSprites = { "tower1/ti00" };
            //gameObjects.Add(new Tower(players[currentPlayer],towerSprites));







            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Indlæser titlescrren baggrunden
            titleScreen = Content.Load<Texture2D>("TitleScreen");

            //Indlæser sprites for togVogne
            locomotiveSprite = Content.Load<Texture2D>("TrainSprites");
            coalCartSprite = Content.Load<Texture2D>("Gold cart");
            passengerCartSprite = Content.Load<Texture2D>("Passenger Cart");
            trainSound = Content.Load<SoundEffect>("bw_sfx/train1").CreateInstance();
            trainSound.Pitch = 1.0f;
            trainSound.Volume = 0.5f;
            trainSound.IsLooped = true;
            

            //Indlæser Sprite for knapper
            buttonSprite = Content.Load<Texture2D>("tempButton");

            //Indlæser pause ikonet
            pause = Content.Load<Texture2D>("pause");

            //Indlæser baggrunden
            background = Content.Load<Texture2D>("baggrund");


            //Indlæser husenes sprites
            bank = Content.Load<Texture2D>("buildings\\bank");
            sheriff = Content.Load<Texture2D>("buildings\\sheriff");
            toilet = Content.Load<Texture2D>("buildings\\shitter");
            barn = Content.Load<Texture2D>("buildings\\barn");
            house = Content.Load<Texture2D>("buildings\\house");
            house2 = Content.Load<Texture2D>("buildings\\house");
            house3 = Content.Load<Texture2D>("buildings\\house");
            church = Content.Load<Texture2D>("buildings\\church");

            //Indlæser spriten for in-game info (players, runde osv.)
            gameInfo = Content.Load<Texture2D>("bw_info");


            //Indlæser spriten for et lige stykke spor
            straightTrackSprite = Content.Load<Texture2D>("track 1");

            smokeSprites = new Texture2D[] { Content.Load<Texture2D>("smoke1") , Content.Load<Texture2D>("smoke2") };

            //loads tower sprites
            towerSprites[0] = Content.Load<Texture2D>("tower1/ti00");
            towerSprites[1] = Content.Load<Texture2D>("ts01");

            towers.Add(new Tower(players[currentPlayer],Vector2.Zero, new Texture2D[] { towerSprites[0] }));
            towers.Add(new Tower(players[currentPlayer], Vector2.Zero, new Texture2D[] { towerSprites[1] }));

            towers[0].LoadContent(Content);
            towers[1].LoadContent(Content);
            foreach (Tower t in towers)
            {
                //t.LoadContent(Content);
            }

            //Indælser den primære font
            main = Content.Load<SpriteFont>("Main");

            //pause = new Texture2D("pause", new Vector2(screenSize.X/2, screenSize.Y/2), new Vector2(300,300));
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.LoadContent(Content);

            }

            // TODO: use this.Content to load your game content here
            base.LoadContent();

            AudioManager.LoadAudio(Content);
        }

        /// <summary>
        /// Henter alle tracks i gameObjects
        /// </summary>
        /// <returns></returns>
        public static List<Track> getTracks()
        {
            List<Track> tracks = new List<Track>();
            foreach (GameObject go in gameObjects) {
                if (go is Track) {
                    tracks.Add((Track)go);
                }
            }
            return tracks;
        }

        /// <summary>
        /// Returnerer den nuværende spiller
        /// </summary>
        /// <returns></returns>
        public static Player GetCurrentPlayer() {
            return players[currentPlayer];
        }

        protected override void Update(GameTime gameTime)
        {

            pad1 = Keyboard.GetState();


            AudioManager.MusicTest();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (pad1.IsKeyDown(Keys.P) && prevPad1.IsKeyUp(Keys.P) && inGame)
            {
                Global.paused = !Global.paused;
                AudioManager.PauseMusic();
            }
            prevPad1 = pad1;


            // TODO: Add your update logic here
            if (!Global.paused)
            {
                if (inGame)
                {

                    foreach (GameObject gameObject in gameObjects)
                    {
                        gameObject.Update(gameTime);
                    }
                }
                else
                {
                    if (Keyboard.GetState().GetPressedKeyCount() > 0)
                    {
                        inGame = true;
                        startGame();
                        trainSound.Play();
                        foreach (GameObject gameObject in gameObjects)
                        {
                            gameObject.Update(gameTime);
                        }
                        foreach (UIElement uiElement in uiElements)
                        {
                            uiElement.Update(gameTime);
                        }

                    }
                    else
                    {
                        if (Keyboard.GetState().GetPressedKeyCount() > 0)
                        {
                            inGame = true;
                            startGame();




                        }
                    }
                
                }
                foreach (UIElement uiElement in uiElements) {
                    uiElement.Update(gameTime);
                }
                

                AddGameObjectsToList();
                AddUIElementsToList();
            }

            AudioManager.NewSong(gameTime);

            RemoveGameObjects();
            RemoveUIElements();

            bool activeLocomotive = false;
            foreach (GameObject go in gameObjects) {
                if (go is Locomotive) {
                    Locomotive lo = (Locomotive)go;
                    if (lo.speed != 0) {
                        activeLocomotive = true;
                        break;
                    }
                }
            }
            if (!activeLocomotive)
            {
                trainSound.Stop();

            }
            


            base.Update(gameTime);
        }


       
        /// <summary>
        /// Genererer Diverse Objekter og uiElementer spillet skal bruge
        /// </summary>
        private void startGame()
        {
            //tower 1
            Vector2 buttonPosition = Vector2.Zero;
            towers[0].Position = buttonPosition + new Vector2(buttonSprite.Width / 4, buttonSprite.Height / 4);
            uiElements.Add(new ShopButton(towers[0], players[currentPlayer], new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPosition, 0, main));
            buttonPosition += new Vector2(0, buttonSprite.Height + 2);
            towers[0].inShop = true;
            towers[1].inShop = true;

            //tower 2
            buttonPosition += new Vector2(0, buttonSprite.Height + 2);
            towers[1].Position = buttonPosition + new Vector2(buttonSprite.Width / 4, buttonSprite.Height / 4);
            uiElements.Add(new ShopButton(towers[1], players[currentPlayer], new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPosition, 0, main));

            //Endphase knap
            buttonPosition = screenSize - new Vector2(buttonSprite.Width/2, buttonSprite.Height/2);
            uiElements.Add(new EndPhaseButton(players[currentPlayer], new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPosition, 0));

            //skinne knap
            buttonPosition = new Vector2(0, 300);
            TurnPoint turnPoint = new TurnPoint(buttonPosition + new Vector2(buttonSprite.Width / 4, buttonSprite.Height / 4), new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, false);
            uiElements.Add(new ShopButton(turnPoint, players[currentPlayer], new Texture2D[] { buttonSprite }, Color.White, Color.Blue, Color.Red, Color.Green, buttonPosition, 0, main));
            
            //Start Skinner
            Vector2 startPoint = new Vector2(screenSize.X + 300, screenSize.Y - 400);
            TurnPoint startPoint0 = new TurnPoint(startPoint, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, true);
            gameObjects.Add(startPoint0);
            Vector2 startPoint1 = new Vector2(buttonSprite.Width / 2 + 20, screenSize.Y - 400);
            TurnPoint startPoint2 = new TurnPoint(startPoint1, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, false);
            gameObjects.Add(startPoint2);
            startPoint0.AddNewConnection(startPoint2);
            List<TurnPoint> player1EndPoints = new List<TurnPoint>();
            List<TurnPoint> spawnPoints = new List<TurnPoint>();
            spawnPoints.Add(startPoint0);
            player1EndPoints.Add(startPoint2);
            float dif = (screenSize.Y - 500)/4;
            for (int i = 1; i < 4; i++) {
                Vector2 startPoint3 = new Vector2(screenSize.X + 300, screenSize.Y - 400 - dif * i);
                TurnPoint startPoint4 = new TurnPoint(startPoint3, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, true);
                gameObjects.Add(startPoint4);
                Vector2 startPoint5 = new Vector2(buttonSprite.Width / 2 + 20, screenSize.Y - 400 - dif * i);
                TurnPoint startPoint6 = new TurnPoint(startPoint5, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, false);
                gameObjects.Add(startPoint6);
                startPoint4.AddNewConnection(startPoint6);
                player1EndPoints.Add(startPoint6);
                spawnPoints.Add(startPoint4);
                player1EndPoints[i - 1].AddNewConnection(startPoint6);
            }

            Vector2 startPoint7 = new Vector2(screenSize.X + 300, screenSize.Y - 400 - dif * 4);
            TurnPoint startPoint8 = new TurnPoint(startPoint7, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, true);
            gameObjects.Add(startPoint8);
            Vector2 startPoint9 = new Vector2(buttonSprite.Width / 2 + 20, screenSize.Y - 400 - dif * 4);
            TurnPoint startPoint10 = new TurnPoint(startPoint9, new Texture2D[] { straightTrackSprite }, SpriteEffects.None, 0, false);
            gameObjects.Add(startPoint10);
            startPoint10.AddNewConnection(startPoint8);
            player1EndPoints.Add(startPoint10);
            player1EndPoints[3].AddNewConnection(startPoint10);

            //Start tog
            for (int i = 0; i < spawnPoints.Count; i++) {
                startPoint = new Vector2(screenSize.X + 300, screenSize.Y - 400 - dif * i);
                Locomotive locomotive = new Locomotive(5, null, players[0], startPoint, new Texture2D[] { locomotiveSprite }, SpriteEffects.None, 75, smokeSprites);
                locomotive.setDirection(spawnPoints[i].NextTrack);
                gameObjects.Add(locomotive);
                for (int j = 0; j < 9; j++)
                {
                    CargoCarriage passengerCart = new CargoCarriage(locomotive, players[0], startPoint, new Texture2D[] { passengerCartSprite }, SpriteEffects.None, 50);
                    passengerCart.setDirection(spawnPoints[i].NextTrack);
                    gameObjects.Add(passengerCart);
                }
            }
            
            
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

            

            //Tegner ting i spillet
            if (inGame)
            {
                _spriteBatch.Draw(background, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
                foreach (GameObject go in gameObjects)
                {
                    go.Draw(_spriteBatch);
                    if (go is Tower)
                    {
                        ((Tower)go).drawRange(_spriteBatch);
                    }
                }
                foreach (UIElement ue in uiElements)
                {
                    ue.Draw(_spriteBatch);
                }
            // Resource info test
               // _spriteBatch.DrawString(main, GetCurrentPlayer().Steel + " steel", new Vector2((int)screenSize.X / 2, (int)screenSize.Y / 2), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);



                //Tegner husene
                _spriteBatch.Draw(bank, new Rectangle((int)screenSize.X / 2, (int)screenSize.Y / 2 + 300, bank.Width, bank.Height), Color.White);
                _spriteBatch.Draw(barn, new Rectangle((int)screenSize.X - 100, (int)screenSize.Y / 2 + 150, barn.Width, barn.Height), Color.White);
                _spriteBatch.Draw(church, new Rectangle((int)screenSize.X / 2 + church.Width + 30, (int)screenSize.Y / 2 + 288, church.Width+10, church.Height+10), Color.White);
                _spriteBatch.Draw(house, new Rectangle((int)screenSize.X -500, (int)screenSize.Y / 2 + 300, house.Width, house.Height), Color.White);
                _spriteBatch.Draw(house2, new Rectangle((int)screenSize.X /2 - bank.Width, (int)screenSize.Y / 2 + 300, house.Width, house.Height), Color.White);
                _spriteBatch.Draw(house3, new Rectangle((int)screenSize.X /2 + church.Width + house.Width + 37, (int)screenSize.Y / 2 + 300, house.Width, house.Height), Color.White);
                _spriteBatch.Draw(sheriff, new Rectangle((int)screenSize.X / 2 - bank.Width - house.Width, (int)screenSize.Y / 2 + 300, sheriff.Width, sheriff.Height), Color.White);
                _spriteBatch.Draw(toilet, new Rectangle((int)screenSize.X -500 + house.Width, (int)screenSize.Y / 2 + 300, toilet.Width - 5, toilet.Height-5), Color.White);

                // In-game info
                GameInfo();

            }
            //Tegner titleScreen
            else
            {
                //_spriteBatch.DrawString(main, "Press any button to continue", new Vector2(1200 / 2, 1000 / 2), Color.Black);

                _spriteBatch.Draw(titleScreen, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                _spriteBatch.DrawString(main, "Press any button to continue", new Vector2((int)screenSize.X / 2, (int)screenSize.Y / 2), Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);

            }
            if (Global.paused)
            {
                _spriteBatch.Draw(pause, new Rectangle(_graphics.PreferredBackBufferWidth / 2 - 150, _graphics.PreferredBackBufferHeight / 2 - 150, 300, 300), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void InstantiateGameObject(GameObject go)
        {
            gameObjectsToAdd.Add(go);
        }

        public static void InstantiateUIElement(UIElement ui)
        {
            uiElementsToAdd.Add(ui);
        }

        public void AddUIElementsToList()
        {
            foreach (UIElement ui in uiElementsToAdd)
            {

                if (uiElements.Contains(ui))
                {
                    throw new Exception("gameObjects allready contains this gameObject");
                }
                else
                {
                    uiElements.Add(ui);
                }



            }

            uiElementsToAdd.Clear();
        }

        /// <summary>
        /// tilføjer gameobjekter fra gameobjectsToAdd til gameobjects
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void AddGameObjectsToList()
        {
            foreach (GameObject obj in gameObjectsToAdd)
            {

                if (gameObjects.Contains(obj))
                {
                    throw new Exception("gameObjects allready contains this gameObject");
                }
                else
                {
                    gameObjects.Add(obj);
                }
                


            }

            gameObjectsToAdd.Clear();
        }

        /// <summary>
        /// Giver en liste over alle ui elementer i vores spil
        /// </summary>
        /// <returns>ui element liste</returns>
        public static List<UIElement> GetUIElements()
        {
            return uiElements;
        }

        /// <summary>
        /// Fjerner alle tracks der ikke er forbundet til et TurnPoint
        /// </summary>
        /// <param name="tracksToRemove"></param>
        public static void ClearTracks(List<Track> tracksToRemove)
        {
            foreach (GameObject go in gameObjects)
            {
                if (go is Track)
                {
                    Track t = (Track)go;
                    if (tracksToRemove.Contains(t))
                    {
                        t.ShouldRemove = true;
                    }

                }
            }
        }

        /// <summary>
        /// Method to end a phase
        /// Phase overview
        /// Phase 0: Attack phase. Trains travel towards the enemy town and then back to their own
        /// Phase 1: Preparation phase. The current player plan their attack and defenses
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void EndPhase()
        {
            if (currentPhase < 1)
            {
                currentPhase++;
            }
            if (currentPhase == 1)
            {
                currentPhase = 0;
                if (currentPlayer == 0)
                {
                    currentPlayer = 1;
                }
                else if (currentPlayer == 1)
                {
                    currentPlayer = 0;
                }
                else
                {
                    throw new Exception("currentPlayer should not be changed outside the method EndPhase");
                }
            }
            else
            {
                throw new Exception("currentPhase should not be changed outside the method EndPhase");
            }
            phaseChanged = true;
        }

        /// <summary>
        /// fjerner alle gameobjekter hvor shouldremove er sandt
        /// </summary>
        private void RemoveGameObjects()
        {
            List<GameObject> gameObjectsToRemove = new List<GameObject>();
            foreach (GameObject go in gameObjects)
            {
                if (go.ShouldRemove)
                {
                    gameObjectsToRemove.Add(go);
                }
            }
            foreach (GameObject goTR in gameObjectsToRemove)
            {
                gameObjects.Remove(goTR);
            }
        }

        /// <summary>
        /// fjerne alle UI elementer hvor shouldremove er sandt
        /// </summary>
        private void RemoveUIElements()
        {
            List<UIElement> UIElementsToRemove = new List<UIElement>();
            foreach (UIElement ui in uiElements)
            {
                if (ui.ShouldRemove)
                {
                    UIElementsToRemove.Add(ui);
                }
            }
            foreach (UIElement ui in UIElementsToRemove)
            {
                uiElements.Remove(ui);
            }
        }

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

        private void GameInfo() // metode kaldes via. draw
        {
            // Position for gameInfo
            Vector2 pos;
            // Vector2 origin for gameInfo sprite;
            Vector2 oPos;

            pos.X = _graphics.PreferredBackBufferWidth/2;
            pos.Y = 0;

            oPos.X = gameInfo.Width / 2;
            oPos.Y = 0;

            // skal lave et draw af ting (Husk layer skal være højere end baggrund men mindre end tekst)
            _spriteBatch.Draw(gameInfo,pos,null,Color.White,0f,oPos,1,SpriteEffects.None,0.5f);
            // tekst og font

            
            // Player 1
            _spriteBatch.DrawString(main, players[0].Coal + " Coal", new Vector2(pos.X-oPos.X+30f,pos.Y+15), Color.Blue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
            _spriteBatch.DrawString(main, players[0].Steel + " Steel", new Vector2(pos.X - oPos.X + 130f, pos.Y + 15), Color.Blue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
            _spriteBatch.DrawString(main, players[0].Health + " Lives", new Vector2(pos.X - oPos.X + 240f, pos.Y + 15), Color.Blue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);

            // Player 2
            _spriteBatch.DrawString(main, players[1].Coal + " Coal", new Vector2(pos.X + oPos.X - 330f, pos.Y + 15), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
            _spriteBatch.DrawString(main, players[1].Steel + " Steel", new Vector2(pos.X + oPos.X - 220f, pos.Y + 15), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
            _spriteBatch.DrawString(main, players[1].Health + " Lives", new Vector2(pos.X + oPos.X - 100f, pos.Y + 15), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);

            // Display round number <-------------------- (Rediger string til at være runde nr.)
            _spriteBatch.DrawString(main, "0", new Vector2(pos.X-5, pos.Y + 35), Color.White, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);

            if (currentPlayer == 0)
            {
                if (CurrentPhase == 0)
                {
                    _spriteBatch.DrawString(main, "player 1 carries out the offensive", new Vector2(pos.X - oPos.X, pos.Y + gameInfo.Height * 0.8f), Color.Blue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
                }
                else
                {
                    // Preperation
                    _spriteBatch.DrawString(main, "Player 1 is preparing", new Vector2(pos.X - oPos.X, pos.Y + gameInfo.Height * 0.8f), Color.Blue, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
                }
            }
            else if (currentPlayer == 0)
            {
                if (CurrentPhase == 1)
                {
                    _spriteBatch.DrawString(main, "player 2 carries out the offensive", new Vector2(pos.X + 40f, pos.Y + gameInfo.Height * 0.8f), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
                }
                else
                {
                    // Preperation
                    _spriteBatch.DrawString(main, "Player 2 is preparing", new Vector2(pos.X + 40f, pos.Y + gameInfo.Height * 0.8f), Color.Red, 0f, Vector2.Zero, 0.8f, SpriteEffects.None, 1);
                }
            }
            else
            {
                throw new Exception("Could not identify current phase");
            }
        }

    }
}