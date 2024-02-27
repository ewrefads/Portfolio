using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{



    public enum DOORDIRECTION {vertical, horizontal};
    public class DoorFactory : Factory
    {
        private static DoorFactory instance;
        private Texture2D doorHorizontalClosed;
        private Texture2D doorVerticalClosed;
        private Texture2D[] verticalDoorArray;
        private Texture2D[] horizontalDoorArray;

        private DoorFactory()
        {
            doorHorizontalClosed = GameWorld.Instance.Content.Load<Texture2D>("door_closed");
            horizontalDoorArray = new Texture2D[] { doorHorizontalClosed, GameWorld.Instance.Content.Load<Texture2D>("door_open") };
            doorVerticalClosed = GameWorld.Instance.Content.Load<Texture2D>("door_closed_vertical");
            verticalDoorArray = new Texture2D[]{ doorVerticalClosed, GameWorld.Instance.Content.Load<Texture2D>("door_open_vertical")};
        }

        public static DoorFactory Instance { get {
                if (instance == null) {
                    instance = new DoorFactory();
                }
                return instance;
            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject door = new GameObject();
            door.Tag = "dungeon";
            Collider col = new Collider();
            col.ActiveCollider = true;
            GameWorld.Instance.DungeonColliders.Add(col);
            door.AddComponent(col);
            
            SpriteRenderer sr = new SpriteRenderer();
            if (type is DOORDIRECTION.horizontal)
            {
                sr.Sprite = doorHorizontalClosed;
                Door d = new Door(horizontalDoorArray, (DOORDIRECTION)type);
                door.AddComponent(d);
            }
            else if (type is DOORDIRECTION.vertical) {
                sr.Sprite = doorVerticalClosed;
                Door d = new Door(verticalDoorArray, (DOORDIRECTION)type);
                door.AddComponent(d);
            }
            door.AddComponent(sr);
            return door;
        }
    }
}
