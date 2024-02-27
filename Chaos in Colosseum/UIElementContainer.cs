using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    
    public class UIElementContainer
    {
        public Vector2 pos;
        public float speed;
        public List<UIElement> subElements;

        public UIElementContainer(Vector2 pos, float speed)
        {
            this.pos = pos;
            this.speed = speed;
            subElements = new List<UIElement>();
        }


    }
}
