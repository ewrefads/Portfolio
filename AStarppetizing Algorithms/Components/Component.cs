using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms
{
    public class Component
    {
        bool isEnabled;
        GameObject gameObject;

        public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
        internal GameObject GameObject { get => gameObject; set => gameObject = value; }

        protected Texture2D[] sprites = new Texture2D[4];

        protected float animationSpeed = new float();

        private float animationTime = new float();

        public virtual void Awake()
        {

        }

        public virtual void Start()
        {

        }
        protected void Animate(GameTime gameTime)
        {
            animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds + animationSpeed;

            if (animationTime > sprites.Length - 1)
            {
                animationTime = 0;
            }
        }
        public virtual void Update(GameTime gameTime)
        {
            Animate(gameTime);
        }
               

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        //public void Animate (GameTime gameTime)
        //{
        //    animationTime += (float)gameTime.ElapsedGameTime.TotalSeconds * animationSpeed;
        //    if(animationTime > sprites.Length - 1)
        //    {
        //        animationTime = 0;
        //    }
        //}
    }
}

