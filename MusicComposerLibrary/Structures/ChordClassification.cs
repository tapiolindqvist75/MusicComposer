using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public enum ChordClassification
    {
        /// <summary>
        /// C
        /// </summary>
        Major,
        /// <summary>
        /// Cm
        /// </summary>
        Minor,
        /// <summary>
        /// Cdim
        /// </summary>
        Diminished,
        /// <summary>
        /// Caug
        /// </summary>
        Augmented,
        /// <summary>
        /// Cmaj7
        /// </summary>
        MajorSeventh,
        /// <summary>
        /// Cm7
        /// </summary>
        MinorSeventh,
        /// <summary>
        /// C7
        /// </summary>
        DominantSeventh,
        /// <summary>
        /// Cdim7
        /// </summary>
        DiminishedSeventh,
        /// <summary>
        /// Cm7(b5)
        /// </summary>
        HalfDiminishedSeventh,
        /// <summary>
        /// Cm(maj7)
        /// </summary>
        MinorMajorSeventh,
        /// <summary>
        /// Cmaj7(#5)
        /// </summary>
        AugmentedMajorSeventh
    }
}
