using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage
{
    public interface IStorageHandler
    {
        void SaveSongData(SongData song);
        SongData RetrieveSongData(string name, string songName);
        void SetRating(string name, string songName, int rating);
    }
}
