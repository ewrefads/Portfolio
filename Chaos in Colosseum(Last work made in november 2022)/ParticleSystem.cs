using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// ParticleSystem blev aldrig inkluderet i spillet pga mange på tid
    /// Ideen var at man kunne spawne et particlesystem feks når en enemy døde og Så spawnede det particlesystem en masse particles med en retning og fart for at give mere visuelt feedback
    /// Det viste sig ikke at være særligt nemt og så scrapped vi ideen.
    /// </summary>
    internal class ParticleSystem : GameObject
    {
        protected List<Particle> particles;
        protected Vector2 lifeTime;
        protected Vector2 dir;
        

        public ParticleSystem(Vector2 position,int particleAmount, Vector2 lifeTime) : base(new string[] {""},position,0,0)
        {
            particles = new List<Particle>();


            this.lifeTime = lifeTime;
            scale = 10;

            for (int i = 0; i < particleAmount; i++)
            {
                particles.Add(new Particle(position,particleAmount,lifeTime));
            }
        }

        

        

        public override void Update(GameTime gameTime)
        {
            foreach (Particle particle in particles)
            {
                particle.Update(gameTime);
            }
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch)
        {
            foreach (Particle particle in particles)
            {
                Vector2 origin = new Vector2(sprite.Width / 2, sprite.Height / 2);

                _spriteBatch.Draw(particle.sprite, particle.position, null, color, 0, origin, scale, spriteEffect, 0);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>("pixel");
        }
    }
}
