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

        // GET: api/GeneralObject/5
        public ServiceObject Get(int id)
        {
            var generalObj = objectService.GetServiceObjectById(id, null);

            return generalObj;
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, string[] properties = null)
        {
            var objectList = objectService.GetAllSerivceObjects(id, properties);

            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        // GET: api/GeneralObjectList
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public async Task<IHttpActionResult> FindServiceObjects(int id, string parameters)
        {
            var objectList = await getAllServiceObjectAsync(id,
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
        public void Post([FromBody]ServiceObject value)
        {
            if (value != null)
            {
                objectService.CreateORUpdateGeneralObject(value);
            }
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
