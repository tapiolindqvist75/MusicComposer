using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage.Azure
{
    public class AzureStorageParameters : StorageParameters
    {
        public bool UseEmulator { get; set; }
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
    }
}
