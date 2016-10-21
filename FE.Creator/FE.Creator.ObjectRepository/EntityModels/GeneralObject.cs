using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.EntityModels
{
   internal class GeneralObject
    {
        public GeneralObject()
        {
            this.GeneralObjectFields = new HashSet<GeneralObjectField>();
        }
        public int GeneralObjectID { get; set; }

        public string GeneralObjectName { get; set; }

        public DateTime Created { get; set; }

        public string CreatedBy { get; set; }

        public DateTime Updated { get; set; }

        public String UpdatedBy { get; set; }

        /// <summary>
        /// for multi tanents cloud enviroment usage
        /// </summary>
        public string ObjectOwner { get; set; }

        public int GeneralObjectDefinitionID { get; set; }

        public bool IsDeleted { get; set; }

        public virtual GeneralObjectDefinition GeneralObjectDefinition { get; set; }

        public virtual ICollection<GeneralObjectField> GeneralObjectFields { get; set; }
    }
}
