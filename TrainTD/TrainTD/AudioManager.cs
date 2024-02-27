using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using System.Runtime.Loader;

using Microsoft.Xna.Framework.Audio;

namespace TrainTD
{
    internal static class AudioManager
    {
        // Fields
        private static Random rnd;
        private static float fadeSpeed = 5f; // Hastigheden for 'fade' effekten af musik.
        private static float waitTimer = 2f; // Brugt som ventetiden mellem afspilninig af nyt musik
        private static bool songStop = false; // En bool som fortæller os om MediaPlayer.Stop() er blevet brugt
        private static bool isDone = true; // En bool til en test metode så den kun kan køre en gang. (Metoden bliver kaldt i GameWorlds update)
        // Note til selv: Lav nogle lister af song (prep & action)

        static Song prep1, prep2, prep3, prep4, action1, action2, action3; 

        static SoundEffect click, cntBuy, cntPlace, swap, train1, trainDestroyed, twrDisable, twrFire1, twrFire2, twrPlace, victory;

        private static List<Song> preps = new List<Song>();
        private static List<Song> actions = new List<Song>();

        private static float masterVol = 1f; // Mastervolume for al musik i spillet
        public static float MasterVol { get => masterVol; set => masterVol = value; } 

        


        // Methods
        /// <summary>
        /// Load Audio in GameWorld
        /// </summary>
        /// <param name="content"></param>
        public static void LoadAudio(ContentManager content)
        {
            // Load background music (bgm)
            prep1 = content.Load<Song>("bw_music/prep1");
            prep2 = content.Load<Song>("bw_music/prep2");
            prep3 = content.Load<Song>("bw_music/prep3");
            prep4 = content.Load<Song>("bw_music/prep4");

            action1 = content.Load<Song>("bw_music/action1");
            action2 = content.Load<Song>("bw_music/action2");
            action3 = content.Load<Song>("bw_music/action3");

            preps.Insert(0,prep1);
            preps.Insert(1,prep2);
            preps.Insert(2, prep3);
            preps.Insert(3, prep4);

            actions.Insert(0, action1);
            actions.Insert(1, action2);
            actions.Insert(2, action3);

            // Load soundeffects
            click = content.Load<SoundEffect>("bw_sfx/click");
            cntBuy = content.Load<SoundEffect>("bw_sfx/cantBuy");
            cntPlace = content.Load<SoundEffect>("bw_sfx/towercantplace");
            swap = content.Load<SoundEffect>("bw_sfx/switch");
            train1 = content.Load<SoundEffect>("bw_sfx/train1");
            trainDestroyed = content.Load<SoundEffect>("bw_sfx/traindestroyed");
            twrDisable = content.Load<SoundEffect>("bw_sfx/towerdisabled");
            twrFire1 = content.Load<SoundEffect>("bw_sfx/towerfire_1");
            twrFire2 = content.Load<SoundEffect>("bw_sfx/towerfire_2");
            twrPlace = content.Load<SoundEffect>("bw_sfx/towerplaced");
            victory = content.Load<SoundEffect>("bw_sfx/victory");
        }

        /// <summary>
        /// A method to test Songs in game. 
        /// </summary>
        public static void MusicTest()
        {
            if (isDone)
            {
                MediaPlayer.Play(prep1);
                isDone = false;
            }
        } 




        public static void NewSong(GameTime gameTime)
        {

            if (GameWorld.PhaseChanged)// End current song
            {
                // Sæt volume ned over tid for at lave 'fade out' af musik.
                MediaPlayer.Volume -= (float)gameTime.ElapsedGameTime.TotalSeconds / fadeSpeed;

                if (MediaPlayer.Volume <= 0)
                {
                    // Stopper alt musik som bliver afspillet
                    MediaPlayer.Stop();

                    // Sæt volumen til at være lig med master volumen
                    MediaPlayer.Volume = MasterVol;
                    songStop = true;
                }

                // Sætter phaseChanged til at være falsk

                GameWorld.PhaseChanged = false;
            }

            // Afspil ny bgm når den mediaplayer har stoppet den daværende bgm
            if (songStop)
            {
                // Venter med at afspille den næste sang
                waitTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;


                // Ventetiden er for at undgå en kort 'static noise' i starten af afspilningen 
                if (waitTimer < 0)
                {

                    if (GameWorld.CurrentPhase == 1)// Preperation song start
                    {
                        rnd.Next(0, 3);

                        MediaPlayer.Play(preps[Convert.ToInt32(rnd)]);

                    }

                    if (GameWorld.CurrentPhase == 0)// Action song start
                    {
                        rnd.Next(0, 2);

                        MediaPlayer.Play(actions[Convert.ToInt32(rnd)]);

                    }
                    songStop = false;
                    waitTimer = 2f;
                }



            }

        }

        /// <summary>
        /// Afspil soundeffects. OBS: Parametren skal kunne matches med en case fra switchcasen i metoden.
        /// </summary>
        /// <param name="sound"></param>
        /// <exception cref="Exception"></exception>
        public static void PlaySFX(string sound)
        {
            switch (sound)
            {
                case "click":
                    click.Play();
                    break;
                case "cntBuy":
                    cntBuy.Play();
                    break;
                case "cntPlace":
                    cntPlace.Play();
                    break;
                case "swap":
                    swap.Play();
                    break;
                case "train1":
                    train1.Play();
                    break;
                case "trainDestroyed":
                    trainDestroyed.Play();
                    break;
                case "twrDisable":
                    twrDisable.Play();
                    break;
                case "twrFire1":
                    twrFire1.Play();
                    break;
                case "twrFire2":
                    twrFire2.Play();
                    break;
                case "twrPlace":
                    twrPlace.Play();
                    break;
                case "victory":
                    victory.Play();
                    break;

                default:
                    throw new Exception("Unable to get a case for method parameter");
                    
            }
        }

        /// <summary>
        /// En metode for at justere lydstyrken for spillet
        /// </summary>
        /// <param name="newVolume"></param>
        public static void ChangeVolume(float newVolume)
        {
            //Mastervolume for MediaPlayer
            masterVol = newVolume;
            // Sæt MediaPlayer volumer til at være lig med master volume
            MediaPlayer.Volume = masterVol;
            //Mastervolume for soundeffects
            SoundEffect.MasterVolume = newVolume;

        }

        /// <summary>
        /// Sætter bgm musik på pasue hvis spillet er på pause
        /// </summary>
        public static void PauseMusic()
        {
            if (Global.paused)
            {
                MediaPlayer.Pause();
            }
            else
            {
                MediaPlayer.Resume();
            }
            

        }


    }
}
