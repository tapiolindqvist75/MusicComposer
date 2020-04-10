using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Midi
{
    public class MidiConverter
    {
        public const int TEMPO_PER_QUARTER = 480;
        public static byte NoteToMidi(char note, int offset)
        {
            switch (note)
            {
                case 'C': return Convert.ToByte(60 + offset);
                case 'D': return Convert.ToByte(62 + offset);
                case 'E': return Convert.ToByte(64 + offset);
                case 'F': return Convert.ToByte(65 + offset);
                case 'G': return Convert.ToByte(67 + offset);
                case 'A': return Convert.ToByte(69 + offset);
                case 'B': return Convert.ToByte(71 + offset);
                default: throw new ArgumentException("Invalid note, valid values 'C','D','E','F','G','A','B'");
            }
        }

        public static int DecimalToDuration(decimal duration)
        {
            decimal tempoPerDuration = TEMPO_PER_QUARTER * 4;
            decimal ticks = duration * tempoPerDuration;
            return Convert.ToInt32(ticks - (ticks / 20) - 1);
        }

        public static int DecimalToPause(decimal duration)
        {
            decimal tempoPerDuration = TEMPO_PER_QUARTER * 4;
            decimal ticks = duration * tempoPerDuration;
            return Convert.ToInt32((ticks / 20) + 1);
        }

        public static void AddNotesToChunks(EventsCollection target, List<Structures.Note> notes)
        {
            decimal lastDuration = 0;
            for(int loop=0;loop<notes.Count;loop++)
            {
                Structures.Note current = notes[loop];
                decimal duration = current.Duration;
                byte note = NoteToMidi(current.Name, current.Offset);
                if (current.Tie == Structures.NoteDuration.TieType.Start)
                {
                    while(current.Tie != Structures.NoteDuration.TieType.End)
                    {
                        loop++;
                        current = notes[loop];
                        duration += current.Duration;
                    }
                }
                target.Add(new NoteOnEvent()
                {
                    NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(note),
                    Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(80),
                    DeltaTime = DecimalToPause(lastDuration)
                });
                target.Add(new NoteOffEvent()
                {
                    NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(note),
                    Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(0),
                    DeltaTime = DecimalToDuration(duration)
                });
                lastDuration = duration;
            }
        }
    }
}
