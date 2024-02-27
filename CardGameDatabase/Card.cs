using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGameDatabase
{
    internal class Card
    {
        private int health;
        private int damage;
        private string name;

        public int Health { get => health; set => health = value; }
        public int Damage { get => damage; set => damage = value; }
        public string Name { get => name; set => name = value; }

        public override string ToString()
        {
            return $"{name}|health: {health}| damage: {damage}";
        }
    }
}
