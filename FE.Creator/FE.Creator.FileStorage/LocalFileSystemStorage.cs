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

        private string getFilePath(string fileName, DirectoryInfo searchRoot)
        {
            foreach(FileInfo file in searchRoot.GetFiles())
            {
                if(file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return file.FullName;
                }
            }

            foreach(DirectoryInfo dir in searchRoot.GetDirectories())
            {
                string dirPath = getFilePath(fileName, dir);

                if (!string.IsNullOrEmpty(dirPath))
                {
                    return dirPath;
                }
            }

            return string.Empty;
        }

        public byte[] getFileContent(string fileName)
        {
            string path = getFilePath(fileName, new DirectoryInfo(StoreRoot));
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

        public FileStorageInfo SaveFile(byte[] fileContents, string fileExtension, bool createThumbnial = false)
        {
            string fileName = Path.GetRandomFileName() + fileExtension;
            string path = Path.Combine(StoreRoot, DateTime.Now.ToString("yyyyMMdd"), fileName);
            string thumbinalPath = Path.Combine(StoreRoot, DateTime.Now.ToString("yyyyMMdd"), fileName + ".thmb");
            Task<FileStorageInfo> storeTask = SaveFileContent(fileContents, createThumbnial, fileName, path, thumbinalPath);

            return storeTask.Result;
        }

        private async Task<FileStorageInfo> SaveFileContent(byte[] fileContents, bool createThumbnial, string fileName, string path, string thumbinalPath)
        {
            FileInfo file = new FileInfo(path);
            //ensure the directory is exists.
            file.Directory.Create();

            File.WriteAllBytes(path, fileContents);

            if (createThumbnial)
            {
                File.WriteAllBytes(thumbinalPath,
                    await CreateThumbnialImage(path, true));
            }

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

        public Task<FileStorageInfo> SaveFileAsync(byte[] fileContents, string fileExtension, bool createThumbnial = false)
        {
            FileStorageInfo fileInfo = SaveFile(fileContents, fileExtension, createThumbnial);

            return Task.FromResult<FileStorageInfo>(fileInfo);
        }

        public void DeleteFile(string fileName)
        {
            string path = getFilePath(fileName, new DirectoryInfo(StoreRoot));
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public Task<byte[]> GetFileThumbinalAsync(string fileName)
        {
            return CreateThumbnialImage(fileName, false);
        }

        private Task<byte[]> CreateThumbnialImage(string fileName, bool isFullPathFileName)
        {
            byte[] returnBytes = null;

            string path = isFullPathFileName ? fileName : getFilePath(fileName, new DirectoryInfo(StoreRoot));
            string thumbnialPath = Path.Combine(path, ".thmb");

            if (File.Exists(thumbnialPath))
            {
                returnBytes = File.ReadAllBytes(thumbnialPath);
            }
            else
            {
                if (File.Exists(path))
                {
                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        int THUMB_SIZE = 256;
                        var thumbinal = WindowsThumbnailProvider.GetThumbnail(path, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);
                        thumbinal.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        returnBytes = ms.ToArray();
                    }
                }
            }

            return Task.FromResult<byte[]>(returnBytes);
        }
    }
}
