using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
    public class FileStorageInfo
    {
        public string FileName { get; set; }
        public string FileFriendlyName { get; set; }
        public string FileUri { get; set; }

        public DateTime Creation { get; internal set; }

        public DateTime LastUpdated { get; internal set; }

        public string CRC { get; internal set; }

        public long Size { get; internal set; }
    }
}
