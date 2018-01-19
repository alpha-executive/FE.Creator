using NLog;
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
        private static ILogger logger = LogManager.GetCurrentClassLogger(); 
        internal static DBObjectContext GetDBObjectContext()
        {
            string dbProvider = ConfigurationManager.AppSettings["databaseprovider"];
            logger.Debug("dbProvider = " + dbProvider.ToLower());
            switch (dbProvider.ToLower())
            {
                case "mysql":
                    return MySQLDBObjectContext.Create();
                case "sqlite":
                    return SQLiteDBObjectContext.Create();
                default:
                    return MSSQLDBObjectContext.Create();
            }
        }
    }
}
