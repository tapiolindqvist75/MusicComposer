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

namespace MusicComposer.Web.Pages
{
    public class LicenseModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageHandler _storageHandler;

        public LicenseModel(ILogger<IndexModel> logger, IConfiguration configuration, IMemoryCache memoryCache)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryCache = memoryCache;
            _storageHandler = BL.StorageHandler.GetStorageHandler(logger, configuration, memoryCache);
        }
        public void OnGet()
        {
        }
    }
}
