using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// Particle nedarver fra particleSystem og vil være en firkant som flyver i en retning med en given fart
    /// Vi nået ikke at blive færdig med 
    /// </summary>
    internal class Particle : ParticleSystem
    {
        private float particleLifetime;
        private float timer;
        public Particle(Vector2 position, int particleAmount, Vector2 lifeTime) : base(position, particleAmount, lifeTime)
        {
            Random random = new Random();
            dir = new Vector2(random.Next(-1,1),random.Next(-1,1));
            particleLifetime = random.Next((int)lifeTime.X,(int)lifeTime.Y);
        }


        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += ((dir * speed) * deltaTime);
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer>particleLifetime)
            {
                shouldRemove = true;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("pixel");
        }
    }
}
