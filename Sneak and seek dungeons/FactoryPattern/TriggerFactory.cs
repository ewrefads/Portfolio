using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    internal class TriggerFactory : Factory
    {
        private static TriggerFactory instance;

        public static TriggerFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TriggerFactory();
                }
                return instance;

            }
        }
        public override GameObject Create(Enum type)
        {
            GameObject trigger = new GameObject();
            trigger.AddComponent(new SpriteRenderer());
            trigger.AddComponent(new Trigger());
            return trigger;
        }
    }
}
