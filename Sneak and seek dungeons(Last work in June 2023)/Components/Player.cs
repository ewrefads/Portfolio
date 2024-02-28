using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sneak_and_seek_dungeons.CommandPattern;
using Sneak_and_seek_dungeons.FactoryPattern;
using Sneak_and_seek_dungeons.ObserverPattern;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// Spiller objektet som spilleren styrer
    /// Frederik
    /// </summary>
    public class Player : Entity, IGameListner
    {

        //De movement keys som bliver klikket på bliver opbevaret sammen med informationen om den er trykket ned eller ej
        private Dictionary<Keys, BUTTONSTATE> movementKeys = new Dictionary<Keys, BUTTONSTATE>();
        private GridField lastValidGridField;
        //nuværende og sidste keyboard states
        private KeyboardState keyState;
        private KeyboardState lastKeyState;
        
        //private List<GameObject> equipeditems = new List<GameObject>();
        //hvor mange skill points spilleren har
        private int skillPoints;

        //spillerens helbred og sneakniveau sliders som opdateres i løbet af spillet
        private Slider healthSlider;
        private Slider sneakSlider;

        //hvilke retninger som der er vægge i
        private List<Vector2> collisionDirections = new List<Vector2>();

        //sneaker spilleren
        private bool isSneaking;
        private float sneakDropOff=1;

        //hvor meget sneak har spilleren
        private float sneakLevel;
        private float maxSneakLevel;

        private bool freshStart;

        //hvor stor rækkevide kan spilleren interegere med objekter i
        private float interActionRange;

        //hvor hurtig spilleren er når de går og sneaker
        private float movementSpeed;
        private float sneakSpeed;
        private bool isMoving;

        private Vector2 movement;

        //det nuværende rum i dungeonen spilleren befinder sig i
        //har ingen funktion mere da det var et tidligere koncept til hvordan kameraet skulle virke
        private Rectangle currentRoom;

        //spillerens collider component
        private Collider playerCollider;

        //de ui elementer som følger rundt med spilleren (HUD)
        private List<GameObject> playerUI = new List<GameObject>();
        public static Vector2 playerPos;

        //private Inventory inventory;

        public int SkillPoints { get => skillPoints; set => skillPoints = value; }
        public List<GameObject> PlayerUI { get => playerUI; set => playerUI = value; }
        public List<Vector2> CollisionDirections { get => collisionDirections; set => collisionDirections = value; }
        public float SneakLevel { get => sneakLevel; private set => sneakLevel = value; }

        //internal Inventory Inventory { get => inventory; set => inventory = value; }


        public override void Start()
        {
            GameObject.Tag = "Player";
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("otherSprites/player_facing_down1");
            sr.Scale = new Vector2(3f,3f);

            playerCollider = GameObject.GetComponent<Collider>() as Collider;
            playerCollider.ActiveCollider = false;

            //inventory = new Inventory();
            
            //initial værdier
            interActionRange = 100;
            movementSpeed = 300;
            sneakSpeed = 100;

            if (freshStart)
            {
                InventorySetup();
            }
            else {
                Inventory.Instance.SetupInventory();
                GameWorld.Instance.Repo.LoadGame();
            }

            Camera cam = ((GameWorld.Instance.FindObjectOfType<Camera>()).GameObject.GetComponent<Camera>() as Camera);
            

            //sætter healthslideren op
            maxHealth = 100;
            health = 100;
            GameObject go = UIFactory.Instance.Create(UITYPE.SLIDER);
            GameWorld.Instance.Instantiate(go);
            healthSlider = go.GetComponent<Slider>() as Slider;
            go.Transform.Position = (GameWorld.ScreenSize / 2) - new Vector2(1600,900);
            go.Transform.Position -= cam.CamOffset;
            healthSlider.MaxValue = maxHealth;
            healthSlider.Value = health;
            healthSlider.SetupText();
            healthSlider.UpdateSlider();

            sneakLevel = 0;
            maxSneakLevel = 100;

            //sætter sneakslideren op
            GameObject go2 = UIFactory.Instance.Create(UITYPE.SLIDER);
            GameWorld.Instance.Instantiate(go2);
            sneakSlider = go2.GetComponent<Slider>() as Slider;
            go2.Transform.Position = (GameWorld.ScreenSize / 2) - new Vector2(1600, 750);
            go2.Transform.Position -= cam.CamOffset;
            sneakSlider.MaxValue = maxSneakLevel;
            sneakSlider.Value = sneakLevel;
            sneakSlider.SetupText();
            sneakSlider.UpdateSlider();
            
            

            

            //ændre spiller huden til midten af skærmen
            Collider col = GameObject.GetComponent<Collider>() as Collider;
            col.CollisionEvent.Attach(this);

            
            //MovePlayerUI(GameObject.Transform.Position);

            

        }


        
        //sætter inventoriet op og tilføjer 3 items til at starte med
        private void InventorySetup() {
            Inventory.Instance.SetupInventory();

            //giver en potion til spilleren
            GameObject potion = new GameObject();
            Potion p = (Potion)potion.AddComponent(new Potion());
            potion.AddComponent(new SpriteRenderer());
            GameWorld.Instance.Instantiate(potion);
            Inventory.Instance.AddItemToInventory(p);

            //giver et sværd til spilleren
            GameObject sword = new GameObject();
            Sword swordComponent = (Sword)sword.AddComponent(new Sword());
            swordComponent.IsInInventory = true;
            sword.AddComponent(new SpriteRenderer());
            GameWorld.Instance.Instantiate(sword);
            //Inventory.Instance.Weapon = swordComponent;
            Inventory.Instance.AddItemToSpeceficItemSlot(10,swordComponent);

            //giver en hjelm til spilleren
            GameObject helmet = new GameObject();
            Helmet helmetComponent = (Helmet)helmet.AddComponent(new Helmet());
            helmetComponent.IsInInventory = true;
            helmet.AddComponent(new SpriteRenderer());
            GameWorld.Instance.Instantiate(helmet);
            //Inventory.Instance.Weapon = swordComponent;
            Inventory.Instance.AddItemToSpeceficItemSlot(-100,helmetComponent);

            
        }

        //når spilleren tager skade opdatere healthslideren også
        public override void OnHit(float damage)
        {
            
            base.OnHit(damage);
            healthSlider.Value = health;
            healthSlider.UpdateSlider();

        }

        //checker for playerinput, dungeon collisions og flytter kameraet
        public override void Update()
        {
            isMoving = false;
            InputHandler.Instance.Execute(this);
            collisionDirections.Clear();

            keyState = Keyboard.GetState();
            MovePlayerCamera();


            GameWorld.Instance.UpdateDungeonColliders(gameObject.Transform.Position);
            playerCollider.CheckDungeonCollision();
            GameObject.Transform.Position += movement;
            MovePlayerUI(movement);
            movement = Vector2.Zero;
            movementKeys.Clear();

            UpdateSneak();

        }

        public void UpdateSneak()
        {
            if (!isMoving && sneakLevel > 0)
            {
                sneakLevel -= sneakDropOff * GameWorld.DeltaTime;
                sneakDropOff += sneakDropOff * GameWorld.DeltaTime;
                sneakSlider.Value = sneakLevel;
                sneakSlider.UpdateSlider();
            }
            else
            {
                sneakDropOff = 1;
            }

            GameWorld.Instance.AlertEnemiesInRadius(GameObject.Transform.Position, sneakLevel*4);
        }

        int num = 0;

        public Player(bool freshStart)
        {
            this.freshStart = freshStart;
        }

        /// <summary>
        /// flytter spilleren en retning medmindre den retning er ind i en væg
        /// </summary>
        /// <param name="velocity"></param>
        public void Move(Vector2 velocity)
        {
            LastVelocity = velocity;
            isMoving = true;
            //returner hvis der er en væg i den retning spilleren gerne vil gå og flytter spilleren en anelse den anden vej
            /*
            foreach (Vector2 v in collisionDirections)
            {
                if (Math.Sign(v.Y) == Math.Sign(velocity.Y) && v.Y != 0)
                {
                    GameObject.Transform.Position += velocity * -1;
                    MovePlayerUI(velocity * -1);
                    return;
                }
                if (Math.Sign(v.X) == Math.Sign(velocity.X) && v.X != 0)
                {
                    GameObject.Transform.Position += velocity * -1;
                    MovePlayerUI(velocity * -1);
                    return; 
                }
               
            }*/

            //List<Vector2> dirs = new List<Vector2>();

            //foreach (Vector2 v in collisionDirections)
            //{
            //    if (!dirs.Contains(v))
            //    {
            //        dirs.Add(v);
            //    }
            //}

            
            


            //if (dirs.Count>3)
            //{
            //    return;
                

            //}
            





            //flytter spilleren med sneak eller walk hastighed + flytter player hud

            playerPos = GameObject.Transform.Position;

            if (!isSneaking) {
                movement += velocity * movementSpeed * GameWorld.DeltaTime;
                //MovePlayerUI(velocity * movementSpeed * GameWorld.DeltaTime);

                if (sneakLevel > 100)
                    return;

                sneakLevel += 20 * GameWorld.DeltaTime;
                sneakSlider.Value = sneakLevel;
                sneakSlider.UpdateSlider();
            }
            else
            {
                movement += velocity * sneakSpeed * GameWorld.DeltaTime;
                //MovePlayerUI(velocity * sneakSpeed * GameWorld.DeltaTime);

                if (sneakLevel > 100)
                    return;

                sneakLevel += 5* GameWorld.DeltaTime;
                sneakSlider.Value = sneakLevel;
                sneakSlider.UpdateSlider();
            }

        }

        /// <summary>
        /// Funktion til at flytte players ui rundt sammen med playeren
        /// </summary>
        /// <param name="vec"></param>
        private void MovePlayerUI(Vector2 vec)
        {
            foreach (GameObject gameObject in playerUI)
            {
                gameObject.Transform.Position += vec;
                
            }
        }

        /// <summary>
        /// checker hvad for et input spilleren tager imod og udføre funktioner ud fra det
        /// </summary>
        public void PlayerInput()
        {
            

            if (keyState.Equals(lastKeyState))
                return;
            

            //if the player sneaks
            if (keyState.IsKeyDown(Keys.LeftShift))
            {
                isSneaking = !isSneaking;
            }

            //if the player opens or closes inventory
            if (keyState.IsKeyDown(Keys.Tab))
            {
                Inventory.Instance.ToggleInventory();
            }

            //if the player Attacks
            if (keyState.IsKeyDown(Keys.Space))
            {
                if (Inventory.Instance.Weapon != null)
                {
                    Inventory.Instance.Weapon.Attack();
                }

                
            }

            //if the player opens or closes the skill tree
            if (keyState.IsKeyDown(Keys.T))
            {

            }

            //if the player interacts
            if (keyState.IsKeyDown(Keys.F))
            {
                Interact();
            }

            lastKeyState = keyState;
        }

        /// <summary>
        /// Når spilleren interegere checker den hvilke interactable gameobjects der er i nærheden og interegere med dem
        /// </summary>
        public void Interact()
        {
            List<GameObject> gameObjects = GameWorld.Instance.GameObjects;
            List<IInteractable> interactableGameObjects = new List<IInteractable>();

            foreach (GameObject gameObject in gameObjects)
            {
                foreach (Component component in gameObject.Components)
                {
                    if (component is IInteractable)
                    {
                        if (component is Item)
                        {
                            if ((component as Item).IsInInventory)
                            {
                                continue;
                            }
                        }
                        if (component is Weapon)
                        {
                            if ((component as Weapon).IsEquipped)
                            {
                                continue;
                            }
                        }

                        interactableGameObjects.Add(((IInteractable)component));
                    }
                }
                
            }

            //finder den interactable som er tættest på player og interacter med den
            IInteractable interact = null;
            float currentDist = 99999999;

            foreach (Component interaction in interactableGameObjects)
            {
                float distance = Vector2.Distance(interaction.GameObject.Transform.Position, GameObject.Transform.Position);
                if (distance<currentDist&&distance< interActionRange && interaction is not Sword)
                {
                    if (interaction is Item) {

                        if (((Item)interaction).IsInInventory)
                        {
                            continue;
                        }
                    }

                    currentDist = distance;
                    interact = (IInteractable)interaction;
                }
            }
            if (interact!=null) {
                interact.Interact();
            }
        }

        /// <summary>
        /// når spilleren trykker på 1,2,3,4,5 bliver denne funktion kaldt med det givne nummer som parameter og aktivere et item i inventoriet
        /// </summary>
        /// <param name="number"></param>
        public void NumberInput(int number)
        {
            

            if(keyState!=lastKeyState)
                Inventory.Instance.UseItemFromHotbar(number);

            lastKeyState = keyState;
        }

        /// <summary>
        /// flytter kameraet til spillerens position
        /// </summary>
        public void MovePlayerCamera()
        {
            Camera playerCam = GameObject.GetComponent<Camera>() as Camera;

            SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();

            playerCam.UpdateCameraPosition(sr.Sprite.Bounds);
        }

        /// <summary>
        /// regner ud hvilke sider der er objekter i
        /// </summary>
        /// <param name="other">other er det andet object spilleren nuværende kollidere med</param>
        /*public void CalculateCounterDirection(GameObject other)
        {
            
            SpriteRenderer spr = (other.GetComponent<SpriteRenderer>() as SpriteRenderer);
            Vector2 spriteSize = new Vector2((float)spr.Sprite.Width/2,spr.Sprite.Height/2);
            spriteSize *= spr.Scale;

            if (other.Transform.Position.X + spriteSize.X/2 >= GameObject.Transform.Position.X)
            {
                Vector2 v = new Vector2(1, 0);
                collisionDirections.Add(v);
                
            }
            if(other.Transform.Position.X - spriteSize.X + spriteSize.X / 2 <= GameObject.Transform.Position.X)
            {
                Vector2 v = new Vector2(-1, 0);
                collisionDirections.Add(v);
                
            }

            if (other.Transform.Position.Y + spriteSize.Y/2 >= GameObject.Transform.Position.Y)
            {
                Vector2 v = new Vector2(0, 1);
                collisionDirections.Add(v);
                
            }
            if (other.Transform.Position.Y - spriteSize.Y + spriteSize.Y / 2 <= GameObject.Transform.Position.Y)
            {
                Vector2 v = new Vector2(0, -1);
                collisionDirections.Add(v);
                
            }

            
        }*/

        // Handle collision with other objects
        public void HandleCollision(GameObject other)
        {
            
            Rectangle bounds = playerCollider.CollisionBox;
            Rectangle otherBounds = (other.GetComponent<Collider>() as Collider).CollisionBox;
            foreach (GridField g in GameWorld.Instance.Grid.Values)
            {
                if (g.position.Intersects(bounds) && (g.enterExitBot || g.enterExitLef || g.enterExitRig || g.enterExitTop))
                {
                    lastValidGridField = g;
                    break;
                }
            }
            float speed = 0;
            Vector2 uiOffSet = new Vector2();
            if (!isSneaking)
            {
                speed = movementSpeed;
            }
            else
            {
                speed = sneakSpeed;
            }
            while (bounds.Intersects(otherBounds)) {
                GameObject.Transform.Translate(new Vector2(-LastVelocity.X, - LastVelocity.Y));
                uiOffSet += new Vector2(-LastVelocity.X, -LastVelocity.Y);
                bounds = playerCollider.CollisionBox;
            }
            if (!lastValidGridField.position.Intersects(bounds)) {
                uiOffSet += new Vector2(lastValidGridField.position.Center.ToVector2().X - GameObject.Transform.Position.X, lastValidGridField.position.Center.ToVector2().Y - GameObject.Transform.Position.Y);
                GameObject.Transform.Position = lastValidGridField.position.Center.ToVector2();
            }
            /*float horizontalDistance = 0;
            if (LastVelocity.X != 0) {
                
                if (LastVelocity.X == 1)
                {
                    horizontalDistance = Vector2.Distance(new Vector2(bounds.X + bounds.Width, bounds.Y), new Vector2(otherBounds.X, bounds.Y));
                }
                else {
                    horizontalDistance = Vector2.Distance(new Vector2(bounds.X, bounds.Y), new Vector2(otherBounds.X + otherBounds.Width, bounds.Y));
                }
            }
            float verticalDistance = 0;
            if (LastVelocity.Y != 0)
            {

                if (LastVelocity.Y == 1)
                {
                    verticalDistance = Vector2.Distance(new Vector2(bounds.X , bounds.Y + bounds.Height), new Vector2(bounds.X, otherBounds.Y));
                }
                else
                {
                    verticalDistance = Vector2.Distance(new Vector2(bounds.X, bounds.Y), new Vector2(bounds.X, otherBounds.Y + otherBounds.Height));
                }
            }
            Vector2 offSet = new Vector2(horizontalDistance, verticalDistance);
            GameObject.Transform.Translate(offSet);
            uiOffSet += offSet;*/
            MovePlayerUI(uiOffSet);
            /*if (Bounds.Intersects(otherBounds))
            {
                // Collision occurred, handle it accordingly
                // Example: Change player behavior or position

                // You can also access specific collision details
                Rectangle collisionArea = Rectangle.Intersect(Bounds, otherBounds);

                

                if (movement.X!=0&&movement.Y!=0)
                {

                    // Example: Resolve collision based on specific areas
                    if (collisionArea.Width > 1)
                    {
                        // Resolve horizontally
                        if (Bounds.Center.X < otherBounds.Center.X)
                        {
                            // Player is to the left of the other object
                            // Move player to the left
                            // Example: playerPosition.X -= collisionArea.Width;
                            Vector2 v = new Vector2(speed * GameWorld.DeltaTime, 0);
                            movement -= v;
                            
                        }
                        else
                        {
                            // Player is to the right of the other object
                            // Move player to the right
                            // Example: playerPosition.X += collisionArea.Width;
                            Vector2 v = new Vector2(speed * GameWorld.DeltaTime, 0);
                            movement += v;
                            
                        }
                    }
                    if (collisionArea.Height > 1)
                    {
                        // Resolve vertically
                        if (Bounds.Center.Y < otherBounds.Center.Y)
                        {
                            // Player is above the other object
                            // Move player up
                            // Example: playerPosition.Y -= collisionArea.Height;
                            Vector2 v = new Vector2(0, speed * GameWorld.DeltaTime);
                            movement -= v;
                            
                        }
                        else
                        {
                            // Player is below the other object
                            // Move player down
                            // Example: playerPosition.Y += collisionArea.Height;
                            Vector2 v = new Vector2(0, speed * GameWorld.DeltaTime);
                            movement += v;
                            
                        }
                    }
                }
                else
                {
                    // Example: Resolve collision based on specific areas
                    if (collisionArea.Width > collisionArea.Height)
                    {
                        // Resolve horizontally
                        if (Bounds.Center.X < otherBounds.Center.X)
                        {
                            // Player is to the left of the other object
                            // Move player to the left
                            // Example: playerPosition.X -= collisionArea.Width;
                            Vector2 v = new Vector2(speed * GameWorld.DeltaTime, 0);
                            movement -= v;
                            
                        }
                        else
                        {
                            // Player is to the right of the other object
                            // Move player to the right
                            // Example: playerPosition.X += collisionArea.Width;
                            Vector2 v = new Vector2(speed * GameWorld.DeltaTime, 0);
                            movement += v;
                            
                        }
                    }
                    if (collisionArea.Height > collisionArea.Width)
                    {
                        // Resolve vertically
                        if (Bounds.Center.Y < otherBounds.Center.Y)
                        {
                            // Player is above the other object
                            // Move player up
                            // Example: playerPosition.Y -= collisionArea.Height;
                            Vector2 v = new Vector2(0, speed * GameWorld.DeltaTime);
                            movement -= v;
                           
                        }
                        else
                        {
                            // Player is below the other object
                            // Move player down
                            // Example: playerPosition.Y += collisionArea.Height;
                            Vector2 v = new Vector2(0, speed * GameWorld.DeltaTime);
                            movement += v;
                           
                        }
                    }
                }
            }*/
        }

        //notifier spilleren omkring events
        public void Notify(GameEvent gameEvent)
        {
            if (gameEvent is ButtonEvent)
            {
                ButtonEvent be = (gameEvent as ButtonEvent);
                
                movementKeys.Add(be.Key,be.State);


            } 
            else if (gameEvent is CollisionEvent)
            {
                CollisionEvent ce = (gameEvent as CollisionEvent);
                //currentRoom = (ce.Other.GetComponent<Room>() as Room).RoomBox;
                //CalculateCounterDirection(ce.Other);
                HandleCollision(ce.Other);
                
            }


        }

        
    }
}
