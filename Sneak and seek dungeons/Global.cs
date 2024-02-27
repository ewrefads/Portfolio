using Microsoft.Xna.Framework;
using Sneak_and_seek_dungeons.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sneak_and_seek_dungeons
{
    /// <summary>
    /// Rasmus
    /// denne klasse blev brugt som et workaround til at både pause spillet, men også at tilgå enemy klassen
    /// </summary>
    internal class Global
    {
        public static bool paused = false;
        /// <summary>
        /// Her under bliver deen bool der bestemmer om enemies bevæger sig, flipped hver gang funktionen bliver kaldt i  GameWorld
        /// </summary>
        public static void pause()
        {
            paused = !paused;
        }
        public static void EnemyStop()
        {
            Enemy.isMoving = !Enemy.isMoving;
        }

        public static void Double()
        {
            paused = !paused;
            Enemy.isMoving = !Enemy.isMoving;
        }

        

        


    }
}
