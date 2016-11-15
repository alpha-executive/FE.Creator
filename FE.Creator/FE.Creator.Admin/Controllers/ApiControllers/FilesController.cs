﻿using FE.Creator.FileStorage;
using FE.Creator.ObjectRepository.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace FE.Creator.Admin.Controllers.ApiControllers
{
    public class FilesController : ApiController
    {
        IFileStorageService storageService = null;
        

        public FilesController(IFileStorageService storageService)
        {
            this.storageService = storageService;
        }

        [HttpGet]
        // GET: api/Files
        public async Task<HttpResponseMessage> DownloadFile(string id, string parameters)
        {
            HttpResponseMessage result = null;

            byte[] content = await storageService.GetFileContentAsync(id);

            // Serve the file to the client
            result = Request.CreateResponse(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(content);
            result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = string.IsNullOrEmpty(parameters) ?  id : parameters;

            return result;
        }

        // POST: api/FileUpload
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {  
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
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
                    var fileBytes = await stream.ReadAsByteArrayAsync();
                    FileStorageInfo info = await storageService.SaveFileAsync(fileBytes);

                    string fileName = !string.IsNullOrEmpty(stream.Headers.ContentDisposition.FileName) ?
                                                stream.Headers.ContentDisposition.FileName : info.FileName;

                    fileName = fileName.Replace("\"", "")
                                    .Replace("'", "")
                                    .Replace("@", "_")
                                    .Replace("&", "_")
                                    .Replace(" ", "_");

                    string extension = fileName.Substring(fileName.LastIndexOf('.')); 
                                            

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

                return Ok(new { status = "success", files = files });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.GetBaseException().Message);
            }
        }

        // DELETE: api/FileUpload/5
        public void Delete(int id)
        {
        }
    }
}
