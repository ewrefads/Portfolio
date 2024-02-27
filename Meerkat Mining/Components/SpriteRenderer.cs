using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class SpriteRenderer: Component
    {
        private Texture2D sprite;
        private Vector2 origin;
        private Color color;
        private float scale;
        private float startScale;
        private float layerDepth = 0.1f;
        private SpriteEffects spriteEffect;

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Origin { get => origin; set => origin = value; }
        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public float LayerDepth { get => layerDepth; set => layerDepth = value; }
        public Color Color { get => color; set => color = value; }
        public float StartScale { get => startScale; set => startScale = value; }
        public SpriteEffects SpriteEffect { get => spriteEffect; set => spriteEffect = value; }

        public override void Draw(SpriteBatch spriteBatch)
        {

            

            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
            spriteBatch.Draw(sprite, GameObject.Transform.Position, null, Color, 0f, Origin,Scale, SpriteEffect, LayerDepth);


        }

        public void SetSprite(string spriteName)
        {
            sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
        }

        public override void Start()
        {
            Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
            if (scale == 0) {
                scale = 1;
            }
            startScale = scale;

            if (Color==new Color()) {
                Color = Color.White;
            }

            SpriteEffect = SpriteEffects.None;
            
        }

        public override string ToString()
        {
            return "SpriteRenderer";
        }

        
    }
}
