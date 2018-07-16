using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FE.Creator.Admin.Areas.Portal.Models
{
    public class PostViewModel
    {
        private string OriginalHost
        {
            get
            {
                Uri currentUri = HttpContext.Current.Request.Url;
                string hostAndPort = string.Empty;
                if (currentUri.OriginalString.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {
                    hostAndPort = string.Format("http://{0}{1}", currentUri.Host,
                        currentUri.Port == 80 || currentUri.Port == 443 ? string.Empty
                        : ":" + currentUri.Port);
                }

                if (currentUri.OriginalString.StartsWith("https", StringComparison.InvariantCultureIgnoreCase))
                {
                    hostAndPort = string.Format("https://{0}{1}", currentUri.Host,
                       currentUri.Port == 80 || currentUri.Port == 443 ? string.Empty
                       : ":" + currentUri.Port);
                }

                return hostAndPort;
            }
        }
        public int ObjectId
        {
            get; set;
        }
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

        public string ContentUrl
        {
            get
            {
                return string.Format("{0}/portal/portalhome/viewarticlecontent/{1}",
                    this.OriginalHost,
                    this.ObjectId);
            }
        }


        public string ImageUrl
        {
            get
            {
                if(this.ObjectId != 0)
                {
                   
                    return string.Format("{0}/api/custom/SharedObjects/DownloadSharedBook/{1}?thumbinal=true",
                       this.OriginalHost,
                       this.ObjectId);
                }

                return string.Empty;
            }
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