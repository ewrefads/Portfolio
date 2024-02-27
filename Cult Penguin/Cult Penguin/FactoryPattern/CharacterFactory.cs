using Cult_Penguin.Components;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.FactoryPattern
{
    public enum playerType {controlled, other }
    public class CharacterFactory : Factory
    {
        private static CharacterFactory instance;

        public static CharacterFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CharacterFactory();
                }
                return instance;

            }
        }

        private CharacterFactory() { 
        
        }
        public override GameObject Create(Enum type)
        {
            GameObject character = new GameObject();
            Collider  col = (Collider)character.AddComponent(new Collider());
            SpriteRenderer sr = (SpriteRenderer)character.AddComponent(new SpriteRenderer());
            character.AddComponent(new ChatBubble());
            Player player = (Player)character.AddComponent(new Player());
            switch (type) { 
                case playerType.controlled:
                    player.controlled = true;
                    sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("penguin");
                    break;
                case playerType.other:
                    sr.Sprite = GameWorld.Instance.Content.Load<Texture2D>("penguin");
                    break;
            }
            return character;
        }
    }
}
