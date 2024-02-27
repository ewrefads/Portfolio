using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    internal interface IBuilding
    {

        internal void Activate();
        internal void DeActivate();
        internal void Interact();
    }
}
