using Sneak_and_seek_dungeons;
using Sneak_and_seek_dungeons.Components;
using Sneak_and_seek_dungeons.FactoryPattern;
using SneekAndSeekDatabase.RepositoryPattern;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace SneekAndSeekDatabase
{
    internal class SneekAndSeekRepository: ISneekAndSeekRepository
    {
        private SneekAndSeekMapper mapper;
        private SQLiteDatabaseProvider provider;

        public SneekAndSeekRepository(SneekAndSeekMapper mapper, SQLiteDatabaseProvider provider)
        {
            this.mapper = mapper;
            this.provider = provider;

            CreateDatabaseTables();
        }


        private void CreateDatabaseTables()
        {
            var connection = provider.CreateConnection();
            connection.Open();

            
            var cmd = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS playerInventory(slotNumber VARCHAR(10) PRIMARY KEY, type VARCHAR(50))", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();

            connection.Close();
        }


        public void ExecuteNonquery(string command) {
            var connection = provider.CreateConnection();
            connection.Open();


            var cmd = new SQLiteCommand(command, (SQLiteConnection)connection);
            try
            {
                int res = cmd.ExecuteNonQuery();
                Console.WriteLine($"{res} rows affected");
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());

            }
            connection.Close();
        }

        public List<string[]> ExecuteReader(string command)
        {
            var connection = provider.CreateConnection();
            connection.Open();
            try
            {
                List<string[]> res = new List<string[]>();
                var cmd = new SQLiteCommand(command, (SQLiteConnection)connection);
                var data = cmd.ExecuteReader();
                int l = data.FieldCount;
                string[] collumns = new string[l];

                for (int j = 0; j < l; j++)
                {
                    collumns[j] = $"{data.GetName(j)}";
                }


                while (data.Read())
                {
                    string[] row = new string[l];
                    for (int i = 0; i < l; i++)
                    {
                        row[i] = $"{data.GetFieldValue<object>(i)}";

                    }
                    res.Add(row);
                }
                connection.Close();
                return res;
            }
            catch
            {
                throw new Exception();
            }
            

            
        }
        
        /// <summary>
        /// Metode til at gemme spillet til databasen
        /// </summary>
        public void SaveGame() {
            Inventory inventory = Inventory.Instance;
            List<Item> inventoryItems = inventory.GetItems();
            List<ItemSlot> inventorySlot = inventory.ItemSlots;
            List<ItemSlot> equiped = inventory.PlayerGear;
            List<ItemSlot> hotBar = inventory.HotBar;
            var connection = provider.CreateConnection();
            connection.Open();
            var cmd = new SQLiteCommand("DELETE FROM playerInventory", (SQLiteConnection)connection);
            cmd.ExecuteNonQuery();
            for (int i = 0; i < inventoryItems.Count; i++) {
                Item c = inventoryItems[i];
                if(c!= null && c.ItemType != ITEMTYPE.KEY){
                    string slot = "";
                    for (int j = 0; j < inventorySlot.Count; j++)
                    {
                        ItemSlot iS = inventorySlot[j];
                        if (iS.Item == c)
                        {
                            slot = "i" + j;
                            break;
                        }
                    }
                    for (int j = 0; j < equiped.Count; j++)
                    {
                        ItemSlot iS = equiped[j];
                        if (iS.Item == c)
                        {
                            slot += "e" + j;
                            break;
                        }
                    }
                    for (int j = 0; j < hotBar.Count; j++)
                    {
                        ItemSlot iS = hotBar[j];
                        if (iS.Item == c)
                        {
                            slot = "h" + j;
                            break;
                        }
                    }

                    if (c != null)
                    {
                        cmd = new SQLiteCommand($"INSERT INTO playerInventory VALUES('{slot}', '{c.ItemType}')", (SQLiteConnection)connection);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            connection.Close();
        }

        /// <summary>
        /// Metode til at indlæse spillet fra databasen
        /// </summary>
        public void LoadGame() {
            var connection = provider.CreateConnection();
            connection.Open();
            List<string[]> inven = ExecuteReader("SELECT * FROM playerInventory");
            connection.Close();
            Dictionary<int, string> equiped = new Dictionary<int, string>();
            Dictionary<int, string> hotBar = new Dictionary<int, string>();
            Dictionary<int, string> inventory = new Dictionary<int, string>();
            foreach (string[] s in inven) {
                char place = s[0][0];
                string slot = s[0].Substring(1);
                if (place == 'i') {
                    inventory.Add(Convert.ToInt32(slot), s[1]);
                }
                else if (place == 'e') {
                    equiped.Add(Convert.ToInt32(slot), s[1]);
                }
                else if (place == 'h') {
                    hotBar.Add(Convert.ToInt32(slot), s[1]);
                }
            }
            GameObject[] equipedItems = new GameObject[Inventory.Instance.PlayerGear.Count];
            Player player = (Player)GameWorld.Instance.FindObjectOfType<Player>();
            foreach(int s in equiped.Keys){
                string item = equiped[s];
                GameObject newItem = new GameObject();
                Item nItem = new Item();
                switch (item)
                {
                    case ("WEAPON"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.WEAPON);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        
                        break;
                    case ("HELMET"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.HELMET);
                        nItem = (Item)newItem.GetComponent<Helmet>();
                        break;
                    case ("CHESTPLATE"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.CHESTPLATE);
                        nItem = (Item)newItem.GetComponent<Chestplate>();
                        break;
                    case ("PANTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.PANTS);
                        nItem = (Item)newItem.GetComponent<Pants>();
                        break;
                    case ("BOOTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Armor>();
                        break;
                    case ("RING"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Item>();
                        break;
                    case ("POTION"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.POTION);
                        nItem = (Item)newItem.GetComponent<Potion>();
                        break;
                    case ("SWORD"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.SWORD);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        Inventory.Instance.EquipWeapon<Sword>(WEAPONTYPE.sword);
                        Inventory.Instance.OnWeaponEquip();
                        break;
                }
                Inventory.Instance.AddItemToSpeceficItemSlot(-1, nItem);
                if (item == "WEAPON" || item == "SWORD") {
                    Inventory.Instance.EquipWeapon<Sword>(WEAPONTYPE.sword);
                    Inventory.Instance.OnWeaponEquip();
                }

            }
            GameObject[] hotBarItems = new GameObject[Inventory.Instance.HotBar.Count];
            foreach (int s in hotBar.Keys)
            {
                string item = hotBar[s];
                GameObject newItem = new GameObject();
                Item nItem = new Item();
                switch (item)
                {
                    case ("WEAPON"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.WEAPON);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        break;
                    case ("HELMET"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.HELMET);
                        nItem = (Item)newItem.GetComponent<Helmet>();
                        break;
                    case ("CHESTPLATE"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.CHESTPLATE);
                        nItem = (Item)newItem.GetComponent<Chestplate>();
                        break;
                    case ("PANTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.PANTS);
                        nItem = (Item)newItem.GetComponent<Pants>();
                        break;
                    case ("BOOTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Armor>();
                        break;
                    case ("RING"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Item>();
                        break;
                    case ("POTION"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.POTION);
                        nItem = (Item)newItem.GetComponent<Potion>();
                        break;
                    case ("SWORD"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.SWORD);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        break;
                    case ("KEY"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.KEY);
                        nItem = (Item)newItem.GetComponent<Item>();
                        break;
                }
                Inventory.Instance.AddItemToSpeceficItemSlot(s, nItem);


            }
            GameObject[] inventoryItems = new GameObject[Inventory.Instance.ItemSlots.Count];
            foreach (int s in inventory.Keys)
            {
                string item = inventory[s];
                GameObject newItem = new GameObject();
                Item nItem = new Item();
                switch (item)
                {
                    case ("WEAPON"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.WEAPON);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        break;
                    case ("HELMET"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.HELMET);
                        nItem = (Item)newItem.GetComponent<Helmet>();
                        break;
                    case ("CHESTPLATE"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.CHESTPLATE);
                        nItem = (Item)newItem.GetComponent<Chestplate>();
                        break;
                    case ("PANTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.PANTS);
                        nItem = (Item)newItem.GetComponent<Pants>();
                        break;
                    case ("BOOTS"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Armor>();
                        break;
                    case ("RING"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.ANY);
                        nItem = (Item)newItem.GetComponent<Item>();
                        break;
                    case ("POTION"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.POTION);
                        nItem = (Item)newItem.GetComponent<Potion>();
                        break;
                    case ("SWORD"):
                        newItem = ItemFactory.Instance.Create(ITEMTYPE.SWORD);
                        nItem = (Item)newItem.GetComponent<Sword>();
                        break;
                }
                Inventory.Instance.AddItemToSpeceficItemSlot(5 + s, nItem);


            }

        }
        
    }
}