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
        private readonly string workingFolder = HttpRuntime.AppDomainAppPath + @"\App_Data";
        // GET: api/FileUpload
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FileUpload/5
        //[HttpGet]
        //public System.Web.Mvc.FileResult GlobalOverview()
        //{
        //    HttpResponseMessage file = new HttpResponseMessage();
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //       // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(this.UserName + ':' + this.Password)));
        //        Task<HttpResponseMessage> response = httpClient.GetAsync("api.someDomain/Reporting/GlobalOverview");
        //        file = response.Result;
        //    }

        //    return File(file.Content.ReadAsByteArrayAsync().Result, "application/octet-stream", "GlobalOverview.csv");
        //}


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
                var provider = new MultipartFormDataStreamProvider(workingFolder);
                await Request.Content.ReadAsMultipartAsync(provider);

                var files =
                  provider.FileData
                         .Select(file => new System.IO.FileInfo(file.LocalFileName))
                         .Select(fileInfo => new ObjectFileField
                         {
                             FileName = fileInfo.Name,
                             FileFullPath = fileInfo.FullName,
                             FileExtension = fileInfo.Extension,
                             Updated = fileInfo.LastWriteTime,
                             Created = fileInfo.CreationTimeUtc,
                             FileSize = (int)(fileInfo.Length / 1024),
                         }).ToList();

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
