using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class SongOutput
    {
        public Scale Scale { get; set; }
        public List<Note> Melody { get; set; }
        public List<Chord> Chords { get; set; }
        public NoteDuration ChordDuration { get; set; }
    }
}
