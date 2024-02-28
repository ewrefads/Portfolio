using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum INGREDIENTTYPE {
Cheese,
Cucumber,
Egg,
Flour,
Milk,
Sugar,
Tomato
}

namespace AStarppetizing_Algorithms.Components
{
    internal class Ingredient : Tile
    {
        public INGREDIENTTYPE ingredientType;


        public Ingredient(INGREDIENTTYPE type)
        {
            this.ingredientType = type;
        }
    }
}
