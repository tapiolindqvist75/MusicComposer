using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class RythmOutput
    {
        public decimal TotalLength { get; set; }
        public List<NoteDuration> Durations { get; set; }
    }
}
