using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    //Frederik
    internal class SliderValue : Component
    {
        /// <summary>
        /// Opdatere sliderens værdi sprite til at bruge de værdier slideren har
        /// </summary>
        /// <param name="value">slider værdi</param>
        /// <param name="maxValue">slider max værdi</param>
        /// <param name="spr">sliderens spriterenderer</param>
        public void UpdateSliderFill(float value, float maxValue, SpriteRenderer spr)
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;

            //hvor meget plads der er på højre og venstre side af slideren
            float nudge = 0.2f;

            //sætter scalen til en værdi ud fra hvad value variablen er ved brug af Map metoden
            sr.Scale = new Vector2(Map(value,0,maxValue,0,spr.Scale.X - nudge),sr.Scale.Y);
            sr.LayerDepth = 0.8f;
            
            //definere positionen længst til venstre i slideren
            float min = spr.GameObject.Transform.Position.X - (int)(spr.Sprite.Width / 2 * spr.Scale.X);
            //definere positionen i midten af slideren
            float max = spr.GameObject.Transform.Position.X;

            //finder ud af sliderens position ud fra sliderens værdi
            GameObject.Transform.Position = new Vector2(Map(
                value,
                0,
                maxValue,
                min,
                max
                ),spr.GameObject.Transform.Position.Y);
            
        }

        /// <summary>
        /// Map funktion tager en værdi og ændre den fra en range til en anden range 
        /// fex: en værdi på 5 med en range fra 1-10 og en range fra 1-100 så bliver værdien 50
        /// Jeg fandt metoden online på siden https://stackoverflow.com/questions/14353485/how-do-i-map-numbers-in-c-sharp-like-with-map-in-arduino
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fromSource"></param>
        /// <param name="toSource"></param>
        /// <param name="fromTarget"></param>
        /// <param name="toTarget"></param>
        /// <returns></returns>
        public float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }


    }

}
