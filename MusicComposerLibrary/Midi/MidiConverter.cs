using Melanchall.DryWetMidi.Core;
using System;
using System.Collections.Generic;

namespace MusicComposerLibrary.Midi
{
    public class MidiConverter
    {
        public const int TEMPO_PER_QUARTER = 480;
        public static int DecimalToDuration(decimal duration)
        {
            if (duration == 0)
                return 0;
            decimal tempoPerDuration = TEMPO_PER_QUARTER * 4;
            decimal ticks = duration * tempoPerDuration;
            return Convert.ToInt32(ticks - (ticks / 20) - 1);
        }
        public static int DecimalToPause(decimal duration)
        {
            if (duration == 0)
                return 0;
            decimal tempoPerDuration = TEMPO_PER_QUARTER * 4;
            decimal ticks = duration * tempoPerDuration;
            return Convert.ToInt32((ticks / 20) + 1);
        }
        public static void AddChordEvents(EventsCollection chordEvents, List<Structures.Chord> chords, byte channel, Structures.NoteDuration chordDuration)
        {
            decimal lastChordDuration = 0;
            decimal duration = chordDuration.Duration;
            foreach (Structures.Chord chord in chords)
            {
                bool firstNoteInChord = true;
                foreach (Structures.NotePitch notePitch in chord.NotePitches)
                {
                    byte chordNote = (byte)notePitch.MidiNumber;
                    chordEvents.Add(new NoteOnEvent()
                    {
                        Channel = new Melanchall.DryWetMidi.Common.FourBitNumber(channel),
                        NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(chordNote),
                        Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(40),
                        DeltaTime = DecimalToPause(firstNoteInChord ? lastChordDuration : 0)
                    });
                    firstNoteInChord = false;
                }
                firstNoteInChord = true;
                foreach (Structures.NotePitch notePitch in chord.NotePitches)
                {
                    byte chordNote = (byte)notePitch.MidiNumber;
                    chordEvents.Add(new NoteOffEvent()
                    {
                        Channel = new Melanchall.DryWetMidi.Common.FourBitNumber(channel),
                        NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(chordNote),
                        Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(0),
                        DeltaTime = DecimalToDuration(firstNoteInChord ? duration : 0)
                    });
                    firstNoteInChord = false;
                }
                lastChordDuration = duration;
            }
        }
        public static void AddMelodyEvents(EventsCollection melodyEvents, List<Structures.Note> melodyNotes)
        {
            decimal lastDuration = 0;
            for(int loop=0;loop<melodyNotes.Count;loop++)
            {
                Structures.Note current = melodyNotes[loop];
                decimal duration = current.Duration;
                byte note = (byte)current.Pitch.MidiNumber;
                if (current.Tie == Structures.NoteDuration.LinkType.Start)
                {
                    while(current.Tie != Structures.NoteDuration.LinkType.End)
                    {
                        loop++;
                        current = melodyNotes[loop];
                        duration += current.Duration;
                    }
                }
                melodyEvents.Add(new NoteOnEvent()
                {
                    NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber(note),
                    Velocity = new Melanchall.DryWetMidi.Common.SevenBitNumber(80),
                    DeltaTime = DecimalToPause(lastDuration)
                });
                melodyEvents.Add(new NoteOffEvent()
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
