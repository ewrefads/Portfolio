using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{



    public enum WALLDIRECTION {vertical, horizontal};
    public class WallFactory : Factory
    {
        private static WallFactory instance;
        private Texture2D wall_horizontal;
        private Texture2D wall_vertical;

        private WallFactory()
        {
            wall_horizontal = GameWorld.Instance.Content.Load<Texture2D>("wall");
            wall_vertical = GameWorld.Instance.Content.Load<Texture2D>("wall_vertical");
        }

        public static WallFactory Instance { get {
                if (instance == null) {
                    instance = new WallFactory();
                }
                return instance;
            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject wall = new GameObject();
            wall.Tag = "dungeon";
            Collider col = new Collider();
            col.ActiveCollider = true;
            GameWorld.Instance.DungeonColliders.Add(col);
            wall.AddComponent(col);
            SpriteRenderer sr = new SpriteRenderer();
          
            if (type is WALLDIRECTION.horizontal)
            {
                sr.Sprite = wall_horizontal;
            }
            else if (type is WALLDIRECTION.vertical) {
                sr.Sprite = wall_vertical;
            }
            wall.AddComponent(sr);
            return wall;
        }
    }
}
