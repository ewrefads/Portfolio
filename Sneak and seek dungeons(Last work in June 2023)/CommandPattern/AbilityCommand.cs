using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.CommandPattern
{
    internal class AbilityCommand : ICommand
    {
        private int number;

        public AbilityCommand(int number)
        {
            this.number = number;
        }
        public void Execute(Player player)
        {
            player.NumberInput(number);
        }
    }
}
