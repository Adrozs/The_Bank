using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Bank.Utilities
{
    public class Sound
    {
        //Part of the NAudio Package, Controls  the Output of the audio and CAN play sounds asynchronously.
        private static WaveOutEvent waveOut = new WaveOutEvent();
        //This can be called to play sound with the correct pathing.
        public static void PlaySound(string soundFileName)
        {
            string soundFilePath = Path.Combine("Sounds", soundFileName);

            try
            {
                using (var audioFile = new AudioFileReader(soundFilePath))
                {
                    // using the WaveOutEvent instance for audio output
                    waveOut.Init(audioFile);
                    //Play the sound
                    waveOut.Play();
                    //wait for the playback to finish before continuing.
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        // "Sleep" for 1/10 of a Second to allow other tasks to execute
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }
    }
}
