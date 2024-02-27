using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos_in_Colosseum
{
    public class UIElement
    {
        private List<UIElement> subElements;
        private string[] subElementAssets;
        private string[] spriteAssets;
        private Vector2 pos;
        private float speed;
        private Actor owner;
        private Texture2D[] sprites;
        private Vector2 velocity;
        private int numSubElements;

        public UIElement(string[] names, Vector2 pos, float speed, Actor owner, int numSubElements, string[] subElementAssets, Vector2 offset)
        {
            subElements = new List<UIElement>();
            this.spriteAssets = names;
            sprites = new Texture2D[spriteAssets.Length];
            this.pos = pos;
            this.speed = speed;
            this.owner = owner;
            this.subElementAssets = subElementAssets;
            this.numSubElements = numSubElements;
        }

        public UIElement(Texture2D[] sprites, Vector2 pos, float speed)
        {
            subElements = new List<UIElement>();
            this.sprites = sprites;
            this.pos = pos;
            this.speed = speed;
        }

        public void AddUIElement(UIElement subElement) {
            subElements.Add(subElement);
        }

        public void RemoveLastUIElement() {
            subElements.RemoveAt(subElements.Count - 1);
        }

        public void LoadContent(ContentManager content)
        {
            for (int i = 0; i < spriteAssets.Length; i++)
            {
                sprites[i] = content.Load<Texture2D>(spriteAssets[i]);
            }
            for (int i = 0; i < numSubElements; i++) { 
                //subElements.Add(new UIElement())
            }
            //sprite = content.Load<Texture2D>(spriteName);
        }

        public void updateVelocity(Vector2 velocity) {
            this.velocity = velocity;

        }

        public void Update(GameTime gameTime, Vector2 newPos)
        {
            pos = new Vector2(newPos.X - 45, newPos.Y - 80);
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(sprites[0], pos, Color.White);
        }
    }
}