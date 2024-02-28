using Meerkat_Mining.CommandPattern;
using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meerkat_Mining
{
    public class InputHandler
    {
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

        private Dictionary<KeyInfo, ICommand> keybinds = new Dictionary<KeyInfo, ICommand>();

        private ButtonEvent buttonEvent = new ButtonEvent();

        private InputHandler()
        {
            Player player = (Player)GameWorld.Instance.FindObjectOfType<Player>();
            Drill drill = (Drill)GameWorld.Instance.FindObjectOfType<Drill>();

            buttonEvent.Attach(player);
            buttonEvent.Attach(drill);

            keybinds.Add(new KeyInfo(Keys.D), new MoveCommand(new Vector2(1, 0)));
            keybinds.Add(new KeyInfo(Keys.A), new MoveCommand(new Vector2(-1, 0)));
            keybinds.Add(new KeyInfo(Keys.W), new MoveCommand(new Vector2(0, -1)));
            keybinds.Add(new KeyInfo(Keys.S), new MoveCommand(new Vector2(0, 1)));
            keybinds.Add(new KeyInfo(Keys.Space), new DrillCommand());
            
        }

        public void Execute(Player player)
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (KeyInfo keyInfo in keybinds.Keys)
            {
                if (keyState.IsKeyDown(keyInfo.Key))
                {
                    keybinds[keyInfo].Execute(player);
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.DOWN);
                    keyInfo.IsDown = true;

                }
                if (!keyState.IsKeyDown(keyInfo.Key) && keyInfo.IsDown == true)
                {
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.UP);
                }
            }
        }

        public void Execute(Drill drill)
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (KeyInfo keyInfo in keybinds.Keys)
            {
                if (keyState.IsKeyDown(keyInfo.Key))
                {
                    keybinds[keyInfo].Execute(drill);
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.DOWN);
                    keyInfo.IsDown = true;

                }
                if (!keyState.IsKeyDown(keyInfo.Key) && keyInfo.IsDown == true)
                {
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.UP);
                }
            }
        }
    }

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


