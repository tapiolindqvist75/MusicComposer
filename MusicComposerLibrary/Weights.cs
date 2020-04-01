using System.Collections.Generic;

namespace MusicComposerLibrary
{
    public class Weights
    {
        private static Weights _instance;
        public static Weights Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Weights();
                return _instance;
            }
        }

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
        private Weights()
        {
            LengthWeights = new Dictionary<Structures.NoteDuration.NoteLengthType, int>();
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Half, 1);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Quarter, 5);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Eigth, 4);
            LengthWeights.Add(Structures.NoteDuration.NoteLengthType.Sixteenth, 0);
            MiddleNoteOfMeasureWeights = new Dictionary<NoteWeightType, int>();
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.NonScaleNote, 1);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.ScaleNote, 50);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Tonic, 50);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Mediant, 50);
            MiddleNoteOfMeasureWeights.Add(NoteWeightType.Dominant, 50);
            LastNoteOfMeasureWeights = new Dictionary<NoteWeightType, int>();
            LastNoteOfMeasureWeights.Add(NoteWeightType.NonScaleNote, 1);
            LastNoteOfMeasureWeights.Add(NoteWeightType.ScaleNote, 50);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Tonic, 200);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Mediant, 100);
            LastNoteOfMeasureWeights.Add(NoteWeightType.Dominant, 150);
            NoteDistanceWeights = new Dictionary<int, int>();
            NoteDistanceWeights.Add(0, 5);
            NoteDistanceWeights.Add(1, 10);
            NoteDistanceWeights.Add(2, 8);
            NoteDistanceWeights.Add(3, 4);
            NoteDistanceWeights.Add(4, 2);
            NoteDistanceWeights.Add(5, 1);
            NoteDistanceWeights.Add(6, 1);
            NoteDistanceWeights.Add(7, 1);
            NoteDistanceWeights.Add(8, 1);
            NoteDistanceWeights.Add(9, 1);
            NoteDistanceWeights.Add(10, 1);
            NoteDistanceWeights.Add(11, 1);
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
