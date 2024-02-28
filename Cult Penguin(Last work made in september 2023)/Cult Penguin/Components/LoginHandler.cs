using Cult_Penguin.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.Components
{
    public class LoginHandler:Component
    {
        private string username = "";
        private string password = "";
        private Button usernameButton;
        private Button passwordButton;
        private static LoginHandler instance;
        int userNameOrPassword = 0;
        private TextField errorBox;
        private LoginHandler()
        {
        }

        public Button UsernameButton { get => usernameButton; set => usernameButton = value; }
        public Button PasswordButton { get => passwordButton; set => passwordButton = value; }
        public string Username { get => username; set => username = value; }
        public string Password { get => password; set => password = value; }
        public static LoginHandler Instance { get {
                if (instance == null) {
                    instance = new LoginHandler();
                } 
                return instance; 
            } 
        }

        public int UserNameOrPassword { get => userNameOrPassword;}

        public void LogIn() {
            if (ValidInput())
            {
                GameWorld.Instance.LogIn(Username, Password);
            }
            else {
                ShowMessage("Missing username or password");
            }
        }

        

        public override void Update()
        {
            if (username != "" || userNameOrPassword == 1)
            {
                usernameButton.ButtonText = username;
            }
            else {
                usernameButton.ButtonText = "username";
            }
            if (password != "" || userNameOrPassword == 2)
            {
                string displayPassword = "";
                for (int i = 0; i < password.Length; i++) {
                    displayPassword += "*";
                }
                passwordButton.ButtonText = displayPassword;
            }
            else {
                passwordButton.ButtonText = "password";
            }
            MouseState m = Mouse.GetState();
            Vector2 mousePos = new Vector2(m.X, m.Y);
            if (m.LeftButton == ButtonState.Pressed && userNameOrPassword > 0) {
                Collider userCol = (Collider)usernameButton.GameObject.GetComponent<Collider>();
                Collider passCol = (Collider)passwordButton.GameObject.GetComponent<Collider>();
                if (!passCol.CollisionBox.Contains(mousePos.X, mousePos.Y) && !userCol.CollisionBox.Contains(mousePos.X, mousePos.Y)) {
                    userNameOrPassword = 0;
                }
            }
            base.Update();
        }

        public bool ValidInput() {
            if (Username.Length > 0 && Password.Length > 0) {
                return true;
            }
            return false;
        }
        public void Register() {
            if (ValidInput())
            {
                GameWorld.Instance.Register(username, password);
            }
            else {
                ShowMessage("Missing username or password");
            }
        }
        public void typeUserName() {
            userNameOrPassword = 1;
        }
        public void typePassword() {
            userNameOrPassword = 2;
        }
        public void stopTyping() {
            userNameOrPassword = 0;
        }

        public void ShowMessage(string errorMessage) {
            if (errorBox == null)
            {
                GameObject eb = ErrorboxFactory.Instance.Create(null);
                errorBox = (TextField)eb.GetComponent<TextField>();
                errorBox.Text = errorMessage;
                eb.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight / 7);
                GameWorld.Instance.NewLoginScreenObjects.Add(eb);
            }
            else if (errorBox.Text != errorMessage)
            {
                errorBox.Text = errorMessage;
                errorBox.Color = Color.Black;
            }
            else if (errorBox.Color == Color.Black)
            {
                errorBox.Color = Color.Red;
            }
            else
            {
                errorBox.Shaking = true;
            }
        }
    }
}
