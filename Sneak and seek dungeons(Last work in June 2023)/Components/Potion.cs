using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// Et item som spilleren kan få og aktivere hvis den er i hotbaren
    /// Frederik
    /// </summary>
    internal class Potion : Item
    {
        public Potion()
        {
            itemType = ITEMTYPE.POTION;
        }

        public override void Activate()
        {
            Player player = GameWorld.Instance.FindObjectOfType<Player>() as Player;
            player.OnHit(-10);

            base.Activate();
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("Sprites/Potions/Resistance");
            sr.Scale = new Vector2(2f, 2f);
            Uses = 1;
            base.Start();

        }
    }
}
