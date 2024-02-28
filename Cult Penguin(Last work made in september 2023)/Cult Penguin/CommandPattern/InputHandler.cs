using Cult_Penguin.Components;
using Cult_Penguin.ObserverPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.CommandPattern
{
    internal class InputHandler
    {
        private bool typing = false;
        private static InputHandler instance;

        public static InputHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InputHandler();
                }
                return instance;
            }
        }

        public bool Typing { get => typing; set => typing = value; }

        MouseState oldMouseState;

        //de commands der kan laves
        private Dictionary<KeyInfo, ICommand> keyBinds = new Dictionary<KeyInfo, ICommand>();

        private Player player;

        //events bliver sat op

        //når den bliver lavet så tilføjer den kommandoer til vores dictionary
        private InputHandler()
        {
            player = (Player)GameWorld.Instance.FindObjectOfType<Player>();


            keyBinds.Add(new KeyInfo(Keys.W), new MoveCommand(new Vector2(0, -1)));
            keyBinds.Add(new KeyInfo(Keys.S), new MoveCommand(new Vector2(0, 1)));
            keyBinds.Add(new KeyInfo(Keys.D), new MoveCommand(new Vector2(1, 0)));
            keyBinds.Add(new KeyInfo(Keys.A), new MoveCommand(new Vector2(-1, 0)));


        }

        internal void Execute(Player player)
        {
            KeyboardState state = Keyboard.GetState();
            if (!typing)
            {
                foreach (KeyInfo key in keyBinds.Keys)
                {
                    if (state.IsKeyDown(key.Key))
                    {
                        keyBinds[key].Execute(player);
                    }
                }
            }
            else {
                if (state.IsKeyDown(Keys.Enter)) {
                    ChatHandler.Instance.SendMessage();
                    ChatHandler.Instance.EndTyping();
                }
            }
        }

        //player bliver notified hvis en knap i som er i dictionarien bliver trykket



    }
    /// <summary>
    /// keyinfo klasse som beskrive ting om en key der er trykket
    /// </summary>
    public class KeyInfo
    {
        public bool IsDown { get; set; }

        public Keys Key { get; set; }

        public KeyInfo(Keys key)
        {
            this.Key = key;
        }
    }
}
