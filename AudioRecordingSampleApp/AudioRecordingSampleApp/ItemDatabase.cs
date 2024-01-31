using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using SQLite;

namespace AudioRecordingSampleApp
{
    public class ItemDatabase
    {
        readonly SQLiteConnection database;

        public ItemDatabase(string dbPath)
        {
            database = new SQLiteConnection(dbPath);
            database.CreateTable<Item>();
        }

        public List<Item> GetItems()
        {
            return database.Table<Item>().ToList();
        }

        public int SaveItem(Item item)
        {
            if (item.Id != 0)
            {
                return database.Update(item);
            }
            else
            {
                return database.Insert(item);
            }
        }

        public int DeleteItem(Item item)
        {
            return database.Delete(item);
        }
    }


}

