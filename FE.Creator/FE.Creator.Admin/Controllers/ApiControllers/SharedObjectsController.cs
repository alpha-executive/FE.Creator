using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    public class SharedObjectsController : ApiController
    {
        public IObjectService objectService = null;
        public static Logger logger = LogManager.GetCurrentClassLogger(typeof(SharedObjectsController));
        public SharedObjectsController(IObjectService objectService)
        {
            this.objectService = objectService;
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

        public List<ServiceObject> SharedArticles(int? page, int? pagesize)
        {
            var articleDef = FindObjectDefinitionByName("Article");
            int currPage = page.HasValue ? page.Value : 1;
            int currPageSize = pagesize.HasValue ? pagesize.Value : int.MaxValue;

            return GetSharedServiceObjects(
                    articleDef.ObjectDefinitionID,
                    new string[] {
                        "articleDesc",
                        "articleImage",
                        "articleGroup"
                    },
                    "articleSharedLevel",
                    currPage > 0 ? currPage : 1,
                    currPageSize > 0 ? currPageSize : int.MaxValue
                );
            
        }
    }
}
