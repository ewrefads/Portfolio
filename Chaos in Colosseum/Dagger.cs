using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    internal class Dagger : Weapon
    {
        /// <summary>
        /// Dagger giver våben stats som damage og range samt tegner texturen med et custom origin point
        /// </summary>
        public Dagger(string[] name, Vector2 pos) : base(name, pos, 0,0)
        {
            //sætter daggers værdier
            scale = 7f;
            damage = 2;
            animationSpeed = 2;
            range = 150;
        }


        public override void Draw(SpriteBatch _spriteBatch)
        {
            //sætter origin pointet nede i venstre hjørne af Texturen så swing animationen ser mere naturlig ud
            Vector2 origin = new Vector2(0, sprites[0].Height);

            _spriteBatch.Draw(sprites[0], position, null, Microsoft.Xna.Framework.Color.White, rotation, origin, scale, SpriteEffects.None, 0);

        }
    }
}
