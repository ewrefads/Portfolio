using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using Sneak_and_seek_dungeons.CommandPattern;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    // Lucas

    /// <summary>
    /// Alert: [Trigger: Suspicion er over maxSuspicion, Tager skade fra player], Enemy kender til player position
    /// Camp: [Trigger: Ved spawn], Enemy movemntspeed er 0, Kan observe
    /// Distracted: [Trigger: Player bruger en bestemt ability, Player har ikke været i observationsfletet i en bestemt mængde af tid hvor enemy er Alert.]
    /// Partol: [Trigger: Ved spawn, Efter et distraced state]
    /// Sleep: [Trigger: Ved spawn], Enemy movement er 0 & Kan ikke observe.
    /// </summary>
    public enum ENEMYBEHAVIOUR { ALERT, CAMP, DISTRACTED, PATROL, SLEEP }


    internal class Enemy : Entity
    {
        // Den afstand enemy gerne vil have før den angriber player. Da det er ret usandsynligt at enemy præcist ville være på en præfereret distance så har vi 2 fields for et minimum og maksimum afstand som enemy vil angribe i.
        private float minPrefDistance;
        private float maxPrefDistance;
        
        // Nedkølingstimer for enemy angreb
        private float attackCooldown;

        // float til at sætte enemy speed til baseSpeed når der er brug for det (f.eks. efter et alert behaviour (hurtiger speed) vil den i distracted behaviour få baseSpeed som speed)
        private float baseSpeed;

        // Når susppicion er størrer eller lig med maxSuspicion skal enemy ændre behaviour til Alert.
        private float maxSuspicion;

        // En int som holder styr på hvilket movementpoint enemy går efter
        private int currentPointIndex = 0;

        // Bool som holder styr på om enemy skal kunne bevæge sig eller ej.
        public static bool isMoving = true;

        // Bool som holder styr på om enemy har fået et endepunkt som den kan få en pathfinding til.
        private bool gotEndPoint = false;

        private bool isRanged;

        // Tråd til enemy movement
        private Thread movementThread;

        private readonly object movementLock;

        // En vector 2 som er endepunktet for pathfinding
        private Vector2 endPoint = new Vector2();

        // En liste af vector2 som enemy bruger som points for movement
        private List<Vector2> movementPoints = new List<Vector2>();

        // field som holder værdien af distance mellem enemy og dets nuværende movementpoint
        private float distanceToMovementpoint;

        // Player (Så enemy kan tilgå player position)
        public Player player;

        private float playerDistance;

        // En værdi for hvor meget en enemy har set af en player. Er blevet gjort til public med properties i det tilfælde at player kan anvende abilities og andre alerted enemies kan alerete enemy.
        public float Suspicion { get; set; }

        //enemy emote
        private GameObject emote;

        // Nuværende enemy opførelsel
        public ENEMYBEHAVIOUR Behaviour { get; set; }


        // Trapez fields (forkortet til Tpz)
        public float TpzWidth { get; set; }
        public float TpzHeight { get; set; }
        public float TpzAngle { get; set; }
        public float TpzViewDistance { get; set; }
        public float TpzViewAngle { get; set; }
        public float TpzRotationAngle { get; set; }

        

        // Constructor (Venter med at skrive summary indtil alle parametre er skrevet i den)
        public Enemy(object syncObject, ENEMYBEHAVIOUR startBehaviour, float health, float speed, float damage, float minPref, float maxPref, float maxSus, bool isRanged/*,has ability*/) // Parameter , ENEMYBEHAVIOUR behaviour (Note: Siden vi ikke har nået at få flere enemy typer ind ende ventes der med at skrive parametre ind i constructoren)
        {
            movementLock = syncObject;


            // SET HP
            this.health = health;

            // SET MOVEMENTSPEED ()

            this.speed = speed; //<-- erstat med this.speed = speed;

            baseSpeed = speed;

            // Set Attack Damage

            this.damage = damage;

            // DEFINE ATTACK METHOD
            this.isRanged = isRanged;
                
            // SET PREFFERED ATTACK DISTANCE

            minPrefDistance = minPref;
            maxPrefDistance = maxPref;

            // SET ABILITES

            // SET BEHAVIOUR
            ChangeBehaviour(startBehaviour);

            // SET OBSERVATION AREA SIZE (Trapezoid)

            TpzWidth = 50f;
            TpzHeight = 300f;
            // Vinkelbredden af trapez
            TpzAngle = 50;
            TpzViewDistance = 300f;
            TpzViewAngle = 180f;
            // Rotationsvinklen af trapezen (Når enemy vender sig om så skal trapezen gøre det samme)
            TpzRotationAngle = 0f;

            // SET SUSPICION 
            maxSuspicion = maxSus;

            
            //isMoving = true;


    }

        public override void Start()
        {
            GameObject.Tag = "Enemy";
            if (emote==null) {
                emote = EmoteFactory.Instance.Create(null);
            }

            // Få player (gameobject) information*
            player = GameWorld.Instance.FindObjectOfType<Player>() as Player;

        }

        public override void Update()
        {
            emote.Transform.Position = GameObject.Transform.Position - new Vector2(-10,50);

            playerDistance = Vector2.Distance(player.GameObject.Transform.Position, GameObject.Transform.Position);

            // Ændrer på rotation vinklen for trapzen så den peger med velocity (i radianer). Fandt ud af at Math.Atan2 gav den forkerte vinkel (90 grader for meget) pga. monogame moment så der bliver trukket pi/2 radianer fra udregningen. 
            TpzRotationAngle = (float)Math.Atan2(velocity.Y, velocity.X) - (float)Math.PI/2;
            
            // Der tjekkes efter om player existerer
            if (player is not null)
            {

                // Opsæt if statement så enemy ikke observer når de ikke skal.
                if (Behaviour != ENEMYBEHAVIOUR.SLEEP)
                {
                    Observe();
                }

                // If statement som tjekker om enemy skal skifte behaviour om til Alert
                if (Suspicion >= maxSuspicion && Behaviour != ENEMYBEHAVIOUR.ALERT)
                {
                    ChangeBehaviour(ENEMYBEHAVIOUR.ALERT);
                }

                // Movement (Kode kommenteret ud for nu)
                //Movement();

                // Metode for test af kode og bugfixing (Ingen intention for at metoden skulle blive brugt på release)
                //CheckMovementStatus();



                // Enemy angreb tjekker efter om player er inden for enemys preferret distance. Hvis det er true så vil enemy udføre sin angrebsmetode
                if (Behaviour == ENEMYBEHAVIOUR.ALERT && Vector2.Distance(player.GameObject.Transform.Position, GameObject.Transform.Position) >= minPrefDistance && Vector2.Distance(player.GameObject.Transform.Position, GameObject.Transform.Position) <= maxPrefDistance)
                {
                    Attack();
                }

            }
        }

        /// <summary>
        /// En metode som for enemy til at gå rundt afhængig af behaviour. Denne metode køres på movementTread.
        /// </summary>
        //private void Movement()
        //{
        //    // En distance tolerence for hvornår enemy movement er i nærheden af den movementpoint. Havde tideliger problemer med movement uden tolernce fordi den ikke kunne præcist være på et movementpoint.
        //    float distanceThreshold = 10f;

        //    // Step 1: Find path (Udfyld movementPoints listen)

        //    while (!gotEndPoint)
        //    {
        //        endPoint = new Vector2(GameWorld.Instance.rnd.Next(GameWorld.Instance.Dun.MinX, GameWorld.Instance.Dun.MaxX), GameWorld.Instance.rnd.Next(GameWorld.Instance.Dun.MinY, GameWorld.Instance.Dun.MaxY));
        //        foreach (Rectangle room in GameWorld.Instance.Dun.Rooms)
        //        {
        //            if (room.Contains(endPoint))
        //            {
        //                gotEndPoint = true;
        //            }
        //        }
        //    }
            
            
            
        //    if (!isMoving)
        //    {
        //        switch (Behaviour)
        //        {
        //            // Alert (Pathfind, Gå mod player til enemy har en preffered distance mellem den og player)
        //            case ENEMYBEHAVIOUR.ALERT:
        //                movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, player.GameObject.Transform.Position);
        //                isMoving = true;
        //                break;
        //            // Distracted (Pathfind, Gå tilfældigt rundt i rummet)
        //            case ENEMYBEHAVIOUR.DISTRACTED:
        //                movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, endPoint);
        //                isMoving = true;
        //                break;
        //            // Patrol (Pathfind, Gå tilfældigt rundt i rummet
        //            case ENEMYBEHAVIOUR.PATROL:
        //                movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, endPoint);
        //                isMoving = true;
        //                break;
        //            default:
        //                // Other (Ingen movement & pathfinding)
        //                break;
        //        }
        //    }

        //    float distanceToMovementpoint = Vector2.Distance(GameObject.Transform.Position, movementPoints[currentPointIndex]);

        //    if (distanceToMovementpoint <= distanceThreshold)
        //    {
        //        // Gå til næste movementPoint
        //        currentPointIndex++;
        //        // Når enemy er nået til enden af pathfinding
        //        if (currentPointIndex >= movementPoints.Count)
        //        {
        //            currentPointIndex = 0;
        //            gotEndPoint = false;
        //            isMoving = false;
        //        }
        //    }

        //    if (isMoving)
        //    {
        //        velocity = Vector2.Normalize(movementPoints[currentPointIndex] - GameObject.Transform.Position);

        //        // Update the object's position based on the direction and speed

        //        GameObject.Transform.Position += velocity * speed * GameWorld.DeltaTime;
        //    }
            
        //}

        private void MoveToTarget()
        {
            while (true)
            {
                // If ser om enemy skal kunne bevæge sig
                while (isMoving)
                {
                    // En distance tolerence for hvornår enemy movement er i nærheden af den movementpoint. Havde tideliger problemer med movement uden tolernce fordi den ikke kunne præcist være på et movementpoint.
                    float distanceThreshold = 10f;

                    // Find path (Udfyld movementPoints listen)

                    // Hvis enemy ikke har et endepunkt at gå til så findes der et nyt tilfældigt punkt (Vecotr2) som er i dungeon.
                    while (!gotEndPoint)
                    {
                        // Lav et tilfældigt punkt inden for det område hvor det er muligt at lave en dungeon.
                        endPoint = new Vector2(GameWorld.Instance.rnd.Next(GameWorld.Instance.Dun.MinX, GameWorld.Instance.Dun.MaxX), GameWorld.Instance.rnd.Next(GameWorld.Instance.Dun.MinY, GameWorld.Instance.Dun.MaxY));
                        // Tjek efter om endepunktet befinder sig i et dungeonroom. Hvis ikke laves der et nyt endPoint.
                        foreach (Rectangle room in GameWorld.Instance.Dun.Rooms)
                        {
                            if (room.Contains(endPoint))
                            {
                                gotEndPoint = true;
                            }
                        }
                    }

                    // Pathfinding afhænig af enemy behaviour.
                    switch (Behaviour)
                    {
                        // Alert (Pathfind, Gå mod player til enemy har en preffered distance mellem den og player)
                        case ENEMYBEHAVIOUR.ALERT:
                            movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, player.GameObject.Transform.Position);
                            break;
                        // Distracted (Pathfind, Gå tilfældigt rundt i rummet)
                        case ENEMYBEHAVIOUR.DISTRACTED:
                            movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, endPoint);
                            break;
                        // Patrol (Pathfind, Gå tilfældigt rundt i rummet)
                        case ENEMYBEHAVIOUR.PATROL:
                            movementPoints = GameWorld.Instance.Pathfinding(GameObject.Transform.Position, endPoint);
                            break;
                        default:
                            // Hvis enemy ikke er i et behaviour hvor der skal pathfindes så clearer vi movemnpoints listen
                            movementPoints.Clear();
                            break;
                    }

                    // Hvis listen har en værdi/værdier så udregn distancen mellem enemy og det movementpoint enenmy skal gå efter
                    if (movementPoints.Count != 0)
                    {
                        distanceToMovementpoint = Vector2.Distance(GameObject.Transform.Position, movementPoints[currentPointIndex]);
                    }
                    
                    // Når enemy er tæt nok ved movementpoint tælller vi op på currentPointIndex så enemy går efter det næste movmentpoint
                    if (distanceToMovementpoint <= distanceThreshold)
                    {
                        // Gå til næste movementPoint
                        currentPointIndex++;
                        // Når enemy er nået til endepunktet af pathfindingen 
                        if (currentPointIndex >= movementPoints.Count)
                        {
                            // Point index blive nulstillet og enemy har ikke længere et endepunkt at gå efter.
                            currentPointIndex = 0;
                            gotEndPoint = false;

                        }
                    }

                    // når der er movementpoints at gå efter så skal enemy have en retning mod punktet og gå mod den.
                    if (movementPoints != null && movementPoints.Count > 0 && playerDistance>minPrefDistance+10)
                    {
                        lock (movementLock) { 

                        velocity = Vector2.Normalize(movementPoints[currentPointIndex] - GameObject.Transform.Position);

                        // Objectets position bliver baseret af dets retning og hastighed. 

                        GameObject.Transform.Position += velocity * speed * GameWorld.DeltaTime;
                        }

                    }
                    Thread.Sleep(20);
                }

                Thread.Sleep(90);
            }

        }




        public void StartMovement()
        {
            if (!isMoving)
            {
                isMoving = true;
            }
        }

        public void StopMovement()
        {
            if (isMoving)
            {
                isMoving = false;
            }
        }

        /// <summary>
        /// Metode til test af movement kode (Ingen intention for at metoden skulle blive brugt på release)
        /// </summary>
        private void CheckMovementStatus()
        {
            
            
                if (!isMoving)
                {
                    switch (Behaviour)
                    {
                        case ENEMYBEHAVIOUR.ALERT:
                            StartMovement();
                            break;
                        case ENEMYBEHAVIOUR.DISTRACTED:
                            StartMovement();
                            break;
                        case ENEMYBEHAVIOUR.PATROL:
                            StartMovement();
                            break;
                        default:

                            break;
                    }
                }
            
        }

        private void Attack()
        {

            // Hvis enemy can se player og er i pref distance så angriber enemy og skader player
            if (CanSeePlayer(player) && playerDistance>minPrefDistance && playerDistance<maxPrefDistance && attackCooldown <= 0)
            {
                if (!isRanged)
                {
                    player.OnHit(damage); // placeholder tal (5)

                    // Sæt nedkæling på angreb
                    attackCooldown = 2;
                }
                else if (isRanged)
                {
                    GameObject projectile = ProjectileFactory.Instance.Create(PROJECTILETYPE.ARROW);
                    projectile.Transform.Position = GameObject.Transform.Position;
                    (projectile.GetComponent<Projectile>() as Projectile).Velocity = velocity;

                    GameWorld.Instance.Instantiate(projectile);

                    // Sæt nedkæling på angreb
                    attackCooldown = 2;

                }

            }
            else
            {
                // Tel næd på nedkæling
                attackCooldown -= 1 * GameWorld.DeltaTime;
                
            }



            // if ranged

        }


        /// <summary>
        /// Når enemy skal ændre adfærd så gøres det med denne metode. Metoden ændrer på enemy speed og behaviour fields.
        /// </summary>
        /// <param name="change">Param til det adfærd enemy skal ændres til (ENEMYBEHAVIOUR.*****)</param>
        public void ChangeBehaviour(ENEMYBEHAVIOUR change)
        {
            if (emote==null)
            {
                emote = EmoteFactory.Instance.Create(null);
            }
            SpriteRenderer emoteSR = emote.GetComponent<SpriteRenderer>() as SpriteRenderer;

            switch (change)
            {
                case ENEMYBEHAVIOUR.ALERT:
                    speed = baseSpeed * 2f;
                    emoteSR.SetSprite("Sprites/Enemy emote/Alert");
                    // Display ! icon over enemy
                    break;
                case ENEMYBEHAVIOUR.CAMP:
                    speed = 0f;
                    break;
                case ENEMYBEHAVIOUR.DISTRACTED:
                    speed = baseSpeed * 0.7f;
                    emoteSR.SetSprite("Sprites/Enemy emote/distraction");
                    // Display ? icon over enemy
                    break;
                case ENEMYBEHAVIOUR.PATROL:
                    speed = baseSpeed;
                    emoteSR.SetSprite("Sprites/Enemy emote/distraction");
                    break;
                case ENEMYBEHAVIOUR.SLEEP:
                    speed = 0f;
                    // Display Zzz icon over enemy
                    break;
                default:
                    break;


            }
            Behaviour = change;

        }

        // Metode som kan observere player
        private void Observe()
        {

            if (CanSeePlayer(player))
            {
                Suspicion += 1 * player.SneakLevel * GameWorld.DeltaTime; // 25 er en placeholder værdi

            }
            else if (Suspicion > 0 && Behaviour == ENEMYBEHAVIOUR.ALERT && !CanSeePlayer(player))
            {
                // Når enemy ikke observer player vil dens suspicion falde
                Suspicion -= 15 * GameWorld.DeltaTime;
                if (Suspicion <= 0)
                {
                    // Lige nu bliver behaviour sat til patrol (Skal ændres til distracted)
                    ChangeBehaviour(ENEMYBEHAVIOUR.PATROL);
                }
            }
            else if (Suspicion > 0 && Behaviour != ENEMYBEHAVIOUR.ALERT && !CanSeePlayer(player))
            {
                Suspicion -= 10 * GameWorld.DeltaTime;
            }
            
                
                // Kode som får enemy til at vende en tilfældig retning (Til camp behaviour)
                
                // Nædtælling
                // turnOverTime -= 1 * GameWorld.DeltaTime;
                //if (turnOverTime <= 0 && Behaviour == ENEMYBEHAVIOUR.CAMP)
                //{
                        // Rotation TEST (Skal finde ud af om rotation skal kun være i fire retninger (NSEW) eller hvliken som helst vinkel (360 grader))
                    // --- TpzRotationAngle = GameWorld.Instance.rnd.Next(0, 90); ---

                        // Lav componenter for vector med en tilfældig værdi
                    //float x = (float)(GameWorld.Instance.rnd.NextDouble() * 2 - 1);
                    //float y = (float)(GameWorld.Instance.rnd.NextDouble() * 2 - 1);

                    // Her giver vi enemy en ny (tilfældig) retning
                    //velocity = new Vector2(x, y);
                    //velocity.Normalize();

                    // Nedtælling til næste gang enemy vender sig om (Mangler tilføjelse af countdown field)
                    //turnOverTime = GameWorld.Instance.rnd.Next(20, 30);
                //}
            

        }
        // Observations området er en trapez. Med hjælp fra Chat GBT (som har lavet koden for trapezen og hvornår player kan blive obsereret samt er i observationsfeltet) 
        // Det meste af koden herunder skrevet af Chat GBT: (Er blevet redigeret så det fungerer med alt andet kode)
        public bool CanSeePlayer(Player player)
        {
            // Calculate the coordinates of the four vertices of the trapezoid
            float halfWidth = TpzWidth / 2;
            Matrix rotationMatrix = Matrix.CreateRotationZ(TpzRotationAngle);
            Vector2 v1 = GameObject.Transform.Position + Vector2.Transform(new Vector2(-halfWidth, 0), rotationMatrix);
            Vector2 v2 = GameObject.Transform.Position + Vector2.Transform(new Vector2(-halfWidth + TpzHeight * (float)Math.Tan(TpzAngle), TpzHeight), rotationMatrix);
            Vector2 v3 = GameObject.Transform.Position + Vector2.Transform(new Vector2(halfWidth - TpzHeight * (float)Math.Tan(TpzAngle), TpzHeight), rotationMatrix);
            Vector2 v4 = GameObject.Transform.Position + Vector2.Transform(new Vector2(halfWidth, 0), rotationMatrix);

            // Check if the player is within the view distance of the enemy
            Vector2 toPlayer = player.GameObject.Transform.Position - GameObject.Transform.Position;
            if (toPlayer.LengthSquared() > TpzViewDistance * TpzViewDistance)
            {
                return false;
            }

            // Check if the player is within the view angle of the enemy
            float angleToPlayer = (float)Math.Atan2(toPlayer.Y, toPlayer.X);
            float angleDiff = MathHelper.WrapAngle(angleToPlayer - TpzAngle);
            if (Math.Abs(angleDiff) > TpzViewAngle / 2)
            {
                return false;
            }

            // Check if the player is within the trapezoid
            if (!IsPointInTrapezoid(player.GameObject.Transform.Position, v1, v2, v3, v4))
            {
                return false;
            }

            return true;
        }

        // Kode skrevet af Chat GBT: Metode for at se om et punkt er inden for trapzen
        private bool IsPointInTrapezoid(Vector2 p, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            float cross1 = CrossProduct(v2 - v1, p - v1);
            float cross2 = CrossProduct(v3 - v2, p - v2);
            float cross3 = CrossProduct(v4 - v3, p - v3);
            float cross4 = CrossProduct(v1 - v4, p - v4);

            if (cross1 > 0 && cross2 > 0 && cross3 > 0 && cross4 > 0)
            {
                return true;
            }

            if (cross1 < 0 && cross2 < 0 && cross3 < 0 && cross4 < 0)
            {
                return true;
            }

            return false;
        }

        // Kode skrevet af Chat GBT: Metode for at finde krydsproduktet fra to vectorer
        private float CrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Med hjælp fra SpriteBatchExtensions kan vi tegne trapzen så det kan ses i spillet
            float halfWidth = TpzWidth / 2;
            Matrix rotationMatrix = Matrix.CreateRotationZ(TpzRotationAngle);
            Vector2 v1 = GameObject.Transform.Position + Vector2.Transform(new Vector2(-halfWidth, 0), rotationMatrix);
            Vector2 v2 = GameObject.Transform.Position + Vector2.Transform(new Vector2(-halfWidth + TpzHeight * (float)Math.Tan(TpzAngle), TpzHeight), rotationMatrix);
            Vector2 v3 = GameObject.Transform.Position + Vector2.Transform(new Vector2(halfWidth - TpzHeight * (float)Math.Tan(TpzAngle), TpzHeight), rotationMatrix);
            Vector2 v4 = GameObject.Transform.Position + Vector2.Transform(new Vector2(halfWidth, 0), rotationMatrix);

            Vector2[] vertices = new Vector2[] { v1, v2, v3, v4 };

            // Her bliver trapzen tegnet med spritebatchmetoden DrawPolygon (SpriteBatchExtensions)
            spriteBatch.DrawPolygon(vertices, Color.Red);
        }



        public override void OnHit(float damage)
        {
            base.OnHit(damage);
            // Vi overrider OnHit Så enemy behaviour kan blive til alert når de bliver skadet og sætter suspicion til maxSuspicion
            Behaviour = ENEMYBEHAVIOUR.ALERT;
            Suspicion = maxSuspicion;
        }

        public override void Awake()
        {
            

            // Start movementThread
            movementThread = new Thread(MoveToTarget);
            movementThread.IsBackground = true;
            movementThread.Start();
            base.Awake();

        }
    }



}
