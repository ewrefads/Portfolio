using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AStarppetizing_Algorithms.FactoryPattern
{
    public abstract class Factory
    {
        public abstract GameObject Create(Enum type);
    }
}
