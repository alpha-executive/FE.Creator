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
    using MVCExtension;
    using NLog;

    /// <summary>
    ///  GET api/objectdefinitions/list/{groupname}
    ///      {groupname}: optional, group name, if not specified, return all the object defintions.
    ///      return all the object defintions of current group specified by {groupname}
    ///  GET  api/custom/ObjectDefinition/GetAllDefinitions
    ///      return all the object definitions in the system.
    ///  GET api/custom/ObjectDefinition/FindObjectDefintionsByGroup/{id}
    ///      {id}: optional, group id, if not specified, return all the object definitions
    ///      return:  return all the object defintions of current group specified by {id}: group id.
    ///   GET: api/custom/ObjectDefinition/getSystemObjectDefinitions
    ///       get system build-in object definitions.
    ///   GET: api/custom/ObjectDefinition/getCustomObjectDefinitions
    ///       get all the custom defined object definitions. 
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
    [UnknownErrorFilter]
    [Authorize]
    public class ObjectDefinitionController : ApiController
    {
        IObjectService objectService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(ObjectDefinitionController));

        public ObjectDefinitionController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private Task<IEnumerable<ObjectDefinition>> getObjectDefinitions()
        {
            logger.Debug("Start getObjectDefinitions");
            var objDefinitions = objectService.GetAllObjectDefinitions();
            logger.Debug(string.Format("count of the object definitions: {0}", objDefinitions != null ? objDefinitions.Count : 0));

            logger.Debug("End getObjectDefinitions");
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
            logger.Debug("Start ObjectDefinitionController.List");
            int groupId = -1;
            if (!string.IsNullOrEmpty(groupname))
            {
                var groups = objectService.GetObjectDefinitionGroups(null);
                var foundGroup = (from g in groups
                                  where g.GroupName.Equals(groupname, StringComparison.InvariantCultureIgnoreCase)
                                  select g).FirstOrDefault();

                groupId = foundGroup != null ? foundGroup.GroupID : -1;
                logger.Debug("groupId = " + groupId);
            }

            logger.Debug("End ObjectDefinitionController.List");

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

        // GET: api/custom/ObjectDefinition/getSystemObjectDefinitions
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        [HttpGet]
        public IHttpActionResult getSystemObjectDefinitions()
        {
            logger.Debug("Start ObjectDefinitionController.getSystemObjectDefinitions");
            var groups = objectService.GetObjectDefinitionGroups(null);
            var sysGroup = (from g in groups
                            where g.GroupName.Equals("FESystem", StringComparison.InvariantCultureIgnoreCase)
                            select g).FirstOrDefault();
            logger.Debug("FESystem group found ? " + sysGroup != null);

            if (sysGroup != null)
            {
#if !DEBUG
                var objDefinitions = objectService.GetObjectDefinitionsByGroup(sysGroup.GroupID, 1, int.MaxValue);
#else
                var objDefinitions = objectService.GetAllObjectDefinitions();
#endif
                logger.Debug("objDefinitions.Count = " + objDefinitions.Count);
                return this.Ok<IEnumerable<ObjectDefinition>>(objDefinitions);
            }

            logger.Error("FESystem group not found, System is not in correct status, check license or database status.");
            return this.NotFound();
        }

        // GET: api/custom/ObjectDefinition/getCustomObjectDefinitions
        [ResponseType(typeof(IEnumerable<ObjectDefinition>))]
        [HttpGet]
        public IHttpActionResult getCustomObjectDefinitions()
        {
            logger.Debug("Start ObjectDefinitionController.getCustomObjectDefinitions");
            var groups = objectService.GetObjectDefinitionGroups(null);
            var sysGroup = (from g in groups
                              where g.GroupName.Equals("FESystem", StringComparison.InvariantCultureIgnoreCase)
                              select g).FirstOrDefault();
            logger.Debug("FESystem group found ? " + sysGroup != null);

            if(sysGroup!= null)
            {
                var objDefinitions = objectService.GetObjectDefinitionsExceptGroup(sysGroup.GroupID);
                logger.Debug("objDefinitions.Count = " + objDefinitions.Count);
                return this.Ok<IEnumerable<ObjectDefinition>>(objDefinitions);
            }

            logger.Error("FESystem group not found, System is not in correct status, check license or database status.");

            return this.NotFound();
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
            logger.Debug("Start Post");
            int objectId = -1;
            if(value != null)
            {
               objectId = objectService.CreateORUpdateObjectDefinition(value);
                logger.Debug("New object defintion with objectId = " + objectId);
            }

            logger.Debug("End Post");
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
            logger.Debug("Start Delete");
            objectService.DeleteObjectDefinition(id);
            logger.Debug("object definition id = " + id + " was deleted");
            logger.Debug("End Delete");
        }
    }
}
