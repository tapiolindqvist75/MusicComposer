using MusicComposerLibrary.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Generators
{
    public class RythmGenerator
    {
        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        public decimal _totalLength;
        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        public List<Structures.NoteDuration> _noteDurations;
        
        private readonly int _partLength;
        private readonly List<decimal> _beatStops;
        private readonly decimal _measureLength;
        private readonly WeightedRandom _weightedRandom;
        private readonly Weights _weights;
        public RythmGenerator(WeightedRandom weightedRandom, Weights weights, int partLength, int beatsPerMeasure, Structures.NoteDuration.NoteLengthType beatUnit)
        {
            if (beatsPerMeasure <= 0)
                throw new ArgumentException($"{nameof(beatsPerMeasure)} must be positive value");
            if (partLength <= 0)
                throw new ArgumentException($"{nameof(partLength)} must be positive value");
            _weightedRandom = weightedRandom;
            _weights = weights;
            _partLength = partLength;
            _beatStops = new List<decimal>();
            decimal stopPosition = 0;
            for (int beatLoop = 0; beatLoop < beatsPerMeasure; beatLoop++)
            {
                stopPosition += beatUnit switch
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

        public RythmOutput CreateNoteDurations()
        {
            _weightedRandom.Reset();
            _totalLength = 0;
            _noteDurations = new List<NoteDuration>();

            List<Structures.NoteDuration> noteDurations = new List<Structures.NoteDuration>();
            while (_totalLength < _partLength)
            {
                Structures.NoteDuration.NoteLengthType noteLengthType = _weightedRandom.GetRandomKey<Structures.NoteDuration.NoteLengthType>(WeightedRandom.RandomType.Duration, 
                    _weights.LengthWeights);
                decimal duration = new Structures.NoteDuration(noteLengthType, false, Structures.NoteDuration.LinkType.None).Duration;
                AddNote(duration, false, false);
            };
            SetBeams();
            return new RythmOutput() { TotalLength = _totalLength, Durations = _noteDurations };
        }

        private void SetBeams()
        {
            for (int loop = 0; loop < _noteDurations.Count - 1; loop++)
            {
                Structures.NoteDuration current = _noteDurations[loop];
                Structures.NoteDuration next = _noteDurations[loop + 1];
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

        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        public decimal RemainingBeforeStop()
        {
            int measureCount = GetMeasureCount();
            decimal fraction = _totalLength - (measureCount * _measureLength);
            foreach (decimal stop in _beatStops)
            {
                if (stop > fraction)
                    return stop - fraction;
            }
            //Should not happen
            throw new Exception("Invalid definitions");
        }

        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        public decimal RemainingBeforeNextMeasure()
        {
            int measureCount = GetMeasureCount();
            return (measureCount + 1) * _measureLength - _totalLength;
        }

        private int GetMeasureCount()
        {
            return Convert.ToInt32(Math.Floor(_totalLength / _measureLength));
        }

        /// <summary>
        /// Public for UnitTesting
        /// </summary>
        public void AddNote(decimal duration, bool tie, bool lastNoteOfMeasure)
        {
            //Total length exceeded, adjust to maximum length
            if (duration + _totalLength > _partLength)
                AddNote(_partLength - _totalLength, false, true);
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
                _noteDurations.Add(noteDuration);
                noteDuration.MeasureNumber = GetMeasureCount();
                if (RemainingBeforeNextMeasure() - noteDuration.Duration == 0)
                    noteDuration.LastOfMeasure = true;
                _totalLength += noteDuration.Duration;
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
                    AddNote(remaining, true, noteDuration.LastOfMeasure);
                }
            }
        }
    }
}
