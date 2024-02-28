using Meerkat_Mining.Observer_pattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class Collider : Component
    {
        // Collisionbox texture
        private Texture2D texture;

        private SpriteRenderer spriteRenderer;
        public CollisionEvent CollisionEvent { get; set; } = new CollisionEvent();

        public override void Start()
        {
            SpriteRenderer = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
            texture = GameWorld.Instance.Content.Load<Texture2D>("amogus");
        }

        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle
                    (
                        (int)(GameObject.Transform.Position.X - (int)(SpriteRenderer.Sprite.Width * SpriteRenderer.Scale) / 2),
                        (int)(GameObject.Transform.Position.Y - (int)(SpriteRenderer.Sprite.Height * SpriteRenderer.Scale) / 2),
                        (int)(SpriteRenderer.Sprite.Width * SpriteRenderer.Scale),
                        (int)(SpriteRenderer.Sprite.Height * SpriteRenderer.Scale)
                    );
            }
        }

        public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

        public override void Update()
        {

        }

        /// <summary>
        /// Metode for at få et visuelt overblik af collisionbox. (Debug)
        /// </summary>
        /// <param name="collisionBox"></param>
        /// <param name="spriteBatch"></param>
        private void DrawRectangle(Rectangle collisionBox, SpriteBatch spriteBatch)
        {
            Rectangle topLine = new Rectangle(collisionBox.X, collisionBox.Y, collisionBox.Width, 1);
            Rectangle bottomLine = new Rectangle(collisionBox.X, collisionBox.Y + collisionBox.Height, collisionBox.Width, 1);
            Rectangle rightLine = new Rectangle(collisionBox.X + collisionBox.Width, collisionBox.Y, 1, collisionBox.Height);
            Rectangle leftLine = new Rectangle(collisionBox.X, collisionBox.Y, 1, collisionBox.Height);

            spriteBatch.Draw(texture, topLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, bottomLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, rightLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
            spriteBatch.Draw(texture, leftLine, null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 1);
        }

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
