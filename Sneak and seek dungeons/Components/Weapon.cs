using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// En parent klasse til alle våben med general funktionalitet
    /// Frederik
    /// </summary>
    internal abstract class Weapon : Item
    {
        protected int damage;
        protected float attackSpeed;
        protected Player player;

        private bool isEquipped;

        private WEAPONTYPE weaponType;

        internal bool IsEquipped { get => isEquipped; set => isEquipped = value; }
        internal WEAPONTYPE WeaponType { get => weaponType; set => weaponType = value; }

        public abstract void Attack();

        public override void Start()
        {
            player = GameWorld.Instance.FindObjectOfType<Player>() as Player;
            ItemType = ITEMTYPE.WEAPON;
            hasInfiniteUses = true;
            
            base.Start();
        }
        public override void Update()
        {
            FollowPlayer();
            base.Update();
        }

        //de equipped våben følger altid spilleren
        private void FollowPlayer()
        {
            if (!IsInInventory) {
                GameObject.Transform.Position = player.GameObject.Transform.Position;
            }
        }

        

    }
}
