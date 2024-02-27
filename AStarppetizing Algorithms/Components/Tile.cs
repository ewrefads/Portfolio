using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Components
{
    public class Tile : Component 
    {
        public Vector2 GridPosition { get; set; }
        public bool Walkable { get; set; }
        public bool Collectable { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
