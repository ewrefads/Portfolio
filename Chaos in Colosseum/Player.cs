using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    /// <summary>
    /// The class of the player
    /// </summary>
    public class Player : Actor
    {
        /// <summary>
        /// Saves the last keystate. Used for weapon pickup and drop
        /// </summary>
        KeyboardState oldState = Keyboard.GetState();

        /// <summary>
        /// How many calls to update befor the player will begin falling
        /// </summary>
        private float jumpTicks = 0;

        /// <summary>
        /// How many ticks before the player is back on the ground
        /// </summary>
        private float fallTicks = 0;

        /// <summary>
        /// The relationship between the number of jumpTicks and fallTicks at the start of the jump
        /// </summary>
        private float ratio = 0;
        //private UIElement healthBar;


        /// <summary>
        /// Constructor for player. This can only be run before loadcontent has been run in gameworld
        /// </summary>
        /// <param name="names">the names of the sprites used for the player</param>
        /// <param name="pos">starting position</param>
        /// <param name="speed">movement speed of the player</param>
        public Player(string[] names, Vector2 pos, float speed) : base(names, pos, speed, 4, 4)
        {

            PickUpWeapon(new Bow(new string[] { "bow1static" }, pos, this));

            GameWorld.InstantiateGameObject(CurrentWeapon);
            position = pos;
            //healthBar = new UIElement(new string[] {"HealthBar" }, new Vector2(pos.X, pos.Y - 60), speed, this,health);
            scale = 10;
            pickupRange = 60;
        }

        /// <summary>
        /// calls update in actor, handles keybord input, handles jump, moves the player, animates the player 
        /// and checks if enough time has passed since the player last took damage to reset their color
        /// </summary>
        /// <param name="gameTime">current gametime</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleInput(gameTime);
            Jump();
            Move(gameTime);
            Animate(gameTime);
            if (colorDecay == 0)
            {
                color = Color.White;
            }
            else
            {
                colorDecay--;
            }
            //healthBar.Update(gameTime, position);
        }


        /// <summary>
        /// Handles jumps taken by the player by first moving them up for a certain number of calls to the method 
        /// and then moving them down for a number of calls.
        /// </summary>
        private void Jump()
        {
            float jumpSpeed = 1;
            float fallSpeed = jumpSpeed * ratio;
            if (jumpTicks > 0)
            {
                velocity += new Vector2(0, -jumpSpeed);
                jumpTicks--;
            }
            else if (fallTicks > 0) {
                velocity += new Vector2(0, fallSpeed);
                fallTicks--;
            }
        }

        /// <summary>
        /// Handles the players input
        /// </summary>
        /// <param name="gameTime"></param>
        private void HandleInput(GameTime gameTime) {
            velocity = Vector2.Zero;
            KeyboardState keyState = Keyboard.GetState();
            //left
            if (keyState.IsKeyDown(Keys.A))
            {
                velocity += new Vector2(-1, 0);
            }
            //right
            if (keyState.IsKeyDown(Keys.D))
            {
                velocity += new Vector2(1, 0);
            }
            //attack
            if (keyState.IsKeyDown(Keys.Space)) {
                Attack(gameTime);
            }
            //jump
            if (keyState.IsKeyDown(Keys.W) && fallTicks == 0) {
                jumpTicks = 20;
                fallTicks = 40;
                ratio = jumpTicks / fallTicks;
            }
            //pickup weapon
            if (keyState.IsKeyDown(Keys.F)&&!oldState.IsKeyDown(Keys.F)) {
                PickUpWeapon(CheckWeaponsNearby());
            }
            
            

            if (velocity != Vector2.Zero)
            {
                velocity.Normalize();
            }

            oldState = keyState;
        }

        //checker hvilke våben som er i nærheden og samler det op som er tættest på spilleren
        private Weapon CheckWeaponsNearby()
        {
            //liste over alle GameObjects
            List<GameObject> objects = new List<GameObject>();
            objects = GameWorld.GetGameObjects;
            
            //våbenet er deadultet til intet våben
            Weapon weapon = new Unarmed(new string[] {},Vector2.Zero);

            foreach (GameObject item in objects)
            {
                //hvis objektet er et våben og er i range af spilleren
                if (item is Weapon && IsInRange(position,item.Position,pickupRange))
                {
                    //hvis våbenet ikke tilhøre en actor i forvejen
                    if (!((Weapon)item).IsEquiped) {
                       
                        weapon = (Weapon)item;
                    }
                }
            }

            //dropper det våben vi har nu på jorden så man kan samle det op igen
            DropWeapon();

            return weapon;
        }

        /// <summary>
        /// Finder distancen mellem 2 vektore og checker om de er højere eller lavere end en distance variable
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="dist"></param>
        /// <returns>Returnere true hvis de to vektore er i range af hinanden og false hvis ude af range</returns>
        private bool IsInRange(Vector2 pos1, Vector2 pos2, float dist)
        {
            //finder distancen mellem de to positioner
            double s = Math.Abs(Math.Sqrt(Math.Pow(pos2.X, 2) + Math.Pow(pos2.Y, 2)) - Math.Sqrt(Math.Pow(pos1.X, 2) + Math.Pow(pos1.Y, 2)));
            //hvis distancen mellem de to positioner er mindre end den givne distance
            if (s<dist)
            {
                return true;
            }

            return false;
        }

        
    }
}
