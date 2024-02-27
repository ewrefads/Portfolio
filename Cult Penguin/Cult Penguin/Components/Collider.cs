using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cult_Penguin.Components
{
    public class Collider : Component
    {
        // Collisionbox texture
        private Texture2D texture;
        private Color color = Color.Red;
        private SpriteRenderer spriteRenderer;
        private bool activeCollider = true;

        public CollisionEvent CollisionEvent { get; set; } = new CollisionEvent();

        public override void Start()
        {
            SpriteRenderer = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();

            texture = GameWorld.Instance.Content.Load<Texture2D>("pixel2");


        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle
                    (
                        (int)(GameObject.Transform.Position.X - (int)(SpriteRenderer.Sprite.Width * SpriteRenderer.Scale.X) / 2),
                        (int)(GameObject.Transform.Position.Y - (int)(SpriteRenderer.Sprite.Height * SpriteRenderer.Scale.Y) / 2),
                        (int)(SpriteRenderer.Sprite.Width * SpriteRenderer.Scale.X),
                        (int)(SpriteRenderer.Sprite.Height * SpriteRenderer.Scale.Y)
                    );
            }

        }

        public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
        public Color Color { get => color; set => color = value; }
        public bool ActiveCollider { get => activeCollider; set => activeCollider = value; }

        public override void Update()
        {
            if (activeCollider)
            {
                CheckCollision();
            }
        }

        /// <summary>
        /// Metode for at få et visuelt overblik af collisionbox. (Debug)
        /// </summary>
        /// <param name="collisionBox"></param>
        /// <param name="spriteBatch"></param>
        /*public void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, rightLine, null, Color, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, leftLine, null, Color, 0, Vector2.Zero, SpriteEffects.None, 1);
        }
        */
        private void CheckCollision()
        {

            foreach (Collider other in GameWorld.Instance.Colliders)
            {
                if (other != this && other.CollisionBox.Intersects(CollisionBox))
                {
                    CollisionEvent.Notify(other.GameObject);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //DrawRectangle(CollisionBox, spriteBatch);
        }
    }
}