using Meerkat_Mining.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.CommandPattern
{
    public class DrillCommand : ICommand
    {
        public void Execute(Player player)
        {
            
        }

        public void Execute(Drill drill)
        {
            drill.Mine();
        }
    }
}
