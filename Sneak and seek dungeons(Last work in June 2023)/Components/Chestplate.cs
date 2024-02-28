using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// Chestplate armor
    /// Frederik
    /// </summary>
    internal class Chestplate : Armor
    {
        public Chestplate()
        {
            ItemType = ITEMTYPE.CHESTPLATE;
            armor = 20;
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("PlaceHolder sprites/missingTexture");
            base.Start();
        }
    }
}
