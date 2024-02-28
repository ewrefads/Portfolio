using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Color = Microsoft.Xna.Framework.Color;

namespace Chaos_in_Colosseum
{
    public abstract class GameObject
    {
        protected Vector2 position;
        protected float rotation;
        protected float scale;
        protected Texture2D sprite;
        protected Texture2D[] sprites;
        protected string[] spriteAssets;
        protected float speed;
        public Vector2 velocity;
        protected bool shouldRemove;
        protected float animationSpeed = 0;
        private float animationTime = 0;
        protected string spriteName;
        protected Rectangle rectangle;
        protected Color color = Color.White;
        protected SpriteEffects spriteEffect;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
        public Vector2 spriteSize
        {
            get
            {

                return new Vector2(sprites[(int)animationTime].Width * scale, sprites[(int)animationTime].Height * scale);

            }
        }


        public Rectangle CollisionBox
        {
            get
            {
                return new Rectangle(
                    (int)(position.X - spriteSize.X / 2),
                    (int)(position.Y - spriteSize.Y / 2),
                    (int)spriteSize.X,
                    (int)spriteSize.Y

                    );
            }
        }


        public GameObject(string[] names, Vector2 pos, float speed, float animationSpeed)
        {
            position = pos;
            this.animationSpeed = animationSpeed;
            spriteAssets = names;
            sprites = new Texture2D[spriteAssets.Length];
            this.speed = speed;

        }


        public GameObject(Texture2D[] sprites, Vector2 pos, float speed, float animationSpeed)
        {
            position = pos;
            this.animationSpeed = animationSpeed;
            spriteAssets = new string[] { };
            this.sprites = sprites;
            this.speed = speed;

        }
        public bool ShouldRemove { get => shouldRemove; set { shouldRemove = value; } }


        public virtual void Draw(SpriteBatch _spriteBatch)
        {
            Vector2 origin = new Vector2(sprites[(int)animationTime].Width / 2, sprites[(int)animationTime].Height / 2);

            _spriteBatch.Draw(sprites[(int)animationTime], position, null, color, 0, origin, scale, spriteEffect, 0);

        }

        public abstract void Update(GameTime gameTime);

        public abstract void LoadContent(ContentManager content);

        public virtual void OnCollision(GameObject other)
        {

        }

        protected void Animate(GameTime gameTime)
        {
            animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds * animationSpeed;
            if (animationTime > sprites.Length - 1)
            {
                animationTime = 0;
            }
        }



        protected Vector2 GetSpriteSize()
        {
            return new Vector2();
        }



        public bool IsColliding(GameObject other)
        {
            if (this == other)
            {
                return false;
            }
            return CollisionBox.Intersects(other.CollisionBox);
        }
    }
}
