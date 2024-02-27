using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    /// <summary>
    /// Spawner de forskellige items som er i inventoriet
    /// Frederik
    /// </summary>
    internal class ItemFactory : Factory
    {
        private static ItemFactory instance;

        public static ItemFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ItemFactory();
                }
                return instance;

            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject newItem = new GameObject();



            if (type is ITEMTYPE.POTION)
            {
                newItem.AddComponent(new Potion());
            }

            else if (type is ITEMTYPE.CHESTPLATE)
            {
                newItem.AddComponent(new Chestplate());
            }
            else if (type is ITEMTYPE.HELMET)
            {
                newItem.AddComponent(new Helmet());
            }
            else if (type is ITEMTYPE.PANTS)
            {
                newItem.AddComponent(new Pants());
            }

            else if (type is ITEMTYPE.SWORD)
            {
                newItem.AddComponent(new Sword());
            }
            else if (type is ITEMTYPE.WEAPON)
            {
                newItem.AddComponent(new Sword());
            }
            else if (type is ITEMTYPE.KEY) {
                Item i = (Item)newItem.AddComponent(new Item());
                i.ItemType = ITEMTYPE.KEY;
                i.Uses = 1;
            }
            newItem.AddComponent(new SpriteRenderer());
            GameWorld.Instance.Instantiate(newItem);

            return newItem;
        }
    }
}
