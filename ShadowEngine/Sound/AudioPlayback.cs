using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.DirectX.AudioVideoPlayback;
using System.Runtime.InteropServices;

//---------------------------------------------------------------------
// Si da loaderdeck error al inicializar el dx device apretar ctrl alt E
// para desabilitar esta exepcion del MDA(Managed Debug Asistant)
//----------------------------------------------------------------------

namespace ShadowEngine.Sound
{ 
    public static class AudioPlayback //Maneja el audio en el juego
    {
        #region private attributes

        static string soundDir;
        static bool audioProblem;
        static bool canPlay = true;
        static uint initialVolume;
        static Dictionary<string, Audio> media = new Dictionary<string, Audio>();
        static Dictionary<string, Audio> mediaBackUp = new Dictionary<string, Audio>();
        static Dictionary<string, Audio> looping = new Dictionary<string, Audio>();
        static bool loaded;

        #endregion

        #region Properties

        public static bool Loaded
        {
            get { return AudioPlayback.loaded; }
            set { AudioPlayback.loaded = value; }
        }

        public static bool CanPlay
        {
            get { return AudioPlayback.canPlay; }
            set { AudioPlayback.canPlay = value; }
        }

        static public string SoundDir
        {
            get { return soundDir; }
            set { soundDir = value; }
        }

        #endregion

        /// <summary>
        /// This function load all the sound located on the specified folder
        /// </summary>
        static public void LoadSounds()
        {
            try
            {
                waveOutGetVolume(new IntPtr(), out initialVolume);
                waveOutSetVolume(new IntPtr(), 0X4000);
                string[] arrayWav = Directory.GetFiles(Globals.ApplicationPath + soundDir, "*.wav");
                string[] arrayMp3 = Directory.GetFiles(Globals.ApplicationPath + soundDir, "*.mp3");

                for (int i = 0; i < arrayWav.Length; i++)
                {
                    Audio audio = new Audio(arrayWav[i], false);
                    audio.Ending += new EventHandler(Loop);    
                    Audio audioBackup = new Audio(arrayWav[i], false);
                    media.Add(arrayWav[i], audio);
                    mediaBackUp.Add(arrayWav[i], audioBackup);
                }
                for (int i = 0; i < arrayMp3.Length; i++)
                {
                    Audio audio = new Audio(arrayMp3[i], false);
                    audio.Ending += new EventHandler(Loop);   
                    Audio audioBackup = new Audio(arrayMp3[i], false);
                    media.Add(arrayMp3[i], audio);
                }       
            }
            catch (Exception)
            {
                audioProblem = true; 
                canPlay = false; 
            }
            loaded = true;  
        }
   
        static public void OnOff()
        {
            if (canPlay == true)
            {
                canPlay = false;
                StopAllSounds(); 
            }
            else
            {
                canPlay = true;
            }
        }

        static public void Play(string name)
        {
            if (canPlay == true && audioProblem == false)
            {
                name = Globals.ApplicationPath + soundDir + name;
                if (media[name].CurrentPosition == media[name].Duration)
                {
                    media[name].Stop();
                }
                if (mediaBackUp[name].CurrentPosition == mediaBackUp[name].Duration)
                {
                    mediaBackUp[name].Stop();
                }
                if (!media[name].Playing)
                {
                    media[name].Play();
                }
                else
                {
                    mediaBackUp[name].Play();
                }
            }     
        }

        static public void PlayOne(string name)
        {
            if (canPlay == true && audioProblem == false)
            {
                name = Globals.ApplicationPath + soundDir + name;
                media[name].Stop();
                media[name].Play();
            }     
        }

        /// <summary>
        /// This function random plays any of the 2 input sounds
        /// </summary>
        static public void PlayAny(string sound1, string sound2)
        {
            int alt = Helper.Alternator();
            if (alt == 1)
            {
                Play(sound1);
            }
            else
                Play(sound2);  
        }

        /// <summary>
        /// This function random plays a sound looping
        /// </summary>
        static public void PlayLoop(string name)
        {
            if (canPlay == true && audioProblem == false)
            {
                name = Globals.ApplicationPath + soundDir + name;
                if (!looping.ContainsKey(name) )
                {
                    looping.Add(name, media[name]);
                }
                media[name].Stop();
                media[name].Play();
            }     
        }

        static public void StopLoop(string name)
        {
            if (canPlay)
            {
                name = Globals.ApplicationPath + soundDir + name;
                looping.Remove(name);
                media[name].Stop();   
            }
        }

        static public void StopAllSounds()
        {
            foreach (var item in media)
            {
                if (item.Value.Playing)
                {
                    item.Value.Stop();   
                }
            }
            foreach (var item in mediaBackUp)
            {
                if (item.Value.Playing)
                {
                    item.Value.Stop();
                }
            }
            looping.Clear();  
        }

        static public void ResumeAllSounds()
        {
            foreach (var item in media)
            {
                if (item.Value.Paused)
                {
                    item.Value.Play();
                }
            }
            foreach (var item in mediaBackUp)
            {
                if (item.Value.Paused)
                {
                    item.Value.Play();
                }
            }
        }

        static void Loop(Object audioSound, EventArgs events)
        {
            if (canPlay == true && audioProblem == false)
            {
                if (looping.ContainsValue((Audio)audioSound))
                {
                    ((Audio)audioSound).Stop();
                    ((Audio)audioSound).Play();
                }
            }
        }

        static public void RestoreVolume()
        {
            waveOutSetVolume(new IntPtr(), initialVolume);   
        }

        [DllImport("winmm.dll")]
        [return: MarshalAs(UnmanagedType.U4)]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
    }
}

