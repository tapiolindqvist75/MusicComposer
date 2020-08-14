using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicComposerLibrary.Generators
{
    public class ChordGenerator
    {
        private WeightData _weightData;
        private Scale _scale;
        
        public ChordGenerator(WeightData weightData, Scale scale)
        {
            _weightData = weightData;
            _scale = scale;
        }
        public List<Structures.Chord> DetermineChords(List<Structures.Note> melodyNotes)
        {
            List<Structures.Chord> chords = new List<Structures.Chord>();
            IEnumerable<IGrouping<int, Structures.Note>> measures = melodyNotes.GroupBy(note => note.MeasureNumber);
            //First note of first measure must be in chord, sounds strange otherwise. 
            Structures.Note firstNoteUnhandled = melodyNotes.First();
            Structures.Chord latestChord = null;
            foreach (IGrouping<int, Structures.Note> measureNotes in measures)
            {
                List<int> degreeOrder = new List<int>() { 1, 5, 3, 2, 4, 6, 7 };
                Tuple<decimal, Structures.Chord> bestMatch = new Tuple<decimal, Structures.Chord>(0, null);
                foreach (int degree in degreeOrder)
                {
                    if (bestMatch.Item1 == 1)
                        break;
                    Structures.Chord chord = _scale.GetDegreeTriad(degree, new Structures.NotePitch("F", 3), 3);
                    if (firstNoteUnhandled == null || IsNoteInChord(firstNoteUnhandled.Pitch, chord))
                    {
                        firstNoteUnhandled = null;
                        decimal match = CompareChord(measureNotes, chord);
                        System.Diagnostics.Debug.WriteLine($"{chord.FullName} {match}");
                        if (match > bestMatch.Item1 && (latestChord == null || chord.FullName != latestChord.FullName))
                            bestMatch = new Tuple<decimal, Structures.Chord>(match, chord);
                    }

                }
                foreach (int degree in degreeOrder)
                {
                    if (bestMatch.Item1 == 1)
                        break;
                    Structures.Chord chord = _scale.GetDegreeSeventhChord(degree, new Structures.NotePitch("E", 3), 3);
                    if (firstNoteUnhandled == null || IsNoteInChord(firstNoteUnhandled.Pitch, chord))
                    {
                        decimal match = CompareChord(measureNotes, chord);
                        System.Diagnostics.Debug.WriteLine($"{chord.FullName} {match}");
                        if (match > bestMatch.Item1 && (latestChord == null || chord.FullName != latestChord.FullName))
                            bestMatch = new Tuple<decimal, Structures.Chord>(match, chord);
                    }
                }
                if (bestMatch.Item2 == null)
                {
                    //Chord could not be resolved. Invalid input, best guess is that first note of the song is non-scale note
                    Structures.Chord chord = _scale.GetDegreeTriad(1, new Structures.NotePitch("F", 3), 3);
                    bestMatch = new Tuple<decimal, Structures.Chord>(1, chord);
                }
                chords.Add(bestMatch.Item2);
                latestChord = bestMatch.Item2;
                measureNotes.First().Chord = bestMatch.Item2;
            }
            return chords;
        }

        private bool IsNoteInChord(Structures.NotePitch note, Structures.Chord chord)
        {
            foreach (Structures.NotePitch chordNote in chord.NotePitches)
            {
                if (chordNote.IsSameIgnoreOctave(note))
                    return true;
            }
            return false;
        }

        private decimal GetNoteWeight(Structures.Note note, Structures.Chord chord)
        {
            if (IsNoteInChord(note.Pitch, chord))
                return note.Duration * _weightData.ChordDetermination_ChordNote;
            Structures.Scale scale = null;
            if (chord.Classification == Structures.ChordClassification.Major ||
                chord.Classification == Structures.ChordClassification.MajorSeventh)
                scale = new Structures.Scale(chord.NotePitches[0], true);
            else if (chord.Classification == Structures.ChordClassification.Minor ||
                chord.Classification == Structures.ChordClassification.MinorSeventh)
                scale = new Structures.Scale(chord.NotePitches[0], false);
            if (scale != null)
            {
                int[] degrees = new int[] { 2, 4, 6, 7 };
                foreach (int degree in degrees)
                {
                    if (scale.GetDegreePitch(degree).IsSameIgnoreOctave(note.Pitch))
                        return note.Duration * _weightData.ChordDetermination_ScaleNote;
                }
            }
            return note.Duration * _weightData.ChordDetermination_NonScaleNote;
        }

        private decimal CompareChord(IEnumerable<Structures.Note> notes, Structures.Chord chord)
        {
            decimal hits = 0;
            foreach (Structures.Note note in notes)
                hits += GetNoteWeight(note, chord);
            return hits;
        }
    }
}