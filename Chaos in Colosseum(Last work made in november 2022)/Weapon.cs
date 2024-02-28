using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq.Expressions;
using System;
using System.Drawing;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// Weapon er den klasse som alle våbentyper i spillet nedarver af feks "sword" og "bow"
    /// </summary>
    public class Weapon : GameObject
    {
        //Hvor meget skade våbenet gøre on hit
        protected int damage;
        //Hvor hurtigt en Actor kan angribe med våbenet
        private float attackSpeed;
        //Hvor lang horizontal og vertikal rækkevide våbenet har
        protected float range;
        //Rectanglet som sætter våbenets hitbox ud fra range
        protected Rectangle hitBox;
        
        protected bool isAttacking;
        protected bool isRangedWeapon;

        //Hvor spriten af våbenet bliver tegnet ud fra hvor Actoren som holder våbenet står
        protected float xOffset;
        //Hvor lang tid det tager for våbenet at despawne efter det er blevet droppet af en Actor
        protected float timeToDespawn;
        //Tager tid på hvor lang tid våbenet har været droppet i
        protected float timer;

        //Hvem er ejeren af våbenet? Enemy eller Player
        protected GameObject owner;


        public GameObject Owner
        {
            set { owner = value; }
        }
        public float XOffset
        {
            get { return xOffset; }
            set { xOffset = value;  }
        }

        //GetHitbox returnere en rectangle som har højde og bredde af range
        public Rectangle GetHitbox
        {
            get { return hitBox = new Rectangle((int)position.X, (int)position.Y, (int)range, (int)range); }
        }

        public bool IsRangedWeapon
        {
            get { return isRangedWeapon; }
        }
        public float Range { get => range;}
        public float AttackSpeed { get => attackSpeed; }
        //isEquiped holder styr på om en Actor holder våbenet
        public bool IsEquiped { get; internal set; }

        /// <summary>
        /// Når våbenet bliver instansieret
        /// </summary>
        /// <param name="name">Strings som definere hvilke sprites der bruges</param>
        /// <param name="pos">Position af spriten</param>
        /// <param name="speed">Hastigheden af objekt</param>
        /// <param name="attackSpeed">Angrebshastigheden</param>
        public Weapon(string[] name, Vector2 pos, float speed, float attackSpeed) : base(name, pos, speed, 0)

        {
            this.attackSpeed = attackSpeed;
            
            timeToDespawn = 20;
        }


        /// <summary>
        /// Skader fjender som står i range af våbenets hitbox (kun melee våben)
        /// </summary>
        /// <param name="whatDealsDamage">Hvem gør skade Player eller Enemy</param>
        public virtual void DamageEnemiesInRange(GameObject whatDealsDamage)

        {
            isAttacking = true;
            
            //Opretter en liste over alle GameObjects i spillet
            List<GameObject> gameobjects = new List<GameObject>();
            gameobjects = GameWorld.GetGameObjects;

            //Hvis en spiller slår
            if (whatDealsDamage is Player) {
                foreach (GameObject obj in gameobjects)
                {
                    //Hvis objektet vi kigger på er en Enemy og Vores våben ikke er Unarmed (intet våben) og hitboxene overlapper
                    if (obj is Enemy && this is not Unarmed && GetHitbox.Intersects(obj.CollisionBox))
                    {
                        //gør skade til den Enemy som blev ramt
                        ((Enemy)obj).TakeDamage(damage);
                        

                    }
                }
            }
            //hvis en Enemy Slår
            else if (whatDealsDamage is Enemy)
            {

                foreach (GameObject obj in gameobjects)
                {
                    //Hvis objektet vi kigger på er en Player og Vores våben ikke er Unarmed (intet våben) og hitboxene overlapper
                    if (obj is Player && this is not Unarmed && GetHitbox.Intersects(obj.CollisionBox))
                    {
                        // gør skade til den Player der blev ramt
                        ((Player)obj).TakeDamage(damage);
                        
                    }

                 }
            }

            



        }

        //Hvis Actoren har et ranged våben og angriber
        public virtual void Shoot(GameObject whatDealsDamage)

        {
            isAttacking = true;
        }
        
        /// <summary>
        /// Flipper våbenet så det altid peger den vej hvor Actoren går
        /// </summary>
        /// <param name="x">Velocity på den Actor som holder våbenet</param>
        public void changeWeaponOffset(float x)
        {
            //right
            if (x>0)
            {
                xOffset = -1;
            }
            //left
            else if(x<0)
            {
                xOffset = 100;
            }
        }

        public override void LoadContent(ContentManager content)
        {
            for (int i = 0; i < spriteAssets.Length; i++)
            {
                sprites[i] = content.Load<Texture2D>(spriteAssets[i]);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //hvis vi angriber og har et meele våben så laver den en animation hvor våbenet svinger
            if (isAttacking&&!isRangedWeapon) {
                rotation += (float)gameTime.ElapsedGameTime.TotalSeconds * animationSpeed;
                rotation = Math.Clamp(rotation, 0, 1.3f);
                //når animationen er færdig så angriber vi ikke mere
                if (rotation>=1.3f)
                {
                    isAttacking = false;
                    rotation = 0;
                }
            }
            //hvis vi ikke holder noget våben så tæller timeren op
            if (!IsEquiped)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                //hvis der er gået nok tid så despawner våbenet
                if (timer>timeToDespawn)
                {
                    shouldRemove = true;
                }
            }
            //timeren bliver reseted hvis vi smaler våbenet op
            else { timer = 0; }
        }


    }
}