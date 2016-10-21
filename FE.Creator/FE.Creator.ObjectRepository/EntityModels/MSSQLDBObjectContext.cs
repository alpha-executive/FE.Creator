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
        public MSSQLDBObjectContext() : base("mssqlconnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //define self reference group.
            modelBuilder.Entity<GeneralObjectDefinitionGroup>()
                .HasOptional(o => o.ParentGroup)
                .WithMany(o => o.ChildrenGroups)
                .Map(k => k.MapKey("ParentGroupID"));

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Types().Configure(c => c.ToTable(c.ClrType.Name));

            //turn off the lazy loading to avoid load complete database into memory.
            this.Configuration.LazyLoadingEnabled = false;
        }
    }
}
