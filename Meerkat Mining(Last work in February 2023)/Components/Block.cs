using Meerkat_Mining.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class Block : Component
    {
        private float hp;
        private bool canBeMined;
        private bool isDead;
        private BLOCKTYPE type;

        private float startingScale;
        private float currentScale;
        private bool beingHit;

        private SpriteRenderer sr;

        public bool CanBeMined { get => canBeMined; set => canBeMined = value; }
        public bool IsDead { get => isDead; set => isDead = value; }

        public BLOCKTYPE Type { get => type; set => type = value; }
        public float StartingScale { get => startingScale; set => startingScale = value; }
        public float Hp { get => hp; set => hp = value; }

        public override void Start()
        {
            //Hp = 50;
            IsDead = false;
            sr = (GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer);
            beingHit = false;
            currentScale = startingScale;

        }
        public void Mined(float damage)
        {
            if (canBeMined == true) {

                Hp -= damage;

                GameAudio.Instance.Play(SFX.BLOCKHIT);


                HitAnimation();

                //ødelægger blokken hvis den ikke har liv tilbage
                if (Hp <= 0)
                {
                    IsDead = true;

                    GameWorld g = GameWorld.Instance;
                    for (int i = 0; i < g.blocks.GetLength(1); i++)
                    {
                        for (int j = 0; j < g.blocks.GetLength(0); j++)
                        {
                            if (g.blocks[j, i] == GameObject)
                            {
                                g.blocks[j, i] = null;
                            }
                        }
                    }

                    GameAudio.Instance.Play(SFX.BLOCKBREAK);
                    
                    GameWorld.Instance.Destroy(GameObject);


                }
                
            }
            

        }

        public void HitAnimation()
        {
            currentScale = StartingScale * 1.5f;
            beingHit = true;
        }

        public override void Update()
        {
            if (beingHit)
            {
                currentScale -= 10*GameWorld.DeltaTime;

                sr.Scale = currentScale;

                if (currentScale<startingScale)
                {
                    currentScale = startingScale;
                    sr.Scale = currentScale;
                    beingHit = false;
                }

            }
        }
    }
}
