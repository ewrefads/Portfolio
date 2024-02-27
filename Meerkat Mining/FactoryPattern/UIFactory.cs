using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining.FactoryPattern
{
    public class UIFactory : Factory
    {
        public override GameObject Create(Enum type)
        {
            Vector2 adjust = new Vector2(0, -50);
            GameObject go = new GameObject();
            ShopWindow sR = new ShopWindow();
            sR.Player = Shop.Instance.Player;
            sR.SetSprite("boxTall");
            go.AddComponent(sR);
            go.Transform.Position = new Vector2(960, 340) + adjust;
            GameObject b1 = new GameObject();
            
            BuyButton bb = new BuyButton(PLAYERSTATS.MOVESPEED, Shop.Instance);
            b1.AddComponent(bb);
            SpriteRenderer sR1 = new SpriteRenderer();
            sR1.SetSprite("box");
            sR1.Scale = 0.6f;
            sR1.LayerDepth = 0.2f;
            b1.AddComponent(sR1);
            Collider col = new Collider();
            b1.AddComponent(col);
            col.Start();
            
            b1.Transform.Position = new Vector2(960, 250) + adjust;

            GameWorld.Instance.NewuiObjects.Add(b1);
            sR.BuyButtons.Add(b1);

            GameObject b2 = new GameObject();

            BuyButton bb1 = new BuyButton(PLAYERSTATS.MININGSPEED, Shop.Instance);
            b2.AddComponent(bb1);
            SpriteRenderer sR2 = new SpriteRenderer();
            sR2.SetSprite("box");
            sR2.Scale = 0.6f;
            sR2.LayerDepth = 0.2f;
            b2.AddComponent(sR2);
            Collider col1 = new Collider();
            b2.AddComponent(col1);
            col1.Start();

            b2.Transform.Position = new Vector2(960, 350) + adjust;
            GameWorld.Instance.NewuiObjects.Add(b2);
            sR.BuyButtons.Add(b2);

            GameObject b3 = new GameObject();

            BuyButton bb2 = new BuyButton(PLAYERSTATS.MININGDAMAGE, Shop.Instance);
            b3.AddComponent(bb2);
            SpriteRenderer sR3 = new SpriteRenderer();
            sR3.SetSprite("box");
            sR3.Scale = 0.6f;
            sR3.LayerDepth = 0.2f;
            b3.AddComponent(sR3);
            Collider col2 = new Collider();
            b3.AddComponent(col2);
            col2.Start();

            b3.Transform.Position = new Vector2(960, 450) + adjust;
            GameWorld.Instance.NewuiObjects.Add(b3);
            sR.BuyButtons.Add(b3);
            return go;
        }
    }
}
