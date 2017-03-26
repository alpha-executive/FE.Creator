using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
   internal class EntityContextFactory
    {
        internal static DBObjectContext GetDBObjectContext()
        {
            string dbProvider = ConfigurationManager.AppSettings["databaseprovider"];
            switch (dbProvider.ToLower())
            {
                case "mysql":
                    return MySQLDBObjectContext.Create();
                default:
                    return MSSQLDBObjectContext.Create();
            }
        }
    }
}
