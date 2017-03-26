using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
    internal class MySQLDBObjectContext : DBObjectContext
    {
        private MySQLDBObjectContext(string connstr) : base(connstr) { }

        public static MySQLDBObjectContext Create()
        {
            return new MySQLDBObjectContext("mysqlconnection");
        }
    }
}
