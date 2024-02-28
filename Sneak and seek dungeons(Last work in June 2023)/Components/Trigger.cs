using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    enum CONNECTEDOBJECT {DOOR, TRAP }
    enum TRIGGERTYPE {INTERACTABLE,  PROXIMITY}

    internal class Trigger : Component, IInteractable
    {
        Component connectedObject;
        bool isTriggered = false;
        CONNECTEDOBJECT objectType;
        TRIGGERTYPE triggerType;
        int triggerRadius;

        public Component ConnectedObject { get => connectedObject; set => connectedObject = value; }
        public int TriggerRadius { get => triggerRadius; set => triggerRadius = value; }
        internal CONNECTEDOBJECT ObjectType { get => objectType; set => objectType = value; }
        internal TRIGGERTYPE TriggerType { get => triggerType; set => triggerType = value; }

        public void Interact()
        {
            if (TriggerType is TRIGGERTYPE.INTERACTABLE && !isTriggered)
            {
                isTriggered = true;
            }
            else if (TriggerType is TRIGGERTYPE.INTERACTABLE) {
                isTriggered = false;
            }
        }

        public override void Update()
        {
            if (IsTriggered()) {
                Execute();
            }
            base.Update();
        }

        private void Execute()
        {
            if (ObjectType == CONNECTEDOBJECT.DOOR) {
                Door d = (Door)connectedObject;
                if (d.Locked)
                {
                    d.Unlock();
                }
                else {
                    d.Lock();
                }
                d.Interact();
                isTriggered = false;
            }
        }

        private bool IsTriggered()
        {
            Vector2 pos = GameObject.Transform.Position;
            if (TriggerType == TRIGGERTYPE.INTERACTABLE && isTriggered)
            {
                return true;
            }
            else if (TriggerType == TRIGGERTYPE.PROXIMITY && Vector2.Distance(pos, GameWorld.Instance.Player.Transform.Position) <= TriggerRadius) {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
