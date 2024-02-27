using Cult_Penguin.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.FactoryPattern
{
    internal class ErrorboxFactory : Factory
    {
        private static ErrorboxFactory instance;

        private ErrorboxFactory()
        {
        }

        internal static ErrorboxFactory Instance { get {
                if (instance == null) {
                    instance = new ErrorboxFactory(); 
                }
                return instance;
            } 
        }

        public override GameObject Create(Enum type)
        {
            GameObject errorBox = new GameObject();
            SpriteRenderer sr = (SpriteRenderer)errorBox.AddComponent(new SpriteRenderer());
            sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("Button");
            TextField tf = (TextField)errorBox.AddComponent(new TextField());
            return errorBox;
        }
    }
}
