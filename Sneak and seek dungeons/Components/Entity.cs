using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    public class Entity : Component
    {
        protected float health;
        protected float maxHealth;
        private float armor;
        protected float speed;
        protected float damage;

        protected int detectionRange;
        protected Vector2 velocity;

        protected Vector2 LastVelocity;

        internal float Armor { get => armor; set => armor = value; }

        protected void ChangeVelocity()
        {

        }

        protected void Attack(GameObject target)
        {

        }

        /// <summary>
        /// når entity tager skade
        /// </summary>
        /// <param name="damage"></param>
        public virtual void OnHit(float damage)
        {

            health -= damage-((damage/100) * armor);

            //entity død hvis den ikke har mere liv
            if (health<=0)
            {
                GameWorld.Instance.DestroyedGameObjects.Add(GameObject);
                //GameWorld.Instance.Colliders.Remove(GameObject.GetComponent<Collider>() as coll);
            }
        }



        public override void Update()
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            
        }
    }
}
