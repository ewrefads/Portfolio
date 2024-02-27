using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meerkat_Mining;

namespace Meerkat_Mining
{
    public class ShopWindow:Component
    {
        private Texture2D sprite;
        private Vector2 origin;
        private Color color;
        private float scale;
        private float layerDepth = 0.1f;
        private List<GameObject> buyButtons = new List<GameObject>();
        private Player player;
        private Texture2D neutralHippo;
        private Texture2D happyHippo;
        private Texture2D currentHippo;
        

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Origin { get => origin; set => origin = value; }
        public float Scale { get => scale; set => scale = value; }
        public float LayerDepth { get => layerDepth; set => layerDepth = value; }
        public Color Color { get => color; set => color = value; }
        public List<GameObject> BuyButtons { get => buyButtons;}
        public Player Player { get => player; set => player = value; }
        public Texture2D HappyHippo { get => happyHippo; set => happyHippo = value; }
        public Texture2D NeutralHippo { get => neutralHippo; set => neutralHippo = value; }
        public Texture2D CurrentHippo { get => currentHippo; set => currentHippo = value; }

        public override void Draw(SpriteBatch spriteBatch)
        {


            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
            spriteBatch.Draw(sprite, GameObject.Transform.Position, null, Color, 0f, Origin, scale, SpriteEffects.None, LayerDepth);
            spriteBatch.Draw(CurrentHippo, GameObject.Transform.Position + new Vector2(199, 182), null, Color, 0f, Origin, scale + 1.25f, SpriteEffects.None, LayerDepth + 0.1f);
            //spriteBatch.DrawString(GameWorld.Instance.defaultFont, "Player Money:" + player.Money, new Vector2(GameObject.Transform.Position.X, 280), Color.Black);
            spriteBatch.DrawString(GameWorld.Instance.defaultFont, "Player Money: " + player.Money, new Vector2(GameObject.Transform.Position.X + 20, 320), Color.Black, 0, Origin, scale, SpriteEffects.None, layerDepth + 0.4f);

        }

        public void SetSprite(string spriteName)
        {
            sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
            NeutralHippo = GameWorld.Instance.Content.Load<Texture2D>("hippo_chat_image");
            HappyHippo = GameWorld.Instance.Content.Load<Texture2D>("hippo_chat_image_happy");
            CurrentHippo = NeutralHippo;
        }

        public override void Start()
        {
            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            if (scale == 0)
            {
                scale = 1;
            }


            Color = Color.White;

        }

        public void DestroyShop() {
            GameWorld.Instance.DestroyeduiObjects.Add(GameObject);
            foreach (GameObject b in BuyButtons) {
                GameWorld.Instance.DestroyeduiObjects.Add(b);
            }
        }

    }
}
