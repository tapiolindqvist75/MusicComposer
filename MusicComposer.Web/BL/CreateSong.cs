using MusicComposerLibrary;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary.Structures;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace MusicComposer.Web.BL
{
    public class CreateSong
    {
        private static string GetCacheKey(string name, string songName, string type)
        {
            return $"Song{name}{songName}{type}";
        }

        public static void CreateAndStoreSong(SongInput songData, IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            if (storageHandler != null)
                storageHandler.SaveSongData(songData);
            memoryCache.Set<SongInput>(GetCacheKey(songData.Name, songData.SongName, nameof(songData)), songData);
        }

        private static SongInput GetSongData(string name, string songName, IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            SongInput songData;
            if (!memoryCache.TryGetValue<SongInput>(GetCacheKey(name, songName, nameof(songData)), out songData))
            {
                if (storageHandler != null)
                    songData = storageHandler.RetrieveSongData(name, songName);
                else
                    return null;
            }
            return songData;
        }

        public static byte[] GetFileBytes(FileGeneratorBase.FileType fileType, string name, string songName,
            IStorageHandler storageHandler, IMemoryCache memoryCache)
        {
            MemoryStream memoryStream = new MemoryStream();
            SongInput songData = GetSongData(name, songName, storageHandler, memoryCache);
            if (songData == null)
                return null;
            if (!memoryCache.TryGetValue<byte[]>(GetCacheKey(name, songName, fileType.ToString()), out byte[] fileBytes))
            {
                SongPartGenerator generator = new SongPartGenerator(songData);
                SongOutput songOutput = generator.CreateSongPart();
                generator.WriteToStream(fileType, songOutput, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                fileBytes = new byte[memoryStream.Length];
                memoryStream.Read(fileBytes, 0, (int)memoryStream.Length);
                memoryCache.Set<byte[]>(GetCacheKey(name, songName, fileType.ToString()), fileBytes);
                if (storageHandler != null)
                    storageHandler.SetFileCreated(name, songName, fileType);
            }
            return fileBytes;
        }
    }
}
