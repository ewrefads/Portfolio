using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Sneak_and_seek_dungeons.FactoryPattern
{



    public enum CONTAINERTYPE {chest, scavengePile, RessourceNode};
    public class ContainerFactory : Factory
    {
        private static ContainerFactory instance;
        private Texture2D chest;
        private Texture2D wall_vertical;
        private Texture2D[] wallArray;

        private ContainerFactory()
        {
            chest = GameWorld.Instance.Content.Load<Texture2D>("Chest_closed");

        }

        public static ContainerFactory Instance { get {
                if (instance == null) {
                    instance = new ContainerFactory();
                }
                return instance;
            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject container = new GameObject();
            container.Tag = "dungeon";
            Collider col = new Collider();
            col.ActiveCollider = false;
            GameWorld.Instance.DungeonColliders.Add(col);
            container.AddComponent(col);
            container.AddComponent(new LootChest());
            SpriteRenderer sr = new SpriteRenderer();
            sr.Sprite = chest;
            container.AddComponent(sr);
            return container;
        }
    }
}
