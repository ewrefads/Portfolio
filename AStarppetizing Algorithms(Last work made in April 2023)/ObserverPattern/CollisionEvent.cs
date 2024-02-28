using System;

namespace AStarppetizing_Algorithms.ObserverPattern
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