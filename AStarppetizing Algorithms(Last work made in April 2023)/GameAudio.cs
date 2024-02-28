using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meerkat_Mining
{
    public enum SFX { }

    internal class GameAudio
    {

        private static GameAudio instance;

        public static GameAudio Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameAudio();
                }

                return instance;
            }
        }


        /// <summary>
        /// Load lyde ind i spillet
        /// </summary>
        /// <param name="content"></param>
        public static void LoadAudio(ContentManager content)
        {


            // Dette for loop skal loope med antallet af Soundeffects for array

        }

        public void Play(SFX type)
        {

            switch (type)
            {
                default:
                    break;
            }



        }

    }
}
