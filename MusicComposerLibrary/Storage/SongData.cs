using Music.Core;
using MusicComposerLibrary.Structures;
using System.Text.Json;

namespace MusicComposerLibrary.Storage
{
    public class SongData
    {
        public string Name { get; set; }
        public string SongName { get; set; }
        public string ScaleKey { get; set; }
        public bool Major { get; set; }
        public double[] Values { get; set; }
        public int PartLength { get; set; }
        public int BeatsPerMeasure { get; set; }
        public NoteDuration.NoteLengthType BeatUnit { get; set; }
        public static string Serialize(SongData songData)
        {
            return JsonSerializer.Serialize<SongData>(songData);
        }
        public static SongData Deserialize(string data)
        {
            return JsonSerializer.Deserialize<SongData>(data);
        }
    }
}
