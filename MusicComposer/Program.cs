using MusicComposerLibrary.Structures;
using MusicComposerLibrary;
using System;
using System.IO;

namespace MusicComposer
{
    class Program
    {
        static void Main(string[] args)
        {
            string songTitle = "MC " + DateTime.Now.ToString("yyyyMMddHHmmss");
            using (StreamWriter writer = new StreamWriter($"{songTitle}.musicxml"))
            {
                SongPartGenerator generator = new SongPartGenerator(4, 4, NoteDuration.NoteLengthType.Quarter);
                generator.CreateSongPart(writer.BaseStream, songTitle);
            }
        }
    }
}
