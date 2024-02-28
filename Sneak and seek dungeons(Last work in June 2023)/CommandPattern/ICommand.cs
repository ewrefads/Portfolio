using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.CommandPattern
{
    internal interface ICommand
    {
        public void Execute(Player player);
    }
}
