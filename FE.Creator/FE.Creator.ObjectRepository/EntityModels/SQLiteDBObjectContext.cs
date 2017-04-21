using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
    internal class SQLiteDBObjectContext : DBObjectContext
    {
        private SQLiteDBObjectContext(string connstr) : base(connstr) { }

        public static SQLiteDBObjectContext Create()
        {
            return new SQLiteDBObjectContext("sqliteconnection");
        }
    }
}
