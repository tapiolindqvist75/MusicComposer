using System;
using System.Collections.Generic;
using System.Text;
using static MusicComposerLibrary.Weights;

namespace MusicComposerLibrary.Structures
{
    public class ScaleNotePitch : NotePitch
    {
        public NoteScaleType ScaleType { get; private set; }
        public ScaleNotePitch(int midiNumber, bool sharp, NoteScaleType noteScaleType) : base(midiNumber, sharp)
        {
            ScaleType = noteScaleType;
        }
    }
}
