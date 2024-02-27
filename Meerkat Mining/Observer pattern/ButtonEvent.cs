using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meerkat_Mining
{
    public enum BUTTONSTATE { UP,DOWN}
    public class ButtonEvent : GameEvent
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
