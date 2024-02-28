using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTD
{
    internal class Descriptionbox : UIElement
    {
        public string[] text;
        public SpriteFont font;
        public int towerLevel;

        public Descriptionbox(Player activePlayer, Texture2D[] sprites, Color activeColor, Color hoverColor, Color clickColor, Color inActiveColor, Vector2 position, float animationTime) : base(activePlayer, sprites, activeColor, hoverColor, clickColor, inActiveColor, position, animationTime)
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            base.HandleInput();
        }

        public override void Draw(SpriteBatch _spriteBatch)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Vector2 textPos = new Vector2(-7 * text.Length -70, -25*i + 50) + spriteSize / 2 + position;
                Color col = Color.Wheat;

                if (towerLevel>i)
                {
                    col = Color.Green;
                } else if (towerLevel == i)
                {
                    col = Color.Gray;
                }

                _spriteBatch.DrawString(font, text[i], textPos, col * opacity, 0f, Vector2.Zero, 1, SpriteEffects.None, 1);
            }

            //tegner knappen
            base.Draw(_spriteBatch);
        }

        //viser knappen ved at justere opacity
        public void Show()
        {
            opacity = 1;
            
        }

        //gemmer knappen ved at justere opacity
        public void Hide()
        {
            opacity = 0;
            
        }
    }
}
