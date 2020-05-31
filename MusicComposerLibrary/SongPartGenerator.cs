using MusicComposerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicComposerLibrary
{
    public class SongPartGenerator
    {
        public decimal TotalLength { get; set; }
        private readonly List<decimal> _beatStops;
        private readonly decimal _measureLength;
        private readonly WeightedRandom _weightedRandom;
        private readonly Structures.Scale _scale;
        private readonly Weights _weights;
        private readonly Structures.NotePitch _lowestPitch;
        private readonly Structures.NotePitch _highestPitch;
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
            if (songInput.Values == null)
                throw new ArgumentNullException(nameof(songInput.Values));
            if (songInput.WeightData == null)
                throw new ArgumentNullException(nameof(songInput.WeightData));
            Input = songInput;
            _weightedRandom = new WeightedRandom(songInput.Values);
            _weights = new Weights(songInput.WeightData);
            TotalLength = 0;
            _beatStops = new List<decimal>();
            decimal stopPosition = 0;
            for (int beatLoop = 0; beatLoop < songInput.BeatsPerMeasure; beatLoop++)
            {
                stopPosition += songInput.BeatUnit switch
                {
                    Structures.NoteDuration.NoteLengthType.Half => 0.5M,
                    Structures.NoteDuration.NoteLengthType.Quarter => 0.25M,
                    Structures.NoteDuration.NoteLengthType.Eigth => 0.125M,
                    _ => throw new ArgumentException("Invalid beat unit, valid values are Half, Quarter and Eigth"),
                };
                _beatStops.Add(stopPosition);
            }
            _measureLength = stopPosition;
        }

        public decimal RemainingBeforeStop()
        {
            int measureCount = GetMeasureCount();
            decimal fraction = TotalLength - (measureCount * _measureLength);
            foreach (decimal stop in _beatStops)
            {
                if (stop > fraction)
                    return stop - fraction;
            }
            //Should not happen
            throw new Exception("Invalid definitions");
        }

        public decimal RemainingBeforeNextMeasure()
        {
            int measureCount = GetMeasureCount();
            return (measureCount + 1) * _measureLength - TotalLength;
        }

        public int GetMeasureCount()
        {
            return Convert.ToInt32(Math.Floor(TotalLength / _measureLength));
        }
        public void AddNote(decimal duration, bool tie, bool lastNoteOfMeasure, List<Structures.NoteDuration> noteDurations)
        {
            //Total length exceeded, adjust to maximum length
            if (duration + TotalLength > Input.PartLength)
                AddNote(Input.PartLength - TotalLength, false, true, noteDurations);
            else
            {
                decimal remainingBeforeStop = RemainingBeforeStop();
                Structures.NoteDuration noteDuration;
                decimal remaining;
                if (remainingBeforeStop < duration)
                {
                    noteDuration = Structures.NoteDuration.DurationToNote(remainingBeforeStop, out _);
                    remaining = duration - remainingBeforeStop;
                }
                else
                {
                    noteDuration = Structures.NoteDuration.DurationToNote(duration, out remaining);
                }
                noteDurations.Add(noteDuration);
                noteDuration.MeasureNumber = GetMeasureCount();
                if (RemainingBeforeNextMeasure() - noteDuration.Duration == 0)
                    noteDuration.LastOfMeasure = true;
                TotalLength += noteDuration.Duration;
                if (tie)
                {
                    noteDuration.Tie = Structures.NoteDuration.LinkType.End;
                    //If note is tied and it is tied to the last note of measure. Then it is the first note on next measure
                    //But it needs to be treated as last according to weight
                    //If note is tied and it is last note of measure, the weight must be according to the fact tied two notes combined are last note of measure
                    //The tied note is not needed to be updated as last because melody is set in reverse order.
                    if (lastNoteOfMeasure)
                        noteDuration.LastOfMeasure = true;
                }
                if (remaining > 0)
                {
                    if (tie)
                        noteDuration.Tie = Structures.NoteDuration.LinkType.Continue;
                    else
                        noteDuration.Tie = Structures.NoteDuration.LinkType.Start;
                    AddNote(remaining, true, noteDuration.LastOfMeasure, noteDurations);
                }
            }
        }

        public void SetBeams(List<Structures.NoteDuration> noteDurations)
        {
            for(int loop = 0; loop < noteDurations.Count - 1; loop++)
            {
                Structures.NoteDuration current = noteDurations[loop];
                Structures.NoteDuration next = noteDurations[loop + 1];
                if (current.Beam != Structures.NoteDuration.LinkType.End && next.NoteLength == Structures.NoteDuration.NoteLengthType.Eigth)
                {
                    current.Beam = Structures.NoteDuration.LinkType.Continue;
                    next.Beam = Structures.NoteDuration.LinkType.End;
                }
                else if (current.NoteLength == Structures.NoteDuration.NoteLengthType.Eigth && next.NoteLength == Structures.NoteDuration.NoteLengthType.Eigth)
                {
                    current.Beam = Structures.NoteDuration.LinkType.Start;
                    next.Beam = Structures.NoteDuration.LinkType.End;
                }
            }
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
                Structures.NotePitch noteIndex = weightedRandom.GetRandomKey<Structures.NotePitch>(adjustedWeights);
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
                    throw new Exception("Chord could not be resolved. Invalid input, best guess is that first note of the song is not non-scale note");
                chords.Add(bestMatch.Item2);
                latestChord = bestMatch.Item2;
                measureNotes.First().Chord = bestMatch.Item2;
            }
            return chords;
        }
        public Structures.SongOutput CreateSongPart()
        {
            _weightedRandom.Reset();
            TotalLength = 0;
            List<Structures.NoteDuration> noteDurations = new List<Structures.NoteDuration>();
            while (TotalLength < Input.PartLength)
            {
                Structures.NoteDuration.NoteLengthType noteLengthType = _weightedRandom.GetRandomKey<Structures.NoteDuration.NoteLengthType>(_weights.LengthWeights);
                decimal duration = new Structures.NoteDuration(noteLengthType, false, Structures.NoteDuration.LinkType.None).Duration;
                AddNote(duration, false, false, noteDurations);
            };
            SetBeams(noteDurations);
            List<Structures.Note> notes = AddMelody(_weightedRandom, noteDurations);
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
