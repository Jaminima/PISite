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
        private static Dictionary<Type, DataEngine<DataClass>> dataEngines = new Dictionary<Type, DataEngine<DataClass>>();

        #region Fields

        public static LiteDatabaseAsync db;

        #endregion Fields

        #region Methods

        public static void Init(string dbFile = "./data/database.db")
        {
            Console.WriteLine("Connecting To LiteDB");

            db = new LiteDatabaseAsync($"Filename={dbFile}");
        }

        public static void RegisterClass<T>(string tableName = null) where T : DataClass
        {
            dataEngines.Add(typeof(T), new DataEngine<T>(db.GetCollection<T>(tableName)));
        }

        #endregion Methods
    }
}