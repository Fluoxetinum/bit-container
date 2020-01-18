using System;
using System.Collections.Generic;
using System.Text;

namespace BitContainer.DataAccess.Scripts
{
    public partial class T4StorageDbInitScript : IT4DbInitScript
    {
        public string DbName { get; private set; }

        public T4StorageDbInitScript(String dbName)
        {
            DbName = dbName;
        }
    }
}
