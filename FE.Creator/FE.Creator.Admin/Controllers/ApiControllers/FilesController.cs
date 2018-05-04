using FE.Creator.Admin.MVCExtension;
using FE.Creator.FileStorage;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    /// <summary>
    ///  GET: /api/custom/Files/DownloadFile/{id}/{parameter}/
    ///      {id}: required: file id
    ///      {parameter}: optional, file name
    ///      return: downloaded file
    ///  POST: api/FileUpload
    ///       upload a file, request must contains multipart/form-data.
    ///  DELETE: api/FileUpload/{id}
    ///       {id}: required string id.
    ///       delete a file with given file {id}. 
    /// </summary>
    [UnknownErrorFilter]
    [Authorize]
    public class FilesController : ApiController
    {
        IFileStorageService storageService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(FilesController));

        public FilesController(IFileStorageService storageService)
        {
            this.storageService = storageService;
        }

        /// <summary>
        /// GET /api/custom/Files/DownloadFile/{0}/{1}/
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpGet]
        // GET: api/Files
        public async Task<HttpResponseMessage> DownloadFile(string id, string parameters = null, bool thumbinal = false)
        {
            logger.Debug("Start DownloadFile");
            HttpResponseMessage result = null;

            if (thumbinal)
            {
                logger.Debug("get thumbinal image");
                result = await this.GetFileThumbinal(id, parameters);
            }
            else
            {
                logger.Debug("get original file content");
                result = await this.GetFileContent(id, parameters);
            }

            logger.Debug("End DownloadFile");
            return result;
        }

        private async Task<HttpResponseMessage> GetFileContent(string id, string parameters = null)
        {
            logger.Debug("Start GetFileContent");
            HttpResponseMessage result = null;
            byte[] content = await storageService.GetFileContentAsync(id);

            if(content != null)
            {
                logger.Debug("content found on the server storage.");
                // Serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(content);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = string.IsNullOrEmpty(parameters) ? id : parameters;
            }
            else
            {
                logger.Debug("content was not found");
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.Debug("End GetFileContent");
            return result;
        }

        private async Task<HttpResponseMessage> GetFileThumbinal(string id,string parameters = null)
        {
            logger.Debug("Start GetFileThumbinal");
            HttpResponseMessage result = null;
            byte[] content = await storageService.GetFileThumbinalAsync(id);

            if(content != null)
            {
                logger.Debug("found content on server storage.");
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(content);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("Inline");
                result.Content.Headers.ContentDisposition.FileName = "file_thumb.png";
            }
            else
            {
                logger.Debug("content was not on server storage.");
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.Debug("End GetFileThumbinal");
            return result;
        }
        // POST: api/FileUpload
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromUri] bool thumbinal = false, [FromUri] bool forContent = false)
        {
            logger.Debug("Start FileController Post");
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                logger.Error("Unsupported media type");
                return BadRequest("Unsupported media type");
            }
            try
            {
                //var provider = new MultipartFormDataStreamProvider(workingFolder);
                //await Request.Content.ReadAsMultipartAsync(provider);
                List<ObjectFileField> files = new List<ObjectFileField>();

                var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
                foreach (var stream in filesReadToProvider.Contents)
                {
                    string fileName = !string.IsNullOrEmpty(stream.Headers.ContentDisposition.FileName) ?
                                                stream.Headers.ContentDisposition.FileName : "file.unknown";

                    logger.Info("the raw file name is : " + fileName);
                    fileName = fileName.Replace("\"", "")
                                    .Replace("'", "")
                                    .Replace("@", "_")
                                    .Replace("&", "_")
                                    .Replace(" ", "_");

                    logger.Info("the encoded file name is " + fileName);
                    string extension = fileName.Substring(fileName.LastIndexOf('.'));

                    var fileBytes = await stream.ReadAsByteArrayAsync();
                    FileStorageInfo info = await storageService.SaveFileAsync(fileBytes, extension, thumbinal);

                    logger.Info("file " + fileName + " was saved to server storage");
                    logger.Debug("file save path : " + info.FileName);

                    files.Add(new ObjectFileField()
                    {
                        FileName = fileName,
                        FileFullPath = info.FileName,
                        FileUrl = string.Format("/api/custom/Files/DownloadFile/{0}/{1}/", info.FileName, fileName),
                        FileExtension = extension,
                        Updated = info.LastUpdated,
                        Created = info.Creation,
                        FileCRC = info.CRC,
                        FileSize = (int)(info.Size / 1024),
                    });
                }

                IHttpActionResult result = forContent ? this.Ok<object>(new { uploaded = 1,  fileName =files[0].FileName, url=files[0].FileUrl})
                    : this.Ok<object>(new { status = "success", files = files });

                logger.Debug("End FileController Post");
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        // DELETE: api/FileUpload/5
        public void DeleteFile(string id)
        {
            logger.Debug("Start DeleteFile with id : " + id);

            storageService.DeleteFile(id);

            logger.Debug("End DeleteFile");
        }
    }
}
