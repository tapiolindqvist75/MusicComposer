using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage
{
    public interface IStorageHandler
    {
        void SaveSongData(SongInput song);
        SongInput RetrieveSongData(string name, string songName);
        void SetRating(string name, string songName, int rating);
        void SetFileCreated(string name, string songName, FileGeneratorBase.FileType fileType);
    }
}
