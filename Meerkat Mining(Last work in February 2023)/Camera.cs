using Meerkat_Mining.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public class Camera
    {
        public Matrix transform;

        public void Update(GameObject player)
        {
            /// <summary>
            /// Den løsning vi gik med læser spillerens midtpunkt og rykker skærmen med samme hastighed indtil den når til enden og skærmen ikke kan rykke mere
            /// Det gør den med at tage spillerens x position og spritesize / 2 for at få midten
            /// </summary>
            /// 
            SpriteRenderer s = player.GetComponent<SpriteRenderer>() as SpriteRenderer;

            transform = Matrix.CreateTranslation(-player.Transform.Position.X, -player.Transform.Position.Y, 0) *
            Matrix.CreateTranslation(GameWorld.screenSize.X / 2, GameWorld.screenSize.Y / 2, 0);

        }
    }

    
}
