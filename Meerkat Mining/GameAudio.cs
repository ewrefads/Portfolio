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
    public enum SFX { BLOCKHIT, BLOCKBREAK}

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

        Random rnd = new Random();
        private int choice;
        private static SoundEffect[] stoneHits = new SoundEffect[3];
        private static SoundEffect[] stoneBreaks = new SoundEffect[3];

        /// <summary>
        /// Load lyde ind i spillet
        /// </summary>
        /// <param name="content"></param>
        public static void LoadAudio(ContentManager content)
        {
            

            // Dette for loop skal loope med antallet af Soundeffects for array
            for (int i = 1; i < stoneHits.Length; i++)
            {
                stoneHits[i] = content.Load<SoundEffect>($"audio/stoneHit{i}");
            }

            for (int i = 1; i < stoneBreaks.Length; i++)
            {
                stoneBreaks[i] = content.Load<SoundEffect>($"audio/stoneBreak{i}");
            }

        }

        public void Play(SFX type)
        {

            switch (type)
            {
                case SFX.BLOCKHIT:
                    choice = rnd.Next(1, 3);

                    stoneHits[choice].Play();

                    break;
                case SFX.BLOCKBREAK:
                    choice = rnd.Next(1, 3);

                    stoneBreaks[choice].Play();

                    break;
                default:
                    break;
            }



        }

    }
}
