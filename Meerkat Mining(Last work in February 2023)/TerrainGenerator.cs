using Meerkat_Mining.Components;
using Meerkat_Mining.FactoryPattern;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class TerrainGenerator
    {
        // Størrelse af terræn. sizeW (size width) er mængden af blocks som bredde. sizeH (size height) er mængden af blocks som højden.

        Random rnd = new Random();
        public int sizeW, sizeH;
        private Vector2 spawnPos;
        private int numb;



        // Et 2d array af blocks som GameObject
        GameObject[,] blocks;

        /// <summary>
        /// TerrainGenerator som singleton
        /// </summary>
        private static TerrainGenerator instance;

        public static TerrainGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TerrainGenerator();
                }

                return instance;
            }
        }

        

        /// <summary>
        /// En metode som definerer størrelsen af blocks 2d array og fylder den med GameObjects lavet af BlockFactory.
        /// </summary>
        /// <returns></returns>
        public GameObject[,] Create(int width, int height)
        {
            sizeW = width;
            sizeH = height;

            blocks = new GameObject[sizeW,sizeH];

            for (int h = 0; h < sizeH; h++)
            {
                for (int w = 0; w < sizeW; w++)
                {

                    GameObject obj = new GameObject();


                    // Mangler collider
                    if (h == 0)
                    {
                        obj = BlockFactory.Instance.Create(BLOCKTYPE.GRASS);
                    }
                    else if (w == 0 || w == sizeW-1)
                    {
                        obj = BlockFactory.Instance.Create(BLOCKTYPE.BORDER);
                    }
                    else if (h == sizeH-1)
                    {
                        obj = BlockFactory.Instance.Create(BLOCKTYPE.BORDER);
                    } 
                    else
                    {
                        numb = rnd.Next(0, 101);

                        if (numb >= 30) // 70% for stone
                        {
                            obj = BlockFactory.Instance.Create(BLOCKTYPE.STONE);
                        }
                        if (numb < 30 && numb >= 22) // 18% for common
                        {
                            obj = BlockFactory.Instance.Create(BLOCKTYPE.IRON);
                        }
                        if (numb < 22 && numb >= 10) // 12% for uncommon
                        {
                            obj = BlockFactory.Instance.Create(BLOCKTYPE.CINNABAR);
                        }
                        if (numb < 10 && numb >= 3) // 7% for rare
                        {
                            obj = BlockFactory.Instance.Create(BLOCKTYPE.GOLD);
                        }
                        if (numb < 3 && numb >= 0) // 3% for extraordianry
                        {
                            obj = BlockFactory.Instance.Create(BLOCKTYPE.IRIDIUM);
                        }

                    }


                    obj.Transform.Position = new Vector2(16+(w*32),700+(h*32));

                    // Smid obj i array
                    blocks[w, h] = obj;
                }
            }

            return blocks;

        }

        




        /*
        List<float> probability = new List<float>();
        List<Block> blockTypes = new List<Block>();
        public void setupBlocks()
        {
            blockTypes.Add(new Block());
        }
        
        public void choseBlock()
        {
            for (int i = 0; i < length; i++)
            {

            }
        }*/

    }
}
