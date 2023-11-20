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
        private static WaveOutEvent waveOut = new WaveOutEvent();

        public static void PlaySound(string soundFileName)
        {
            string soundFilePath = Path.Combine("Sounds", soundFileName);

            try
            {
                using (var audioFile = new AudioFileReader(soundFilePath))
                {
                    waveOut.Init(audioFile);
                    waveOut.Play();
                    while (waveOut.PlaybackState == PlaybackState.Playing)
                    {
                        // Wait for playback to finish
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
