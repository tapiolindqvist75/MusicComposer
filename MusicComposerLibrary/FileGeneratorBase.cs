using MusicComposerLibrary.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MusicComposerLibrary
{
    public abstract class FileGeneratorBase
    {
        public enum FileType { MusicXml, Midi };
        protected SongData SongData { get; private set; }
        public FileGeneratorBase(SongData songData)
        {
            SongData = songData;
        }
        public abstract void WriteToStream(List<Structures.Note> notes, Stream target);
    }
}
