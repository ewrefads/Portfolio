using Meerkat_Mining.Components;
using Meerkat_Mining.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class Shop: Component
    {
        private Dictionary<PLAYERSTATS, int> upgradePrices;
        private Player player;
        private static Shop instance;
        private Collider collider;
        private Collider playerCollider;
        private Dictionary<BLOCKTYPE, int> sellPrices;
        private GameObject ui;

        private Shop() {
            upgradePrices = new Dictionary<PLAYERSTATS, int>
            {
                { PLAYERSTATS.MININGSPEED, 1 },
                { PLAYERSTATS.MOVESPEED, 1 },
                { PLAYERSTATS.MININGDAMAGE, 1 }
            };
            sellPrices = new Dictionary<BLOCKTYPE, int>
            {
                { BLOCKTYPE.IRON, 1 },
                { BLOCKTYPE.STONE, 1 },
                { BLOCKTYPE.GOLD, 1 },
                { BLOCKTYPE.BAUXITE, 1 },
                { BLOCKTYPE.CINNABAR, 1 },
                { BLOCKTYPE.IRIDIUM, 1 },
                { BLOCKTYPE.PYRITE, 1 },
                { BLOCKTYPE.AIR, 0 }
            };
            
        }

        public Dictionary<PLAYERSTATS, int> Prices { get => upgradePrices;}
        public static Shop Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Shop();
                }
                return instance;
            }
        }

        public Player Player { get => player; set => player = value; }

        public void Initiate(Player player) {
            instance.Player = player;
            playerCollider = (Collider)player.GameObject.GetComponent<Collider>();
            collider = (Collider)GameObject.GetComponent<Collider>();
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        public bool Buy(PLAYERSTATS stat) {

            

            if (Player.Money >= upgradePrices[stat]) {
                Player.Money -= upgradePrices[stat];
                

                if (Player.Stats[stat]>1)
                {
                    Player.Stats[stat] *= 1.5f;
                }
                else { Player.Stats[stat] %= 1.5f; }

                player.UpdateStats();

                instance.upgradePrices[stat] = upgradePrices[stat] * 2;
                ShopWindow sh = (ShopWindow)ui.GetComponent<ShopWindow>();
                sh.CurrentHippo = sh.HappyHippo;
                return true;
                
            }
            return false;
        }

        public void SellAll() {
            foreach (BLOCKTYPE block in Player.Inventory.Keys) {
                Sell(block, Player.Inventory[block]);
            }
            Player.Inventory.Clear();
        }

        private void Sell(BLOCKTYPE mineral, int amount) {
            Player.Money += instance.sellPrices[mineral] * amount;
        }

        public override void Update()
        {
            base.Update();
            if (collider.CollisionBox.Intersects(playerCollider.CollisionBox) && ui == null)
            {
                ui = new UIFactory().Create(null);
                GameWorld.Instance.NewuiObjects.Add(ui);
                SellAll();

            }
            else if (!collider.CollisionBox.Intersects(playerCollider.CollisionBox) && ui != null) {
                ShopWindow s = (ShopWindow)ui.GetComponent <ShopWindow>();
                s.DestroyShop();
                ui = null;
            }
        }
    }
}
