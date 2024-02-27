using AStarppetizing_Algorithms.Components;
using Microsoft.Xna.Framework;
using System;


namespace AStarppetizing_Algorithms.FactoryPattern
{
    enum UITYPE {BUTTON,MENUBUTTON,IMAGE };
    internal class UIFactory : Factory
    {
        private static UIFactory instance;

        public static UIFactory Instance
        {
            get {
                    if (instance == null)
                    {
                        instance = new UIFactory();
                    }
                return instance; 
            
                }
        }
        public override GameObject Create(Enum type)
        {
            GameObject go = new GameObject();
            SpriteRenderer sr = (SpriteRenderer)go.AddComponent(new SpriteRenderer());


            if (type is UITYPE.BUTTON)
            {
                //button stuff her
                Button butt = (Button)go.AddComponent(new Button());

                sr.SetSprite("knap");
                Collider c = (Collider)go.AddComponent(new Collider());
                GameWorld.Instance.Colliders.Add(c);
                butt.SetSprite();
                //sr.LayerDepth = 1;
            }
            if (type is UITYPE.MENUBUTTON)
            {

                Button butt = (Button)go.AddComponent(new Button());

                sr.SetSprite("knap");
                sr.LayerDepth = 1;
                go.AddComponent(new Collider());

                butt.SetSprite();
                //sr.LayerDepth = 1;
            }
            else if (type is UITYPE.IMAGE)
            {
                //image stuff her
            }
            

            return go;
        }
    }
}
