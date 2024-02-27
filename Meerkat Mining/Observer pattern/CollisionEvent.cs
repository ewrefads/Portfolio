using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.Observer_pattern
{
    public class CollisionEvent : GameEvent
    {
        public GameObject Other { get; set; }

        public void Notify(GameObject other)
        {
            this.Other = other;

            base.Notify();
        }

    }
}
