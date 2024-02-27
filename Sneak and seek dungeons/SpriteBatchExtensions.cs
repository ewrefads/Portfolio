using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    // Denne extension til spritebatch er blevet skrevet af Chat GPT
    public static class SpriteBatchExtensions
    {
        private static Texture2D pixelTexture;

        public static void LoadContent(GraphicsDevice graphicsDevice)
        {
            pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });
        }

        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2[] vertices, Color color, float thickness = 1f)
        {
            LoadContent(GameWorld.Instance.GraphicsDevice);

            if (vertices == null || vertices.Length < 2)
            {
                return;
            }

            Vector2[] lineVertices = new Vector2[vertices.Length + 1];
            for (int i = 0; i < vertices.Length; i++)
            {
                lineVertices[i] = vertices[i];
            }
            lineVertices[vertices.Length] = vertices[0];

            for (int i = 0; i < lineVertices.Length - 1; i++)
            {
                Vector2 p1 = lineVertices[i];
                Vector2 p2 = lineVertices[i + 1];
                Vector2 edge = p2 - p1;

                float angle = (float)Math.Atan2(edge.Y, edge.X);
                float length = edge.Length();

                spriteBatch.Draw(
                    pixelTexture,
                    p1,
                    null,
                    color,
                    angle,
                    Vector2.Zero,
                    new Vector2(length, thickness),
                    SpriteEffects.None,
                    0.6f);
            }
        }

        
    }
}
