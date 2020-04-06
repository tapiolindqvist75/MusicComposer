using System;
using MusicComposerLibrary.Storage;
using Music.Core;
using Music.Core.Scales.Diatonic;
using System.Linq;

namespace MusicComposerLibrary.Extensions
{
    public static class SongDataExtension
    {
        public static int GetKey(this SongData songData)
        {
            ScaleBase scale;
            if (songData.Major)
                scale = new IonianScale(MusicNotes.FromString(songData.ScaleKey));
            else
                scale = new AeolianScale(MusicNotes.FromString(songData.ScaleKey));
            int key = scale.Notes.Sum(n => n.Note.Label.Offset);
            return key;
        }

        public static int GetBeatType(this SongData songData)
        {
            switch(songData.BeatUnit)
            {
                case Structures.NoteDuration.NoteLengthType.Half:
                    return 2;
                case Structures.NoteDuration.NoteLengthType.Quarter:
                    return 4;
                case Structures.NoteDuration.NoteLengthType.Eigth:
                    return 8;
                default:
                    return 16;
            };
        }
    }
}
