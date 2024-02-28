using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Components
{
    internal class Player : Component
    {
        private static Player instance;
        private CodeManager codeManager = CodeManager.Instance;
        /// <summary>
        /// Player får lavet en Instance af sig selv
        /// </summary>
        public static Player Instance
        {

            get
            {
                if (instance == null)
                {
                    instance = new Player();
                }
                return instance;
            }
        }

        private Texture2D[] sprites;


        public Vector2 GridPosition { get; set; }

        /// <summary>
        /// I Start bliver der loaded sprites for Playeren og sat størrelser med henvændelser til SpriteRenerer klassen
        /// </summary>
        public override void Start()
        {
            
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            
            sr.SetSprite("Dude1");
            sprites = new Texture2D[4];
            //sr.SetSprite("Dude1");
            sr.LayerDepth = 1;
            sr.Scale = 1.5f;
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>($"playersprt/Dude{i + 1}");
            }
            Animator animator = (Animator)GameObject.GetComponent<Animator>();
            animator.AddAnimation(BuildAnimation("Move", new string[] { "playersprt/dude1", "playersprt/dude2", "playersprt/dude3", "playersprt/dude4" }));
        }
        /// <summary>
        /// BuildAnimation bygger animationen for Player karakteren med de sprites loaded i Start
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="spriteNames"></param>
        /// <returns></returns>
        private Animation BuildAnimation(string animationName, string[] spriteNames)
        {
            Texture2D[] sprites = new Texture2D[spriteNames.Length];
            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i] = GameWorld.Instance.Content.Load<Texture2D>(spriteNames[i]);
            }

            Animation animation = new Animation(animationName, sprites, 4);


            return animation;
        }
        public override void Update(GameTime gameTime)
        {



            //    Animate(gameTime);
            //    SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            //    if (true)
            //    {
            //        sr.Sprite = sprites[4];
            //    }
            //    else
            //    {
            //        sr.SetSprite("Dude1");
            //    }



        }
        
    }   
}
