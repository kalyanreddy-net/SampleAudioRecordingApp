using System;
using SQLite;

namespace AudioRecordingSampleApp
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string AudioFilePath { get; set; }
    }
}

