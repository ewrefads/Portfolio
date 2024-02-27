using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
        public enum PROJECTILETYPE { ARROW, OTHER}; // Other er en placeholder navn for et andet projektil

        internal class ProjectileFactory : Factory
        {
        

        private static ProjectileFactory instance;

            public static ProjectileFactory Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = new ProjectileFactory();
                    }
                    return instance;

                }
            }

            public override GameObject Create(Enum type)
            {
            float speed;
            float damage;

            

            switch (type)
            {
                case PROJECTILETYPE.ARROW:
                    speed = 300;
                    damage = 10;
                    break;
                case PROJECTILETYPE.OTHER:
                    speed = 200;
                    damage = 20;
                    break;
                default:
                    throw new NotImplementedException("Ukendt PROJECTILETYPE. Tjek om parametret er blevet skrevet korrekt");
            }


            GameObject go = new GameObject();

            SpriteRenderer sr = (SpriteRenderer)go.AddComponent(new SpriteRenderer());

                Projectile p = (Projectile)go.AddComponent(new Projectile(speed, damage));

                sr.SetSprite("otherSprites/arrow"); // Skal ændres til projektil sprite
                sr.Scale = Vector2.One*2;
                sr.LayerDepth = 0.7f;
                

                Collider col = (Collider)go.AddComponent(new Collider());
                col.CollisionEvent.Attach(p);

            GameWorld.Instance.Instantiate(go);

                return go;
             
            }
        }
}