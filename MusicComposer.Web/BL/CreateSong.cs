using MusicComposerLibrary;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MusicComposer.Web.BL
{
    public class CreateSong
    {
        private static string GetCacheKey(string name, string songName)
        {
            return $"Song{name}{songName}";
        }

        public static void CreateAndStoreSong(string name, string songName,
            IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            SongData songData = new SongData()
            {
                BeatsPerMeasure = 4,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                Major = true,
                Name = name,
                SongName = songName,
                PartLength = 4,
                ScaleKey = "C",
                Values = WeightedRandom.GetRandomValues()
            };
            storageHandler.SaveSongData(songData);
            memoryCache.Set<SongData>(GetCacheKey(name, songName), songData);
        }

        private static SongData GetSongData(string name, string songName, IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            SongData songData;
            if (!memoryCache.TryGetValue<SongData>(GetCacheKey(name, songName), out songData))
                songData = storageHandler.RetrieveSongData(name, songName);
            return songData;
        }

        public static byte[] GetMusicXml(string name, string songName,
            IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            MemoryStream memoryStream = new MemoryStream();
            SongData songData = GetSongData(name, songName, storageHandler, memoryCache);
            byte[] musicXmlBytes;
            if (!memoryCache.TryGetValue<byte[]>(GetCacheKey(name, songName), out musicXmlBytes))
            {
                SongPartGenerator generator = new SongPartGenerator(songData);
                var song = generator.CreateSongPart();
                generator.WriteMusicXmlToStream(song, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                memoryStream.Read(musicXmlBytes, 0, (int)memoryStream.Length);
            }
            return musicXmlBytes;
        }
    }
}
