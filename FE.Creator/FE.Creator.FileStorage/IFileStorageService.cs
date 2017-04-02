using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
   public interface IFileStorageService
    {
        FileStorageInfo SaveFile(byte[] fileContents, string fileExtension);

        Task<FileStorageInfo> SaveFileAsync(byte[] fileContents, string fileExtension);

        byte[] getFileContent(string fileName);

        Task<byte[]> GetFileContentAsync(string fileName);

        Task<byte[]> GetFileThumbinalAsync(string fileName);

        void DeleteFile(string fileName);
    }
}
