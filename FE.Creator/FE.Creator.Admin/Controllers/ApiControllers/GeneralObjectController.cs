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
    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        // GET: api/GeneralObject/5
        public ServiceObject Get(int id, [FromBody]string []properties = null)
        {
            var generalObj = objectService.GetServiceObjectById(id, properties);

            return generalObj;
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
