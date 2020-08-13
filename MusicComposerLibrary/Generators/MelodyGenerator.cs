using MusicComposerLibrary.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Generators
{
    public class MelodyGenerator
    {
        private readonly Scale _scale;
        private readonly Weights _weights;
        private readonly WeightedRandom _weightedRandom;
        private readonly Structures.NotePitch _lowestPitch;
        private readonly Structures.NotePitch _highestPitch;
        public MelodyGenerator(WeightedRandom weightedRandom, Weights weights, Scale scale, string melodyLowestNoteFullNameWithOctave, string melodyHighestNoteFullNameWithOctave)
        {
            _scale = scale;
            _weights = weights;
            _weightedRandom = weightedRandom;
            if (string.IsNullOrWhiteSpace(melodyLowestNoteFullNameWithOctave))
                _lowestPitch = new Structures.NotePitch("A3");
            else
                _lowestPitch = new Structures.NotePitch(melodyLowestNoteFullNameWithOctave);
            if (string.IsNullOrWhiteSpace(melodyHighestNoteFullNameWithOctave))
                _highestPitch = new Structures.NotePitch("C6");
            else
                _highestPitch = new Structures.NotePitch(melodyHighestNoteFullNameWithOctave);
        }
        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        /// <returns></returns>
        public Structures.NotePitch GetLastNotePitch()
        {
            Tuple<int, Structures.NotePitch> minimumDifference = new Tuple<int, Structures.NotePitch>(int.MaxValue, null);
            for (int loop = _lowestPitch.Octave; loop < _highestPitch.Octave; loop++)
            {
                Structures.NotePitch current = _scale.GetDegreePitch(1, loop);
                int difference = Math.Abs((current.MidiNumber - _lowestPitch.MidiNumber) - (_highestPitch.MidiNumber - current.MidiNumber));
                if (difference < minimumDifference.Item1)
                    minimumDifference = new Tuple<int, Structures.NotePitch>(difference, current);
            }
            return minimumDifference.Item2;
        }

        public List<Structures.Note> AddMelody(List<Structures.NoteDuration> noteDurations)
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
                foreach (Structures.NotePitch notePitch in noteWeights.Keys)
                {
                    int distance = Math.Abs(latestNoteMidiNumber - notePitch.MidiNumber);
                    adjustedWeights.Add(notePitch, Convert.ToInt32(Math.Round((decimal)noteWeights[notePitch]
                        * (decimal)noteDistanceWeights[distance], MidpointRounding.AwayFromZero)));
                }
                Structures.NotePitch noteIndex = _weightedRandom.GetRandomKey<Structures.NotePitch>(WeightedRandom.RandomType.Pitch, adjustedWeights);
                latestNoteMidiNumber = noteIndex.MidiNumber;
                notes.Add(new Structures.Note(noteDuration, new Structures.NotePitch(noteIndex.MidiNumber, _scale.Sharp)));
            }
            notes.Reverse();
            return notes;
        }
    }
}
