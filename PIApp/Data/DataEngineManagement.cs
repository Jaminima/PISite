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
        private static Dictionary<Type, object> tableStore = new Dictionary<Type, object>();

        #endregion Fields

        #region Methods

        public static void Init(string dbFile = "./data/database.db")
        {
            Console.WriteLine("Connecting To LiteDB");

            db = new LiteDatabaseAsync($"Filename={dbFile}");
        }

        public static DataEngine<T> SummonTable<T>() where T : DataClass
        {
            var t = new DataEngine<T>(db.GetCollection<T>());

            tableStore.Add(typeof(T), t);

            return t;
        }

        public static DataEngine<T> GetTable<T>() where T : DataClass
        {
            if (tableStore.TryGetValue(typeof(T), out object table))
            {
                return (DataEngine<T>)table;
            }
            else
            {
                throw new Exception("Class Is Not Summoned");
            }
        }

        #endregion Methods
    }
}