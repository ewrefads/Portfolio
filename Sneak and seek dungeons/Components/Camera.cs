using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons.Components
{
    /// <summary>
    /// det camera som styrer hvor henne sprites bliver loadet og hvor zoomet ind vi er på spillet
    /// Frederik
    /// </summary>
    internal class Camera : Component
    {
        public Matrix transform;
        public float zoom;
        private GameObject player;
        private Vector2 camOffset;
        
        public Vector2 CamOffset { get { return new Vector2(transform.Translation.X, transform.Translation.Y); } }
        

        public override void Start()
        {
            zoom = 0.5f;
            player = GameWorld.Instance.FindObjectOfType<Player>().GameObject;
            transform = Matrix.CreateTranslation(-player.Transform.Position.X, -player.Transform.Position.Y, 0) * Matrix.CreateScale(zoom) *
            Matrix.CreateTranslation(GameWorld.ScreenSize.X / 2, GameWorld.ScreenSize.Y / 2, 0);
        }
        public void UpdateCameraPosition(Rectangle room)
        {
            /// <summary>
            /// Den løsning vi gik med læser spillerens midtpunkt og rykker skærmen med samme hastighed indtil den når til enden og skærmen ikke kan rykke mere
            /// Det gør den med at tage spillerens x position og spritesize / 2 for at få midten
            /// </summary>
            /// 
            //SpriteRenderer s = player.GetComponent<SpriteRenderer>() as SpriteRenderer;

            /*transform = Matrix.CreateTranslation(-room.X - room.Width/2, -room.Y -room.Height/2, 0) * Matrix.CreateScale(zoom) *
            Matrix.CreateTranslation(GameWorld.ScreenSize.X / 2, GameWorld.ScreenSize.Y / 2, 0);*/
            transform = Matrix.CreateTranslation(-player.Transform.Position.X, -player.Transform.Position.Y, 0) * Matrix.CreateScale(zoom) *
            Matrix.CreateTranslation(GameWorld.ScreenSize.X / 2, GameWorld.ScreenSize.Y / 2, 0);



        }
    }
}
