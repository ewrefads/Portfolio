using Cult_Penguin.Components;
using Microsoft.Xna.Framework;

namespace Cult_Penguin.CommandPattern
{
    internal class MoveCommand : ICommand
    {
        private Vector2 velocity;

        public MoveCommand(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void Execute(Player player)
        {
            player.ChangeVelocity(velocity);
        }
    }
}