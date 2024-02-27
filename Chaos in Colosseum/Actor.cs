using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// Class that contains general code for all gameobjects which can take actions on their own
    /// </summary>
    public abstract class Actor : GameObject
    {
        /// <summary>
        /// The current health of the Actor. If this gets to 0 the actor dies
        /// </summary>
        protected int health;

        /// <summary>
        /// Not implented
        /// </summary>
        protected int strength;

        /// <summary>
        /// The health the actor when it was created. Only used for the healthbar
        /// </summary>
        protected int startingHealth;

        /// <summary>
        /// How far away the actor can be from a weapon in order to pick it up
        /// </summary>
        protected float pickupRange;

        /// <summary>
        /// The current weapon of the actor
        /// </summary>
        protected Weapon currentWeapon;

        /// <summary>
        /// How much time had passed since the game started
        /// </summary>
        private float lastAttackTime = 0;

        /// <summary>
        /// How many ticks the actor will be red 
        /// </summary>
        protected int colorDecay = 0;

        /// <summary>
        /// The texture for the black part of the healthbar displaying max health
        /// </summary>
        private Texture2D healthBar;

        /// <summary>
        /// The texture for the red part the healthbar displaying remaining health
        /// </summary>
        private Texture2D healthBarElement;

        /// <summary>
        /// the texture of the cooldownbar displaying remaining time until the actor can attack again
        /// </summary>
        private Texture2D coolDownBar;

        /// <summary>
        /// The next time the actor can attack. Used for the cooldown bar
        /// </summary>
        protected float nextAttackTime = 0;

        /// <summary>
        /// Time as of last call to Update. Used for the cooldown bar
        /// </summary>
        protected float currentTime = 0;

        /// <summary>
        /// Remaining time until next attack. Used for Cooldown bar
        /// </summary>
        protected float timeRemaining = 0;





        /// <summary>
        /// Constructor used for actors created before LoadContent in gameworld has been run
        /// </summary>
        /// <param name="names">names of the sprites used by the actor</param>
        /// <param name="pos">the starting position of the actor</param>
        /// <param name="speed">the movement speed of the actor</param>
        /// <param name="animationSpeed">the animation speed of the actor</param>
        /// <param name="health">starting health of the actor</param>
        protected Actor(string[] names, Vector2 pos, float speed, float animationSpeed, int health) : base(names, pos, speed, animationSpeed)
        {

            this.health = health;
            startingHealth = health;
            //PickUpWeapon(new Bow(new string[] { "bow" }, pos, this));

            //PickUpWeapon(new Bow(new string[] { "bow" }, pos));

            //GameWorld.InstantiateGameObject(CurrentWeapon);

        }

        /// <summary>
        /// Constructor for the actor used after loadcontent has been run in gameworld
        /// </summary>
        /// <param name="names">The sprites used by the actor</param>
        /// <param name="pos">starting position</param>
        /// <param name="speed">the movement speed</param>
        /// <param name="animationSpeed">how fast animation is run</param>
        /// <param name="health">starting health of the actor</param>
        /// <param name="healthBar">sprite used to display max health</param>
        /// <param name="healthBarElement">sprite used to display current health</param>
        /// <param name="coolDownBar">sprite used to display cooldown</param>
        protected Actor(Texture2D[] names, Vector2 pos, float speed, float animationSpeed, int health, Texture2D healthBar, Texture2D healthBarElement, Texture2D coolDownBar) : base(names, pos, speed, animationSpeed)

        {

            this.health = health;
            this.healthBar = healthBar;
            this.healthBarElement = healthBarElement;
            this.coolDownBar = coolDownBar;
            startingHealth = health;

            //PickUpWeapon(new Bow(new string[] { "bow" }, pos, this));

            //GameWorld.InstantiateGameObject(CurrentWeapon);
        }






        public Vector2 Velocity { get => velocity; set => velocity = value; }
        protected float Speed { get => speed; set => speed = value; }
        public int Health { get => health; }
        public float LastAttackTime { get => lastAttackTime; }
        public Weapon CurrentWeapon { get => currentWeapon; }


        /// <summary>
        /// Method to determine if an attack can be made and which type of weapon is used
        /// </summary>
        /// <param name="gameTime">current gametime</param>
        public void Attack(GameTime gameTime) {

            //hvis der gået lang nok tid siden sidste angreb
            if (LastAttackTime + CurrentWeapon.AttackSpeed <= (float)gameTime.TotalGameTime.TotalSeconds) {

                //hvis Actoren har et close range våben

                nextAttackTime = (float)gameTime.TotalGameTime.TotalSeconds + CurrentWeapon.AttackSpeed;

                if (!currentWeapon.IsRangedWeapon) {
                    //bruger våbenets funktion til at gøre close range damage
                    CurrentWeapon.DamageEnemiesInRange(this);
                } 
                //Hvis Actoren har et long range våben
                else if (currentWeapon.IsRangedWeapon)
                {
                    //bruger våbenets funktion til at skyde
                    currentWeapon.Shoot(this);
                }

                //Sidste gang vi angreb er nu
                lastAttackTime = (float)gameTime.TotalGameTime.TotalSeconds;

            }

        }

        /// <summary>
        /// Moves the actor
        /// </summary>
        /// <param name="gameTime">current gametime</param>
        public void Move(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += ((Velocity * Speed) * deltaTime);

            currentWeapon.changeWeaponOffset(velocity.X);
            currentWeapon.Position = position - new Vector2(currentWeapon.XOffset,-40);

        }

        /// <summary>
        /// Picks up a weapon from the ground
        /// </summary>
        /// <param name="weapon">the weapon to be picked up</param>
        public void PickUpWeapon(Weapon weapon)
        {
            //Actorens nuværende våben bliver til det nye våben
            currentWeapon = weapon;

            //det nuværende våben er i brug
            currentWeapon.IsEquiped = true;

            //Hviser hvem den nuværende ejer af våbenet er (Enemy eller Player)
            currentWeapon.Owner = this;
        }

        /// <summary>
        /// drops the current weapon
        /// </summary>
        public void DropWeapon()
        {
            //hvis det nuværende våben er ingenting så bare slet det fra spillet
            if (currentWeapon is Unarmed)
            {
                currentWeapon.ShouldRemove = true;
            }
            //det nuværende våben er ikke i brug mere og kan godt samles op
            currentWeapon.IsEquiped = false;
            //PickUpWeapon(new Unarmed(new string[] { "afafwa" },Vector2.Zero);
        }

        /// <summary>
        /// Calculates remaining health sets the draw color to red and checks if the actor is at or below 0 health
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            //sætter Actorens farve til rød for at indikere at de har taget skade
            color = Color.Red;
            //hvor hurtigt farven decayer tilbage til normal
            colorDecay = 10;

            //Hvis Actoren er død
            if (health <= 0)
            {
                //Actoren dropper altid det våben de holder når de dør
                DropWeapon();
                //fjerne denne Actor fra spillet
                shouldRemove = true;
            }

            
        }

        /// <summary>
        /// Draws the healthbar, cooldownbar and calls the draw method in gameobject
        /// </summary>
        /// <param name="_spriteBatch">the Spritebatch used to draw</param>
        public override void Draw(SpriteBatch _spriteBatch)
        {
            Vector2 origin = new Vector2(healthBar.Width / 2, healthBar.Height / 2);
            int elementLength = healthBar.Width / startingHealth;
            _spriteBatch.Draw(healthBar, new Rectangle((int)position.X - 80, (int)position.Y - 100, healthBar.Width, healthBar.Height), null, Color.White, 0, Vector2.Zero, spriteEffect, 0);
            base.Draw(_spriteBatch);
            
            for (int i = 0; i < health; i++)
            {
                _spriteBatch.Draw(healthBarElement, new Rectangle((int)position.X - 80 + i * elementLength, (int)position.Y - 100, elementLength, healthBarElement.Height), null, Color.White, 0, Vector2.Zero, spriteEffect, 1);

            }
            if (currentTime < nextAttackTime) {
                float lengthFactor = timeRemaining / currentWeapon.AttackSpeed * 100;
                int length = (coolDownBar.Width / 100) * (int)lengthFactor;
                _spriteBatch.Draw(coolDownBar, new Rectangle((int)position.X - 80, (int)position.Y - 130, (int)timeRemaining, coolDownBar.Height), null, Color.White, 0, Vector2.Zero, spriteEffect, 1);
            }
            
            //_spriteBatch.Draw(healthBar, new Vector2(position.X - 80, position.Y - 80), Color.White);
            
            

            //healthBar.Draw(_spriteBatch);
        }

        /// <summary>
        /// Loads the the sprites for the healthbar, cooldownbar and the actor itself
        /// </summary>
        /// <param name="content"></param>
        public override void LoadContent(ContentManager content)
        {
            healthBar = content.Load<Texture2D>("HealthBar");
            healthBarElement = content.Load<Texture2D>("HealthBarElement");
            coolDownBar = content.Load<Texture2D>("coolDownBar");
            for (int i = 0; i < spriteAssets.Length; i++)
            {
                sprites[i] = content.Load<Texture2D>(spriteAssets[i]);
            }
        }

        /// <summary>
        /// updates the time related fields used by the cooldown bar
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            currentTime = gameTime.TotalGameTime.Seconds;
            timeRemaining = nextAttackTime * 1000 - (float)gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}
