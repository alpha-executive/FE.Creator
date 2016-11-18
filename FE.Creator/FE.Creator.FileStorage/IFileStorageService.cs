using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
   public interface IFileStorageService
    {
        FileStorageInfo SaveFile(byte[] fileContents);

        Task<FileStorageInfo> SaveFileAsync(byte[] fileContents);

        byte[] getFileContent(string fileName);

        Task<byte[]> GetFileContentAsync(string fileName);

        void DeleteFile(string fileName);
    }
}
