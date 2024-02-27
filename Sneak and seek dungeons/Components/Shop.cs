using Microsoft.Xna.Framework;
using Sneak_and_seek_dungeons.FactoryPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    internal class Shop : Component, IBuilding
    {
        private Text text;

        public Text Text
        {
            get
            {
                if (text == null)
                {
                    text = (UIFactory.Instance.Create(UITYPE.TEXT)).GetComponent<Text>() as Text;
                }
                return text;
            }
        }

        public override void Start()
        {
            SpriteRenderer sr = GameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
            sr.SetSprite("bank");
            sr.Scale = new Vector2(10f, 10f);

            //text = (UIFactory.Instance.Create(UITYPE.TEXT)).GetComponent<Text>() as Text;
            GameWorld.Instance.Instantiate(text.GameObject);
            text.TextColor = Color.White;
            text.Scale = 5;
            text.Tekst = "       SHOP \n press 'SPACE' to enter";
            text.GameObject.Transform.Position = GameObject.Transform.Position + new Vector2(0,-300);
            text.GameObject.Enabled = false;
        }
        void IBuilding.Activate()
        {
            Text.GameObject.Enabled = true;
        }

        void IBuilding.DeActivate()
        {
            Text.GameObject.Enabled = false;
        }

        void IBuilding.Interact()
        {
            
        }
    }
}
