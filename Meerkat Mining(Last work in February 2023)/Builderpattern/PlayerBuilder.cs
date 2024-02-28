using Meerkat_Mining.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.Builderpattern
{

    public class PlayerBuilder : IBuilder
    {
        private GameObject gameObject;

        public void BuildGameObject()
        {
            gameObject = new GameObject();
            BuildComponents();
            
        }

        private void BuildComponents()
        {
            Player p = (Player)gameObject.AddComponent(new Player());
            SpriteRenderer sR = new SpriteRenderer();
            sR.LayerDepth = 0.9f;
            gameObject.AddComponent(sR);
            gameObject.AddComponent(new Collider());
            
            Animator animator = (Animator)gameObject.AddComponent(new Animator());

            animator.AddAnimation(BuildAnimation("idle", new string[] { "player/meerkat1", "player/meerkat2", "player/meerkat3" }));
        }

        private Animation BuildAnimation(string animationName, string[] spriteNames)
        {
            Texture2D[] sprites = new Texture2D[spriteNames.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(spriteNames[i]);
            }

            Animation animation = new Animation(animationName, sprites, 5);

            return animation;
        }


        public GameObject GetResult()
        {
            return gameObject;
        }
    }
}
