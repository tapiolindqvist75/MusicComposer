using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage
{
    public class StorageFactory
    {
        public static IStorageHandler GetStorage(StorageParameters parameters)
        {
            if (parameters is Azure.AzureStorageParameters)
                return Azure.StorageHandler.Initialize(parameters as Azure.AzureStorageParameters);
            throw new NotImplementedException();
        }
    }
}
