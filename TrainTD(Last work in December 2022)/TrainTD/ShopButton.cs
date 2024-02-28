using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTD
{
    /// <summary>
    /// En klasse som viser en knap hvor spilleren kan købe nye tårne, tog osv
    /// Shopbutton er et uielement
    /// </summary>
    public class ShopButton:UIElement
    {
        //hvilke item er i shoppen
        private GameObject item;
        //det item som spilleren lige har købt
        private GameObject currentPlacement;

        SpriteFont shopFont;

        public ShopButton(GameObject item, Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime, SpriteFont shopFont) : base(activePlayer, sprites, activeColor, hoverColor, clickColor, inActiveColor, position, animationTime)
        {
            this.item = item;
            item.Layer = 0.8f;
            this.shopFont = shopFont;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            
            base.Draw(_spriteBatch);
            item.Draw(_spriteBatch);
            Vector2 pricePosition = new Vector2(position.X + (sprites[0].Width * scale) / 2 - 50, position.Y + sprites[0].Height * scale - 40);
            _spriteBatch.DrawString(shopFont, "Price: " + activePlayer.getSteelPrice(item), pricePosition, Color.Black, 0f, Vector2.Zero, 1, SpriteEffects.None, 0.9f);

        }

        /// <summary>
        /// Tjekker hvilken farve knappen bør være
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (activePlayer.getSteelPrice(item) > activePlayer.Steel)
            {
                active = false;
                currentColor = inActiveColor;
            }
            else
            {
                active = true;
                currentColor = activeColor;
            }
            if (currentPlacement != null) {
                if (!currentPlacement.BeingPlaced) {
                    idleActive = false;
                    currentPlacement = null;
                }
            }
            base.Update(gameTime);
            
        }
        protected override void HandleInput() {
            base.HandleInput();
        }

        /// <summary>
        /// når knappen klikkes så laves et nyt objekt i spilverdnen
        /// </summary>
        protected override void OnCLick()
        {
            base.OnCLick();
            //laver et nyt objekt af samme type som det item der er på knappen
            GameObject placeableItem = item.getCopy();
            //vi er i gang med at plasere det item vi lige har købt
            placeableItem.BeingPlaced = true;
            currentPlacement = placeableItem;
            //det købte item for at vide at det er blevet købt og opdatere sine værdier
            placeableItem.Bought(item);
            //itemet bliver spawned ind i spilberdnen
            GameWorld.InstantiateGameObject(placeableItem);
        }
    }
}
