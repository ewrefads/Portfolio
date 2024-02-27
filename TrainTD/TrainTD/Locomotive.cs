using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace TrainTD
{
    /// <summary>
    /// Et lokomotiv er en togvogn der er krævet for at et tog kan køre
    /// </summary>
    internal class Locomotive : TrainCarriage
    {

        private int maxCarriages;
        private int coal;
        private int healthprCarriage;
        private Texture2D[] smokeSprites;
        private bool waiting = false;
        public int Coal { get => coal; set => coal += value; }

        public Locomotive(int coal, TrainCarriage carrigeAhead, Player owner, Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue, Texture2D[] smokeSprites) : base(carrigeAhead, owner, position, sprites, spriteEffect, lootValue, 20)
        {
            this.coal = coal;
            this.smokeSprites = smokeSprites;
            
        }

        /// <summary>
        /// Returnerer altid falsk da lokomotivet ikke kan have loot
        /// </summary>
        /// <param name="ahead">Retningen i toget requesten er blevet sendt</param>
        /// <param name="amount">hvor meget der skal være plads til</param>
        /// <returns></returns>
        public override bool ConnectedToEmptyCargoCapacity(bool ahead, int amount)
        {
            return false;
        }

        
        public override bool ConnectedToLocomotive()
        {
            return true;
        }

        /// <summary>
        /// En ikke testet metode hvor at hvis lokomotivet tager skade begynder det at frakoble vogne
        /// </summary>
        public void handleTrain() {
            int idealMaxCarriages = health / healthprCarriage;
            if (maxCarriages > idealMaxCarriages + 1) {
                maxCarriages = idealMaxCarriages;
            }
            if (maxCarriages < trainLength(false)) {
                while (maxCarriages < trainLength(false)) {
                    DetacthLastCarriage();
                }
            }
        }

        /// <summary>
        /// Ikke i brug
        /// </summary>
        /// <returns></returns>
        private bool checkDirection() {
            if (trackHistory.Contains(direction) || trainContainsTrack(direction)) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ikke i brug
        /// </summary>
        private void SetDirection() {
            Track tempDirection = currentTrack.getDirection();
            List<Track> invalidDirections = new List<Track>();
            if (trainContainsTrack(tempDirection)) { 
                
            }
        }

        /// <summary>
        /// Gør ikke noget for lokomotiv da den ikke bør blive kaldt her.
        /// </summary>
        /// <param name="loot"></param>
        /// <param name="ahead"></param>
        /// <exception cref="Exception"></exception>
        public override void HandleLoot(int loot, bool ahead)
        {
            throw new Exception("This method should not be called if there is insufficient space");
        }

        public override void LoadContent(ContentManager content)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// I det andet kald fra Update rykker det sig selv og vogne frem så de ikke ligger oven på hinanden
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            if (direction != null && (direction.HasCarriage(this) || currentTrack.HasCarriage(this)))
            {
                SetSpeed(0);
                waiting = true;
            }
            else {
                waiting = false;
            }

            if (!waiting) {
                SetSpeed(100);
            }
            /*else {
                SetSpeed(250);
            }*/
            if (updateCalls == 1)
            {
                if (carriageBehind != null)
                {
                    for (int i = 0; i < trainLength(false); i++)
                    {
                        MoveForward();
                    }
                }
                updateCalls++;
            }
            else if (updateCalls % 5 == 0 && !waiting) {
                updateCalls++;
                ParticleEffect smoke = new ParticleEffect(position, smokeSprites, SpriteEffects.None, 0, 0f, 1, Vector2.Zero);
                GameWorld.InstantiateGameObject(smoke);
            }
            else
            {
                updateCalls++;
            }
            
        }

        /// <summary>
        /// Returnerer hvor mange vogne der er i toget
        /// </summary>
        /// <param name="ahead"></param>
        /// <returns></returns>
        public override int trainLength(bool ahead)
        {
            if (!ahead)
            {
                return carriageBehind.trainLength(ahead);
            }
            else {
                return 0;
            }
        }

        public override GameObject getCopy()
        {
            return new Locomotive(coal, null, owner, position, sprites, spriteEffect, lootValue, smokeSprites);
        }

        public override void TakeDamage(int damage)
        {
            
        }
    }
}
