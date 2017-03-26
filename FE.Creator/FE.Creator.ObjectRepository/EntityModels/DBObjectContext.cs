using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
   internal class DBObjectContext : DbContext
    {
        public DBObjectContext(string connstr) : base(connstr){
        }

        public IDbSet<GeneralObjectDefinition> GeneralObjectDefinitions { get; set; }
        public IDbSet<GeneralObjectDefinitionGroup> GeneralObjectDefinitionGroups { get; set; }

        public IDbSet<GeneralObject> GeneralObjects { get; set; }

        public IDbSet<GeneralObjectField> GeneralObjectFields { get; set; }

        public IDbSet<GeneralObjectDefinitionField> GeneralObjectDefinitionFields { get; set; }

        public IDbSet<GeneralObjectDefinitionSelectItem> GeneralObjectDefinitionSelectItems { get; set; }

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
