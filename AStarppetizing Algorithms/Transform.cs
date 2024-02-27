using Microsoft.Xna.Framework;

namespace AStarppetizing_Algorithms
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