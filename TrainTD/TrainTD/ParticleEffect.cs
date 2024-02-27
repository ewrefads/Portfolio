using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTD
{
    internal class ParticleEffect : GameObject
    {
        Vector2 velocity;
        public ParticleEffect(Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, float rotation, float animationSpeed, Vector2 velocity) : base(position, sprites, spriteEffect, lootValue, rotation, animationSpeed)
        {
            this.velocity = velocity;
            layer = 0.7f;
        }

        public override GameObject getCopy()
        {
            throw new NotImplementedException();
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public override int lootFromObject()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            scale += 0.01f;
            opacity -= 0.01f;
            if (opacity < 0) {
                ShouldRemove = true;
            }
            position += velocity;
        }

        protected override bool PlacementExceptions(GameObject go)
        {
            throw new NotImplementedException();
        }
    }
}
