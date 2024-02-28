using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// en overklasse til alt rustning med general funktionalitet
    /// Frederik
    /// </summary>
    internal class Armor : Item
    {
        protected float armor;
        private Player player;

        public override void Start()
        {
            player = GameWorld.Instance.FindObjectOfType<Player>() as Player;
            hasInfiniteUses = true;
            base.Start();
        }
        public void Equipped()
        {
            player.Armor += armor;
        }

        public void UnEquipped()
        {
            player.Armor -= armor;
        }
    }
}
