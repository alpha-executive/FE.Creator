using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http.Description;
    using FE.Creator.ObjectRepository;
    using FE.Creator.ObjectRepository.ServiceModels;

    public class ObjectDefinitionController : ApiController
    {
        IObjectService objectService = null;

        public ObjectDefinitionController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private Task<IEnumerable<ObjectDefinition>> getObjectDefinitions()
        {
            var objDefinitions = objectService.GetAllObjectDefinitions();
            return Task.FromResult<IEnumerable<ObjectDefinition>>(objDefinitions);
        }

        // GET: api/ObjectDefinition
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        public async Task<IHttpActionResult> Get()
        {
            var objDefintions = await getObjectDefinitions();
            return this.Ok<IEnumerable<ObjectDefinition>>(objDefintions);
        }


        private Task<ObjectDefinition> getObjectDefinition(int id)
        {
            var objectDefinition = objectService.GetObjectDefinitionById(id);

            return Task.FromResult<ObjectDefinition>(objectDefinition);
        }

        [ResponseType(typeof(ObjectDefinition))]
        // GET: api/ObjectDefinition/5
        public async Task<IHttpActionResult> Get(int id)
        {
            var objDefinition = await getObjectDefinition(id);

            return this.Ok<ObjectDefinition>(objDefinition);
        }

        // POST: api/ObjectDefinition
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/ObjectDefinition/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ObjectDefinition/5
        public void Delete(int id)
        {
        }
    }
}
