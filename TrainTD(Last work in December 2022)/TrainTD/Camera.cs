using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainTD
{
    internal class Camera
    {
        public Matrix transform;


        public void Update(Vector2 position)
        {
            /// <summary>
            /// Den løsning vi gik med læser spillerens midtpunkt og rykker skærmen med samme hastighed indtil den når til enden og skærmen ikke kan rykke mere
            /// Det gør den med at tage spillerens x position og spritesize / 2 for at få midten
            /// </summary>
            transform = Matrix.CreateTranslation(-position.X - (position.X / 2), -500, 0) *
                Matrix.CreateTranslation(GameWorld.screenSize.X / 2, GameWorld.screenSize.Y / 2, 0);

        }
    }
}
