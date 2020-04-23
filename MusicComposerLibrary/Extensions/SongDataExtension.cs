using MusicComposerLibrary.Storage;

namespace MusicComposerLibrary.Extensions
{
    public static class SongDataExtension
    {
        public static int GetBeatType(this SongInput songData)
        {
            return songData.BeatUnit switch
            {
                Structures.NoteDuration.NoteLengthType.Half => 2,
                Structures.NoteDuration.NoteLengthType.Quarter => 4,
                Structures.NoteDuration.NoteLengthType.Eigth => 8,
                _ => 16,
            };
            ;
        }
    }
}
