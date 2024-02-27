using Microsoft.Xna.Framework;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Helmet : Armor
    {
        /// <summary>
        /// hjelm armor
        /// Frederik
        /// </summary>
        public Helmet()
        {
            ItemType = ITEMTYPE.HELMET;
            armor = 10;
        }
        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("helmet");
            sr.Scale = new Vector2(0.8f,0.8f);
            base.Start();
        }
    }
}
