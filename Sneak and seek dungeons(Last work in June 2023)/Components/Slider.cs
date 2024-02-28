using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// En bar som går op og ned efter en værdi
    /// Frederik
    /// </summary>
    internal class Slider : Component
    {
        private float value;
        private float maxValue;
        public SliderValue Slidervalue;
        public Text sliderText;

        public float Value { get => value; set => this.value = value; }
        public float MaxValue { get => maxValue; set => maxValue = value; }

        /// <summary>
        /// opsætter sliderens start værdier og referencer
        /// </summary>
        /// 

        public override void Awake()
        {
            ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(GameObject);
            ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(Slidervalue.GameObject);
            ((GameWorld.Instance.FindObjectOfType<Player>()).GameObject.GetComponent<Player>() as Player).PlayerUI.Add(sliderText.GameObject);
        }
        public override void Start()
        {

            //sætter slider fyldet til sliderens position
            //GameObject.Transform.Position -= GameWorld.ScreenSize / 2 + Vector2.One*-100;
            
            Slidervalue.GameObject.Transform.Position = GameObject.Transform.Position;
            SpriteRenderer sr = (SpriteRenderer) GameObject.GetComponent<SpriteRenderer>();
            sr.LayerDepth = 0.7f;
            UpdateSlider();

            
        }

        public void SetupText()
        {
            GameObject go = UIFactory.Instance.Create(UITYPE.TEXT);
            sliderText = go.GetComponent<Text>() as Text;

            //sliderText.Origin = GameObject.Transform.Position;
            go.Transform.Position = GameObject.Transform.Position + new Vector2(20,10) ;
            sliderText.Scale = 4;
            sliderText.TextColor = Color.White;
            

            GameWorld.Instance.NewGameObjects.Add(go);
        }

        //kan kaldes når sliderens værdi skal opdateres
        public void UpdateSlider()
        {
            
            sliderText.Tekst = $" {Math.Round(value)}/{maxValue}";
            Slidervalue.UpdateSliderFill(value, maxValue, GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer);
        }
    }
}
