using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIApp_Lib.Data
{
    public abstract class DataClass
    {
        #region Properties

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Id { get; set; }

        #endregion Properties
    }
}
