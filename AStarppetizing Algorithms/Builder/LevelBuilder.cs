using AStarppetizing_Algorithms.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AStarppetizing_Algorithms.Builder
{
    /// <summary>
    /// Levelbuilder er en overklasse til Level som har funktioner til at sætte et grid op med obstacles og ingredients
    /// </summary>
    public abstract class LevelBuilder
    {
        //De tile gameobjects der skal spawnes ind i gameworld
        public GameObject[,] tiles;
        //Alle tile gameobjects Tile components
        public Tile[,] t;
        //hvor meget plads der er mellem hvert tal
        protected int tileSpacing;

        //hvor mange tiles er der i griddet
        protected Vector2 dimensions;
        //positionerne for hvor obstacles i griddet skal være
        protected Vector2[] obstacles;
        //positionerne for hvor ingredients i griddet skal være
        protected Vector2[] ingredients;
        //de forskellige typer af ingredienser som kan spawnes ind som tomat, ost og agurk
        protected INGREDIENTTYPE[] ingredientTypes;
        //holder styr på hvilken ingrediens vi er nået til
        protected int ingredientIndex;
        //spritestørrelsen på tiles
        protected float scale = 1.5f;


        /// <summary>
        /// Laver et gulv som har et sort hvidt skak pattern
        /// </summary>
        public void SetupFloor()
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    tiles[x, y] = new GameObject();
                    t[x,y] = (Tile) tiles[x, y].AddComponent(new Floor());
                    SpriteRenderer sr = (SpriteRenderer)tiles[x, y].AddComponent(new SpriteRenderer());
                    tiles[x, y].AddComponent(new Collider());
                    
                    sr.Scale = scale;

                    if (x%2==0 && y%2==0 || x%2==1 && y%2==1)
                    {
                        sr.SetSprite("tilefloor1");
                    }
                    else { sr.SetSprite("tilefloor2"); }
                }
            }
        }

        /// <summary>
        /// sætter obstacles og ingredients på banen hvis der skal være en på den givne position
        /// </summary>
        public void ConstructLevel1()
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    if (TileContainsObstacle(x, y))
                    {
                        tiles[x, y] = new GameObject();
                        t[x, y] = (Tile)tiles[x, y].AddComponent(new Obstacle());
                        
                        SpriteRenderer sr = (SpriteRenderer)tiles[x, y].AddComponent(new SpriteRenderer());
                        tiles[x, y].AddComponent(new Collider());
                        sr.SetSprite("Table");

                        sr.Scale = 1.5f;

                    }
                    else if (TileContainsIngredient(x, y))
                    {
                        tiles[x, y] = new GameObject();
                        t[x, y] = (Tile)tiles[x, y].AddComponent(new Ingredient(ingredientTypes[ingredientIndex]));
                        SpriteRenderer sr = (SpriteRenderer)tiles[x, y].AddComponent(new SpriteRenderer());
                        tiles[x, y].AddComponent(new Collider());
                        sr.SetSprite(ingredientTypes[ingredientIndex].ToString());

                        sr.Scale = scale;



                        if (ingredientIndex < ingredientTypes.Length) {
                            ingredientIndex++;
                        }
                        
                    }
                }
            }
        }

        /// <summary>
        /// Placere alle de tiles som er blevet genered i en grid formation og fortæller hvert tile deres position
        /// </summary>
        public void SetupLevel()
        {
            for (int y = 0; y < dimensions.Y; y++)
            {
                for (int x = 0; x < dimensions.X; x++)
                {
                    SpriteRenderer sr = (SpriteRenderer)tiles[x, y].GetComponent<SpriteRenderer>();
                    tiles[x, y].Transform.Position = new Vector2(x * tileSpacing * sr.Scale + 250, y * tileSpacing * sr.Scale + 34);
                    t[x, y].GridPosition = new Vector2(x,y);
                }
            }
        }


        /// <summary>
        /// Finder ud af om der skal være en obstacle på en position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>returnere true hvis der skal være en obstacle og false hvis ikke</returns>
        public bool TileContainsObstacle(int x, int y)
        {
            for (int i = 0; i < obstacles.Length; i++)
            {
                if (x == obstacles[i].X && y == obstacles[i].Y)
                {
                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Finder ud af om der skal være en ingredient på en position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>returnere true hvis der skal være en ingredient og false hvis ikke</returns>
        public bool TileContainsIngredient(int x, int y)
        {
            for (int i = 0; i < ingredients.Length; i++)
            {
                if (x == ingredients[i].X && y == ingredients[i].Y)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
