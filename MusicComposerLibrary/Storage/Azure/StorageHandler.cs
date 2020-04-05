using System;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;


namespace MusicComposerLibrary.Storage.Azure
{
    public class StorageHandler : IStorageHandler
    {
        private CloudTable _songTable;
        private StorageHandler(AzureStorageParameters parameters)
        {
            CloudStorageAccount cloudStorageAccount;
            if (parameters.UseEmulator)
                cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            else
                cloudStorageAccount = new CloudStorageAccount(
                    new Microsoft.Azure.Cosmos.Table.StorageCredentials(parameters.AccountName, parameters.AccountKey), true);
            CloudTableClient tableClient = cloudStorageAccount.CreateCloudTableClient();
            _songTable = tableClient.GetTableReference("Song");
        }
        public static StorageHandler Initialize(AzureStorageParameters parameters)
        {
            return new StorageHandler(parameters);
        }
        public SongData RetrieveSongData(string name, string songName)
        {
            SongTable songTable = _songTable.ExecuteQuery<SongTable>(new TableQuery<SongTable>()
            {
                FilterString = TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition(nameof(TableEntity.PartitionKey), QueryComparisons.Equal, name),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(TableEntity.RowKey), QueryComparisons.Equal, songName))
            }).FirstOrDefault();
            return SongData.Deserialize(songTable.Data);
        }

        public void SaveSongData(SongData song)
        {
            SongTable songTable = new SongTable()
            {
                PartitionKey = song.Name,
                RowKey = song.SongName,
                Data = SongData.Serialize(song)
            };
            _songTable.Execute(TableOperation.InsertOrMerge(songTable));
        }

        public void SetRating(string name, string songName, int rating)
        {
            SongTable songTable = new SongTable()
            {
                PartitionKey = name,
                RowKey = songName,
                Rating = rating,
            };
            _songTable.Execute(TableOperation.InsertOrMerge(songTable));
        }
    }
}
