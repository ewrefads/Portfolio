using Cult_Penguin.CommandPattern;
using Cult_Penguin.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin
{
    public class ChatHandler
    {
        private static ChatHandler instance;

        private string message = "";

        private Rectangle chatBox;

        private Button chatButton;

        public static ChatHandler Instance { get {
                if (instance == null) {
                    instance = new ChatHandler();
                }
                return instance; 
            } 
        }

        public string Message { get => message; set {
                string oldMessage = message;
                if (value.Length <= 64) {
                    message = value;
                }
            } 
        }
        public Rectangle ChatBox { get => chatBox; set => chatBox = value; }

        private ChatHandler() { }

        public void DisplayMessage(string sender, string message) {
            foreach (string player in GameWorld.Instance.OnlinePlayers.Keys) {
                if (sender == player) {
                    GameObject g = GameWorld.Instance.OnlinePlayers[sender];
                    ChatBubble c = (ChatBubble)g.GetComponent<ChatBubble>();
                    c.Text = message;
                }
            }
        }

        public void SendMessage() {
            if (Message != "") {
                GameWorld.Instance.HandleMessage(Message);
            }
            Message = "";
        }
        public void Update() {
            if (chatButton == null) { 
                chatButton = (Button)GameWorld.Instance.chatWindow.GetComponent<Button>();
                //chatButton.DefaultScale = new Vector2(1, 1);
                chatButton.CustomOffset = new Vector2(0, 20);
            }
            MouseState m = Mouse.GetState();
            Collider col = (Collider)chatButton.GameObject.GetComponent<Collider>();
            
            if (!col.CollisionBox.Contains(m.Position.ToVector2().X, m.Position.ToVector2().Y) && m.LeftButton == ButtonState.Pressed)
            {
                EndTyping();
                if (message == "") {
                    chatButton = (Button)GameWorld.Instance.chatWindow.GetComponent<Button>();
                    chatButton.ButtonText = "Click to type";
                }
            }
            else if(InputHandler.Instance.Typing)
            {
                chatButton = (Button)GameWorld.Instance.chatWindow.GetComponent<Button>();
                chatButton.ButtonText = message;
            }
        }
        public void StartTyping() {
            if (!InputHandler.Instance.Typing) {
                InputHandler.Instance.Typing = true;
            }
        }

        public void EndTyping() {
            InputHandler.Instance.Typing = false;
            Button b = (Button)GameWorld.Instance.chatWindow.GetComponent<Button>();
            b.ButtonText = "Click to type";
        }

    }
}
