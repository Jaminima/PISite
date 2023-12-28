using LiteDB.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib.Data
{
    public class DataEngine<T> where T : DataClass
    {
        #region Fields

        public ILiteCollectionAsync<T> table;

        #endregion Fields

        #region Constructors

        public DataEngine(ILiteCollectionAsync<T> _table)
        {
            table = _table;
            table.EnsureIndexAsync(x => x.Id, true).Wait();
        }

        #endregion Constructors

        #region Methods

        public async void Add(T item)
        {
            await table.InsertAsync(item);
        }

        public async Task<int> Count(Expression<Func<T, bool>> filter)
        {
            return await table.Query().Where(filter).CountAsync();
        }

        public async Task<T> Get(int id)
        {
            return await table.FindByIdAsync(id);
        }

        public async Task<bool> Remove(int id)
        {
            return await table.DeleteAsync(id);
        }

        public async Task<T[]> Search(Expression<Func<T, bool>> filter)
        {
            return await table.Query().Where(filter).ToArrayAsync();
        }

        public async Task<T> TryFind(Expression<Func<T, bool>> filter)
        {
            return await table.FindOneAsync(filter);
        }

        public async void Update(T item)
        {
            if (item.Id == 0)
                throw new ArgumentNullException("Expected An ID!");

            if (!await table.UpdateAsync(item))
                throw new ArgumentException("Data Engine Does Not Contain Key");
        }

        #endregion Methods
    }
}
