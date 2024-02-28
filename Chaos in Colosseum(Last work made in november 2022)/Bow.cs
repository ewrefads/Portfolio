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
    /// Buen er en subklasse af våben som er ranged og spawner klassen Projectile hver gang Actoren angriber
    /// </summary>
    internal class Bow : Weapon
    {

        public Bow(string[] name, Vector2 pos, GameObject owner) : base(name, pos, 0, 0.5f)
        {
            //sætter variabler for Bow
            scale = 7f;
            damage = 1;
            speed = 750;
            animationSpeed = 2;
            isRangedWeapon = true;

            
            //ejeren af våbnet bliver deklareret så man ikke kommer til at skyde sig selv
            this.owner = owner;
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            //flipper spriten hvis Spilleren vender den anden retning
            SpriteEffects s;
            
            if (xOffset < 0)
            {
                s = SpriteEffects.None;
                rotation = 0f;
            }
            else { s = SpriteEffects.FlipHorizontally; rotation = -0f; }

            Vector2 origin = new Vector2(0, sprites[0].Height);
            
            _spriteBatch.Draw(sprites[0], position, null, Microsoft.Xna.Framework.Color.White, rotation, origin, scale, s, 0);
         
           
            
            
        }

        public override void Update(GameTime gameTime)
        {

            //hvis vi angriber så få den nuværende retning og spawn et projectil som kære i den retning
            if (isAttacking)
            {
                int dir = 0;
                if (xOffset<0)
                {
                    dir = 1;
                }
                else { dir = -1; }

                Projectile projectile = new Projectile(new string[] { "arrow" }, new Vector2(position.X + dir,position.Y),new Vector2(dir,0),speed, owner);
                //spawn projectilet i vores verden
                GameWorld.InstantiateGameObject(projectile);
                isAttacking = false;
            }
            //despawner våbenet hvis det ligger på jorden i for lang tid
            if (!IsEquiped)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timer > timeToDespawn)
                {
                    shouldRemove = true;
                }
            }
            else { timer = 0; }
        }

    }
}
