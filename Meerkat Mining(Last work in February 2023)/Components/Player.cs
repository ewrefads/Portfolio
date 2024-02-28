using Meerkat_Mining.Builderpattern;
using Meerkat_Mining.Components;
using Meerkat_Mining.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    
    /// <summary>
    /// Player classen laver en spiller figur som kan styrers rund på mappet med WASD
    /// Spilleren kan også aktivere sit drill til at mine blokke hvis spilleren går ind i dem
    /// spilleren har et inventory over alle de blocke som er blevet mined og kan sælges senere
    /// </summary>
    public class Player : Component, IGameListner
    {
        

        //De movement keys som bliver klikket på bliver opbevaret sammen med informationen om den er trykket ned eller ej
        private Dictionary<Keys, BUTTONSTATE> movementKeys = new Dictionary<Keys, BUTTONSTATE>();

        //spillerens inventory som opbevare alle de block typer som spilleren har samlet og hvor mange spilleren har af den type
        private Dictionary<BLOCKTYPE, int> inventory = new Dictionary<BLOCKTYPE, int>();
        //De stats som spilleren bruger som kan opgraderes



        private Dictionary<PLAYERSTATS, float> stats;
        //hvor mange penge spilleren har til at bruge i shoppen
        private int money;

        //public FACING lookingDirection{ get; private set; }
        //en vector2 som peger i den retning spilleren sidst trykket i (højre = (1,0))
        public Vector2 lookingDirection { get; private set; }
        //spillerens position på det 2D array som hedder "grid" i gameworld
        public Vector2 gridPosition { get; private set; }

        //properties
        public Dictionary<PLAYERSTATS, float> Stats { get => stats;}
        public int Money { get => money; set => money = value; }
        public Dictionary<BLOCKTYPE, int> Inventory { get => inventory;}

        //det drill gameobjekt som spilleren bruger til at grave med
        public GameObject drill;

        //hvor langt væk spilleren kan se af bloks
        private int vision;

        //spillerens animator
        private Animator animator;

        public override void Awake()
        {

        }

        public override void Start()
        {
            //sætter spillerens sprite
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("player/meerkat1");
            sr.Scale = 1.5f;

            //får animator componenet
            animator = (Animator)GameObject.GetComponent<Animator>();

            //spillerens start stats bliver sat op
            stats = new Dictionary<PLAYERSTATS, float>
            {
                { PLAYERSTATS.MININGSPEED, 0.3f },
                { PLAYERSTATS.MOVESPEED, 100 },
                { PLAYERSTATS.MININGDAMAGE,10}
            };
            //ingen penge til at starte med :(
            money = 0;

            //spillerens start position bliver angivet i gridposition og oversat til en ingame position
            gridPosition = new Vector2(10,0);
            GameObject.Transform.Position = GameWorld.Instance.grid[(int)gridPosition.X, (int)gridPosition.Y];
            //den block spilleren spawner på bliver mined
            (GameWorld.Instance.blocks[(int)gridPosition.X, (int)gridPosition.Y].GetComponent<Block>() as Block).Mined(99999);

            //animator = (Animator)GameObject.GetComponent<Animator>();
            UpdateStats();

            //blok vision erklæret
            vision = 5;

            //opdatere hvad vi kan se på banen
            UpdateVision();
            
        }

        public void UpdateStats()
        {
            //spilleren sætter drill op
            Drill d = (drill.GetComponent<Drill>() as Drill);
            d.MiningSpeed = stats[PLAYERSTATS.MININGSPEED];
            d.Damage = stats[PLAYERSTATS.MININGDAMAGE];
        }
        /// <summary>
        /// tilføjer en blocktype til inventory dictionary medmindre den blocktype allerede er i
        /// Hvis blocktypen allerede er i så bliver værdien 1 støre
        /// </summary>
        /// <param name="type"></param>
        public void AddItemToInventory(BLOCKTYPE type)
        {
            //hvis inventory har denne blocktype
            if (inventory.ContainsKey(type))
            {
                //vi har en mere af den type i invetory
                inventory[type]++;
            }
            //vi indsætter den nye blocktype i vores inventory
            else { inventory.Add(type,1); }
            
        }

        public override void Update()
        {
            //lytter efter spillerens input
            InputHandler.Instance.Execute(this);

            //plays player animation
            animator.PlayAnimation("idle");

        }

        /// <summary>
        /// spilleren har trykket på en knap og så checkes der om spilleren kan bevæge sig eller mine en block
        /// </summary>
        /// <param name="direction">indtager en retning i vector2 som argument</param>
        public void Move(Vector2 direction)
        {
            //nye retning vi kigger i
            lookingDirection = direction;
            


             //bevægelsen vil være...
             //for langt til højre eller venstre på banen
             if (gridPosition.X==0 && direction.X == -1 || gridPosition.X>= GameWorld.Instance.grid.GetLength(0) - 1 && direction.X == 1) 
             {
                 return;
             }
             //for langt op eller ned på banen
             else if (gridPosition.Y == 0 && direction.Y == -1 || gridPosition.Y >= GameWorld.Instance.grid.GetLength(1) - 1 && direction.Y == 1)
             {
                    return;
             }

             UpdateLookingDirection();
             //får den block der ligger på den position vi gerne vil hen på
             GameObject go = GameWorld.Instance.blocks[(int)(gridPosition.X + direction.X), (int)(gridPosition.Y + direction.Y)];

             //hvis der er en block i den retning vi gerne vil gå så miner vores drill blokken istedet
             if (go!=null)
             {
                 Block b = GameWorld.Instance.blocks[(int)(gridPosition.X + direction.X), (int)(gridPosition.Y + direction.Y)].GetComponent<Block>() as Block;
                 (drill.GetComponent<Drill>() as Drill).Dig();
                 return;
             }


            //hvis der ikke er en block i feltet så flytter spilleren
            //hvis vi ikke holder knappen nede
            if (!movementKeys.ContainsValue(BUTTONSTATE.DOWN))
            {
                MovePlayer();
            }

                /*if (movementKeys.ContainsKey(Keys.Space))
                {
                    Drill d = drill.GetComponent<Drill>() as Drill;
                    d.Dig();
                }*/
            

            
            //gammelt direction system som brugte en enum istedet

            /*if (direction == new Vector2(0,1))
            {
                lookingDirection = FACING.RIGHT;
            }
            else if (direction == new Vector2(0, -1))
            {
                lookingDirection = FACING.LEFT;
            }
            else if (direction == new Vector2(1, 0))
            {
                lookingDirection = FACING.UP;
            }
            else if (direction == new Vector2(-1, 0))
            {
                lookingDirection = FACING.DOWN;
            }*/


        }
        /// <summary>
        /// opdatere den retning spillerens sprite vender efter om lookingdirection er positiv eller negativ
        /// </summary>
        public void UpdateLookingDirection()
        {
            SpriteRenderer sr = (GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer);

            if (lookingDirection.X==-1)
            {
                sr.SpriteEffect = SpriteEffects.FlipHorizontally;
            }
            else if (lookingDirection.X == 1)
            {
                sr.SpriteEffect = SpriteEffects.None;
            }
        }

        /// <summary>
        /// flytter spillerens position ved at gå i retningen som spilleren kigger
        /// spillerens position bliver til hvad grid positionen i gameworld er
        /// </summary>
        public void MovePlayer()
        {
            //move player
            gridPosition = new Vector2(gridPosition.X + lookingDirection.X, gridPosition.Y + lookingDirection.Y);
            Vector2 move = GameWorld.Instance.grid[(int)(gridPosition.X), (int)(gridPosition.Y)];
            GameObject.Transform.Position = move;

            //opdatere hvor meget af banen vi nu kan se
            UpdateVision();
        }

        /// <summary>
        /// styrer hvor mange blocks væk spilleren kan se
        /// alle blocke er sorte indtil spilleren er i range af dem og bliver synlige
        /// </summary>
        public void UpdateVision()
        {
            //får alle blocke fra gameworld
            GameObject[,] blocks = GameWorld.Instance.blocks;

            //looper igennem så der dannes en firkant rundt om spilleren af ikke sorte blocke
            for (int i = -vision; i < vision+1; i++)
            {
                for (int j = -vision; j < vision+1; j++)
                {
                    //checker om der overhovedet er en block på positionen
                    if (i + gridPosition.Y>0&&j + gridPosition.X>0&&i<blocks.GetLength(1)-gridPosition.Y&&j<blocks.GetLength(0)-gridPosition.X) {
                        if (blocks[j + (int)gridPosition.X, i + (int)gridPosition.Y] != null)
                        {
                            //sætter blockens farve til hvid og den synlig
                            SpriteRenderer s = (blocks[(int)gridPosition.X + j, (int)gridPosition.Y + i].GetComponent<SpriteRenderer>() as SpriteRenderer);
                            s.Color = Color.White;
                        }
                    }
                }
            }
            
        }

        //bruger observer pattern til at opdatere hvad knapper spilleren trykker på og tilføje det til dictionarien
        public void Notify(GameEvent gameEvent)
        {
            
            if (gameEvent is ButtonEvent)
            {
                ButtonEvent be = (gameEvent as ButtonEvent);

                movementKeys[be.Key] = be.State;


            }


        }

    }
}
