using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace MusicComposerLibrary.Structures
{
    public class Scale
    {
        public Scale(Structures.NotePitch baseNote, bool major)
        {
            Tonic = baseNote;
            Major = major;
            if (major)
            {
                DegreeIntervals = new List<int>() { 2, 2, 1, 2, 2, 2, 1 };
                switch (baseNote.FullName)
                {
                    case "C": Key = 0; InitDegreeNotes("C", "D", "E", "F", "G", "A", "B"); break;
                    case "G": Key = 1; InitDegreeNotes("G", "A", "B", "C", "D", "E", "F#"); break;
                    case "D": Key = 2; InitDegreeNotes("D", "E", "F#", "G", "A", "B", "C#"); break;
                    case "A": Key = 3; InitDegreeNotes("A", "B", "C#", "D", "E", "F#", "G#"); break;
                    case "E": Key = 4; InitDegreeNotes("E", "F#", "G#", "A", "B", "C#", "D#"); break;
                    case "B": Key = 5; InitDegreeNotes("B", "C#", "D#", "E", "F#", "G#", "A#"); break;
                    case "F#": Key = 6; InitDegreeNotes("F#", "G#", "A#", "B", "C#", "D#", "E#"); break;
                    case "C#": Key = 7; InitDegreeNotes("C#", "D#", "E#", "F#", "G#", "A#", "B#"); break;
                    case "F": Key = -1; InitDegreeNotes("F", "G", "A", "Bb", "C", "D", "E"); break;
                    case "Bb": Key = -2; InitDegreeNotes("Bb", "C", "D", "Eb", "F", "G", "A"); break;
                    case "Eb": Key = -3; InitDegreeNotes("Eb", "F", "G", "Ab", "Db", "C", "D"); break;
                    case "Ab": Key = -4; InitDegreeNotes("Ab", "Bb", "C", "Db", "Eb", "F", "G"); break;
                    case "Db": Key = -5; InitDegreeNotes("Db", "Eb", "F", "Gb", "Ab", "Bb", "C"); break;
                    case "Gb": Key = -6; InitDegreeNotes("Gb", "Ab", "Bb", "Cb", "Db", "Eb", "F"); break;
                    case "Cb": Key = -7; InitDegreeNotes("Cb", "Db", "Eb", "Fb", "Gb", "Ab", "Bb"); break;
                    default: throw new ArgumentException("Invalid base note");
                }
            }
            else
            {
                DegreeIntervals = new List<int>() { 2, 1, 2, 2, 1, 2, 2 };
                switch (baseNote.FullName)
                {
                    case "A": Key = 0; InitDegreeNotes("A", "B", "C", "D", "E", "F", "G"); break;
                    case "E": Key = 1; InitDegreeNotes("E", "F#", "G", "A", "B", "C", "D"); break;
                    case "B": Key = 2; InitDegreeNotes("B", "C#", "D", "E", "F#", "G", "A"); break;
                    case "F#": Key = 3; InitDegreeNotes("F#", "G#", "A", "B", "C#", "D", "E"); break;
                    case "C#": Key = 4; InitDegreeNotes("C#", "D#", "E", "F#", "G#", "A", "B"); break;
                    case "G#": Key = 5; InitDegreeNotes("G#", "A#", "B", "C#", "D#", "E", "F#"); break;
                    case "D#": Key = 6; InitDegreeNotes("D#", "E#", "F#", "G#", "A#", "B", "C#"); break;
                    case "A#": Key = 7; InitDegreeNotes("A#", "B#", "C#", "D#", "E#", "F#", "G#"); break;
                    case "D": Key = -1; InitDegreeNotes("D", "E", "F", "G", "A", "Bb", "C"); break;
                    case "G": Key = -2; InitDegreeNotes("G", "A", "Bb", "C", "D", "Eb", "F"); break;
                    case "C": Key = -3; InitDegreeNotes("C", "D", "Eb", "F", "G", "Ab", "Db"); break;
                    case "F": Key = -4; InitDegreeNotes("F", "G", "Ab", "Bb", "C", "Db", "Eb"); break;
                    case "Bb": Key = -5; InitDegreeNotes("Bb", "C", "Db", "Eb", "F", "Gb", "Ab"); break;
                    case "Eb": Key = -6; InitDegreeNotes("Eb", "F", "Gb", "Ab", "Bb", "Cb", "Db"); break;
                    case "Ab": Key = -7; InitDegreeNotes("Ab", "Bb", "Cb", "Db", "Eb", "Fb", "Gb"); break;
                    default: throw new ArgumentException("Invalid base note");
                }
            }
        }
        private void InitDegreeNotes(params string[] notes)
        {
            int octave = 4;
            _degreeNotes = new List<Structures.NotePitch>();
            foreach (string note in notes)
            {
                if (note.StartsWith("C"))
                    octave++;
                _degreeNotes.Add(new Structures.NotePitch(note, octave));
            }
        }
        public bool Major { get; private set; }
        public int Key { get; private set; }
        public List<int> DegreeIntervals { get; private set; }
        
        public Structures.NotePitch Tonic { get; private set; }
        public Structures.NotePitch Mediant { get { return GetDegreePitch(3); } }
        public Structures.NotePitch Dominant { get { return GetDegreePitch(5); } }
        public Structures.NotePitch Subdominant { get { return GetDegreePitch(7); } }
        public bool Sharp { get { return Key >= 0; } }
        //Note! Degrees in music are 1 based. Always use -1 when using this list
        private List<Structures.NotePitch> _degreeNotes;
        public Structures.NotePitch GetChromaticPitch(int interval, int octave = 4)
        {
            return new Structures.NotePitch(Tonic.MidiNumber + interval, Sharp);
        }
        public Structures.NotePitch GetDegreePitch(int degree, int octave = 4)
        {
            while (degree > 7)
                degree -= 7;
            return new Structures.NotePitch((12 * (octave + 1)) + (_degreeNotes[degree - 1].MidiNumber % 12), Sharp);
        }

        public Structures.ScaleNotePitch GetScaleNote(NotePitch notePitch)
        {
            if (notePitch.IsSameIgnoreOctave(Tonic))
                return new ScaleNotePitch(notePitch.MidiNumber, Sharp, NoteScaleType.Tonic);
            else if (notePitch.IsSameIgnoreOctave(Mediant))
                return new ScaleNotePitch(notePitch.MidiNumber, Sharp, NoteScaleType.Mediant);
            else if (notePitch.IsSameIgnoreOctave(Dominant))
                return new ScaleNotePitch(notePitch.MidiNumber, Sharp, NoteScaleType.Dominant);
            else if (notePitch.IsSameIgnoreOctave(GetDegreePitch(2)) ||
                notePitch.IsSameIgnoreOctave(GetDegreePitch(4)) ||
                notePitch.IsSameIgnoreOctave(GetDegreePitch(6)) ||
                notePitch.IsSameIgnoreOctave(GetDegreePitch(7)))
                return new ScaleNotePitch(notePitch.MidiNumber, Sharp, NoteScaleType.ScaleNote);
            return new ScaleNotePitch(notePitch.MidiNumber, Sharp, NoteScaleType.NonScaleNote);
        }
        public List<Structures.ScaleNotePitch> GetChromaticNotes(Structures.NotePitch lowest, Structures.NotePitch highest)
        {
            List<Structures.ScaleNotePitch> list = new List<ScaleNotePitch>();
            for (int loop = lowest.MidiNumber; loop <= highest.MidiNumber; loop++)
                list.Add(GetScaleNote(new NotePitch(loop, Sharp)));
            return list;
        }
        public Structures.Chord GetDegreeTriad(int degree, Structures.NotePitch highestRoot, int maxOctave = 3)
        {
            int octave = maxOctave;
            while (GetDegreePitch(degree, octave).MidiNumber > highestRoot.MidiNumber)
                octave--;
            return GetDegreeTriad(degree, octave);
        }
        public Structures.Chord GetDegreeTriad(int degree, int octave = 4)
        {
            ChordClassification chordClassification;
            Structures.NotePitch root = new Structures.NotePitch(GetDegreePitch(degree, octave).MidiNumber, Sharp);
            if ((Major && (degree == 1 || degree == 4 || degree == 5)) ||
                (!Major && (degree == 3 || degree == 6 || degree == 7)))
                chordClassification = ChordClassification.Major;
            else if ((Major && (degree == 2 || degree == 3 || degree == 6)) ||
                (!Major && (degree == 1 || degree == 4 || degree == 5)))
                chordClassification = ChordClassification.Minor;
            else
                chordClassification = ChordClassification.Diminished;
            List<Structures.NotePitch> notePitches = new List<Structures.NotePitch>() { root };
            for (int loop = 1; loop <= 4; loop++)
            {
                if (GetDegreePitch(degree + loop).Name == 'C')
                    octave++;
                if (loop == 2 || loop == 4) notePitches.Add(GetDegreePitch(degree + loop, octave));
            }
            return new Chord(notePitches, chordClassification);
        }
        public Structures.Chord GetDegreeSeventhChord(int degree, Structures.NotePitch highestRoot, int maxOctave = 3)
        {
            int octave = maxOctave;
            while (GetDegreePitch(degree, octave).MidiNumber > highestRoot.MidiNumber)
                octave--;
            return GetDegreeSeventhChord(degree, octave);
        }
        public Structures.Chord GetDegreeSeventhChord(int degree, int octave = 4)
        {
            ChordClassification chordClassification;
            Structures.NotePitch root = new Structures.NotePitch(GetDegreePitch(degree, octave).MidiNumber, Sharp);
            if ((Major && (degree == 1 || degree == 4)) ||
                (!Major && (degree == 3 || degree == 6)))
                chordClassification = ChordClassification.MajorSeventh;
            else if ((Major && (degree == 5)) ||
                (!Major && (degree == 7)))
                chordClassification = ChordClassification.DominantSeventh;
            else if ((Major && (degree == 2 || degree == 3 || degree == 6)) ||
                (!Major && (degree == 1 || degree == 4 || degree == 5)))
                chordClassification = ChordClassification.MinorSeventh;
            else
                chordClassification = ChordClassification.HalfDiminishedSeventh;
            List<Structures.NotePitch> notePitches = new List<Structures.NotePitch>() { root };
            for (int loop = 1; loop <= 6; loop++)
            {
                if (GetDegreePitch(degree + loop).Name == 'C')
                    octave++;
                if (loop == 2 || loop == 4 || loop == 6) notePitches.Add(GetDegreePitch(degree + loop, octave));
            }
            return new Chord(notePitches, chordClassification);
        }
    }
}
