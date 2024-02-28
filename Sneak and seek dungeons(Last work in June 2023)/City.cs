using Microsoft.Xna.Framework;
using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    internal class City
    {

        private List<Vector2> positions = new List<Vector2>();
        private List<IBuilding> buildings = new List<IBuilding>();


        private static City instance;

        public static City Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new City();
                }
                return instance;
            }
        }

        public List<Vector2> Positions { get => positions; set => positions = value; }
        internal List<IBuilding> Buildings { get => buildings; set => buildings = value; }

        public void SetupCity()
        {

            //makes new player object

            //Camera cam = ((GameWorld.Instance.FindObjectOfType<Camera>()).GameObject.GetComponent<Camera>() as Camera);

            positions.Add(new Vector2(GameWorld.ScreenSize.X / 4 * 1, GameWorld.ScreenSize.Y * 0.8f));
            positions.Add(new Vector2(GameWorld.ScreenSize.X / 4 * 2, GameWorld.ScreenSize.Y * 0.8f));
            positions.Add(new Vector2(GameWorld.ScreenSize.X / 4 * 3, GameWorld.ScreenSize.Y*0.8f));

            float spacing = 400;

            GameObject shop = new GameObject();
            Shop sh =(Shop) shop.AddComponent(new Shop());
            shop.AddComponent(new SpriteRenderer());
            SceneManager.Instance.CityGameObjects1.Add(shop);
            buildings.Add(sh);
            shop.Transform.Position = positions[0] + new Vector2(0, -spacing);


            GameObject smith = new GameObject();
            Smith sm = (Smith)smith.AddComponent(new Smith());
            smith.AddComponent(new SpriteRenderer());
            SceneManager.Instance.CityGameObjects1.Add(smith);
            buildings.Add(sm);
            smith.Transform.Position = positions[1] + new Vector2(0, -spacing);


            GameObject dungeon = new GameObject();
            DungeonBuilding db = (DungeonBuilding)dungeon.AddComponent(new DungeonBuilding());
            dungeon.AddComponent(new SpriteRenderer());
            SceneManager.Instance.CityGameObjects1.Add(dungeon);
            buildings.Add(db);
            dungeon.Transform.Position = positions[2] + new Vector2(0, -spacing);

            foreach (IBuilding building in buildings)
            {
                building.DeActivate();
            }

            //player spawn
            GameObject player = new GameObject();
            player.AddComponent(new PlayerCity());
            player.AddComponent(new SpriteRenderer());
            SceneManager.Instance.CityGameObjects1.Add(player);

        }
    }
}
