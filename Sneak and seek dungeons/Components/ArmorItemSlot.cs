using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// et itemslot med events når man smider eller tager et item derfra
    /// </summary>
    internal class ArmorItemSlot : ItemSlot
    {
        public ArmorItemSlot(ITEMTYPE type) : base(type)
        {
        }

        public override void ItemDropped()
        {
            base.ItemDropped();
            ((Armor)Item).Equipped();
        }

        public override void ItemPickedUp(Item it)
        {
            base.ItemPickedUp(it);
            ((Armor)Item).UnEquipped();
        }
    }
}
