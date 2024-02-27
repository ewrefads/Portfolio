using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meerkat_Mining
{
    public class MoveCommand : ICommand
    {
        private Vector2 velocity;

        public MoveCommand(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void Execute(Player player)
        {
            player.Move(velocity);
        }

        public void Execute(Drill drill)
        {
            
        }
    }
}
