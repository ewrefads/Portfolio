using Meerkat_Mining.Builderpattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    /// <summary>
    /// Drill er et component som bruges af spilleren til at grave blocke
    /// Drill kan aktiveres med spacebar eller et spiller move WASD
    /// </summary>
    public class Drill : Component, IGameListner
    {
        //reference til spiller objektet
        public Player player;
        //checker om vi holder spacebar nede
        private Dictionary<Keys, BUTTONSTATE> spaceKey = new Dictionary<Keys, BUTTONSTATE>();

        //hvor meget skade gør drill til blokke | kan opgraderes
        private float damage;
        //hvor hurtigt kan spilleren mine | kan opgraderes
        private float miningSpeed;

        //holder styr på om der kan mines igen
        private float miningTime;

        public float Damage { get => damage; set => damage = value; }
        public float MiningSpeed { get => miningSpeed; set => miningSpeed = value; }

        public override void Awake()
        {
            
        }

        public override void Start()
        {
            //sætter drillets sprite
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("block/stone/stone1");

        }

        public override void Update()
        {
            //lytter efter spillerens input
            InputHandler.Instance.Execute(this);
            //tæller op hvornår vi kan mine igen
            miningTime += GameWorld.DeltaTime;
        }

        /// <summary>
        /// aktiveres når spilleren trykker spacebar
        /// </summary>
        public void Mine()
        {
            //checker om spilleren holder spacebar nede
            if (spaceKey.ContainsValue(BUTTONSTATE.DOWN))
            {
                //grav blok
                Dig();
            }
        }
        /// <summary>
        /// kalder Mined funktionen på en blok hvis den kan finde en
        /// </summary>
        public void Dig() 
        {
            //kan vi mine igen
            if (miningSpeed<miningTime) {

                //er vi for langt til højre eller venstre
                if (player.gridPosition.X == 0 && player.lookingDirection.X == -1 || player.gridPosition.X > TerrainGenerator.Instance.sizeW - 1)
                {
                    return;
                }
                //er vi for langt op eller ned
                else if (player.gridPosition.Y == 0 && player.lookingDirection.Y == -1 || player.gridPosition.Y > TerrainGenerator.Instance.sizeH - 1)
                {
                    return;
                }
                // får alle gameworlds blocke
                GameObject[,] blocks = GameWorld.Instance.blocks;

                //x og y som erklære hvad for en block der skal mines
                int x = (int)(player.gridPosition.X + player.lookingDirection.X);
                int y = (int)(player.gridPosition.Y + player.lookingDirection.Y);

                //hvis der er en block på den position
                if (blocks[x, y] != null) {
                    //får block componentet
                    Block block = blocks[x, y].GetComponent<Block>() as Block;

                    //Block block = blocks[(int)player.gridPosition.X, (int)player.gridPosition.Y].GetComponent<Block>() as Block;
                    //miner blocken
                    block.Mined(damage);
                    miningTime = 0;

                    //hvis blocken ikke har mere hp tilbage så skal spilleren flyttes
                    
                    if (block.IsDead == true)
                    {
                        player.MovePlayer();

                        //spilleren tilføjer blocken til sit inventory
                        player.AddItemToInventory(block.Type);
                    }
                }
                //der var ikke nogen block og spilleren flytter sig til den position istedet for at mine
                else { player.MovePlayer(); }
            }
            
        }

        /// <summary>
        /// bruger observer pattern til at opdatere de knapper spilleren trykker på og gemmer den i spacekey dictionarien
        /// </summary>
        /// <param name="gameEvent"></param>
        public void Notify(GameEvent gameEvent)
        {

            if (gameEvent is ButtonEvent)
            {
                ButtonEvent be = (gameEvent as ButtonEvent);

                spaceKey[be.Key] = be.State;


            }


        }
    }
}
