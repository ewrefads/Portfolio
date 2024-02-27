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
    /// OBS. DENNE SUBKLASSE TIL ENEMY FUNGERER IKKE OG BLIVER DERFOR IKKE BRUGT UNDER RUNTIME!!!
    /// </summary>
    internal class Boss : Enemy
    {
        // Cooldown for attack
        private float atkCD;
        // Type of attack
        private int atk;
        private Boss1Weapon boss1Weapon;
        // Vector 2 for projectile velocity
        

        public Boss(Texture2D[] name, Vector2 pos, int tp, float df, float speed, Texture2D healthBar, Texture2D healthBarElement, Texture2D coolDownBar) : base(name, pos, tp, df, speed, healthBar, healthBarElement, coolDownBar)
        {
            scale = 10;

            boss1Weapon = new Boss1Weapon(new string[] { "club1" }, position, 4, 4, this);


            DefineStats(tp, df);

        }

        public override void Update(GameTime gameTime)
        {
            GetPlayerPos();
            Move(gameTime);
            SpecialAttack();
            Animate(gameTime);

            

        }

        protected override void DefineStats(int tp, float df)
        {
            
            // Definition of enemy stats by its type & difficulty modifier: health, strenght, speed, 
            switch (tp) // Type of monster are we dealing with
            {
                case 1: // Boss1
                    health = 100; strength = 1; speed = 30;
                    hasWpn = true; prefDistance = 300; PickUpWeapon(boss1Weapon);
                    break;
                case 2: // Boss2
                    health = 100; strength = 1; speed = 10;
                    hasWpn = true; prefDistance = 120;
                    break;
                case 3: // Boss3
                    health = 100; strength = 1; speed = 10;
                    hasWpn = true;
                    // . . .
                    break;
                default:
                    break;
            }
            GameWorld.InstantiateGameObject(CurrentWeapon);
            baseSpeed = speed;

            // Enemy stats multiplied by the difficulty modifier
            health += (int)((float)health * df);

            strength += (int)((float)strength * df);

        }



        private void SpecialAttack()
        {
            rnd = new Random();
            atk = rnd.Next(1, 3);

            if (atk == 1)
            {
                //projVel = playerPos - position;
                //projVel.Normalize();

                boss1Weapon.Shoot(this, playerPos);
            }
            if (atk == 2)
            {
                boss1Weapon.Shoot(this, playerPos);
            }
            if (atk == 3)
            {
                boss1Weapon.Shoot(this, playerPos);
            }
            


        }

    }
}
