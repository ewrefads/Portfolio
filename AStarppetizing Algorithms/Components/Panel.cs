using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Components
{
    internal class Panel : Component
    {
        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("knap");
            sr.LayerDepth = 0f;
        }
    }
}
