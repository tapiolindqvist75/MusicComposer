using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary
{
    public static class NoteExtension
    {
        public static string NoteString(this Music.Core.ScaleNote note)
        {
            return note.Note.NoteString();
        }
        public static string NoteString(this Music.Core.Note note)
        {
            if (note.Label.Offset == -1)
                return note.Name + "b";
            else if (note.Label.Offset == +1)
                return note.Name + "#";
            return note.Name;
        }
    }
}
