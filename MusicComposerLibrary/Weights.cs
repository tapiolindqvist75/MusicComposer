using System.Collections.Generic;

namespace MusicComposerLibrary
{
    public class Weights
    {
        public enum NoteWeightType
        {
            NonScaleNote,
            ScaleNote,
            Tonic,
            Mediant,
            Dominant
        }
        public Dictionary<Structures.NoteDuration.NoteLengthType, int> LengthWeights { get; private set; }
        public Dictionary<NoteWeightType, int> MiddleNoteOfMeasureWeights { get; private set; }
        public Dictionary<NoteWeightType, int> LastNoteOfMeasureWeights { get; private set; }
        public Dictionary<int, int> NoteDistanceWeights { get; set; }
        public Weights(Storage.WeightData weightData)
        {
            LengthWeights = new Dictionary<Structures.NoteDuration.NoteLengthType, int>();
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Half, weightData.LengthHalfWeight);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Quarter, weightData.LengthQuarterWeight);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Eigth, weightData.LengthEightWeight);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Sixteenth, weightData.LengthSixteenthWeight);
            MiddleNoteOfMeasureWeights = new Dictionary<NoteWeightType, int>();
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.NonScaleNote, weightData.MiddleNonScale);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.ScaleNote, weightData.MiddleScale);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Tonic, weightData.MiddleTonic_1);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Mediant, weightData.MiddleMediant_3);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Dominant, weightData.MiddleDominant_5);
            LastNoteOfMeasureWeights = new Dictionary<NoteWeightType, int>();
            LastNoteOfMeasureWeights.Add(NoteWeightType.NonScaleNote, weightData.LastNonScale);
            LastNoteOfMeasureWeights.Add(NoteWeightType.ScaleNote, weightData.LastScale);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Tonic, weightData.LastTonic_1);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Mediant, weightData.LastMediant_3);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Dominant, weightData.LastDominant_5);
            NoteDistanceWeights = new Dictionary<int, int>();
            NoteDistanceWeights.Add(0, weightData.Distance_0);
            NoteDistanceWeights.Add(1, weightData.Distance_1);
            NoteDistanceWeights.Add(2, weightData.Distance_2);
            NoteDistanceWeights.Add(3, weightData.Distance_3);
            NoteDistanceWeights.Add(4, weightData.Distance_4);
            NoteDistanceWeights.Add(5, weightData.Distance_5);
            NoteDistanceWeights.Add(6, weightData.Distance_6);
            NoteDistanceWeights.Add(7, weightData.Distance_7AndUp);
            NoteDistanceWeights.Add(8, weightData.Distance_7AndUp);
            NoteDistanceWeights.Add(9, weightData.Distance_7AndUp);
            NoteDistanceWeights.Add(10, weightData.Distance_7AndUp);
            NoteDistanceWeights.Add(11, weightData.Distance_7AndUp);
        }

        private int[] GetMinorWeights(Dictionary<NoteWeightType, int> weights)
        {
            return new int[12]
            {
                weights[NoteWeightType.Tonic], //A
                weights[NoteWeightType.NonScaleNote], //A#
                weights[NoteWeightType.ScaleNote], //B
                weights[NoteWeightType.Mediant], // C
                weights[NoteWeightType.NonScaleNote], // C#
                weights[NoteWeightType.ScaleNote], // D
                weights[NoteWeightType.NonScaleNote], // D#
                weights[NoteWeightType.Dominant], //E
                weights[NoteWeightType.ScaleNote], //F
                weights[NoteWeightType.NonScaleNote], //F#
                weights[NoteWeightType.ScaleNote], //G
                weights[NoteWeightType.NonScaleNote], //G#
            };
        }
        public int[] GetMinorWeightsMiddleNote()
        {
            return GetMinorWeights(MiddleNoteOfMeasureWeights);
        }
        public int[] GetMinorWeightsLastNote()
        {
            return GetMinorWeights(LastNoteOfMeasureWeights);
        }
        private int[] GetMajorWeights(Dictionary<NoteWeightType, int> weights)
        {
            return new int[12]
            {
                weights[NoteWeightType.Tonic], //C
                weights[NoteWeightType.NonScaleNote], //C#
                weights[NoteWeightType.ScaleNote], //D
                weights[NoteWeightType.NonScaleNote], //D#
                weights[NoteWeightType.Mediant], //E
                weights[NoteWeightType.ScaleNote], //F
                weights[NoteWeightType.NonScaleNote], //F#
                weights[NoteWeightType.Dominant], //G
                weights[NoteWeightType.NonScaleNote], //G#
                weights[NoteWeightType.ScaleNote], //A
                weights[NoteWeightType.NonScaleNote], //A
                weights[NoteWeightType.ScaleNote] //B
            };
        }
        public int[] GetMajorWeightsMiddleNote()
        {
            return GetMajorWeights(MiddleNoteOfMeasureWeights);
        }
        public int[] GetMajorWeightsLastNote()
        {
            return GetMajorWeights(LastNoteOfMeasureWeights);
        }
    }
}
