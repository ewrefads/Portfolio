using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.FactoryPattern
{
    public class ShopFactory : Factory
    {
        public override GameObject Create(Enum type)
        {
            GameObject shop = new GameObject();
            SpriteRenderer spriteRenderer = new SpriteRenderer();
            Collider collider = new Collider();
            shop.AddComponent(collider);
            spriteRenderer.SetSprite("Shop/Shop");
            spriteRenderer.Scale = 2f;

            spriteRenderer.Color = Color.White;

            shop.AddComponent(spriteRenderer);
            shop.AddComponent(Shop.Instance);
            
            shop.Transform.Position = new Vector2(960, 670);

            

            return shop;
        }
    }
}
