using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.FactoryPattern
{
    internal enum UITYPE { BUTTON, IMAGE, SLIDER, TEXT };

    /// <summary>
    /// UI factory så der hurtigt kan laves ui elementer under runtime. Der er ingen der bliver spawnet under runtime indtil videre men ellers en god ide
    /// Frederik
    /// </summary>
    internal class UIFactory : Factory
    {
        private static UIFactory instance;

        public static UIFactory Instance
        {
            get
            {
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

                sr.SetSprite("button");
                go.AddComponent(new Collider());

                butt.SetSprite();
                sr.Scale = new Vector2(2, 2);
                sr.LayerDepth = 0.8f;
            }
            else if (type is UITYPE.IMAGE)
            {
                //image stuff her
            }
            else if(type is UITYPE.SLIDER)
            {
                sr.SetSprite("otherSprites/HealthBar");
                Slider s = (Slider) go.AddComponent(new Slider());
                

                GameObject go2 = new GameObject();
                s.Slidervalue = (SliderValue) go2.AddComponent(new SliderValue());
                go2.AddComponent(new SpriteRenderer());
                GameWorld.Instance.NewGameObjects.Add(go2);
                sr.Scale = new Vector2(2,5);

                sr = go2.GetComponent<SpriteRenderer>() as SpriteRenderer;
                sr.SetSprite("otherSprites/HealthBarElement");
                sr.Scale = new Vector2(2,5);
                sr.LayerDepth = 0.2f;
                
            }
            else if (type is UITYPE.TEXT)
            {
                go.RemoveComponent(sr);
                go.AddComponent(new Text());
            }


            return go;
        }
    }
}
