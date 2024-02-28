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
    public class Projectile : GameObject
    {
        private int damage;
        private Vector2 direction;
        private float speed;

        private float timeToDespawn;
        private float timer;


        /// <summary>
        /// Projectile er et objekt som flyver i en hvis retning indtil den rammer et andet objekt og gør skade til det ramte objekt
        /// </summary>
        /// <param name="damage">Hvor meget skade projektilet gør on collision</param>
        /// <param name="direction">en vector2 som peger mod et target position</param>
        /// <param name="speed">hastigheden som projektilet flyver med</param>
        /// <param name="t">sprites</param>
        /// <param name="v">start position</param>
        public Projectile(int damage, Vector2 direction, float speed, Texture2D[] t, Vector2 v) : base(v,t,SpriteEffects.None,0,0,0)

        {
            this.damage = damage;
            this.direction = direction;
            this.speed = speed;

            //efter 10 sekunder så despawner projektilet for at skabe lag
            timeToDespawn = 10;

            scale = 2;
            color = Color.Black;
            //Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue
        }

        /// <summary>
        /// Loader sprites
        /// </summary>
        /// <param name="content"></param>
        public override void LoadContent(ContentManager content)
        {
            Texture2D[] s = new Texture2D[assets.Length];

            for (int i = 0; i < assets.Length; i++)
            {
                s[i] = content.Load<Texture2D>(assets[i]);
            }

            sprites = s;
        }

        public override int lootFromObject()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ændre projektilets position i en retning med en givet fart og hvis der gået for lang tid så depspawner det
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //ændre positionen
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += ((direction * speed) * deltaTime);
            foreach (GameObject go in GameWorld.GetGameObjects){
                if (go.CollisionBox.Intersects(CollisionBox) && go is TrainCarriage) {
                    OnCollision(go);
                    shouldRemove = true;
                    break;
                }
            }
            //styrer despawning
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer>=timeToDespawn)
            {
                shouldRemove = true;
            }
        }
        /// <summary>
        /// bliver kaldt når der er en kollision med dette objekt
        /// </summary>
        /// <param name="other"></param>
        public override void OnCollision(GameObject other)
        {
            //hvis vi rammer et tog så gør skade til toget
            if (other is TrainCarriage)
            {
                ((TrainCarriage)other).TakeDamage(damage);
            }
        }

        public override GameObject getCopy()
        {
            throw new NotImplementedException();
        }

        protected override bool PlacementExceptions(GameObject go)
        {
            throw new NotImplementedException();
        }
    }
}
