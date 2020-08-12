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
            SongInput songData = new SongInput()
            {
                BeatsPerMeasure = 4,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                Major = new Random().Next(0, 2) == 0 ? true : false,
                Name = "Tapio Lindqvist",
                PartLength = 4,
                ScaleKeyFullName = "C",
                SongName = songTitle,
                DurationValues = WeightedRandom.GetRandomValues(10),
                PitchValues = WeightedRandom.GetRandomValues(10),
                WeightData = WeightData.GetDefaults()
            };

            string doh = SongInput.Serialize(songData);

            SongPartGenerator generator = new SongPartGenerator(songData);
            var notes = generator.CreateSongPart();
            using (StreamWriter writer = new StreamWriter($"{songTitle}.musicxml"))
            {
                generator.WriteToStream(FileGeneratorBase.FileType.MusicXml, notes, writer.BaseStream);
            }
        }
    }
}
