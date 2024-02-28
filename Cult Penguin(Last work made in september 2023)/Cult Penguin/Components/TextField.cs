using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cult_Penguin.Components
{
    internal class TextField : Component
    {
        private string text;
        private Vector2 textLength;
        private Texture2D sprite;
        private Color color = Color.Black;
        private Vector2 textScale = new Vector2(1.5f, 1.5f);
        private bool shaking = false;
        private float remainingTime;
        private float shakeMod = 0.005f;

        public string Text { get => text; set {
                text = value;
                textLength = GameWorld.Instance.defaultFont.MeasureString(text);
                SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                if (sprite == null) {
                    sprite = sr.Sprite;
                }
                if (textLength.X * textScale.X > sprite.Width * sr.Scale.X * 0.9f) {
                    float newLength = textLength.X * 1.5f * textScale.X;
                    float newScale = newLength / sprite.Width;
                    sr.Scale = new Vector2(newScale, sr.Scale.Y);
                }
                if (textLength.Y * textScale.Y > sprite.Height * sr.Scale.Y * 0.9f) {
                    float newHeight = textLength.Y * 2f * textScale.Y;
                    float newScale = newHeight / sprite.Height;
                    sr.Scale = new Vector2(sr.Scale.X, newScale);
                }
            } 
        
        }

        public Color Color { get => color; set => color = value; }
        public bool Shaking { get => shaking; set {
                shaking = value;
                remainingTime = 0.5f;
                SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                sr.Rotation = 0.025f;
            } 
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(textLength.X / 2, textLength.Y / 2);
            spriteBatch.DrawString(GameWorld.Instance.defaultFont, text, GameObject.Transform.Position, color, 0, origin, textScale, SpriteEffects.None, 0.9f);
            base.Draw(spriteBatch);
        }

        public override void Update()
        {
            if (shaking) {
                remainingTime -= GameWorld.DeltaTime;
                if (remainingTime > 0)
                {
                    SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                    if (sr.Rotation >= 0.025f)
                    {
                        shakeMod = -0.010f;
                    }
                    else if (sr.Rotation <= -0.025f) {
                        shakeMod = 0.010f;
                    }
                    sr.Rotation += shakeMod;
                }
                else {
                    shaking = false;
                    SpriteRenderer sr = (SpriteRenderer)GameObject.GetComponent<SpriteRenderer>();
                    sr.Rotation = 0f;
                }
            }
            base.Update();
        }
    }
}
