using AStarppetizing_Algorithms.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Builder
{
    /// <summary>
    /// Level er en underklasse til levelbuilder og bruges til at opstille levels inde i gameworld
    /// </summary>
    public class Level : LevelBuilder
    {
        
        /// <summary>
        /// tager information om hvor der skal være ting henne på griddet
        /// </summary>
        /// <param name="dimensions">hvor stor skal griddet være</param>
        /// <param name="obstacles">hvor skal obstacles være</param>
        /// <param name="ingredients">hvor skal ingredients være</param>
        /// <param name="ingredientTypes">hvad for nogen ingredients skal det være</param>
        public Level(Vector2 dimensions, Vector2[] obstacles, Vector2[] ingredients, INGREDIENTTYPE[] ingredientTypes)
        {
            this.dimensions = dimensions;
            this.obstacles = obstacles;
            this.ingredients = ingredients;
            this.ingredientTypes = ingredientTypes;

            ingredientIndex = 0;
            tileSpacing = 32;


            tiles = new GameObject[(int)dimensions.X, (int)dimensions.Y];
            t = new Tile[(int)dimensions.X, (int)dimensions.Y];


            //sætter griddet op
            SetupFloor();
            ConstructLevel1();
            SetupLevel();
            

        }

        

        


    }
}
