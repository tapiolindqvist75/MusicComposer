using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class Note : NoteDuration
    {
        public NotePitch Pitch { get; private set; }
        public Chord Chord { get; set; }
        public Note(NoteDuration noteDuration, NotePitch notePitch) 
            : base(noteDuration)
        {
            Pitch = notePitch;
        }
    }
}
