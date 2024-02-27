using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.FactoryPattern
{
    public abstract class Factory
    {
        public abstract GameObject Create(Enum type);
    }
}
