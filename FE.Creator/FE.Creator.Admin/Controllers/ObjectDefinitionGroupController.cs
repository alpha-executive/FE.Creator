using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.Controllers
{
    using ObjectRepository;
    using ObjectRepository.ServiceModels;
    using System.Threading.Tasks;

    public class ObjectDefinitionGroupController : ApiController
    {
        IObjectService objectService = null;

        public ObjectDefinitionGroupController(IObjectService service)
        {
            this.objectService = service;
        }

        // GET: api/ObjectDefinitionGroup
        [ResponseType(typeof(IEnumerable<ObjectDefinitionGroup>))]
        public async Task<IHttpActionResult> Get()
        {
            IEnumerable<ObjectDefinitionGroup> objDefGroups = await GetAllObjectDefinitionGroups();

            return this.Ok<IEnumerable<ObjectDefinitionGroup>>(objDefGroups);
        }

        private Task<IEnumerable<ObjectDefinitionGroup>> GetAllObjectDefinitionGroups()
        {
            return Task.FromResult<IEnumerable<ObjectDefinitionGroup>>(objectService.GetObjectDefinitionGroups());
        }



        // GET: api/ObjectDefinitionGroup/5

        private Task<ObjectDefinitionGroup> GetObjectDefinitionGroup(int id)
        {
            return Task.FromResult<ObjectDefinitionGroup>(
                    objectService.GetObjectDefinitionGroupById(id)
                );
        }

        [ResponseType(typeof(ObjectDefinitionGroup))]
        public async Task<IHttpActionResult> Get(int id)
        {
            var objDefGroup = await GetObjectDefinitionGroup(id);


            return this.Ok<ObjectDefinitionGroup>(objDefGroup);
        }


        // POST: api/ObjectDefinitionGroup
        public void Post([FromBody]ObjectDefinitionGroup value)
        {

        }

        // PUT: api/ObjectDefinitionGroup/5
        public void Put(int id, [FromBody]ObjectDefinitionGroup value)
        {
        }

        // DELETE: api/ObjectDefinitionGroup/5
        public void Delete(int id)
        {

        }
    }
}
