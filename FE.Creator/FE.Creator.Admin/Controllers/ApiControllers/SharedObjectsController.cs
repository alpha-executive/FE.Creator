using FE.Creator.Admin.MVCExtension;
using FE.Creator.FileStorage;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    [UnknownErrorFilter]
    public class SharedObjectsController : ApiController
    {
        public IObjectService objectService = null;
        IFileStorageService storageService = null;
        public static Logger logger = LogManager.GetCurrentClassLogger(typeof(SharedObjectsController));
        public SharedObjectsController(IObjectService objectService, IFileStorageService storageService)
        {
            this.objectService = objectService;
            this.storageService = storageService;
        }

        private List<ServiceObject> GetSharedServiceObjects(int objDefID, 
            string[] properties,
            string shareFieldName,
            int page, 
            int pageSize)
        {
            var svcObjLists = objectService.GetServiceObjects(
                objDefID,
                properties,
                page,
                pageSize,
                null);

            var sharedObjects = (from s in svcObjLists
                                   where s.GetPropertyValue<PrimeObjectField>(shareFieldName)
                                            .GetStrongTypeValue<int>() == 1
                                   select s
                                  )
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();


            return sharedObjects;
        }


        private ObjectDefinition FindObjectDefinitionByName(string defname)
        {
            logger.Debug("Start FindObjectDefinitionByName");

            var objDefs = objectService.GetAllObjectDefinitions();

            logger.Debug("objDefs.count : " + objDefs.Count);

            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defname, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            logger.Debug("End FindObjectDefinitionByName");
            return findObjDef;
        }


        private async Task<HttpResponseMessage> DownloadSharedObjectFile(string Id, 
            string filePropertyName, 
            string shareFieldName,
            bool isThumbinal)
        {
            byte[] content = null;

            ServiceObject svo = objectService.GetServiceObjectById(int.Parse(Id),
                new string[] { filePropertyName, shareFieldName },
                null);

            if(svo != null && 
                svo.GetPropertyValue<PrimeObjectField>(shareFieldName)
                                            .GetStrongTypeValue<int>() == 1)
            {
               string fileFullPath = svo.GetPropertyValue<ObjectFileField>(filePropertyName)
                    .FileFullPath;

                if (isThumbinal)
                {
                    content = await storageService
                        .GetFileThumbinalAsync(fileFullPath);
                }
                else
                {
                    content = await storageService
                        .GetFileContentAsync(fileFullPath);
                }
            }

            HttpResponseMessage message = CreateResponseMessage(content,
                svo != null ? svo.GetPropertyValue<ObjectFileField>(filePropertyName)
                .FileName : string.Empty);

            return message;
        }


        private HttpResponseMessage CreateResponseMessage(byte[] content, string fileName)
        {
            logger.Debug("Start CreateResponseMessage");
            HttpResponseMessage result = null;

            if (content != null)
            {
                logger.Debug("content found on the server storage.");
                // Serve the file to the client
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(content);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = fileName;
            }
            else
            {
                logger.Debug("content was not found");
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            logger.Debug("End GetFileContent");
            return result;
        }

        /// <summary>
        /// For thumbinal download : /api/custom/SharedObjects/DownloadSharedBook/{objectid}?thumbinal=true
        /// For File Download : /api/custom/SharedObjects/DownloadSharedBook/{objectid}
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thumbinal"></param>
        /// <returns></returns>
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadSharedBook(string Id, bool thumbinal = false)
        {
            return await DownloadSharedObjectFile(Id,
                "bookFile", 
                "bookSharedLevel", 
                thumbinal);
        }

        /// <summary>
        /// For thumbinal download : /api/custom/SharedObjects/DownloadSharedImage/{objectid}?thumbinal=true
        /// For File Download : /api/custom/SharedObjects/DownloadSharedImage/{objectid}
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="thumbinal"></param>
        /// <returns></returns>
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadSharedImage(string Id, bool thumbinal = false)
        {
            return await DownloadSharedObjectFile(Id,
                "imageFile",
                "imageSharedLevel",
                thumbinal);
        }

        /// <summary>
        ///  For File Download : /api/custom/SharedObjects/DownloadArticleImage/{objectid}
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [ResponseType(typeof(HttpResponseMessage))]
        [HttpGet]
        public async Task<HttpResponseMessage> DownloadArticleImage(string Id)
        {
            return await DownloadSharedObjectFile(Id,
                "articleImage",
                "articleSharedLevel",
                 false);
        }

        /// <summary>
        /// /api/custom/SharedObjects/SharedArticles
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public IHttpActionResult SharedArticles(int page = 1, int pagesize = int.MaxValue)
        {
            var articleDef = FindObjectDefinitionByName("Article");
            int currPage = page > 0 ? page : 1;
            int currPageSize = pagesize > 0 ? pagesize : int.MaxValue;

            var sharedObjects = GetSharedServiceObjects(
                    articleDef.ObjectDefinitionID,
                    new string[] {
                        "articleDesc",
                        "isOriginal",
                        "articleImage",
                        "articleGroup",
                        "articleSharedLevel"
                    },
                    "articleSharedLevel",
                    currPage > 0 ? currPage : 1,
                    currPageSize > 0 ? currPageSize : int.MaxValue
                );

            return this.Ok<IEnumerable<ServiceObject>>(sharedObjects);
        }

        /// <summary>
        /// /api/custom/SharedObjects/SharedImages
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public IHttpActionResult SharedImages(int page = 1, int pagesize = int.MaxValue)
        {
            var imageDef = FindObjectDefinitionByName("Photos");
            int currPage = page > 0 ? page : 1;
            int currPageSize = pagesize > 0 ? pagesize : int.MaxValue;

            var sharedObjects = GetSharedServiceObjects(
                 imageDef.ObjectDefinitionID,
                 new string[] {
                        "imageFile",
                        "imageDesc",
                        "imageCategory",
                        "imageSharedLevel"
                 },
                 "imageSharedLevel",
                 currPage > 0 ? currPage : 1,
                 currPageSize > 0 ? currPageSize : int.MaxValue
             );

            return this.Ok<IEnumerable<ServiceObject>>(sharedObjects);
        }

        /// <summary>
        /// /api/custom/SharedObjects/SharedBooks
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public IHttpActionResult SharedBooks(int page = 1, int pagesize = int.MaxValue)
        {
            var imageDef = FindObjectDefinitionByName("Books");
            int currPage = page > 0 ? page : 1;
            int currPageSize = pagesize > 0 ? pagesize : int.MaxValue;

            var sharedObjects = GetSharedServiceObjects(
                 imageDef.ObjectDefinitionID,
                 new string[] {
                        "bookFile",
                        "bookDesc",
                        "bookAuthor",
                        "bookVersion",
                        "bookSharedLevel",
                        "bookCategory",
                        "bookISBN"
                 },
                 "bookSharedLevel",
                 currPage > 0 ? currPage : 1,
                 currPageSize > 0 ? currPageSize : int.MaxValue
             );

            return this.Ok<IEnumerable<ServiceObject>>(sharedObjects);
        }


        [HttpGet]
        public int GetSharedImageCount()
        {
            var imageDef = FindObjectDefinitionByName("Photos");

            var images = GetSharedServiceObjects(
                 imageDef.ObjectDefinitionID,
                 new string[] { "imageSharedLevel" },
                 "imageSharedLevel",
                 1,
                 int.MaxValue
                );

            return images.Count;
        }

        [HttpGet]
        public int GetSharedBookCount()
        {
            var imageDef = FindObjectDefinitionByName("Books");

            var sharedObjects = GetSharedServiceObjects(
               imageDef.ObjectDefinitionID,
               new string[] {
                        "bookSharedLevel"
               },
               "bookSharedLevel",
               1,
               int.MaxValue
           );

            return sharedObjects.Count;
        }

        [HttpGet]
        public int GetSharedArticleCount()
        {
            var articleDef = FindObjectDefinitionByName("Article");

            var sharedObjects = GetSharedServiceObjects(
                   articleDef.ObjectDefinitionID,
                   new string[] {
                        "articleSharedLevel"
                   },
                   "articleSharedLevel",
                   1,
                   int.MaxValue
               );

            return sharedObjects.Count;
        }
    }
}
