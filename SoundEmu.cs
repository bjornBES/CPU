using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace CPUTing
{
    public enum Tone
    {
        REST = 0,
        GbelowC = 196,
        A = 220,
        Asharp = 233,
        B = 247,
        C = 262,
        Csharp = 277,
        D = 294,
        Dsharp = 311,
        E = 330,
        F = 349,
        Fsharp = 370,
        G = 392,
        Gsharp = 415,
    }

    public enum Duration
    {
        WHOLE = 1600,
        HALF = WHOLE / 2,
        QUARTER = HALF / 2,
        EIGHTH = QUARTER / 2,
        SIXTEENTH = EIGHTH / 2,
    }

    public struct Note
    {
        Tone toneVal;
        Duration durVal;

        // Define a constructor to create a specific note.
        public Note(Tone frequency, Duration time)
        {
            toneVal = frequency;
            durVal = time;
        }

        // Define properties to return the note's tone and duration.
        public Tone NoteTone { get { return toneVal; } }
        public Duration NoteDuration { get { return durVal; } }
    }
    public class SoundEmu
    {
        bool[] Voices = new bool[4];
        Thread[] VoiceThreads = new Thread[4];
        public SoundEmu()
        {
            for (int i = 0; i < VoiceThreads.Length; i++)
            {
                Voices[i] = false;
                VoiceThreads[i] = new Thread(new ThreadStart(Play));
            }
        }
        public void UpdateVoices()
        {
            for (int i = 0; i < Voices.Length; i++)
            {
                if (Voices[i] == true)
                {
                    VoiceThreads[i].Start();
                }
            }
        }
        public void SetVoice(int index)
        {
            Voices[index] = !Voices[index];
        }
        void Play()
        {
            for (int i = 0; i < Voices.Length; i++)
            {
                if (Voices[i] == true)
                {
                    Console.Beep(800, 200);
                }
            }
        }
        public void Play(int index)
        {
            if (Voices[index] == true)
            {
                Console.Beep(800, 200);
            }
        }
        public void PlayNotes(Note[] tune)
        {
            foreach (Note n in tune)
            {
                if (n.NoteTone == Tone.REST)
                    Thread.Sleep((int)n.NoteDuration);
                else
                    Console.Beep((int)n.NoteTone, (int)n.NoteDuration);
            }
        }
    }
}
