using MusicComposerLibrary.Structures;
using MusicComposerLibrary;
using System;
using System.IO;
using MusicComposerLibrary.Storage;

namespace MusicComposer
{
    class Program
    {
        static void Main(string[] args)
        {
            string songTitle = "MC " + DateTime.Now.ToString("yyyyMMddHHmmss");
            SongData songData = new SongData()
            {
                BeatsPerMeasure = 4,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                Major = new Random().Next(0, 2) == 0 ? true : false,
                Name = "Tapio Lindqvist",
                PartLength = 4,
                ScaleKey = "C",
                SongName = songTitle,
                Values = WeightedRandom.GetRandomValues()
            };
            SongPartGenerator generator = new SongPartGenerator(songData);
            var notes = generator.CreateSongPart();
            using (StreamWriter writer = new StreamWriter($"{songTitle}.musicxml"))
            {
                generator.WriteMusicXmlToStream(notes, writer.BaseStream);
            }
        }
    }
}
