using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageHandler _storageHandler;

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configuration = configuration;
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

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Form == "create")
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    Name = BL.StorageHandler.ConvertToValidPartitionKey(Name);
                    SongTitle = "MC" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    SongData songData = new SongData()
                    {
                        BeatsPerMeasure = 4,
                        BeatUnit = NoteDuration.NoteLengthType.Quarter,
                        Major = Key == "cmajor" ? true : false,
                        Name = Name,
                        SongName = SongTitle,
                        PartLength = 4,
                        ScaleKey = "C",
                        Values = WeightedRandom.GetRandomValues(),
                        WeightData = WeightData.GetDefaults()
                    };
                    BL.CreateSong.CreateAndStoreSong(songData, _storageHandler, _memoryCache);
                }
                return Page();
            }
            else if (Form == "createAdvanced")
            {
                if (!string.IsNullOrWhiteSpace(Name))
                {
                    Name = BL.StorageHandler.ConvertToValidPartitionKey(Name);
                    SongTitle = "MC" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    SongData songData = new SongData()
                    {
                        BeatsPerMeasure = 4,
                        BeatUnit = NoteDuration.NoteLengthType.Quarter,
                        Major = Key == "cmajor" ? true : false,
                        Name = Name,
                        SongName = SongTitle,
                        PartLength = 4,
                        ScaleKey = "C",
                        Values = WeightedRandom.GetRandomValues(),
                        WeightData = Weights
                    };
                    BL.CreateSong.CreateAndStoreSong(songData, _storageHandler, _memoryCache);
                }
                return Page();
            }
            else if (Form == "downloadMusicXml")
            {
                byte[] musicXmlBytes = BL.CreateSong.GetFileBytes(FileGeneratorBase.FileType.MusicXml, Name, SongTitle, _storageHandler, _memoryCache);
                return File(musicXmlBytes, "application/vnd.recordare.musicxml+xml", $"{SongTitle}.musicxml");
            }
            else if (Form == "downloadMidi")
            {
                byte[] musicXmlBytes = BL.CreateSong.GetFileBytes(FileGeneratorBase.FileType.Midi, Name, SongTitle, _storageHandler, _memoryCache);
                return File(musicXmlBytes, "audio/midi", $"{SongTitle}.mid");
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
