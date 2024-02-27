using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Threading;
using System;
using System.ComponentModel;
using Cult_Penguin.Components;
using Cult_Penguin.FactoryPattern;
using Cult_Penguin.CommandPattern;
using System.Net.Sockets;
using SharpDX.Direct2D1.Effects;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
//using Cult_Penguin_Server;

namespace Cult_Penguin
{
    public class GameWorld : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private static Vector2 screenSize;
        private static GameWorld instance;
        public SpriteFont defaultFont;
        public Random rnd = new Random();
        private int lastUpdate = int.MinValue;
        private bool updateAvailable = false;
        private bool offline = false;
        private bool loggedIn = false;

        private bool loginResponse=false;

        private int messageDisplayTime = 1;

        Texture2D backgroundSprite;
        Vector2 backgroundScale = new Vector2(1, 1);
        //Gameobjects
        private List<GameObject> gameObjects = new List<GameObject>();

        private List<GameObject> newGameObjects = new List<GameObject>();

        private List<GameObject> destroyedGameObjects = new List<GameObject>();

        //UI ELEMENTS
        private List<GameObject> uiObjects = new List<GameObject>();

        private List<GameObject> newUIObjects = new List<GameObject>();

        private List<GameObject> destroyeduiObjects = new List<GameObject>();

        public static Vector2 ScreenSize { get => screenSize; set => screenSize = value; }
        public GameObject player;
        private string playerName;
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

        public List<Collider> Colliders { get; private set; } = new List<Collider>();

        public List<GameObject> NewGameObjects { get => newGameObjects; }
        public List<GameObject> DestroyedGameObjects { get => destroyedGameObjects; }
        public GraphicsDeviceManager Graphics { get => _graphics; }

        public List<GameObject> GameObjects { get => gameObjects; set => gameObjects = value; }
        public List<GameObject> DestroyeduiObjects { get => destroyeduiObjects; }
        public List<GameObject> NewUIObjects { get => newUIObjects; }

        public Dictionary<string, GameObject> OnlinePlayers { get => onlinePlayers;}
        public int MessageDisplayTime { get => messageDisplayTime; private set => messageDisplayTime = value; }
        public List<GameObject> UiObjects { get => uiObjects; private set => uiObjects = value; }
        public List<GameObject> NewLoginScreenObjects { get => newLoginScreenObjects; set => newLoginScreenObjects = value; }
        public int LastUpdate { get => lastUpdate; set => lastUpdate = value; }
        public bool UpdateAvailable { get => updateAvailable; set => updateAvailable = value; }
        public bool LoggedIn { get => loggedIn; set => loggedIn = value; }
        public string PlayerName { get => playerName; set => playerName = value; }
        public Client Client { get => client; set => client = value; }

        private List<GameObject> logInScreenObjects = new List<GameObject>();
        private List<GameObject> newLoginScreenObjects = new List<GameObject>();

        private Dictionary<string, GameObject> onlinePlayers = new Dictionary<string, GameObject>();

        public GameObject chatWindow;

        private Client client;

        private GameWorld()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            client = new Client();

            //tråden sover lige så klienten kan når at få serverens public key
            //ik slet pls :pray:
            while (!client.ReadyToSendMessage)
            {

            }
            //gammel testmessage
            //client.SendMessage(new GlobalMessage() { Message = "A message that works!!!!!!!!!!!!!!!!!!!" });


            // TODO: Add your initialization logic here
            if (offline)
            {
                spawnAI();
            }
            else { 
                
            }

            //sætter størrelsen af spillets vindue
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 900;

            ScreenSize = new Vector2(1920, 1080);

            //_graphics.ToggleFullScreen();

            _graphics.ApplyChanges();
            defaultFont = Content.Load<SpriteFont>("DefaultFont");
            chatWindow = ChatWindowFactory.Instance.Create(null);
            SpriteRenderer sr = (SpriteRenderer)chatWindow.GetComponent<SpriteRenderer>();
            ChatHandler.Instance.ChatBox = new Rectangle((int)chatWindow.Transform.Position.X - sr.Sprite.Width / 2, (int)chatWindow.Transform.Position.Y - sr.Sprite.Height / 2, sr.Sprite.Width, sr.Sprite.Height);
            UiObjects.Add(chatWindow);

            //definere størrelsen i vores static variable screenSize til brug i andre klasser
            screenSize.X = _graphics.PreferredBackBufferWidth;
            screenSize.Y = _graphics.PreferredBackBufferHeight;
            Window.TextInput += TextInputHandler;
            GameObject userName = LoginScreenFactory.Instance.Create(buttonType.username);
            userName.Transform.Position = new Vector2(screenSize.X / 2, (screenSize.Y / 7) * 2);
            GameObject password = LoginScreenFactory.Instance.Create(buttonType.password);
            password.Transform.Position = new Vector2(screenSize.X / 2, (screenSize.Y / 7) * 3);
            GameObject login = LoginScreenFactory.Instance.Create(buttonType.login);
            login.Transform.Position = new Vector2(screenSize.X / 2, (screenSize.Y / 7) * 4);
            GameObject register = LoginScreenFactory.Instance.Create(buttonType.register);
            register.Transform.Position = new Vector2(screenSize.X / 2, (screenSize.Y / 7) * 5);
            logInScreenObjects.Add(userName);
            logInScreenObjects.Add(password);
            logInScreenObjects.Add(login);
            logInScreenObjects.Add(register);
            
            for (int i = 0; i < gameObjects.Count; i++)
            {
                GameObjects[i].Awake();
            }
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Awake();
            }
            for (int i = 0; i < logInScreenObjects.Count; i++) {
                logInScreenObjects[i].Awake();
            }
            base.Initialize();
        }

        private void TextInputHandler(object sender, TextInputEventArgs e)
        {
            if (InputHandler.Instance.Typing)
            {
                if (e.Key == Keys.Back)
                {
                    if (ChatHandler.Instance.Message.Length > 0)
                    {
                        ChatHandler.Instance.Message = ChatHandler.Instance.Message.Remove(ChatHandler.Instance.Message.Length - 1);
                    }
                }

                else
                {
                    ChatHandler.Instance.Message += e.Character;
                }
            }
            else if (!loggedIn) {
                if (LoginHandler.Instance.UserNameOrPassword == 1)
                {
                    if (e.Key == Keys.Back)
                    {
                        if (LoginHandler.Instance.Username.Length > 0) {
                            LoginHandler.Instance.Username = LoginHandler.Instance.Username.Remove(LoginHandler.Instance.Username.Length - 1);
                        }
                        
                    }
                    else if (e.Key == Keys.Tab) { 
                        
                    }
                    else
                    {
                        LoginHandler.Instance.Username += e.Character;
                    }

                }
                else if (LoginHandler.Instance.UserNameOrPassword == 2) {
                    if (e.Key == Keys.Back)
                    {
                        LoginHandler.Instance.Password = LoginHandler.Instance.Password.Remove(LoginHandler.Instance.Password.Length - 1);
                    }
                    else
                    {
                        LoginHandler.Instance.Password += e.Character;
                    }
                }
            }
            
        }

        private void spawnAI()
        {
            for (int i = 0; i < 5; i++) {
                GameObject ai = CharacterFactory.Instance.Create(playerType.other);
                GameObjects.Add(ai);
                ai.Transform.Position = new Vector2(rnd.Next(_graphics.PreferredBackBufferWidth), rnd.Next(_graphics.PreferredBackBufferHeight));
                string name = "AI" + i;
                Player p = (Player)ai.GetComponent<Player>();
                p.name = name;
                OnlinePlayers.Add(name, ai);
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            backgroundSprite = Content.Load<Texture2D>("backgroumd");
            float newScaleX = Graphics.PreferredBackBufferWidth / backgroundSprite.Width;
            float newScaleY = Graphics.PreferredBackBufferHeight / backgroundSprite.Height;
            backgroundScale = new Vector2(newScaleX, newScaleY);
            for (int i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Start();
            }
            for (int i = 0; i < uiObjects.Count; i++)
            {
                uiObjects[i].Start();
            }
            for (int i = 0; i < logInScreenObjects.Count; i++)
            {
                logInScreenObjects[i].Start();
            }

            // TODO: use this.Content to load your game content here
        }

        protected override async void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (loggedIn)
            {
                if (offline)
                {
                    HandleAI();
                }

                await RESTHandler.Instance.CheckForUpdateAsync();
                if (updateAvailable) {
                    //updatere shit
                    UpdateMessage uMes;
                    while (client.UpdateMessages.Count > 0) {
                        uMes = client.UpdateMessages.Dequeue();
                        for (int i = 0; i < uMes.Names.Length; i++)
                        {
                            if (uMes.Names[i] == "")
                            {
                                continue;
                            }
                            if (onlinePlayers.ContainsKey(uMes.Names[i]) && uMes.Names[i] != playerName)
                            {
                                (onlinePlayers[uMes.Names[i]].GetComponent<Player>() as Player).GameObject.Transform.Position = new Vector2(uMes.xPos[i], uMes.yPos[i]);
                            }
                            else if (uMes.Names[i] != playerName){
                                SpawnNewPlayer(uMes.Names[i] , new Vector2(uMes.xPos[i], uMes.yPos[i]));
                            }
                        }
                    }
                    

                    


                    //foreach (UpdateMessage uMes in client.UpdateMessages)
                    //{
                    //    for (int i = 0; i < uMes.Names.Length; i++)
                    //    {
                    //        if (uMes.Names[i] == "")
                    //        {
                    //            continue;
                    //        }

                    //        (onlinePlayers[uMes.Names[i]].GetComponent<Player>() as Player).destination = new Vector2(uMes.xPos[i], uMes.yPos[i]);
                    //    }
                    //}
                    //client.UpdateMessages.Clear();
                    updateAvailable = false;
                }

                ChatHandler.Instance.Update();
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObjects[i].Update();
                }
                for (int i = 0; i < UiObjects.Count; i++)
                {
                    UiObjects[i].Update();
                }
            }
            else {
                //LoginHandler.Instance.Update();
                for (int i = 0; i < logInScreenObjects.Count; i++)
                {
                    logInScreenObjects[i].Update();
                }
            }
            
            // TODO: Add your update logic here
           
            
            Cleanup();
            base.Update(gameTime);
        }

        private void HandleAI()
        {
            foreach (GameObject g in gameObjects) { 
                if(g.GetComponent<Player> != null){
                    Player p = (Player)g.GetComponent<Player>();
                    if (!p.controlled) {
                        if (p.destination == new Vector2(-1, -1))
                        {
                            SpriteRenderer sr = (SpriteRenderer)g.GetComponent<SpriteRenderer>();
                            p.destination = new Vector2(rnd.Next(sr.Sprite.Width / 2 + 50, _graphics.PreferredBackBufferWidth - sr.Sprite.Width / 2 - 50), rnd.Next(sr.Sprite.Height / 2 + 50, _graphics.PreferredBackBufferHeight - sr.Sprite.Height / 2 - 50));
                            Vector2 startVelocity = new Vector2(g.Transform.Position.X - p.destination.X, g.Transform.Position.Y - p.destination.Y);
                            startVelocity = -Vector2.Normalize(startVelocity);
                            
                            p.ChangeVelocity(startVelocity);
                        }
                        else if (g.Transform.Position != p.destination)
                        {
                            Vector2 startVelocity = new Vector2(g.Transform.Position.X - p.destination.X, g.Transform.Position.Y - p.destination.Y);
                            if (startVelocity.X < 1 && startVelocity.X > -1 && startVelocity.Y < 1 && startVelocity.Y > -1)
                            {
                                g.Transform.Position = p.destination;
                            }
                            else {
                                startVelocity = -Vector2.Normalize(startVelocity);
                            }
                            
                            p.ChangeVelocity(startVelocity);
                        }
                        else {
                            p.destination = new Vector2(-1, -1);
                        }
                        foreach (string name in OnlinePlayers.Keys) {
                            if (OnlinePlayers[name] == g && rnd.Next(100) > 95) {
                                ChatHandler.Instance.DisplayMessage(name, "test");
                            }
                        }
                    }
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: Microsoft.Xna.Framework.Graphics.SamplerState.PointClamp, sortMode: SpriteSortMode.FrontToBack);
            // TODO: Add your drawing code here
            if (loggedIn)
            {
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObjects[i].Draw(_spriteBatch);
                }
                for (int i = 0; i < UiObjects.Count; i++)
                {
                    UiObjects[i].Draw(_spriteBatch);
                }
                Vector2 origin = new Vector2((backgroundSprite.Width * backgroundScale.X) / 2 , backgroundSprite.Height / 2 * backgroundScale.Y);
                //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
                _spriteBatch.Draw(backgroundSprite, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White);
            }
            else {
                for (int i = 0; i < logInScreenObjects.Count; i++)
                {
                    logInScreenObjects[i].Draw(_spriteBatch);
                }
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
                UiObjects.Add(NewUIObjects[i]);
                NewUIObjects[i].Awake();
                NewUIObjects[i].Start();



            }

            for (int i = 0; i < DestroyeduiObjects.Count; i++)
            {


                UiObjects.Remove(DestroyeduiObjects[i]);


            }

            if (!loggedIn) {
                for (int i = 0; i < newLoginScreenObjects.Count; i++) {
                    logInScreenObjects.Add(newLoginScreenObjects[i]);
                    newLoginScreenObjects[i].Awake();
                    newLoginScreenObjects[i].Start();
                }
                newLoginScreenObjects.Clear();
            }
            
            DestroyeduiObjects.Clear();
            NewUIObjects.Clear();
        }

        internal void HandleMessage(string message)
        {
            //ChatHandler.Instance.DisplayMessage(playerName, message);

            //sender besked til server
            GlobalMessage gMes = new GlobalMessage() { Message=message, senderName = playerName};
            client.SendMessage(gMes);
        }


        internal void LogIn(string username, string password)
        {
            if (offline)
            {

                loggedIn = true;
            }
            else {
                

                AccountLogin loginObject = new AccountLogin() { username =username,password = password};
                client.SendMessage(loginObject);

                //holder tråden her indtil der er respons fra serveren
                while (!loginResponse)
                {

                }
                
            }

            if (loggedIn) {
                logInScreenObjects.Clear();
                player = CharacterFactory.Instance.Create(playerType.controlled);
                player.Transform.Position = new Vector2(400, 400);
                NewGameObjects.Add(player);
                OnlinePlayers.Add(username, player);
                playerName = username;
                Player p = (Player)player.GetComponent<Player>();
                p.name = username;

                MovementUpdate mMes = new MovementUpdate() { PositionX = player.Transform.Position.X, PositionY = player.Transform.Position.Y };
                Client.SendMessage(mMes);
            }
        }



        public void Register(string username, string password) {
            if (offline)
            {

                //LoginHandler.Instance.ShowMessage("Account created");


            }
            else {

                CreateAccount loginObject = new CreateAccount() { username = username, password = password };
                
                client.SendMessage(loginObject);


                //LoginHandler.Instance.ShowMessage("Account created");

            }
            
        }

        public void LoginResponse()
        {
            loginResponse = true;

            Thread.Sleep(100);

            loginResponse = false;
        }


        public void SpawnNewPlayer(string newPlayerName, Vector2 startPos)
        {
            GameObject player = CharacterFactory.Instance.Create(playerType.other);
            newGameObjects.Add(player);
            player.Transform.Position = startPos;
            string name = newPlayerName;
            Player p = (Player)player.GetComponent<Player>();
            p.name = name;
            OnlinePlayers.Add(name, player);
        }
    }
}