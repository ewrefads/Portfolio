using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.FactoryPattern
{
    public abstract class Factory
    {
        public abstract GameObject Create(Enum type);
    }
}
