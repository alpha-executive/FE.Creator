using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
    [System.ComponentModel.DataAnnotations.Schema.Table("LocalFileIndex")]
    internal class LocalFileIndex
    {

        [System.ComponentModel.DataAnnotations.Key]
        public string fileName { get; set; }

        public string fileFullName { get; set; }

        public string fileCRC { get; set; }

        public Int64 fileSize { get; set; }

        public string fileThumbinalFullName { get; set; }
    }

    internal class SqliteLocalFileIndexDBContext : DbContext
    {
        public IDbSet<LocalFileIndex> Files { get; set; }

        public SqliteLocalFileIndexDBContext()
            : base("localfileindex")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //turn off the lazy loading to avoid load complete database into memory.
            this.Configuration.LazyLoadingEnabled = false;
        }
    }
}
