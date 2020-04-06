using Music.Core;
using Music.Core.Scales.Diatonic;
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
        public List<Structures.NoteDuration> NoteDurations { get; private set; }
        private List<decimal> _beatStops;
        private decimal _measureLength;
        private WeightedRandom _weightedRandom;
        private ScaleBase _scale;
        public SongData SongData { get; private set; }

        public SongPartGenerator(SongData songData)
        {
            if (songData == null)
                throw new ArgumentNullException(nameof(songData));
            if (string.IsNullOrWhiteSpace(songData.Name))
                throw new ArgumentException($"{nameof(songData.Name)} is empty or null");
            if (string.IsNullOrWhiteSpace(songData.SongName))
                throw new ArgumentException($"{nameof(songData.SongName)} is empty or null");
            if (songData.PartLength <= 0)
                throw new ArgumentException($"{nameof(songData.PartLength)} must be positive value");
            if (songData.Major)
                _scale = new IonianScale(MusicNotes.FromString(songData.ScaleKey));
            else
                _scale = new AeolianScale(MusicNotes.FromString(songData.ScaleKey));
            if (songData.BeatsPerMeasure <= 0)
                throw new ArgumentException($"{nameof(songData.BeatsPerMeasure)} must be positive value");
            if (songData.Values == null)
                throw new ArgumentNullException(nameof(songData.Values));
            SongData = songData;
            _weightedRandom = new WeightedRandom(songData.Values);
            TotalLength = 0;
            NoteDurations = new List<Structures.NoteDuration>();
            _beatStops = new List<decimal>();
            decimal stopPosition = 0;
            for (int beatLoop = 0; beatLoop < songData.BeatsPerMeasure; beatLoop++)
            {
                switch (songData.BeatUnit)
                {
                    case Structures.NoteDuration.NoteLengthType.Half:
                        stopPosition += 0.5M;
                        break;
                    case Structures.NoteDuration.NoteLengthType.Quarter:
                        stopPosition += 0.25M;
                        break;
                    case Structures.NoteDuration.NoteLengthType.Eigth:
                        stopPosition += 0.125M;
                        break;
                    default:
                        throw new ArgumentException("Invalid beat unit, valid values are Half, Quarter and Eigth");
                }
                _beatStops.Add(stopPosition);
            }
            _measureLength = stopPosition;
        }

        public decimal RemainingBeforeStop()
        {
            int measureCount = Convert.ToInt32(Math.Floor(TotalLength / _measureLength));
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
            int measureCount = Convert.ToInt32(Math.Floor(TotalLength / _measureLength));
            return (measureCount + 1) * _measureLength - TotalLength;
        }
        public void AddNote(decimal duration, bool tie)
        {
            //Total length exceeded, adjust to maximum length
            if (duration + TotalLength > SongData.PartLength)
                AddNote(SongData.PartLength - TotalLength, false);
            else
            {
                decimal remainingBeforeStop = RemainingBeforeStop();
                Structures.NoteDuration noteDuration;
                decimal remaining;
                if (remainingBeforeStop < duration)
                {
                    noteDuration = Structures.NoteDuration.DurationToNote(remainingBeforeStop, out remaining);
                    remaining = duration - remainingBeforeStop;
                }
                else
                {
                    noteDuration = Structures.NoteDuration.DurationToNote(duration, out remaining);
                }
                NoteDurations.Add(noteDuration);
                if (RemainingBeforeNextMeasure() - noteDuration.Duration == 0)
                    noteDuration.LastOfMeasure = true;
                TotalLength += noteDuration.Duration;
                if (tie)
                    noteDuration.Tie = Structures.NoteDuration.TieType.End;
                if (remaining > 0)
                {
                    if (tie)
                        noteDuration.Tie = Structures.NoteDuration.TieType.Both;
                    else
                        noteDuration.Tie = Structures.NoteDuration.TieType.Start;
                    AddNote(remaining, true);
                }
            }
        }
        private List<Structures.Note> AddMelody(WeightedRandom weightedRandom, List<Structures.NoteDuration> noteDurations)
        {
            List<Structures.Note> notes = new List<Structures.Note>();
            int? latestNote = null;
            bool major = _scale is IonianScale ? true : false;
            int[] lastNoteOfMeasureWeights = major ? Weights.Instance.GetMajorWeightsLastNote() : Weights.Instance.GetMinorWeightsLastNote();
            int[] middleNoteOfMeasureWeights = major ? Weights.Instance.GetMajorWeightsMiddleNote() : Weights.Instance.GetMinorWeightsMiddleNote();
            
            Dictionary<int, int> noteDistanceWeights = Weights.Instance.NoteDistanceWeights;
            foreach (Structures.NoteDuration noteDuration in noteDurations)
            {
                //Splitted note continue with same scalenote
                if (noteDuration.Tie == Structures.NoteDuration.TieType.Both || noteDuration.Tie == Structures.NoteDuration.TieType.End)
                {
                    notes.Add(NoteFactory.GetNoteByScaleNote(noteDuration, _scale.ChromaticNotes[latestNote.Value]));
                    continue;
                }
                List<int> adjustedWeights;

                if (noteDuration.LastOfMeasure)
                {
                    adjustedWeights = lastNoteOfMeasureWeights.ToList();
                }
                else
                    adjustedWeights = middleNoteOfMeasureWeights.ToList();
                if (latestNote != null)
                {
                    for (int loop = 0; loop < adjustedWeights.Count; loop++)
                    {
                        int distance = Math.Abs(latestNote.Value - loop);
                        adjustedWeights[loop] = Convert.ToInt32(Math.Round((decimal)adjustedWeights[loop] * (decimal)noteDistanceWeights[distance], MidpointRounding.AwayFromZero));
                    }
                }
                int noteIndex = weightedRandom.GetRandomIndex(adjustedWeights);
                latestNote = noteIndex;
                ScaleNote scaleNote = _scale.ChromaticNotes[noteIndex];
                notes.Add(NoteFactory.GetNoteByScaleNote(noteDuration, scaleNote));
            }
            //Last note on key
            Structures.Note lastNote = notes.Last();
            lastNote.Name = _scale.Notes[0].Note.Name[0];
            lastNote.Offset = 0;
            return notes;
        }
        public List<Structures.Note> CreateSongPart()
        {
            _weightedRandom.Reset();
            TotalLength = 0;
            NoteDurations = new List<Structures.NoteDuration>();
            while (TotalLength < SongData.PartLength)
            {
                Structures.NoteDuration.NoteLengthType noteLengthType = _weightedRandom.GetRandomKey<Structures.NoteDuration.NoteLengthType>(Weights.Instance.LengthWeights);
                decimal duration = new Structures.NoteDuration(noteLengthType, false, Structures.NoteDuration.TieType.None, false).Duration;
                AddNote(duration, false);
            }
            List<Structures.Note> notes = AddMelody(_weightedRandom, NoteDurations);
            return notes;
        }
        public void WriteToStream(FileGeneratorBase.FileType fileType, List<Structures.Note> notes, Stream target)
        {
            FileGeneratorBase generator;
            if (fileType == FileGeneratorBase.FileType.Midi)
                generator = new Midi.MidiGenerator(SongData);
            else
                generator = new MusicXml.MusicXmlGenerator(SongData);
            generator.WriteToStream(notes, target);
        }
    }
}
