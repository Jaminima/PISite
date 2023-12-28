using LiteDB.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib.Data
{
    public static class DataEngineManagement
    {
        #region Fields

        public static LiteDatabaseAsync db;

        #endregion Fields

        #region Methods

        public static void Init(string dbFile = "./data/database.db")
        {
            Console.WriteLine("Connecting To LiteDB");

            db = new LiteDatabaseAsync($"Filename={dbFile}");
        }

        public static DataEngine<T> SummonTable<T>(string tableName = null) where T : DataClass
        {
            var t = new DataEngine<T>(tableName == null ? db.GetCollection<T>() : db.GetCollection<T>(tableName));

            return t;
        }

        #endregion Methods
    }
}