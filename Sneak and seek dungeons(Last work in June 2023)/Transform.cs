using Microsoft.Xna.Framework;

namespace Sneak_and_seek_dungeons
{
    public class Transform
    {
        private Vector2 position;

        public Vector2 Position { get => position; set => position = value; }

        public void Translate(Vector2 translation)
        {
            Position += translation;
        }
    }
}