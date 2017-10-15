using NLog;
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
        private static ILogger logger = LogManager.GetCurrentClassLogger(typeof(LocalFileSystemStorage));

        public LocalFileSystemStorage(string storeRootPath)
        {
            this.StoreRoot = storeRootPath;
        }

        private string searchFileInFileIndex(string fileName)
        {
            logger.Debug("Start searchFileInFileIndex");
            logger.Debug("FileName : " + fileName);
            using (SqliteLocalFileIndexDBContext dbContext = new SqliteLocalFileIndexDBContext())
            {
                try
                {
                    var fileInfo = (from f in dbContext.Files
                                    where f.fileName.Equals(fileName)
                                    select f).FirstOrDefault();

                    if (fileInfo != null)
                    {
                        logger.Debug("found file : " + fileInfo.fileFullName);
                        return fileInfo.fileFullName;
                    }
                }
                catch(Exception e)
                {
                    logger.Error(e);
                }
                finally
                {
                    logger.Debug("End searchFileInFileIndex");
                }
            }

            return string.Empty;
        }
        private string getFilePath(string fileName, DirectoryInfo searchRoot, bool fileIndexFailed = false)
        {
            if (!fileIndexFailed)
            {
                logger.Debug("try to search file in the file index database.");
                string searchResult = searchFileInFileIndex(fileName);

                //if hit the cache, just return the value.
                if (!string.IsNullOrEmpty(searchResult))
                {
                    return searchResult;
                }
                else
                {
                    logger.Info("file " + fileName + " was not found in file index database");
                }
                    
            }
            logger.Info("search file " + fileName + " from the local storage");
            foreach (FileInfo file in searchRoot.GetFiles())
            {
                if (file.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.Info("found file at " + file.FullName);
                    return file.FullName;
                }
            }

            foreach (DirectoryInfo dir in searchRoot.GetDirectories())
            {
                //if we go here, the sqllite file cache failed.
                string dirPath = getFilePath(fileName, dir, true);

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
                logger.Error("file " + path + " is not exists");
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
            logger.Debug("Start SaveFile");
            string fileName = Path.GetRandomFileName() + fileExtension;
            string path = Path.Combine(StoreRoot, DateTime.Now.ToString("yyyyMMdd"), fileName);
            string thumbinalPath = Path.Combine(StoreRoot, DateTime.Now.ToString("yyyyMMdd"), fileName + ".thmb");
            logger.Debug("fileName : " + fileName);
            logger.Debug("path : " + path);
            logger.Debug("thumbinalPath : " + thumbinalPath);

            Task<FileStorageInfo> storeTask = SaveFileContent(fileContents, createThumbnial, fileName, path, thumbinalPath);

            logger.Debug("End SaveFile");
            return storeTask.Result;
        }

        private void saveLocalFileIndex(LocalFileIndex indexInfo)
        {
            logger.Debug("Start saveLocalFileIndex");
            if (indexInfo == null || string.IsNullOrEmpty(indexInfo.fileFullName))
            {
                logger.Error("file path is null or empty");
                return;
            }
              

            using (SqliteLocalFileIndexDBContext dbContext = new SqliteLocalFileIndexDBContext())
            {
                try
                {
                    dbContext.Files.Add(indexInfo);
                    dbContext.SaveChanges();
                }
                catch(Exception ex)
                {
                    logger.Error(ex);
                }
               
            }
            logger.Debug("End saveLocalFileIndex");
        }


        private async Task<FileStorageInfo> SaveFileContent(byte[] fileContents, bool createThumbnial, string fileName, string path, string thumbinalPath)
        {
            logger.Debug("Start SaveFileContent");
            LocalFileIndex indexInfo = new LocalFileIndex();
            FileInfo file = new FileInfo(path);
            string fileCrc = CalculateCRC(fileContents);
            logger.Debug("path: " + path);
            logger.Debug("fileCrc : " + fileCrc);
            //ensure the directory is exists.
            file.Directory.Create();

            File.WriteAllBytes(path, fileContents);


            if (createThumbnial)
            {
                logger.Debug("creating file thumbnail");
                File.WriteAllBytes(thumbinalPath,
                    await CreateThumbnialImage(path, true));

                indexInfo.fileThumbinalFullName = thumbinalPath;
                logger.Debug("thumbinalPath : " + thumbinalPath);
            }

            indexInfo.fileName = fileName;
            indexInfo.fileFullName = path;
            indexInfo.fileCRC = fileCrc;
            indexInfo.fileSize = fileContents.Length;
            saveLocalFileIndex(indexInfo);

            logger.Debug("End SaveFileContent");

            return new FileStorageInfo()
            {
                FileName = fileName,
                FileUri = path,
                Creation = DateTime.Now,
                LastUpdated = DateTime.Now,
                Size = fileContents.Length,
                CRC = fileCrc
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
            logger.Debug("Start DeleteFile");
            string path = getFilePath(fileName, new DirectoryInfo(StoreRoot));
            logger.Debug("path : " + path);
            if (File.Exists(path))
            {
                logger.Debug("file was found and about to be deleted.");
                File.Delete(path);
            }

            logger.Debug("End DeleteFile");
        }

        public Task<byte[]> GetFileThumbinalAsync(string fileName)
        {
            return CreateThumbnialImage(fileName, false);
        }

        private Task<byte[]> CreateThumbnialImage(string fileName, bool isFullPathFileName)
        {
            logger.Debug("Start CreateThumbnialImage");
            byte[] returnBytes = null;

            string path = isFullPathFileName ? fileName : getFilePath(fileName, new DirectoryInfo(StoreRoot));
            string thumbnialPath = Path.Combine(path, ".thmb");
            logger.Info("path : " + path);
            logger.Info("thumbnialPath : " + thumbnialPath);

            if (File.Exists(thumbnialPath))
            {
                logger.Info("thumbnialPath already exists.");
                returnBytes = File.ReadAllBytes(thumbnialPath);
            }
            else
            {
                if (File.Exists(path))
                {
                    using (System.IO.MemoryStream ms = new MemoryStream())
                    {
                        int THUMB_SIZE = 256;
                        logger.Info("create new thumbinal file by WindowsThumbnailProvider");
                        var thumbinal = WindowsThumbnailProvider.GetThumbnail(path, THUMB_SIZE, THUMB_SIZE, ThumbnailOptions.None);
                        thumbinal.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                        returnBytes = ms.ToArray();
                        logger.Info("size of returnBytes : " + returnBytes.Length);
                    }
                }
            }

            return Task.FromResult<byte[]>(returnBytes);
        }
    }
}
