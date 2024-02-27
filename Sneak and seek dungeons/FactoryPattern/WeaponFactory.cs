using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    internal enum WEAPONTYPE {sword, bow, wand };

    /// <summary>
    /// Laver et våben og instansiere det i verden
    /// Frederik
    /// </summary>
    internal class WeaponFactory : Factory
    {

        private static WeaponFactory instance;

        public static WeaponFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WeaponFactory();
                }
                return instance;

            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject newWeapon = new GameObject();
            
            

            if (type is WEAPONTYPE.sword)
            {
                newWeapon.AddComponent(new Sword());
            }

            newWeapon.AddComponent(new SpriteRenderer());
            GameWorld.Instance.Instantiate(newWeapon);
            newWeapon.Transform.Position = GameWorld.Instance.FindObjectOfType<Player>().GameObject.Transform.Position;

            return newWeapon;
        }
    }
}
