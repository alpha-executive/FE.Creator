using FE.Creator.ObjectRepository.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.ObjectRepository.ServiceModels
{
   public class PrimeObjectField : ServiceObjectField
    {
        public PrimeFieldDataType PrimeDataType { get; set; }

        public object Value { get; set; }

        public T GetStrongTypeValue<T>(){
                return (T)this.Value;
        }
    }
}
