using Cult_Penguin.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.FactoryPattern
{
    public enum buttonType { username, password, login, register}
    public class LoginScreenFactory : Factory
    {
        private static LoginScreenFactory instance;

        private TextField errorBox;

        internal static LoginScreenFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LoginScreenFactory();
                }
                return instance;
            }
        }
        public override GameObject Create(Enum type)
        {
            GameObject loginScreenButton = new GameObject();
            SpriteRenderer sr = (SpriteRenderer)loginScreenButton.AddComponent(new SpriteRenderer());
            sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Button");
            loginScreenButton.AddComponent(new Collider());
            Button b = (Button)loginScreenButton.AddComponent(new Button());
            switch (type) {
                case (buttonType.username):

                    b.ButtonText = "username";
                    b.OnClick = LoginHandler.Instance.typeUserName;
                    LoginHandler.Instance.UsernameButton = b;
                    break;
                case (buttonType.password):
                    b.ButtonText = "password";
                    b.OnClick = LoginHandler.Instance.typePassword;
                    LoginHandler.Instance.PasswordButton = b;
                    break;
                case (buttonType.login):
                    b.ButtonText = "Login";
                    b.OnClick = LoginHandler.Instance.LogIn;
                    loginScreenButton.AddComponent(LoginHandler.Instance);
                    break;
                case (buttonType.register):
                    b.ButtonText = "Register";
                    b.OnClick = LoginHandler.Instance.Register;
                    break;

            }
            return loginScreenButton;
        }
    }
}
