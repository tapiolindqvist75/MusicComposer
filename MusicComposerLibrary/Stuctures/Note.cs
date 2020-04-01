using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Structures
{
    public class Note : NoteDuration
    {
        public Note(NoteDuration noteDuration, char name = 'C', short offset = 0) : base(noteDuration.NoteLength, noteDuration.Dot, noteDuration.Tie, noteDuration.LastOfMeasure)
        {
            base.LastOfMeasure = noteDuration.LastOfMeasure;
            Name = name;
            Offset = offset;
        }
        public short Offset { get; set; }
        public char Name { get; set; }
        
    }
}
