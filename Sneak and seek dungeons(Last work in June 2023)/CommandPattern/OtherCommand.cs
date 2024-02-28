using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.CommandPattern
{
    /// <summary>
    /// andre commands der kan ske fx F,R,T,TAB eller andet
    /// Frederik
    /// </summary>
    internal class OtherCommand : ICommand
    {
        public void Execute(Player player)
        {
            player.PlayerInput();
        }
    }
}
