using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Factory = Sneak_and_seek_dungeons.FactoryPattern.Factory;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    internal class EmoteFactory : Factory
    {

        private static EmoteFactory instance;

        public static EmoteFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmoteFactory();
                }
                return instance;

            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = (SpriteRenderer)go.AddComponent(new SpriteRenderer());

            sr.SetSprite("Sprites/Enemy emote/distraction");

            GameWorld.Instance.Instantiate(go);

            return go;
        }
    }
}
