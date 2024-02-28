using Cult_Penguin.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.FactoryPattern
{
    internal class ChatWindowFactory : Factory
    {
        private static ChatWindowFactory instance;

        internal static ChatWindowFactory Instance { get {
                if (instance == null) {
                    instance = new ChatWindowFactory();
                }
                return instance;
            }
        }

        private ChatWindowFactory() { 
            
        }

        public override GameObject Create(Enum type)
        {
            GameObject chatWindow = new GameObject();
            SpriteRenderer  sr = (SpriteRenderer)chatWindow.AddComponent(new SpriteRenderer());
            sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("ChatBox");

            chatWindow.AddComponent(new Collider());
            Button b = (Button)chatWindow.AddComponent(new Button());
            b.ButtonText = "Click to type";
            b.OnClick = ChatHandler.Instance.StartTyping;
            chatWindow.Transform.Position = new Vector2(GameWorld.Instance.Graphics.PreferredBackBufferWidth / 2, GameWorld.Instance.Graphics.PreferredBackBufferHeight - sr.Sprite.Height - 10);
            return chatWindow;
        }
    }
}
