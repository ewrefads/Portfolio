using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.FactoryPattern
{
    public enum BLOCKTYPE { AIR, BAUXITE, BORDER, CINNABAR, GOLD, GRASS, STONE, IRON , IRIDIUM, PYRITE}

    public class BlockFactory : Factory
    {

        private static BlockFactory instance;

        public static BlockFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BlockFactory();

                }

                return instance;
            }
        }




        public override GameObject Create(Enum type)
        {
            Random rnd = new Random();
            int choice = rnd.Next(1, 6);

            GameObject obj = new GameObject();

            SpriteRenderer spr = (SpriteRenderer)obj.AddComponent(new SpriteRenderer());
            spr.Scale = 2f;
            spr.Color = Color.Black;
            spr.LayerDepth = 1;

            Block b =(Block) obj.AddComponent(new Block());
            b.CanBeMined = true;
            b.StartingScale =2;

            obj.AddComponent(new Collider());

            



            switch (type)
            {
                case BLOCKTYPE.AIR:

                    break;
                case BLOCKTYPE.BORDER:
                    spr.SetSprite($"block/border/border{choice}");
                    b.CanBeMined = false;
                    b.Type = BLOCKTYPE.BORDER;
                    spr.Color = Color.White;
                    break;
                case BLOCKTYPE.BAUXITE:
                    spr.SetSprite($"block/bauxite/bauxite{choice}");
                    b.Type = BLOCKTYPE.BAUXITE;
                    b.Hp = 50;
                    break;
                case BLOCKTYPE.CINNABAR:
                    spr.SetSprite($"block/cinnabar/cinnabar{choice}");
                    b.Type = BLOCKTYPE.CINNABAR;
                    b.Hp = 60;
                    break;
                case BLOCKTYPE.GOLD:
                    spr.SetSprite($"block/gold/gold{choice}");
                    b.Type = BLOCKTYPE.GOLD;
                    b.Hp = 100;
                    break;
                case BLOCKTYPE.GRASS:
                    choice = rnd.Next(1, 3);
                    spr.SetSprite($"block/grass/grass{choice}");
                    spr.Color = Color.White;
                    b.Hp = 1;
                    break;
                case BLOCKTYPE.IRIDIUM:
                    spr.SetSprite($"block/iridium/iridium{choice}");
                    b.Type = BLOCKTYPE.IRIDIUM;
                    b.Hp = 150;
                    break;
                case BLOCKTYPE.IRON:
                    spr.SetSprite($"block/iron/iron{choice}");
                    b.Type = BLOCKTYPE.IRON;
                    b.Hp = 90;
                    break;
                case BLOCKTYPE.PYRITE:
                    spr.SetSprite($"block/pyrite/pyrite{choice}");
                    b.Type = BLOCKTYPE.PYRITE;
                    b.Hp = 70;
                    break;
                case BLOCKTYPE.STONE:
                    spr.SetSprite($"block/stone/stone{choice}");
                    b.Type = BLOCKTYPE.STONE;
                    b.Hp = 40;
                    break;

                default:
                    break;
            }


            return obj;
        }
    }
}
