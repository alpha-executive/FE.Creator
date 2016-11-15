using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FE.Creator.FileStorage
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class LocalFileSystemStorage : IFileStorageService
    {
        private string StoreRoot;
        public LocalFileSystemStorage(string storeRootPath)
        {
            this.StoreRoot = storeRootPath;
        }

        public byte[] getFileContent(string fileName)
        {
            string path = Path.Combine(StoreRoot, fileName);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(fileName);
            }

            return File.ReadAllBytes(path);
        }

        public Task<byte[]> GetFileContentAsync(string fileName)
        {
            byte[] contents = getFileContent(fileName);

            return Task.FromResult<byte[]>(contents);
        }

        public FileStorageInfo SaveFile(byte[] fileContents)
        {
            string fileName = Path.GetRandomFileName();
            string path = Path.Combine(StoreRoot, fileName);

            File.WriteAllBytes(path, fileContents);

            return new FileStorageInfo()
            {
                FileName = fileName,
                FileUri = path,
                Creation = DateTime.Now,
                LastUpdated = DateTime.Now,
                Size = fileContents.Length,
                CRC = CalculateCRC(fileContents)
            };
        }

        private string CalculateCRC(byte[] fileContents)
        {
            using (var sha = SHA256.Create())
            {
                byte[] checksum = sha.ComputeHash(fileContents);

                return Convert.ToBase64String(checksum);
            }
        }

        public Task<FileStorageInfo> SaveFileAsync(byte[] fileContents)
        {
            FileStorageInfo fileInfo = SaveFile(fileContents);

            return Task.FromResult<FileStorageInfo>(fileInfo);
        }
    }
}
