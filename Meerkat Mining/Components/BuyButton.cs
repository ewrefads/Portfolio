using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.Components
{
    public class BuyButton:Component
    {
        private Texture2D sprite;
        private Vector2 origin;
        private float scale = 1f;
        private float layerDepth = 0.3f;
        private bool cPress = false;

        private PLAYERSTATS stat;
        private Shop shop;

        public BuyButton(PLAYERSTATS stat, Shop shop)
        {
            this.stat = stat;
            this.shop = shop;
        }

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Origin { get => origin; set => origin = value; }
        public float Scale { get => scale; set => scale = value; }
        public float LayerDepth { get => layerDepth; set => layerDepth = value; }

        public override void Draw(SpriteBatch spriteBatch)
        {

            SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            
            Sprite = sr.Sprite;
            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            Vector2 textPos = GameObject.Transform.Position + Origin * scale + new Vector2(-65, 0);
            //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
            spriteBatch.DrawString(GameWorld.Instance.defaultFont, stat.ToString() + ": " + shop.Prices[stat], textPos, Color.Black, 0, Origin, scale, SpriteEffects.None, layerDepth);

        }

        public void SetSprite(string spriteName)
        {
            sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
        }

        public override void Update()
        {
            SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            if (sr.Color != Color.AntiqueWhite) {
                sr.Color = Color.AntiqueWhite;
            }
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);
            Collider collider = (Collider)GameObject.GetComponent<Collider>();
            if (collider.CollisionBox.Contains(mousePos))
            {
                sr.Color = Color.DarkSalmon;
            }
            else {
                sr.Color = Color.AntiqueWhite;
            }
            if (collider.CollisionBox.Contains(mousePos) && mouse.LeftButton == ButtonState.Pressed && !cPress)
            {
                cPress = true;
                sr.Color = Color.Brown;
                shop.Buy(stat);

            }

            else if (cPress && mouse.LeftButton == ButtonState.Released)
            {
                cPress = false;
            }
            else if (cPress) {
                sr.Color = Color.Brown;
            }
            base.Update();
        }
    }
}
