using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.ApiControllers.Controllers
{
    using ObjectRepository;
    using ObjectRepository.ServiceModels;
    using System.Threading.Tasks;
    using System.Web.Http.Description;

    /// <summary>
    /// API for service objects
    ///  GET: api/objects/list/{definitionname}/{parameters}?pageSize=xx&pageIndex=xx
    ///         {parameters}: optional, peroperties name, split by comma.
    ///         {definitionname}: required, object definition name
    ///         pageSize and PageIndex: optional
    ///       return: the object list of the specific object definition, 
    ///  GET: api/custom/GeneralObject/CountObjects/id
    ///       return: the count of objects of the specific object definition, by definition id.
    ///  GET:  api/objects/CountObjects/{definitionname}
    ///       return: the count of objects of the specific object definition, by definition name.
    ///  GET: api/custom/GeneralObject/FindServiceObjects/{id}/parameters?pageIndex=xxx&pageSize=xxx
    ///         {parameters}: optional, peroperties name, split by comma.
    ///         {id}: required, object definition id
    ///         pageSize and PageIndex: optional
    ///       return: the object list of the specific object definition.
    ///  GET: api/custom/GeneralObject/FindServiceObject/{id}
    ///       return: find the general object by object id.
    ///       
    ///  POST api/GeneralObject
    ///       creat new service object
    ///       required body parameter: ServiceObject value
    ///  
    ///  PUT: api/GeneralObject/{id}
    ///       update the service object
    ///       {id}, required service object id.
    ///  DELETE: api/GeneralObject/{id}
    ///        delete a service object
    ///        {id}, required service object id
    ///       
    /// </summary>
    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
        }


        private ObjectDefinition FindObjectDefinitionByName(string defname)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defname, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            return findObjDef;
        }
        /// <summary>
        /// api/objects/list/{definitionname}/{parameters}
        /// </summary>
        /// <param name="definitionname"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public async Task<IHttpActionResult> List(string definitionname, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue)
        {
            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue && pageSize <= 0)
                pageSize = int.MaxValue;


            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID, parameters, pageIndex, pageSize);
            }

            return this.NotFound();
        }

        /// <summary>
        ///  api/objects/CountObjects/{definitionname}
        /// </summary>
        /// <param name="definitionname"></param>
        /// <returns></returns>
        [ResponseType(typeof(int))]
        [HttpGet]
        public async Task<IHttpActionResult> CountObjects(string definitionname)
        {
            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                Func<int> findObjectsCount = delegate(){
                    return this.CountObjects(findObjDef.ObjectDefinitionID);
                };
                return this.Ok<int>(await Task.Run<int>(findObjectsCount));
            }

            return this.NotFound();
        }

        /// <summary>
        ///  Get the count of General objects of the specific object definition.
        ///  GET: api/custom/GeneralObject/CountObjectServices/{id}
        /// </summary>
        /// <param name="id">the id of the object definition</param>
        /// <returns></returns>
        [HttpGet]
        public int CountObjects(int id) {
            return objectService.GetGeneralObjectCount(id);
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, int pageIndex, int pageSize, string[] properties = null)
        {
            var objectList = objectService.GetServiceObjects(id, properties,pageIndex, pageSize);

            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        /// <summary>
        ///     GET: api/custom/GeneralObject/FindServiceObjects/{id}/parameters?pageIndex=xxx&pageSize=xxx
        /// </summary>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public async Task<IHttpActionResult> FindServiceObjects(int id, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue)
        {
            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue && pageSize <= 0)
                pageSize = int.MaxValue;

            var objectList = await getAllServiceObjectAsync(id,
                pageIndex.Value,
                pageSize.Value,
                string.IsNullOrEmpty(parameters) ? null : parameters.Split(new char[] { ',' }));

            return this.Ok<IEnumerable<ServiceObject>>(objectList);
        }

        /// <summary>
        ///  GET: api/custom/GeneralObject/FindServiceObject/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ServiceObject))]
        [HttpGet]
        public IHttpActionResult FindServiceObject(int id)
        {
            var obj = objectService.GetServiceObjectById(id, null);

            return this.Ok<ServiceObject>(obj);
        }

        /// <summary>
        /// POST: api/GeneralObject
        /// </summary>
        [ResponseType(typeof(ServiceObject))]
        public IHttpActionResult Post([FromBody]ServiceObject value)
        {
            if (value != null)
            {
                value.CreatedBy = "Administrator";
                value.UpdatedBy = "Administrator";
                value.ObjectID =  objectService.CreateORUpdateGeneralObject(value);
            }

            return this.Created<ServiceObject>(Request.RequestUri, value);
        }

        /// <summary>
        /// PUT: api/GeneralObject/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>

        public void Put(int id, [FromBody]ServiceObject value)
        {
            Post(value);
        }

        /// <summary>
        /// DELETE: api/GeneralObject/5
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            objectService.DeleteServiceObject(id);
        }
    }
}
