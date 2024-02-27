using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sneak_and_seek_dungeons.ObserverPattern
{
    /// <summary>
    /// Clickevent er et event der bliver kaldet når spilleren klikker på et specefikt objekt
    /// </summary>
    internal class ClickEvent : GameEvent
    {
        public void Notify()
        {
            

            base.Notify();
        }
    }
}
