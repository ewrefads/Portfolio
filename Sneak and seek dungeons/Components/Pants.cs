using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// bukse armor
    /// Frederik
    /// </summary>
    internal class Pants : Armor
    {
        public Pants()
        {
            ItemType = ITEMTYPE.PANTS;
            armor = 15;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("PlaceHolder sprites/missingTexture");
            base.Start();
        }
    }
}
