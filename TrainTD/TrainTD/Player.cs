using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TrainTD
{
    /// <summary>
    /// Klasse for spilleren
    /// </summary>
    public class Player : GameObject
    {
        //Stål bruges til at købe ting for og kul bruges af lokomotiver til at bevæge sig(ikke implementeret)
        private int steel;
        private int coal;

        //Hvor meget skade spilleren kan tage før de taber
        private int health;
        
        //Hvor meget de forskellige ting spilleren kan købe koster
        private PriceList priceList;

        public Player(Vector2 position, string[] assets, float animationSpeed) : base(position, assets, animationSpeed)
        {
            priceList = new PriceList();
            Steel = 20;
            Coal = 10;
        }

        public Player(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, float animationSpeed) : base(position, sprites, spriteEffect, lootValue, 0, animationSpeed)
        {
            priceList = new PriceList();
            Steel = 15;
            Coal = 10;
        }

        public PriceList PriceList { get => priceList; }
        public int Steel { get => steel; set => steel = value; }
        public int Coal { get => coal; set => coal = value; }
        public int Health { get => health; set => health = value; }

        public override GameObject getCopy()
        {
            throw new NotImplementedException();
        }

        public int getSteelPrice(GameObject objectToBuy) {
            if (objectToBuy is Tower)
            {
                return priceList.towerPrice;
            }
            else if (objectToBuy is Locomotive)
            {
                return priceList.locomotivePrice;
            }
            else if (objectToBuy is CargoCarriage)
            {
                return priceList.cargoCarriagePrice;
            }
            else if (objectToBuy is WeaponsCarriage)
            {
                return priceList.weaponsCarriagePrice;
            }
            else if (objectToBuy is Track) {
                return priceList.trackPrice;
            }
            else
            {
                return 0;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            Texture2D[] s = new Texture2D[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                s[i] = content.Load<Texture2D>(assets[i]);
            }

            sprites = s;
        }

        public override int lootFromObject()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        protected override bool PlacementExceptions(GameObject go)
        {
            return false;
        }
    }
}
