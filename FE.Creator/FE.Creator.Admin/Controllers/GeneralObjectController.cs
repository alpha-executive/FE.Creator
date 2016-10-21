using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers
{
    using ObjectRepository;

    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        // GET: api/GeneralObject
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/GeneralObject/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GeneralObject
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/GeneralObject/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/GeneralObject/5
        public void Delete(int id)
        {
        }
    }
}
