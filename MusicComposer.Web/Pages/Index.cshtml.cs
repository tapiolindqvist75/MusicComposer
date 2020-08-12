using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using MusicComposerLibrary.Storage;
using MusicComposerLibrary;
using MusicComposerLibrary.Structures;

namespace MusicComposer.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageHandler _storageHandler;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            _storageHandler = BL.StorageHandler.GetStorageHandler(logger, configuration, memoryCache);
        }
        [BindProperty]
        public string Mode { get; set; }

        [BindProperty]
        public string SongTitle { get; set; }
        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Form { get; set; }
        [BindProperty]
        public string Rating { get; set; }
        [BindProperty]
        public bool Rated { get; set; }
        [BindProperty]
        public WeightData Weights { get; set; }
        [BindProperty]
        public string Key { get; set; }
        [BindProperty]
        public bool Error { get; set; }
        [BindProperty]
        public string Chord { get; set; }

        public void OnGet()
        {
        }

        private void CreateAndStoreSong(bool advanced)
        {
            Name = BL.StorageHandler.ConvertToValidPartitionKey(Name);
            SongTitle = "MC" + DateTime.Now.ToString("yyyyMMddHHmmss");
            SongInput songData = new SongInput()
            {
                BeatsPerMeasure = 4,
                BeatUnit = NoteDuration.NoteLengthType.Quarter,
                Major = (Key.Substring(1) == "major"),
                Name = Name,
                SongName = SongTitle,
                PartLength = 4,
                ScaleKeyFullName = Key.Substring(0, 1),
                DurationValues = WeightedRandom.GetRandomValues(10),
                PitchValues = WeightedRandom.GetRandomValues(10),
                Chords = (Chord == "Chords")
            };                     
            if (advanced)
                songData.WeightData = Weights;
            else
                songData.WeightData = WeightData.GetDefaults();
            BL.CreateSong.CreateAndStoreSong(songData, _storageHandler, _memoryCache);
        }

        public IActionResult OnPost()
        {
            if (Form == "create")
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    CreateAndStoreSong(false);
                }
                return Page();
            }
            else if (Form == "createAdvanced")
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    CreateAndStoreSong(true);
                }
                return Page();
            }
            else if (Form == "downloadMusicXml")
            {
                byte[] musicXmlBytes = BL.CreateSong.GetFileBytes(FileGeneratorBase.FileType.MusicXml, Name, SongTitle, _storageHandler, _memoryCache);
                if (musicXmlBytes == null)
                {
                    Error = true;
                    return Page();
                }
                return File(musicXmlBytes, "application/vnd.recordare.musicxml+xml", $"{SongTitle}.musicxml");
            }
            else if (Form == "downloadMidi")
            {
                byte[] midiBytes = BL.CreateSong.GetFileBytes(FileGeneratorBase.FileType.Midi, Name, SongTitle, _storageHandler, _memoryCache);
                if (midiBytes == null)
                {
                    Error = true;
                    return Page();
                }
                return File(midiBytes, "audio/midi", $"{SongTitle}.mid");
            }
            else if (Form == "rating")
            {
                int rating = Convert.ToInt32(Rating);
                _storageHandler.SetRating(Name, SongTitle, rating);
                return Page();
            }
            else
            {
                Weights = WeightData.GetDefaults();
                return Page();
            }
        }
    }
}
