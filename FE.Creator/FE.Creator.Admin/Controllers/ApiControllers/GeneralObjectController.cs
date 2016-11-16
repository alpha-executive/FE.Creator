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

        [HttpGet]
        public int CountObjectServices(int id) {
            return objectService.GetGeneralObjectCount(id);
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, int pageIndex, int pageSize, string[] properties = null)
        {
            var objectList = objectService.GetServiceObjects(id, properties,pageIndex, pageSize);

            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        // GET: api/GeneralObjectList
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public async Task<IHttpActionResult> FindServiceObjects(int id, string parameters, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0)
                pageIndex = 1;

            if (pageSize <= 0)
                pageSize = int.MaxValue;

            var objectList = await getAllServiceObjectAsync(id,
                pageIndex,
                pageSize,
                string.IsNullOrEmpty(parameters) ? null : parameters.Split(new char[] { ',' }));

            return this.Ok<IEnumerable<ServiceObject>>(objectList);
        }

        [ResponseType(typeof(ServiceObject))]
        [HttpGet]
        public IHttpActionResult FindServiceObject(int id)
        {
            var obj = objectService.GetServiceObjectById(id, null);

            return this.Ok<ServiceObject>(obj);
        }

        // POST: api/GeneralObject
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

        // PUT: api/GeneralObject/5
        public void Put(int id, [FromBody]ServiceObject value)
        {
            Post(value);
        }

        // DELETE: api/GeneralObject/5
        public void Delete(int id)
        {
            objectService.DeleteServiceObject(id);
        }
    }
}
