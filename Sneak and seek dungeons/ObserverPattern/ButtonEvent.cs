using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.ObserverPattern
{
    public enum BUTTONSTATE { UP, DOWN }
    internal class ButtonEvent : GameEvent
    {
        public Keys Key { get; private set; }

        public BUTTONSTATE State { get; private set; }

        public void Notify(Keys key, BUTTONSTATE state)
        {
            this.Key = key;
            this.State = state;
            base.Notify();
        }

        
    }
}
