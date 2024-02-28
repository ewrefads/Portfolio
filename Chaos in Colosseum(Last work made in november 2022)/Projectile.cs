using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{   
    /// <summary>
    /// Projectil er et objekt som bevæger sig i en givet retning indtil den rammer noget eller ryger ud fra skærmen
    /// nedarver fra Weapon som gør at vi kan tilgå værdier som damage og owner
    /// </summary>
    internal class Projectile : Weapon
    {

        int direction;
        public Projectile(string[] name, Vector2 pos, Vector2 dir, float speed, GameObject owner) : base(name, pos, speed,2)
        {
            //sætter variabler for projectilet
            scale = 7f;
            damage = 1;

            this.owner = owner;
            this.speed = speed;
            velocity = dir;
            direction = (int)dir.X;

            animationSpeed = 2;

            //sørger for at vi ikke kan samle pilen op
            IsEquiped = true;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            //flipper spriten ud fra den ritning den flyver i
            Vector2 origin = new Vector2(0, sprites[0].Height);

            SpriteEffects s = SpriteEffects.None;

            if (direction<0)
            {
                s = SpriteEffects.FlipHorizontally;
            }

            _spriteBatch.Draw(sprites[0], position, null, Color.White, rotation, origin, scale, s, 0);

        }

        public override void Update(GameTime gameTime)
        {
            move(gameTime);
            Animate(gameTime);
            OutOfBounds();
        }

        public void move(GameTime gameTime)
        {
            //bevæg projektilet i en retning
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += ((velocity * speed) * deltaTime);
        }

        //hvis projektilet er ude af skærmen
        private void OutOfBounds() {
            //hvis projektilet er uden for spillebanen så slet den
            if (position.X < GameWorld.worldBounds.X || position.X > GameWorld.worldBounds.Y) {
                base.shouldRemove = true;
            }
        }

        public override void OnCollision(GameObject other)
        {
            //hvis objektet vi støder ind i ikke er et våben
            if (other is not Weapon) {
                //hvis ejeren af projektilet er en Player og det andet objekt ikke er en spiller
                if (owner is Player &&  other is not Player)
                {
                    //gør skade til Enemy som blev ramt
                    ((Enemy)other).TakeDamage(damage);
                    //fjern Projektilet efter collision
                    ShouldRemove = true;
                    //hvis ejeren af projektilet er en Enemy og det andet objekt ikke er en Enemy
                } else if (owner is Enemy && other is not Enemy)
                {
                    //gør skade til Player som blev ramt
                    ((Player)other).TakeDamage(damage);
                    //fjern Projektilet efter collision
                    ShouldRemove = true;
                }
            }
        }
    }
}
