using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TrainTD
{
    /// <summary>
    /// En tog vogn der kan holde på på ting plyndret fra modstanderens by
    /// </summary>
    internal class CargoCarriage : TrainCarriage
    {
        int storageCapacity;
        int currentStorage;

        

        public CargoCarriage(TrainCarriage carrigeAhead, Player owner, Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue) : base(carrigeAhead, owner, position, sprites, spriteEffect, lootValue, 20)
        {
        }


        /// <summary>
        /// Tjekker om vognen har plads til noget af det loot som skal placeres i toget.
        /// </summary>
        /// <param name="ahead">Hvorvidt tjekket er startet foran eller bagved vognen</param>
        /// <param name="amount">Hvor meget loot der skal fordeles</param>
        /// <returns></returns>
        public override bool ConnectedToEmptyCargoCapacity(bool ahead, int amount)
        {
            if (currentStorage + amount < storageCapacity)
            {
                return true;
            }
            else if (ahead && carriageAhead != null)
            {
                return carriageAhead.ConnectedToEmptyCargoCapacity(ahead, currentStorage + amount - storageCapacity);
            }
            else if (carriageBehind != null)
            {
                return carriageBehind.ConnectedToEmptyCargoCapacity(ahead, currentStorage + amount - storageCapacity);
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Hvorvidt vognen er forbundet til et lokomotive
        /// </summary>
        /// <returns></returns>
        public override bool ConnectedToLocomotive()
        {
            if (carriageAhead != null)
            {
                return carriageAhead.ConnectedToLocomotive();
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Ikke implementeret
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override GameObject getCopy()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fordeler det loot der kan være i vognen og sender resten videre
        /// </summary>
        /// <param name="loot">Resterende mængde der skal fordeles i vognen</param>
        /// <param name="ahead">Retningnen resten skal sendes videre</param>
        public override void HandleLoot(int loot, bool ahead)
        {
            if (currentStorage + loot <= storageCapacity)
            {
                currentStorage += loot;
            }
            else if (carriageAhead != null && ahead)
            {
                currentStorage = storageCapacity;
                carriageAhead.HandleLoot(currentStorage + loot - storageCapacity, ahead);
            }
            else if (carriageBehind != null) {
                currentStorage = storageCapacity;
                carriageBehind.HandleLoot(currentStorage + loot - storageCapacity, ahead);
            }
        }

        /// <summary>
        /// Ikke implementeret
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Mængden der returneres når vognen bliver ødelagt
        /// </summary>
        /// <returns></returns>
        public override int lootFromObject()
        {
            return currentStorage + lootValue;
        }

        /// <summary>
        /// Gør ikke noget specifikt ud over hvad alle trainCarriages gør 
        /// </summary>
        /// <param name="gameTime">Nuværende GameTime fra GameWorld</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
