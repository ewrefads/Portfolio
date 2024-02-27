using System;

namespace Sneak_and_seek_dungeons.ObserverPattern
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