using MusicComposerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicComposerLibrary
{
    public class SongPartGenerator
    {
        private readonly WeightedRandom _weightedRandom;
        private readonly Structures.Scale _scale;
        private readonly Weights _weights;
        private readonly Structures.NotePitch _lowestPitch;
        private readonly Structures.NotePitch _highestPitch;
        private readonly Generators.RythmGenerator _rythmGenerator;
        public SongInput Input { get; private set; }
        public SongPartGenerator(SongInput songInput)
        {
            if (songInput == null)
                throw new ArgumentNullException(nameof(songInput));
            if (string.IsNullOrWhiteSpace(songInput.Name))
                throw new ArgumentException($"{nameof(songInput.Name)} is empty or null");
            if (string.IsNullOrWhiteSpace(songInput.SongName))
                throw new ArgumentException($"{nameof(songInput.SongName)} is empty or null");
            if (songInput.PartLength <= 0)
                throw new ArgumentException($"{nameof(songInput.PartLength)} must be positive value");
            if (songInput.Major)
                _scale = new Structures.Scale(new Structures.NotePitch(songInput.ScaleKeyFullName, 4), true);
            else
                _scale = new Structures.Scale(new Structures.NotePitch(songInput.ScaleKeyFullName, 4), false);

            if (string.IsNullOrWhiteSpace(songInput.MelodyLowestNoteFullNameWithOctave))
                _lowestPitch = new Structures.NotePitch("A3");
            else
                _lowestPitch = new Structures.NotePitch(songInput.MelodyLowestNoteFullNameWithOctave);
            if (string.IsNullOrWhiteSpace(songInput.MelodyHighestNoteFullNameWithOctave))
                _highestPitch = new Structures.NotePitch("C6");
            else
                _highestPitch = new Structures.NotePitch(songInput.MelodyHighestNoteFullNameWithOctave);

            if (songInput.BeatsPerMeasure <= 0)
                throw new ArgumentException($"{nameof(songInput.BeatsPerMeasure)} must be positive value");
            if (songInput.DurationValues == null)
                throw new ArgumentNullException(nameof(songInput.DurationValues));
            if (songInput.PitchValues == null)
                throw new ArgumentNullException(nameof(songInput.PitchValues));
            if (songInput.WeightData == null)
                throw new ArgumentNullException(nameof(songInput.WeightData));
            Input = songInput;
            _weightedRandom = new WeightedRandom(songInput.DurationValues, songInput.PitchValues);
            _weights = new Weights(songInput.WeightData);
            
            _rythmGenerator = new Generators.RythmGenerator(_weightedRandom, _weights, songInput.PartLength, songInput.BeatsPerMeasure, songInput.BeatUnit);
        }

        public Structures.NotePitch GetLastNotePitch()
        {
            Tuple<int, Structures.NotePitch> minimumDifference = new Tuple<int, Structures.NotePitch>(int.MaxValue, null); 
            for(int loop = _lowestPitch.Octave; loop < _highestPitch.Octave; loop++)
            {
                Structures.NotePitch current = _scale.GetDegreePitch(1, loop);
                int difference = Math.Abs((current.MidiNumber - _lowestPitch.MidiNumber) - (_highestPitch.MidiNumber - current.MidiNumber));
                if (difference < minimumDifference.Item1)
                    minimumDifference = new Tuple<int, Structures.NotePitch>(difference, current);
            }
            return minimumDifference.Item2;
        }
        private List<Structures.Note> AddMelody(WeightedRandom weightedRandom, List<Structures.NoteDuration> noteDurations)
        {
            List<Structures.Note> notes = new List<Structures.Note>();
            int latestNoteMidiNumber = 0;
            List<Structures.ScaleNotePitch> scaleNotes = _scale.GetChromaticNotes(_lowestPitch, _highestPitch);

            Dictionary<Structures.NotePitch, int> lastNoteOfMeasureWeights = _weights.GetWeights(scaleNotes, true, false);
            Dictionary<Structures.NotePitch, int> middleNoteOfMeasureWeights = _weights.GetWeights(scaleNotes, false, false);
            Dictionary<Structures.NotePitch, int> firstNoteWeights = _weights.GetWeights(scaleNotes, false, true);
           
            List<int> noteDistanceWeights = _weights.GetDistanceWeights(scaleNotes.Count);
            //Reverse the order, because last note is normally to scale key.
            for (int loop = noteDurations.Count - 1; loop >= 0; loop--)
            {
                Structures.NoteDuration noteDuration = noteDurations[loop];
                if (loop == noteDurations.Count - 1) 
                {
                    //Last note on key
                    Structures.NotePitch notePitch = GetLastNotePitch();
                    notes.Add(new Structures.Note(noteDuration, notePitch));
                    latestNoteMidiNumber = notePitch.MidiNumber;
                    continue;
                }
                //Splitted note continue with same scalenote
                if (noteDuration.Tie == Structures.NoteDuration.LinkType.Continue || noteDuration.Tie == Structures.NoteDuration.LinkType.Start)
                {
                    notes.Add(new Structures.Note(noteDuration, new Structures.NotePitch(latestNoteMidiNumber, _scale.Sharp)));
                    continue;
                }
                Dictionary<Structures.NotePitch, int> noteWeights;

                if (loop == 0)
                    noteWeights = firstNoteWeights;
                else if (noteDuration.LastOfMeasure)
                    noteWeights = lastNoteOfMeasureWeights;
                else
                    noteWeights = middleNoteOfMeasureWeights;

                Dictionary<Structures.NotePitch, int> adjustedWeights = new Dictionary<Structures.NotePitch, int>();
                foreach(Structures.NotePitch notePitch in noteWeights.Keys)
                { 
                    int distance = Math.Abs(latestNoteMidiNumber - notePitch.MidiNumber);
                    adjustedWeights.Add(notePitch, Convert.ToInt32(Math.Round((decimal)noteWeights[notePitch] 
                        * (decimal)noteDistanceWeights[distance], MidpointRounding.AwayFromZero)));
                }
                Structures.NotePitch noteIndex = weightedRandom.GetRandomKey<Structures.NotePitch>(WeightedRandom.RandomType.Pitch, adjustedWeights);
                latestNoteMidiNumber = noteIndex.MidiNumber;
                notes.Add(new Structures.Note(noteDuration, new Structures.NotePitch(noteIndex.MidiNumber, _scale.Sharp)));
            }
            notes.Reverse();
            return notes;
        }

        private bool IsNoteInChord(Structures.NotePitch note, Structures.Chord chord)
        {
            foreach(Structures.NotePitch chordNote in chord.NotePitches)
            {
                if (chordNote.IsSameIgnoreOctave(note))
                    return true;
            }
            return false;
        }

        private decimal GetNoteWeight(Structures.Note note, Structures.Chord chord)
        {
            if (IsNoteInChord(note.Pitch, chord))
                return note.Duration * Input.WeightData.ChordDetermination_ChordNote;
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
                        return note.Duration * Input.WeightData.ChordDetermination_ScaleNote;
                }
            }
            return note.Duration * Input.WeightData.ChordDetermination_NonScaleNote;
        }

        private decimal CompareChord(IEnumerable<Structures.Note> notes, Structures.Chord chord)
        {
            decimal hits = 0;
            foreach (Structures.Note note in notes)
                hits += GetNoteWeight(note, chord);
            return hits;
        }

        public List<Structures.Chord> DetermineChords(List<Structures.Note> melodyNotes)
        {
            List<Structures.Chord> chords = new List<Structures.Chord>();
            IEnumerable<IGrouping<int, Structures.Note>> measures = melodyNotes.GroupBy(note => note.MeasureNumber);
            //First note of first measure must be in chord, sounds strange otherwise. 
            Structures.Note firstNoteUnhandled = melodyNotes.First();
            Structures.Chord latestChord = null;
            foreach(IGrouping<int, Structures.Note> measureNotes in measures)
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
        public Structures.SongOutput CreateSongPart()
        {
            _weightedRandom.Reset();

            Structures.RythmOutput rythmOutput = _rythmGenerator.CreateNoteDurations();
            List<Structures.Note> notes = AddMelody(_weightedRandom, rythmOutput.Durations);
            Structures.SongOutput songOutput = new Structures.SongOutput() { Scale = _scale, Melody = notes };
            if (Input.Chords)
            {
                songOutput.Chords = DetermineChords(notes);
                decimal duration = Structures.NoteDuration.NoteToDuration(Input.BeatUnit) * Input.BeatsPerMeasure;
                songOutput.ChordDuration = Structures.NoteDuration.DurationToNote(duration, out decimal extra);
            }
            return songOutput;
        }
        public void WriteToStream(FileGeneratorBase.FileType fileType, Structures.SongOutput songOutput, Stream target)
        {
            FileGeneratorBase generator;
            if (fileType == FileGeneratorBase.FileType.Midi)
                generator = new Midi.MidiGenerator(Input);
            else
                generator = new MusicXml.MusicXmlGenerator(Input);
            generator.WriteToStream(songOutput, target);
        }
    }
}
