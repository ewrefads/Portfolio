using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TrainTD
{
    /// <summary>
    /// En togvogn der kan give skade(ikke implementeret)
    /// </summary>
    internal class WeaponsCarriage : TrainCarriage
    {
        public WeaponsCarriage(TrainCarriage carrigeAhead, Player owner, Vector2 position, Texture2D[] sprites, SpriteEffects spriteEffect, int lootValue) : base(carrigeAhead, owner, position, sprites, spriteEffect, lootValue, 20)
        {
        }

        public override bool ConnectedToEmptyCargoCapacity(bool ahead, int amount)
        {
            if (ahead && carriageAhead != null)
            {
                return carriageAhead.ConnectedToEmptyCargoCapacity(ahead, amount);
            }
            else if (carriageBehind != null) {
                return carriageBehind.ConnectedToEmptyCargoCapacity(ahead, amount);
            }
            else
            {
                return false;
            }
        }

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

        public override GameObject getCopy()
        {
            throw new NotImplementedException();
        }

        public override void HandleLoot(int loot, bool ahead)
        {
            if (carriageAhead != null && ahead)
            {
                carriageAhead.HandleLoot(loot, ahead);
            }
            else if (carriageBehind != null)
            {
                carriageBehind.HandleLoot(loot, ahead);
            }
            else {
                throw new Exception("This method should not be called if there is insufficient space");
            }
        }

        public override void LoadContent(ContentManager content)
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
