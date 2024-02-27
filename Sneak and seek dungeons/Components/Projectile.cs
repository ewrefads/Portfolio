using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sneak_and_seek_dungeons.FactoryPattern;
using Sneak_and_seek_dungeons.ObserverPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Projectile : Component, IGameListner
    {

        private Texture2D texture;
        private Vector2 velocity;
        private float speed;
        private float damage;

        private Collider wallCollider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Spawnposition</param>
        /// <param name="velocity">Retning</param>
        /// <param name="speed">Hastighed</param>
        /// <param name="damage">Skade</param>

        public Vector2 Velocity { get => velocity; set => velocity = value; }

        public Projectile(float speed, float damage)
        {

            this.speed = speed;
            this.damage = damage;

        }

        public override void Start()
        {
            Collider col = GameObject.GetComponent<Collider>() as Collider;
            wallCollider = col;
            GameWorld.Instance.Colliders.Add(col);

        }

        public override void Update()
        {
            GameWorld.Instance.UpdateDungeonColliders(gameObject.Transform.Position);
            GameObject.Transform.Position += speed * velocity * GameWorld.DeltaTime;
            wallCollider.CheckDungeonCollision();

            // Check collision method
        }

        public void Notify(GameEvent gameEvent)
        {
            if (gameEvent is CollisionEvent)
            {
                CollisionEvent ce = (gameEvent as CollisionEvent);
                if (ce.Other.Tag == "Player")
                {
                    (ce.Other.GetComponent<Player>() as Player).OnHit(damage);

                    GameWorld.Instance.DestroyedGameObjects.Add(GameObject);

                }
                else if (ce.Other.Tag == "dungeon")
                {
                    GameWorld.Instance.DestroyedGameObjects.Add(GameObject);
                }
                
            }
        }

        // Collsion method

    }
}
