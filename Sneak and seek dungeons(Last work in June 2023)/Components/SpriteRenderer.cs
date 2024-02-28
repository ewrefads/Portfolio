using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Sneak_and_seek_dungeons
{
    public class SpriteRenderer : Component
    {
        private Texture2D sprite;
        private Vector2 origin;
        private Color color;
        private Color defaultColor = Color.White;
        private float remainingTime;
        private Vector2 scale;
        private float startScale;
        private float layerDepth = 0.1f;
        private SpriteEffects spriteEffect;

        public Texture2D Sprite { get => sprite; set => sprite = value; }
        public Vector2 Origin { get => origin; set => origin = value; }
        public Vector2 Scale
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
        public Color Color { get => color; set { 
                color = value;
                defaultColor = value;
            } }
        public float StartScale { get => startScale; set => startScale = value; }
        public SpriteEffects SpriteEffect { get => spriteEffect; set => spriteEffect = value; }

        public void setTempColor(Color color, float time)
        {
            this.color = color;
            remainingTime = time;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (color != defaultColor && GameWorld.DeltaTime >= remainingTime)
            {
                remainingTime = 0;
                color = defaultColor;
            }
            else if (color != defaultColor)
            {
                remainingTime -= GameWorld.DeltaTime;
            }
            if (isEnabled) {
                Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
                //spriteBatch.Draw(sprite, new Rectangle((int)Origin.X, (int)Origin.Y, sprite.Width, sprite.Height), Color.White);
                spriteBatch.Draw(sprite, GameObject.Transform.Position, null, Color, 0f, Origin, scale, SpriteEffect, LayerDepth);
            }
            


        }

        public void SetSprite(string spriteName)
        {
            sprite = GameWorld.Instance.Content.Load<Texture2D>(spriteName);
        }

        public override void Start()
        {
            if (isEnabled) {
                Origin = new Vector2(Sprite.Width / 2, Sprite.Height / 2);
                if (scale.X == 0 && scale.Y == 0)
                {
                    scale = Vector2.One;
                }
                //startScale = scale;

                if (Color == new Color())
                {
                    Color = Color.White;
                }

                SpriteEffect = SpriteEffects.None;
            }
        }

        public override string ToString()
        {
            return "SpriteRenderer";
        }


    }
}
