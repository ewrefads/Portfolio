using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class LootChest : Component, IInteractable
    {
        private int itemDropAmount=3;
        List<Item> items = new List<Item>();

        public List<Item> Items { get => items;}

        public override void Awake()
        {
            for (int i = 0; i < itemDropAmount; i++)
            {
                ITEMTYPE[] itemTypes = (ITEMTYPE[])Enum.GetValues(typeof(ITEMTYPE));
                
                Random random = new Random();
                int num = random.Next(1, itemTypes.Length);
                while (itemTypes[num] == ITEMTYPE.KEY) {
                    num = random.Next(1, itemTypes.Length);
                }
                GameObject go = ItemFactory.Instance.Create(itemTypes[num]);
                SpriteRenderer sr = (SpriteRenderer)go.GetComponent<SpriteRenderer>();
                sr.IsEnabled = false;
                Item item = (Item)go.GetComponent<Item>();
                if (item == null) {
                    ITEMTYPE itemType = ITEMTYPE.ANY;
                    while (item == null) {
                        if (itemType == ITEMTYPE.ANY)
                        {
                            item = (Item)go.GetComponent<Helmet>();
                            itemType = ITEMTYPE.HELMET;
                        }
                        else if (itemType == ITEMTYPE.HELMET) {
                            item = (Item)go.GetComponent<Chestplate>();
                            itemType = ITEMTYPE.CHESTPLATE;
                        }
                        else if (itemType == ITEMTYPE.CHESTPLATE)
                        {
                            item = (Item)go.GetComponent<Pants>();
                            itemType = ITEMTYPE.PANTS;
                        }
                        else if (itemType == ITEMTYPE.PANTS)
                        {
                            item = (Item)go.GetComponent<Potion>();
                            itemType = ITEMTYPE.POTION;
                        }
                        else if (itemType == ITEMTYPE.POTION)
                        {
                            item = (Item)go.GetComponent<Sword>();
                            itemType = ITEMTYPE.SWORD;
                        }
                    }
                }
                Items.Add(item);
            }
            base.Awake();
        }

        public void Interact()
        {
            
            foreach (Item item in items) {
                SpriteRenderer sr = (SpriteRenderer)item.GameObject.GetComponent<SpriteRenderer>();
                sr.IsEnabled = true;
                item.Start();
                sr.Start();
                Inventory.Instance.AddItemToInventory(item);
            }
        }

        public override void Update()
        {
            if (itemDropAmount < items.Count) {
                foreach (Item item in items)
                {
                    SpriteRenderer sr = (SpriteRenderer)item.GameObject.GetComponent<SpriteRenderer>();
                    if (sr.IsEnabled)
                    {
                        sr.IsEnabled = false;
                        itemDropAmount++;
                    }
                }
            }
            
            base.Update();
        }
    }
}
