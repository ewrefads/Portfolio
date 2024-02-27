using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Room : Component
    {
        private Rectangle roomBox;

        public Rectangle RoomBox { get => roomBox; set => roomBox = value; }
    }
}
