using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.ApiControllers.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http.Description;
    using FE.Creator.ObjectRepository;
    using FE.Creator.ObjectRepository.ServiceModels;

    /// <summary>
    ///  GET api/objectdefinitions/list/{groupname}
    ///      {groupname}: optional, group name, if not specified, return all the object defintions.
    ///      return all the object defintions of current group specified by {groupname}
    ///  GET  api/custom/ObjectDefinition/GetAllDefinitions
    ///      return all the object definitions in the system.
    ///  GET api/custom/ObjectDefinition/FindObjectDefintionsByGroup/{id}
    ///      {id}: optional, group id, if not specified, return all the object definitions
    ///      return:  return all the object defintions of current group specified by {id}: group id.
    ///  GET: api/ObjectDefinition/{id}
    ///      {id}: required, object definition id.
    ///      return: return the specific definition by id.
    ///  POST api/ObjectDefinition
    ///     create a object definition instance, required ObjectDefinition json parameter in the body.
    ///  PUT: api/ObjectDefinition/{id}
    ///     {id}, required object definition id.
    ///     update a object definition instance. required ObjectDefinition json parameter in the body.
    ///  DELETE: api/ObjectDefinition/{id}
    ///     delete a object definition by {id}
    /// </summary>
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


        /// <summary>
        /// api/objectdefinitions/list/{groupname}
        /// </summary>
        /// <param name="groupname"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        [HttpGet]
        public async Task<IHttpActionResult> List(string groupname = null)
        {
            int groupId = -1;
            if (!string.IsNullOrEmpty(groupname))
            {
                var groups = objectService.GetObjectDefinitionGroups(null);
                var foundGroup = (from g in groups
                                  where g.GroupName.Equals(groupname, StringComparison.InvariantCultureIgnoreCase)
                                  select g).FirstOrDefault();

                groupId = foundGroup != null ? foundGroup.GroupID : -1;
            }

            if (groupId == -1)
                return await this.FindObjectDefintionsByGroup();
            else
                return await this.FindObjectDefintionsByGroup(groupId);
        }

        // GET: api/custom/ObjectDefinition/GetAllDefinitions
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetAllDefinitions()
        {
            var objDefintions = await getObjectDefinitions();
            return this.Ok<IEnumerable<ObjectDefinition>>(objDefintions);
        }

        /// <summary>
        ///  GET api/custom/ObjectDefinition/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        [HttpGet]
        public async Task<IHttpActionResult> FindObjectDefintionsByGroup(int? id = null)
        {
            var objDefinitions = id.HasValue ?
                objectService.GetObjectDefinitionsByGroup(id.Value, 1, int.MaxValue) :
                await getObjectDefinitions();

            return this.Ok<IEnumerable<ObjectDefinition>>(objDefinitions);
        }

        private Task<ObjectDefinition> getObjectDefinition(int id)
        {
            var objectDefinition = objectService.GetObjectDefinitionById(id);

            return Task.FromResult<ObjectDefinition>(objectDefinition);
        }

        [ResponseType(typeof(ObjectDefinition))]
        [HttpGet]
        // GET: api/ObjectDefinition/5
        public async Task<IHttpActionResult> FindObjectDefinition(int id)
        {
            var objDefinition = await getObjectDefinition(id);

            return this.Ok<ObjectDefinition>(objDefinition);
        }

        
        // POST: api/ObjectDefinition
        public async Task<IHttpActionResult> Post([FromBody]ObjectDefinition value)
        {
            int objectId = -1;
            if(value != null)
            {
               objectId = objectService.CreateORUpdateObjectDefinition(value);
            }

            return await this.FindObjectDefinition(objectId);
        }

        // PUT: api/ObjectDefinition/5
        public void Put(int id, [FromBody]ObjectDefinition value)
        {
            if(value != null)
            {
                objectService.CreateORUpdateObjectDefinition(value);
            }
        }

        // DELETE: api/ObjectDefinition/5
        public void Delete(int id)
        {
            objectService.DeleteObjectDefinition(id);
        }
    }
}
