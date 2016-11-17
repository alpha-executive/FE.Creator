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

    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
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

            var objDefs =  objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(definitionname, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();
            
            if(findObjDef != null)
            {
                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID, parameters, pageIndex, pageSize);
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
        public int CountObjectServices(int id) {
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
