using Microsoft.Xna.Framework;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// en underklasse til weapon og bruges til at angribe med
    /// Frederik
    /// </summary>
    internal class Sword : Weapon
    {
        private Rectangle hitBox;

        public Sword()
        {
            ItemType = ITEMTYPE.SWORD;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("otherSprites/sword1");
            sr.Scale = new Vector2(5f,5f);
            WeaponType = WEAPONTYPE.sword;
            damage = 20;
            
            base.Start();

            
        }

        /// <summary>
        /// Attack laver en 100x100 box på sværdets position og checker for alle andre colliders der intersecter den box
        /// Når sværdet slår gør den skade til alle enemies i nærheden som er ramt
        /// </summary>
        public override void Attack()
        {
            hitBox = new Rectangle((int)GameObject.Transform.Position.X, (int)GameObject.Transform.Position.Y, 100, 100);

            List<Collider> colliders = new List<Collider>();
            colliders = GameWorld.Instance.Colliders;

            foreach (Collider col in colliders)
            {
                if (col.CollisionBox.Intersects(hitBox)&&col.GameObject.GetComponent<Player>() is null)
                {
                    Enemy ent = col.GameObject.GetComponent<Enemy>() as Enemy;
                    ent.OnHit(damage);
                    (ent.GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer).setTempColor(Color.Blue,0.3f);
                }
            }
        }
    }
}
