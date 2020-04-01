using Music.Core;

namespace MusicComposerLibrary
{
    public class NoteFactory
    {
        public static Structures.Note GetNoteByScaleNote(Structures.NoteDuration noteDuration, ScaleNote scaleNote)
        {
            return new Structures.Note(noteDuration, scaleNote.Note.Name[0], (short)scaleNote.Note.Label.Offset);
        }
    }
}
