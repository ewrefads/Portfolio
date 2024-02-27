using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Viewport = Microsoft.Xna.Framework.Graphics.Viewport;

namespace Chaos_in_Colosseum
{
    public class Camera
    {
        /// <summary>
        /// Først defineres de forskellige variabler der skal bruges
        /// </summary>
        private Vector2 position;
        private float scrollingSpeed;
        private Vector2 playerPos;
        private Vector2 velocity;
        public Texture2D sprite;
        public Rectangle rectangle;
        //private Vector2 centre;
        public Matrix transform;
        //Viewport view;
        public Camera()
        {

        }
        public Vector2 Position()
        {
            return new Vector2();
        }
        public void LoadContent(ContentManager Content, Player player)
        {
            /// <summary>
            /// Her loades det content der skal bruges. Spriten der bliver taget fra Content mappen og laves til en texture2D
            /// Gameworld bounds der bliver defineret og den rektangel spriten sættes til.
            /// </summary>
            sprite = Content.Load<Texture2D>("background1");

            GameWorld.worldBounds = new Vector2(-sprite.Width / 3, Convert.ToInt32(sprite.Width * 3));

            rectangle = new Rectangle((int)GameWorld.worldBounds.X, 0, (int)GameWorld.worldBounds.Y, Convert.ToInt32(GameWorld.screenSize.Y));

            

        }
        /// <summary>
        /// De 2 kommende funktioner: FollowPlayer & Move var brugt tidligt til at prøve at få baggrunden til at flytte side
        /// før en anden løsning blev brugt som virkede og ikke længere havde brug for dem
        /// </summary>
        public void FollowPlayer(Player player)
        {
            //velocity = Vector2.Zero;
            //playerPos = player.Position;
            //position = new Vector2();
            //velocity = new Vector2();
            
        }
        public void Move(GameTime gameTime)
        {
            //scrollingSpeed = 1000f;
            //float deltatime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //position += ((velocity * scrollingSpeed) * deltatime);
        }

        public void Update(Player player)
        {
            /// <summary>
            /// Den løsning vi gik med læser spillerens midtpunkt og rykker skærmen med samme hastighed indtil den når til enden og skærmen ikke kan rykke mere
            /// Det gør den med at tage spillerens x position og spritesize / 2 for at få midten
            /// </summary>
            transform = Matrix.CreateTranslation(-player.Position.X - (player.spriteSize.X / 2), -500, 0) *
                Matrix.CreateTranslation(GameWorld.screenSize.X / 2, GameWorld.screenSize.Y / 2, 0);

        }

            
            

    }
            
            
        
}
