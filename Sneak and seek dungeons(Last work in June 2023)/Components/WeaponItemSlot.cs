using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// et itemslot som kalder på vvåbenskift events når et item bliver droppet eller trukket fra
    /// Frederik
    /// </summary>
    internal class WeaponItemSlot : ItemSlot
    {
        public WeaponItemSlot(ITEMTYPE type) : base(type)
        {
        }

        public override void ItemDropped()
        {
            base.ItemDropped();
            Inventory.Instance.OnWeaponEquip();
        }

        public override void ItemPickedUp(Item it)
        {
            base.ItemPickedUp(it);
            Inventory.Instance.UnEquipWeapon();
        }
    }
}
