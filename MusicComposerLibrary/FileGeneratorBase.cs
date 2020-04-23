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
        protected SongInput Input { get; private set; }
        public FileGeneratorBase(SongInput input)
        {
            Input = input;
        }
        public abstract void WriteToStream(Structures.SongOutput songOutput, Stream target);
    }
}
