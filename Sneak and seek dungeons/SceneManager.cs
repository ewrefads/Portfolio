using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    public enum SCENEPROPERTY { DUNGEON, CITY}
    
    
    internal class SceneManager
    {

        private List<GameObject> DungeonGameObjects = new List<GameObject>();
        private List<GameObject> CityGameObjects = new List<GameObject>();

        public SCENEPROPERTY scene = SCENEPROPERTY.CITY;

        private bool dungeonLoadedBefore=false;
        private bool cityLoadedBefore = false;

        private static SceneManager instance;

        public static SceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneManager();
                }
                return instance;
            }
        }

        public List<GameObject> DungeonGameObjects1 { get => DungeonGameObjects; set => DungeonGameObjects = value; }
        public List<GameObject> CityGameObjects1 { get => CityGameObjects; set => CityGameObjects = value; }
        public bool DungeonLoadedBefore { get => dungeonLoadedBefore; set => dungeonLoadedBefore = value; }
        public bool CityLoadedBefore { get => cityLoadedBefore; set => cityLoadedBefore = value; }

        public void LoadDungeon()
        {
            GameWorld.Instance.GameObjects = DungeonGameObjects;

            if (dungeonLoadedBefore)
                return;
            foreach (GameObject go in GameWorld.Instance.GameObjects)
            {
                go.Awake();
                go.Start();
            }

            dungeonLoadedBefore = true;
        }

        public void UnloadDungeon()
        {
            DungeonGameObjects = GameWorld.Instance.GameObjects;
            
        }

        public void LoadCity()
        {
            GameWorld.Instance.GameObjects = CityGameObjects;

            if (cityLoadedBefore)
                return;
            foreach (GameObject go in GameWorld.Instance.GameObjects)
            {
                go.Awake();
                go.Start();
            }

            cityLoadedBefore = true;
        }

        public void UnloadCity()
        {
            CityGameObjects = GameWorld.Instance.GameObjects;
        }
        
    }
}
