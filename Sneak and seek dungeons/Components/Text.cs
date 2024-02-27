using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// En anden approach til at lave tekst elementer i verdnen ved at gøre dem til gameobjekter
    /// Frederik
    /// </summary>
    internal class Text : Component
    {
        private string tekst = "";
        private Color color;
        private SpriteFont font;
        private float scale;
        private Vector2 origin;

        public string Tekst { get => tekst; set => tekst = value; }
        public Color TextColor { get => color; set => color = value; }
        public SpriteFont Font { get => font; set => font = value; }
        public float Scale { get => scale; set => scale = value; }
        public Vector2 Origin { get => origin; set => origin = value; }

        public override void Start()
        {
            font = GameWorld.Instance.defaultFont;
            
            
            
        }
        //teksten har sin egen draw metode da spriterenderer kun tegner billeder
        public override void Draw(SpriteBatch spriteBatch)
        {

            origin = new Vector2(font.MeasureString(tekst).X/2, font.MeasureString(tekst).Y/2);
            spriteBatch.DrawString(font,tekst,GameObject.Transform.Position-font.MeasureString(tekst)/2,color,0,origin,scale,SpriteEffects.None,0.9f);
        }
    }
}
