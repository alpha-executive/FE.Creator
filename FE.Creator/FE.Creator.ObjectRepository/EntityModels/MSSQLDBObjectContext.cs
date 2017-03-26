using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
    internal class MSSQLDBObjectContext : DBObjectContext
    {
        private MSSQLDBObjectContext(string connstr) : base(connstr) { }

        public static MSSQLDBObjectContext Create()
        {
            return new MSSQLDBObjectContext("mssqlconnection");
        }
    }
}
