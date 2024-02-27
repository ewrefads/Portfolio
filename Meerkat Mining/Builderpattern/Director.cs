using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.Builderpattern
{
    public class Director
    {
        private IBuilder builder;

        public Director(IBuilder builder)
        {
            this.builder = builder;
        }

        public GameObject Construct()
        {
            builder.BuildGameObject();

            return builder.GetResult();
        }
    }
}
