using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    internal class Enemy : Actor
    {
        

        
        protected float prefDistance; // En Float værdi for den distance Enemy vil gerne have til Player.
        protected bool inRange; // Bool som fortæller om at Enemy har en prefDistance til Player.
        protected float baseSpeed; // Den normale hastighed for Enemy. (Bliver brugt til at sætte Enemys 'speed' til normal efter man har ændret objektets 'speed')
        protected bool hasWpn; // En bool som fortæller os om Enemy har et våben eller ej.
        public Vector2 playerPos; // En vector2 som skal have den samme værdi som Player position.
        protected Random rnd; // En Random som lige nu kun bliver brugt af Boss objekt. OBS. Boss er ikke færdigudviklet, så derfor vil denne field ikke blive brugt under runtime.


        // METHODS - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        /// <summary>
        /// A constructor for the Enemy class
        /// </summary>
        /// <param name="sprites">Texture2D array for Enemy sprites</param>
        /// <param name="pos">Spawn position af object </param>
        /// <param name="tp">Type af enemy </param>
        /// <param name="df">Difficulty modifier (sværhedsgrad for enemy)</param>
        /// <param name="speed">Object speed </param>
        /// <returns>An Enemy class object</returns>
        public Enemy(Texture2D[] sprites, Vector2 pos, int tp, float df, float speed, Texture2D healthBar, Texture2D healthBarElement, Texture2D coolDownBar) : base(sprites, pos, speed, 0, 4, healthBar, healthBarElement, coolDownBar)
        {
            // Bestemmer størrelsen på Enemy.
            scale = 5;

            DefineStats(tp, df);

        }


        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);


            velocity = VelocityTowardsPlayer(playerPos);
            if (colorDecay == 0)
            {
                color = Color.White;
            }
            else {
                colorDecay--;
            }
            Move(gameTime);
            Animate(gameTime);


            // En if statement som sørger for at Enemy kun kan angribe når 'inRange' true
            if (inRange)
            {
                if (hasWpn) // Der tjekkes om Enemy har våben
                {
                    Attack(gameTime);  // Enemy udfører et angreb.
                }
                else if (!hasWpn)
                {
                    // Der findes ikke noget unarmed combat så hvis enemy ikke har et våben sker der ingenting...
                }
            }
        }

        public override void OnCollision(GameObject other)
        {
            
        }

        /// <summary>
        /// En metode for at udregne en ny velocity vector2 for Enemy ud fra Enemy og Player position.
        /// </summary>
        /// <param name="pos">Player position</param>
        /// <returns>En vector2 værdi for objektets velocity</returns>
        private Vector2 VelocityTowardsPlayer(Vector2 pos)
        {

            GetPlayerPos();

            Vector2 newVelocity;
            newVelocity.X = 0;
            newVelocity.Y = 0; // Enemies Y værdi i velocity skal ikke blive påvirket af spillerns position

            // Vi starter med at undersøge om Enemy har opnået prefDistance (Præfererede distance)
            // Hvis den er i præfererede distance så sætter vi inRange bool til at være true og speed til 0;
            if (position.X <= pos.X + prefDistance && position.X >= pos.X + prefDistance -1) //-1 bliver brugt for at undgå Enemy 'jitter'
            {
                inRange = true;
                speed = 0;
            }
            else if (position.X >= pos.X - prefDistance && position.X <= pos.X - prefDistance + 1) //+1 bliver brugt for at undgå Enemy 'jitter'
            {
                inRange = true;
                speed = 0;
            }
            else
            {
                // Hvis Enemy ikke har den præfererede distance så bliver inRange lig med false og speed til baseSpped (normal hastighed)
                inRange = false;
                speed = baseSpeed;
            }

            
                if (position.X < pos.X && inRange == false) // Hvis Enemy X position er mindre end player X position og den er ikke 'inRange'
                {
                    // Sæt newVelicoty X til at gå mod player.
                    newVelocity.X = 1; 
                    

                    if (position.X > pos.X - prefDistance)  // Hvis Enemy er for tæt på Player og Enemy X position er større end player X position - prefDistance
                {
                        // Sæt newVelicoty X til at gå væk fra player.
                        newVelocity.X = -1; 

                }
                }
                else if (position.X > pos.X && inRange == false) // Hvis Enemy X position er størrer end player X position og den er ikke 'inRange'
                {       
                        // Sæt newVelicoty X til at gå mod player.
                        newVelocity.X = -1;


                if (position.X < pos.X + prefDistance) // Hvis Enemy er for tæt på Player og Enemy X position er mindre end player X position + prefDistance
                {

                        newVelocity.X = 1; // Sæt newVelicoty X til at gå væk fra player.

                }
                }

            // Hvis Enemy position X er størrer end player position X så sæt spriteeffekt til at flippe sprites.
            if (position.X > pos.X)
            {
                spriteEffect = SpriteEffects.FlipHorizontally;
            }
            else // ellers sæt spirteeffekt til none.
            {
                spriteEffect = SpriteEffects.None;
            }
            
            return newVelocity; 
        }




        /// <summary>
        /// En metode for at definere health, strength, speed, hasWpn, prefDistance og hvilket våben Enemy objektet har.
        /// Værdierne bliver bestemt ud fra tp (type) parameteren i switch casen.
        /// </summary>
        /// <param name="tp">Type</param>
        /// <param name="df">Difficulty modifier</param>
        protected virtual void DefineStats(int tp, float df)
        {
            // Definition af enemy stats ud fra dens type & difficulty modifier: health, strenght, speed, 
            switch (tp) // Type af monster vi har at gøre med
            {
                case 1: // Melee

                    health = 10; strength = 1; speed = 60;
                    hasWpn = true; prefDistance = 100; PickUpWeapon(new Sword(new string[] { "sword1" }, position));
                    break;
                case 2: // Ranger

                    health = 10; strength = 1; speed = 30;
                    hasWpn = true; prefDistance = 300; PickUpWeapon(new Bow(new string[] { "bow1static" }, position, this));

                    break;
                case 3: // Club boi :)
                    health = 10; strength = 1; speed = 30;
                    hasWpn = true; prefDistance = 120; PickUpWeapon(new Club(new string[] { "club1" }, position));

                    // . . .
                    break;
                default:
                    throw new Exception("Undefined Enemy type");
                    
            }
            
            GameWorld.InstantiateGameObject(CurrentWeapon);

            // Vi definerer baseSpeed for Enemy speed
            baseSpeed = speed;

            // Enemy stats multipliceret af df (difficulty modifier)
            health += (int)((float)health * df);
            strength += (int)((float)strength * df);

            startingHealth = health;
        }

        /// <summary>
        /// En metode for at opdatere Vector2 playerPos så den er lig med Player position.
        /// </summary>
        protected void GetPlayerPos()
        {
            foreach (GameObject obj in GameWorld.GetGameObjects)
            {
                if (obj is Player)
                {
                    playerPos = obj.Position;
                }
            }
        }

    }
}