using FE.Creator.Admin.Models;
using FE.Creator.FileStorage;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FE.Creator.Admin.Controllers.ckEditorUploadController
{
    public class FileUploadController : Controller
    {
        IFileStorageService storageService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(FileUploadController));

        public FileUploadController(IFileStorageService storageService)
        {
            this.storageService = storageService;
        }
        // post: Upload File
        public async Task<ActionResult> CKEditorUpload(HttpPostedFileWrapper upload)
        {
            //string rootPath = System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, "App_Data");
            //IFileStorageService storageService = new LocalFileSystemStorage(rootPath);
            logger.Debug("Start CKEditorUpload");
            CKUploadModels model = new CKUploadModels();
            model.FunctionNumber = ControllerContext.RequestContext.HttpContext.Request["CKEditorFuncNum"];
            byte[] buffers = new byte[upload.ContentLength];
            await upload.InputStream.ReadAsync(buffers, 0, upload.ContentLength);
            var fileInfo = await storageService.SaveFileAsync(buffers, (new FileInfo(upload.FileName)).Extension, false);
            model.Url =  string.Format("/api/custom/Files/DownloadFile/{0}/{1}/", fileInfo.FileName, (new FileInfo(upload.FileName)).Name);
            logger.Debug("download url: " + model.Url);
            logger.Debug("End CKEditorUpload");
            return View(model);
        }
    }
}