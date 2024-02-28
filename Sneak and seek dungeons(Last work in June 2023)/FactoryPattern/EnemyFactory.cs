using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    // Lucas
    
    // Forskællige typer af enemy
    public enum ENEMYTYPE { ARCHER, GRUNT, SCOUT, BOSS}
    internal class EnemyFactory : Factory
    {
        // Ligesom andre factories er enemy factory en singleton
        private static EnemyFactory instance;

        public static EnemyFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EnemyFactory();
                }
                return instance;
            }
        }

        public override GameObject Create(Enum type)
        {
            GameObject go = new GameObject();

            SpriteRenderer sr = (SpriteRenderer)go.AddComponent(new SpriteRenderer());

            Collider col = (Collider) go.AddComponent(new Collider());
            GameWorld.Instance.Colliders.Add(col);

            switch (type)
            {
                case ENEMYTYPE.ARCHER: // Implimeted

                    //sr.SetSprite("otherSprites/Boss1");
                    sr.SetSprite("Enemies/blueArcher");
                    go.AddComponent(new Enemy(GameWorld.Instance.syncEnemyMovement,ENEMYBEHAVIOUR.PATROL, 50, 60, 10, 250, 500, 160, true));
                    sr.LayerDepth = 0.5f;
                    break;

                case ENEMYTYPE.GRUNT: // Implimeted

                    sr.SetSprite("otherSprites/Boss1");
                    go.AddComponent(new Enemy(GameWorld.Instance.syncEnemyMovement, ENEMYBEHAVIOUR.PATROL, 100, 100, 5, 20, 150, 150, false));
                    sr.LayerDepth = 0.5f;
                    break;

                case ENEMYTYPE.SCOUT: // Not Implimeted (missing ablilty feature)

                    sr.SetSprite("otherSprites/Boss1");
                    go.AddComponent(new Enemy(GameWorld.Instance.syncEnemyMovement, ENEMYBEHAVIOUR.PATROL, 80, 100, 2, 0, 250, 100, true));
                    sr.LayerDepth = 0.5f;

                    break;
                case ENEMYTYPE.BOSS: // Not Implimeted

                    break;

                default:
                    throw new NotImplementedException("Ukendt ENEMYTYPE. Tjek om parametret er blevet skrevet korrekt");
                    
            }


            return go;

            
        }
    }
}
