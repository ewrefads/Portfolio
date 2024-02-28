using Cult_Penguin.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.CommandPattern
{
    internal interface ICommand
    {
        public void Execute(Player player);
    }
}
