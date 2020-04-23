using System.Collections.Generic;

namespace MusicComposerLibrary
{
    public class Weights
    {
        public Dictionary<Structures.NoteDuration.NoteLengthType, int> LengthWeights { get; private set; }
        private Dictionary<Structures.NoteScaleType, int> _middleNoteOfMeasureWeights { get; set; }
        private Dictionary<Structures.NoteScaleType, int> _lastNoteOfMeasureWeights { get; set; }
        private List<int> _noteDistanceWeights { get; set; }
        public Weights(Storage.WeightData weightData)
        {
            LengthWeights = new Dictionary<Structures.NoteDuration.NoteLengthType, int>
            {
                { Structures.NoteDuration.NoteLengthType.Half, weightData.LengthHalfWeight },
                { Structures.NoteDuration.NoteLengthType.Quarter, weightData.LengthQuarterWeight },
                { Structures.NoteDuration.NoteLengthType.Eigth, weightData.LengthEightWeight },
                { Structures.NoteDuration.NoteLengthType.Sixteenth, weightData.LengthSixteenthWeight }
            };
            _middleNoteOfMeasureWeights = new Dictionary<Structures.NoteScaleType, int>
            {
                { Structures.NoteScaleType.NonScaleNote, weightData.MiddleNonScale },
                { Structures.NoteScaleType.ScaleNote, weightData.MiddleScale },
                { Structures.NoteScaleType.Tonic, weightData.MiddleTonic_1 },
                { Structures.NoteScaleType.Mediant, weightData.MiddleMediant_3 },
                { Structures.NoteScaleType.Dominant, weightData.MiddleDominant_5 }
            };
            _lastNoteOfMeasureWeights = new Dictionary<Structures.NoteScaleType, int>
            {
                { Structures.NoteScaleType.NonScaleNote, weightData.LastNonScale },
                { Structures.NoteScaleType.ScaleNote, weightData.LastScale },
                { Structures.NoteScaleType.Tonic, weightData.LastTonic_1 },
                { Structures.NoteScaleType.Mediant, weightData.LastMediant_3 },
                { Structures.NoteScaleType.Dominant, weightData.LastDominant_5 }
            };
            _noteDistanceWeights = new List<int>
            {
                weightData.Distance_0,
                weightData.Distance_1,
                weightData.Distance_2,
                weightData.Distance_3,
                weightData.Distance_4,
                weightData.Distance_5,
                weightData.Distance_6,
                weightData.Distance_7
            };
        }

        private void AddIfTypeMatches(Structures.ScaleNotePitch scaleNote, Structures.NoteScaleType scaleType, 
            Dictionary<Structures.NotePitch, int> weights, bool lastNote, bool nonScaleNotes0Weight)
        {
            if (scaleNote.ScaleType == scaleType)
            {
                if (nonScaleNotes0Weight && scaleType == Structures.NoteScaleType.NonScaleNote)
                    weights.Add(scaleNote, 0);
                else
                    weights.Add(scaleNote, lastNote ?
                        _lastNoteOfMeasureWeights[scaleType] :
                        _middleNoteOfMeasureWeights[scaleType]);
            }
        }
        public Dictionary<Structures.NotePitch, int> GetWeights(List<Structures.ScaleNotePitch> scaleNotes, bool lastNote, bool nonScaleNotes0Weight)
        {
            Dictionary<Structures.NotePitch, int> weights = new Dictionary<Structures.NotePitch, int>();
            foreach(Structures.ScaleNotePitch scaleNote in scaleNotes)
            {
                AddIfTypeMatches(scaleNote, Structures.NoteScaleType.Tonic, weights, lastNote, nonScaleNotes0Weight);
                AddIfTypeMatches(scaleNote, Structures.NoteScaleType.Mediant, weights, lastNote, nonScaleNotes0Weight);
                AddIfTypeMatches(scaleNote, Structures.NoteScaleType.Dominant, weights, lastNote, nonScaleNotes0Weight);
                AddIfTypeMatches(scaleNote, Structures.NoteScaleType.ScaleNote, weights, lastNote, nonScaleNotes0Weight);
                AddIfTypeMatches(scaleNote, Structures.NoteScaleType.NonScaleNote, weights, lastNote, nonScaleNotes0Weight);
            }
            return weights;
        }

        public List<int> GetDistanceWeights(int maxDistance)
        {
            for (int loop = 0; loop < maxDistance; loop++)
                _noteDistanceWeights.Add(0);
            return _noteDistanceWeights;
        }
    }
}
