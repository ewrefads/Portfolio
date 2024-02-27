using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// OBS. DETTE VÅBEN VAR LAVET TIL BOSS KLASSEN (SOM IKKE FUNGERER). DERFOR VIL KLASSEN IKKE BLIVE BRUGT UNDER RUNTIME.
    /// </summary>
    internal class Boss1Weapon : Weapon
    {
        Texture2D[] fireBall;
        private Vector2 fireVel;
        float fireSpeed = 5;

        public Boss1Weapon(string[] name, Vector2 pos, float speed, float attackSpeed, GameObject owner) : base(name, pos, speed, attackSpeed)
        {


        }

        public void Shoot(GameObject whatDealsDamage, Vector2 posP)
        {
            fireVel = posP - position;
            fireVel.Normalize();

            int dir = 0;
            if (xOffset < 0)
            {
                dir = 1;
            }
            else { dir = -1; }


            // Her udregense velocity for projektil
            

            // Vector


            Projectile projectile = new Projectile(new string[] { "fireball1", "fireball2", "fireball3", "fireball4", "fireball5", "fireball6" }, new Vector2(position.X + dir, position.Y), fireVel, speed, owner);

        }

    }
}
