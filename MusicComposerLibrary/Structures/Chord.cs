using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class Chord
    {   
        public Chord(List<NotePitch> notePitches, ChordClassification classification)
        {
            NotePitches = notePitches;
            Classification = classification;
            switch(Classification)
            {
                case ChordClassification.Augmented:
                case ChordClassification.Diminished:
                case ChordClassification.Major:
                case ChordClassification.Minor:
                    if (NotePitches.Count != 3)
                        throw new ArgumentException("Classification is triad and notePiches count != 3");
                    break;
                case ChordClassification.AugmentedMajorSeventh:
                case ChordClassification.DiminishedSeventh:
                case ChordClassification.DominantSeventh:
                case ChordClassification.HalfDiminishedSeventh:
                case ChordClassification.MajorSeventh:
                case ChordClassification.MinorMajorSeventh:
                case ChordClassification.MinorSeventh:
                    if (NotePitches.Count != 4)
                        throw new ArgumentException("Classification is seventh chord and notePitches count != 4");
                    break;
                default:
                    throw new ArgumentException("Invalid value Classification");
            }
        }
        public List<NotePitch> NotePitches { get; private set; }
        public ChordClassification Classification { get; private set; }
        public string GetClassificationString()
        {
            switch(Classification)
            {
                case ChordClassification.Augmented:
                    return "aug";
                case ChordClassification.AugmentedMajorSeventh:
                    return "maj7(#5)";
                case ChordClassification.Diminished:
                    return "dim";
                case ChordClassification.DiminishedSeventh:
                    return "dim7";
                case ChordClassification.DominantSeventh:
                    return "7";
                case ChordClassification.HalfDiminishedSeventh:
                    return "m7(b5)";
                case ChordClassification.Major:
                    return String.Empty;
                case ChordClassification.MajorSeventh:
                    return "maj7";
                case ChordClassification.Minor:
                    return "m";
                case ChordClassification.MinorMajorSeventh:
                    return "m(maj7)";
                case ChordClassification.MinorSeventh:
                    return "m7";
                default:
                    throw new ArgumentException("Invalid value Classification");
            }
        }
        public string FullName
        {
            get
            {
                if (NotePitches.Count > 0)
                    return NotePitches[0].FullName + GetClassificationString();
                throw new Exception("NotePitches not initialized");
            }
        }
    }
}
