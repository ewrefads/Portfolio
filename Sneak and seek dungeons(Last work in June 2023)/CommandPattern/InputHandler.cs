using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sneak_and_seek_dungeons.Components;
using Sneak_and_seek_dungeons.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Sneak_and_seek_dungeons.CommandPattern
{
    /// <summary>
    /// Spillerens input i spillet 
    /// Frederik
    /// </summary>
    internal class InputHandler
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

        MouseState oldMouseState;

        //de commands der kan laves
        private Dictionary<KeyInfo, ICommand> keyBinds = new Dictionary<KeyInfo, ICommand>();

        private Player player;

        //events bliver sat op
        private ButtonEvent buttonEvent = new ButtonEvent();
        private CollisionEvent collisionEvent = new CollisionEvent();
        
        //når den bliver lavet så tilføjer den kommandoer til vores dictionary
        private InputHandler()
        {
            player = (Player)GameWorld.Instance.FindObjectOfType<Player>();
        
            buttonEvent.Attach(player);
            collisionEvent.Attach(player);


            keyBinds.Add(new KeyInfo(Keys.W), new MoveCommand(new Vector2(0, -1)));
            keyBinds.Add(new KeyInfo(Keys.S), new MoveCommand(new Vector2(0, 1)));
            keyBinds.Add(new KeyInfo(Keys.D), new MoveCommand(new Vector2(1, 0)));
            keyBinds.Add(new KeyInfo(Keys.A), new MoveCommand(new Vector2(-1, 0)));
            

            keyBinds.Add(new KeyInfo(Keys.D1), new AbilityCommand(1));
            keyBinds.Add(new KeyInfo(Keys.D2), new AbilityCommand(2));
            keyBinds.Add(new KeyInfo(Keys.D3), new AbilityCommand(3));
            keyBinds.Add(new KeyInfo(Keys.D4), new AbilityCommand(4));
            keyBinds.Add(new KeyInfo(Keys.D5), new AbilityCommand(5));

            keyBinds.Add(new KeyInfo(Keys.LeftShift), new OtherCommand());
            keyBinds.Add(new KeyInfo(Keys.Tab), new OtherCommand());
            keyBinds.Add(new KeyInfo(Keys.T), new OtherCommand());
            keyBinds.Add(new KeyInfo(Keys.Space), new OtherCommand());
            keyBinds.Add(new KeyInfo(Keys.F), new OtherCommand());
            

        }

        //player bliver notified hvis en knap i som er i dictionarien bliver trykket
        public void Execute(Player player)
        {
            KeyboardState keyState = Keyboard.GetState();

            foreach (KeyInfo keyInfo in keyBinds.Keys)
            {
                if (keyState.IsKeyDown(keyInfo.Key))
                {
                    keyBinds[keyInfo].Execute(player);
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.DOWN);
                    keyInfo.IsDown = true;

                }
                if (!keyState.IsKeyDown(keyInfo.Key) && keyInfo.IsDown == true)
                {
                    buttonEvent.Notify(keyInfo.Key, BUTTONSTATE.UP);
                }


            }
            Player p = player.GameObject.GetComponent<Player>() as Player;
            p.CollisionDirections.Clear();

            Collider playerCollider = player.GameObject.GetComponent<Collider>() as Collider;

            foreach (Rectangle room in GameWorld.Instance.Dun.Rooms)
            {
                if (playerCollider.CollisionBox.Intersects(room))
                {
                    GameObject other = new GameObject();
                    Room r =(Room) other.AddComponent(new Room());
                    r.RoomBox = room;
                    //collisionEvent.Notify(other);
                }
            }


        }

        /// <summary>
        /// itemslottet finder ud af om det bliver trykket på
        /// </summary>
        /// <param name="itemSlot">det itemslot som bliver notified at det bliver trykke tpå</param>
        public void Execute(ItemSlot itemSlot)
        {
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            Camera cam = ((GameWorld.Instance.FindObjectOfType<Camera>()).GameObject.GetComponent<Camera>() as Camera);
            Vector2 camOffset = new Vector2(cam.transform.Translation.X,cam.transform.Translation.Y);

            Collider collider = (Collider)itemSlot.GameObject.GetComponent<Collider>();
            Vector2 adjustedMousePos = (mousePos - camOffset) / cam.zoom;

            if (collider.CollisionBox.Contains(adjustedMousePos) && mouse.LeftButton == ButtonState.Pressed && mouse != oldMouseState)
            {
                itemSlot.Notify(new ClickEvent());
                oldMouseState = mouse;
            }
        }

        

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
