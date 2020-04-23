using MusicComposerLibrary.Structures;
using System.Text.Json;

namespace MusicComposerLibrary.Storage
{
    public class SongInput
    {
        public string Name { get; set; }
        public string SongName { get; set; }
        public bool Major { get; set; }
        public string ScaleKeyFullName { get; set; }
        public string LowestNoteFullName { get; set; }
        public int LowestNoteOctave { get; set; }
        public string HighestNoteFullName { get; set; }
        public int HighestNoteOctave { get; set; }
        public double[] Values { get; set; }
        public WeightData WeightData { get; set; }
        public int PartLength { get; set; }
        public int BeatsPerMeasure { get; set; }
        public NoteDuration.NoteLengthType BeatUnit { get; set; }
        public bool Chords { get; set; }
        public static string Serialize(SongInput songData)
        {
            return JsonSerializer.Serialize<SongInput>(songData);
        }
        public static SongInput Deserialize(string data)
        {
            return JsonSerializer.Deserialize<SongInput>(data);
        }
    }
}
