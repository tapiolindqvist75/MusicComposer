using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace MusicComposerLibrary.Storage.Azure
{
    /// <summary>
    /// PartitionKey = Name
    /// RowKey = SongName
    /// </summary>
    public class SongTable : TableEntity
    {
        public SongTable() { }
        public string Data { get; set; }
        public int? Rating { get; set; }
        public bool? MusicXmlGenerated { get; set; }
        public bool? MidiGenerated { get; set; }
    }
}
