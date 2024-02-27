using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Sneak_and_seek_dungeons.CommandPattern;
using Sneak_and_seek_dungeons.Components;
using Sneak_and_seek_dungeons.FactoryPattern;
using Sneak_and_seek_dungeons.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    /// <summary>
    /// Inventory er en klasse som bliver brugt til at manage de items som spilleren optjener undervejs i spillet
    /// Frederik
    /// </summary>
    internal class Inventory
    {
        //de forskellige itemslots som holder items
        private List<ItemSlot> hotBar = new List<ItemSlot>();
        private List<ItemSlot> itemSlots = new List<ItemSlot>();
        private List<ItemSlot> playerGear = new List<ItemSlot>();

        //bool som styrer om inventoriet vises
        private bool inventoryHidden = false;

        //clickevent er et event der sker når spilleren klikker på en af itemslotsne
        private ClickEvent clickEvent = new ClickEvent();

        //de item spilleren trækker rundt på i inventoriet
        private Item draggedItem;

        internal Item DraggedItem { get => draggedItem; set => draggedItem = value; }

        //det nuværende equipped våben
        private Weapon weapon;
        internal Weapon Weapon { get => weapon; set => weapon = value; }

        //inventory er en singelton da mange forskellige klasser skal access den
        private static Inventory instance;

        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Inventory();
                }
                return instance;
            }
        }

        public bool InventoryHidden { get => inventoryHidden; set => inventoryHidden = value; }
        internal List<ItemSlot> HotBar { get => hotBar; private set => hotBar = value; }
        internal List<ItemSlot> PlayerGear { get => playerGear; private set => playerGear = value; }
        internal List<ItemSlot> ItemSlots { get => itemSlots; private set => itemSlots = value; }

        //instansiere en masse itemslots som inventoriet består af
        public void SetupInventory()
        {

            Texture2D s = GameWorld.Instance.Content.Load<Texture2D>("PlaceHolder sprites/placeholderItem");
            int spacing = s.Width*2;
            //sets inventory itemslots up
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    GameObject itemSlot = new GameObject();
                    ItemSlot ItemSlotComponent = (ItemSlot)itemSlot.AddComponent(new ItemSlot(ITEMTYPE.ANY));
                    ItemSlots.Add(ItemSlotComponent);
                    itemSlot.AddComponent(new SpriteRenderer());
                    itemSlot.AddComponent(new Collider());
                    GameWorld.Instance.Instantiate(itemSlot);
                    itemSlot.Transform.Position = new Vector2(
                        GameWorld.ScreenSize.X/2 + (x + 5) * spacing,
                        GameWorld.ScreenSize.Y/2 + (y + 8.5f) * spacing
                        );
                    clickEvent.Attach(ItemSlotComponent);
                   
                }
            }
            //sets hotbar itemslots up
            for (int x = 0; x < 5; x++)
            {
                GameObject itemSlot = new GameObject();
                ItemSlot ItemSlotComponent = (ItemSlot)itemSlot.AddComponent(new ItemSlot(ITEMTYPE.ANY));
                HotBar.Add(ItemSlotComponent);
                itemSlot.AddComponent(new SpriteRenderer());
                itemSlot.AddComponent(new Collider());
                GameWorld.Instance.Instantiate(itemSlot);
                itemSlot.Transform.Position = new Vector2(
                    GameWorld.ScreenSize.X / 2 + (x + 5) * spacing,
                    GameWorld.ScreenSize.Y / 2 + (12f) * spacing
                    );
                clickEvent.Attach(ItemSlotComponent);
               
            }

            

            //player weapon slots

            GameObject itemSlot2 = new GameObject();
            WeaponItemSlot ItemSlotComponent2 = (WeaponItemSlot)itemSlot2.AddComponent(new WeaponItemSlot(ITEMTYPE.WEAPON));
            ItemSlotComponent2.AcceptableItems.Add(ITEMTYPE.SWORD);
            PlayerGear.Add(ItemSlotComponent2);
            itemSlot2.AddComponent(new SpriteRenderer());
            itemSlot2.AddComponent(new Collider());
            GameWorld.Instance.Instantiate(itemSlot2);
            itemSlot2.Transform.Position = new Vector2(
                GameWorld.ScreenSize.X / 2 + (3.5f) * spacing,
                GameWorld.ScreenSize.Y / 2 + (8.5f) * spacing
                );
            clickEvent.Attach(ItemSlotComponent2);

            //player helmet slot
            GameObject itemSlot3 = new GameObject();
            ArmorItemSlot ItemSlotComponent3 = (ArmorItemSlot)itemSlot3.AddComponent(new ArmorItemSlot(ITEMTYPE.HELMET));
            PlayerGear.Add(ItemSlotComponent3);
            itemSlot3.AddComponent(new SpriteRenderer());
            itemSlot3.AddComponent(new Collider());
            GameWorld.Instance.Instantiate(itemSlot3);
            itemSlot3.Transform.Position = new Vector2(
                GameWorld.ScreenSize.X / 2 + (2.25f) * spacing,
                GameWorld.ScreenSize.Y / 2 + (8.5f) * spacing
                );
            clickEvent.Attach(ItemSlotComponent3);

            // player chestplate slot
            GameObject itemSlot4 = new GameObject();
            ArmorItemSlot ItemSlotComponent4 = (ArmorItemSlot)itemSlot4.AddComponent(new ArmorItemSlot(ITEMTYPE.CHESTPLATE));
            PlayerGear.Add(ItemSlotComponent4);
            itemSlot4.AddComponent(new SpriteRenderer());
            itemSlot4.AddComponent(new Collider());
            GameWorld.Instance.Instantiate(itemSlot4);
            itemSlot4.Transform.Position = new Vector2(
                GameWorld.ScreenSize.X / 2 + (2.25f) * spacing,
                GameWorld.ScreenSize.Y / 2 + (9.5f) * spacing
                );
            clickEvent.Attach(ItemSlotComponent4);

            //player leggings slot
            GameObject itemSlot5 = new GameObject();
            ArmorItemSlot ItemSlotComponent5 = (ArmorItemSlot)itemSlot5.AddComponent(new ArmorItemSlot(ITEMTYPE.PANTS));
            PlayerGear.Add(ItemSlotComponent5);
            itemSlot5.AddComponent(new SpriteRenderer());
            itemSlot5.AddComponent(new Collider());
            GameWorld.Instance.Instantiate(itemSlot5);
            itemSlot5.Transform.Position = new Vector2(
                GameWorld.ScreenSize.X / 2 + (2.25f) * spacing,
                GameWorld.ScreenSize.Y / 2 + (10.5f) * spacing
                );
            clickEvent.Attach(ItemSlotComponent5);

            //ToggleInventory();
        }


        //viser inventoriet frem ved at enable alle itemslots
        public void ShowInventory()
        {
            foreach (ItemSlot itemSlot in ItemSlots)
            {
                itemSlot.GameObject.Enabled = true;
                if (itemSlot.Item != null)
                    itemSlot.Item.GameObject.Enabled = true;

            }
            foreach (ItemSlot itemSlot in HotBar)
            {
                itemSlot.GameObject.Enabled = true;
                if (itemSlot.Item != null)
                    itemSlot.Item.GameObject.Enabled = true;
            }
            foreach (ItemSlot itemSlot in PlayerGear)
            {
                itemSlot.GameObject.Enabled = true;
                if (itemSlot.Item != null)
                    itemSlot.Item.GameObject.Enabled = true;
            }

        }

        //gemmer inventoriet væk ved at disable alle itemslots
        public void HideInventory()
        {
            foreach  (ItemSlot itemSlot in ItemSlots)
            {
                itemSlot.GameObject.Enabled = false;
                if (itemSlot.Item!=null) 
                    itemSlot.Item.GameObject.Enabled = false;
            }
            /*foreach (ItemSlot itemSlot in HotBar)
            {
                itemSlot.GameObject.Enabled = false;
                if (itemSlot.Item != null)
                    itemSlot.Item.GameObject.Enabled = false;
            }*/
            foreach (ItemSlot itemSlot in PlayerGear)
            {
                itemSlot.GameObject.Enabled = false;
                if (itemSlot.Item != null)
                    itemSlot.Item.GameObject.Enabled = false;
            }

        }

        //skifter inventoriet til at have den modsatte synlighed af hvad den nuværende har
        public void ToggleInventory()
        {
            inventoryHidden = !inventoryHidden;

            if (inventoryHidden)
                HideInventory();
            if (!inventoryHidden)
                ShowInventory();
        }

        //tilføjer et item til spillerens inventory på den første plads der er ledig
        public void AddItemToInventory(Item item)
        {
            foreach (ItemSlot itemSlot in HotBar)
            {
                if (itemSlot.Item == null)
                {
                    itemSlot.Item = item;
                    item.GameObject.Transform.Position = itemSlot.GameObject.Transform.Position;
                    item.removeEvent += itemSlot.RemoveItem;
                    item.IsInInventory = true;
                    ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(item.GameObject);
                    return;
                }
            }

            foreach (ItemSlot itemSlot in ItemSlots)
            {
                if (itemSlot.Item==null)
                {
                    itemSlot.Item = item;
                    item.GameObject.Transform.Position = itemSlot.GameObject.Transform.Position;
                    item.removeEvent += itemSlot.RemoveItem;
                    item.IsInInventory = true;
                    ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(item.GameObject);
                    return;
                }
            }

        }

        /// <summary>
        /// Tilføjer et item til et specefikt plads i inventoriet. bruges sammen med databasen til at sætte items på den plads de var på før
        /// </summary>
        /// <param name="index">hvad for et nummer itemet skal ind på</param>
        /// <param name="item">det item som skal sættes ind</param>
        public void AddItemToSpeceficItemSlot(int index, Item item)
        {
            item.IsInInventory = true;

            if (item.GameObject==null)
            {
                return;
            }

            if (index>4) {
                index -= 5;
                itemSlots[index].Item = item;
                item.GameObject.Transform.Position = itemSlots[index].GameObject.Transform.Position;
                item.removeEvent += itemSlots[index].RemoveItem;
                item.IsInInventory = true;
                ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(item.GameObject);
            }
            else if(index>=0)
            {
                hotBar[index].Item = item;
                item.GameObject.Transform.Position = hotBar[index].GameObject.Transform.Position;
                item.removeEvent += hotBar[index].RemoveItem;
                item.IsInInventory = true;
                ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(item.GameObject);
            }
            else if (index<0)
            {
                foreach (ItemSlot slot in playerGear)
                {
                    if (slot.AcceptableItems.Contains(item.ItemType))
                    {
                        slot.Item = item;
                        item.GameObject.Transform.Position = slot.GameObject.Transform.Position;
                        item.removeEvent += slot.RemoveItem;
                        item.IsInInventory = true;
                        ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(item.GameObject);
                        break;
                    }
                }

            }
        }

        /// <summary>
        /// bruger et item som er i hotbarslottet hvis der er et
        /// </summary>
        /// <param name="hotbarSlot">tal mellem 1-5 som beskriver hvad for et slot aktiveres</param>
        public void UseItemFromHotbar(int hotbarSlot)
        {
            if (HotBar[hotbarSlot-1].Item !=null) {
                HotBar[hotbarSlot - 1].Item.Activate();
            }
            else
            {
                //hvis der ikke er et item i det slot

            }
        }

        //får en liste af alle items i alle itemslots, bruges til at gemme spillet
        public List<Item> GetItems()
        {
            List<Item> itemList = new List<Item>();

            foreach (ItemSlot itemSlot in ItemSlots)
            {
                itemList.Add(itemSlot.Item);
            }
            foreach (ItemSlot itemSlot in HotBar)
            {
                itemList.Add(itemSlot.Item);
            }
            foreach (ItemSlot itemSlot in PlayerGear)
            {
                itemList.Add(itemSlot.Item);
            }

            return itemList;
        }

        /// <summary>
        /// finder et item det nuværende equipped gear som matcher en specefik item type
        /// </summary>
        /// <param name="itemtype">en itemtype som sword eller potion</param>
        /// <returns></returns>
        public Item GetGearOfType(ITEMTYPE itemtype)
        {
            foreach (ItemSlot itemSlot in PlayerGear)
            {
                if (itemSlot.AcceptableItems.Contains(itemtype))
                {
                    return itemSlot.Item;
                }
            }

            return null;
        }

        /// <summary>
        /// når våbenet equippes efter spilleren har trukket et våben i våben slottet
        /// </summary>
        public void OnWeaponEquip()
        {
            //Inventory.Instance.Weapon = ((Weapon)Inventory.Instance.GetGearOfType(ITEMTYPE.WEAPON));
            Weapon w = (Weapon)Inventory.Instance.GetGearOfType(ITEMTYPE.WEAPON);
            if (w == null) { 
                w = (Weapon)Inventory.Instance.GetGearOfType(ITEMTYPE.SWORD);
            }
            Type weaponType = w.GetType();
            //Inventory.Instance.EquipWeapon<weaponType>(w.WeaponType);

            //chat gpt kode
            MethodInfo equipMethod = typeof(Inventory).GetMethod("EquipWeapon").MakeGenericMethod(weaponType);
            equipMethod.Invoke(Inventory.Instance, new object[] { w.WeaponType });
        }

        /// <summary>
        /// equipper et våben
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="weaponType"></param>
        public void EquipWeapon<T>(WEAPONTYPE weaponType) where T : Weapon
        {
            GameObject newWeapon= WeaponFactory.Instance.Create(weaponType);

            if (weapon != null)
            {
                UnEquipWeapon();
            }

            

            weapon = newWeapon.GetComponent<T>() as T;
            weapon.IsEquipped = true;
        }

        /// <summary>
        /// unequipper det nuværende våben
        /// </summary>
        public void UnEquipWeapon()
        {
            GameWorld.Instance.DestroyedGameObjects.Add(weapon.GameObject);
            weapon = null;
        }

        
    }
}
