using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FE.Creator.Admin.Areas.Portal.Models
{
    public class PostViewModel
    {
        public string PostTitle
        {
            get;set;
        }
        
        public string PostDesc
        {
            get;set;
        }

        public string PostContent
        {
            get;set;
        }

        public string Author
        {
            get;set;
        }

        public bool IsOriginal
        {
            get;set;
        }

        public DateTime Created
        {
            get;set;
        }
    }
}