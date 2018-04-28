using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.ApiControllers.Controllers
{
    using MVCExtension;
    using NLog;
    using ObjectRepository;
    using ObjectRepository.ServiceModels;
    using System.Threading.Tasks;

    /// <summary>
    /// GET api/custom/ObjectDefinitionGroup/GetByParentId/{id}
    ///     {id}: optional, the id of parent definition group.
    ///     return list of the definition groups, if {id} is not provided, return all the groups.
    /// GET: api/ObjectDefinitionGroup/{id}
    ///     {id}: required, the id of parent definition group.
    ///     return the specific object defintion group.
    /// POST: api/ObjectDefinitionGroup
    ///     create a new ObjectDefinitionGroup instance,required ObjectDefinitionGroup in the body.
    /// PUT: api/ObjectDefinitionGroup/{id}
    ///    {id}: required object definition group id.
    ///    update a new object definition group by group id.
    /// DELETE: api/ObjectDefinitionGroup/{id}
    ///    {id}: required object definition group id.
    ///    delete a object definition instance.
    /// </summary>
    [UnknownErrorFilter]
    [Authorize]
    public class ObjectDefinitionGroupController : ApiController
    {
        IObjectService objectService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(ObjectDefinitionGroupController));

        public ObjectDefinitionGroupController(IObjectService service)
        {
            this.objectService = service;
        }

        /// <summary>
        /// GET api/custom/ObjectDefinitionGroup/GetByParentId/{id}
        ///     {id}: optional, the id of parent definition group.
        ///     return list of the definition groups, if {id} is not provided, return all the groups.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(IEnumerable<ObjectDefinitionGroup>))]
        [HttpGet]
        public async Task<IHttpActionResult> GetByParentId(int? id = null)
        {
            IEnumerable<ObjectDefinitionGroup> objDefGroups = await GetAllObjectDefinitionGroups(id);
#if !DEBUG
            objDefGroups = objDefGroups.Where(def =>
            {
                return !def.GroupName.Equals("FESystem")
                && !def.GroupKey.Equals("FE_SYS_APP_CONFIG");
            });
#endif

            return this.Ok<IEnumerable<ObjectDefinitionGroup>>(objDefGroups);
        }

        private Task<IEnumerable<ObjectDefinitionGroup>> GetAllObjectDefinitionGroups(int? parentGroupId)
        {
            return Task
                .FromResult<IEnumerable<ObjectDefinitionGroup>>(objectService.GetObjectDefinitionGroups(parentGroupId, 
                    null));
        }

        

        private Task<ObjectDefinitionGroup> GetObjectDefinitionGroup(int id)
        {
            return Task.FromResult<ObjectDefinitionGroup>(
                    objectService.GetObjectDefinitionGroupById(id)
                );
        }

        /// <summary>
        ///  GET: api/ObjectDefinitionGroup/5
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ObjectDefinitionGroup))]
        public async Task<IHttpActionResult> Get(int id)
        {
            logger.Debug("Start Get : " + id);
            var objDefGroup = await GetObjectDefinitionGroup(id);

            logger.Debug(String.Format("get object definition group: {0} ", objDefGroup != null ? objDefGroup.GroupName : string.Empty));

            logger.Debug("End Get : " + id);
            return this.Ok<ObjectDefinitionGroup>(objDefGroup);
        }


        // POST: api/ObjectDefinitionGroup
        [ResponseType(typeof(ObjectDefinitionGroup))]
        public IHttpActionResult Post([FromBody]ObjectDefinitionGroup value)
        {
            logger.Debug("Start Post ObjectDefinitionGroup");
            if (value != null)
            {
               value.GroupID =  objectService.CreateOrUpdateObjectDefinitionGroup(value);
                logger.Debug("New Created ObjectDefinitionGroup : " + value.GroupID);
            }
            logger.Debug("End Post ObjectDefinitionGroup");
            return this.Created<ObjectDefinitionGroup>(this.Url.ToString(), value);
        }

        // PUT: api/ObjectDefinitionGroup/5
        public void Put(int id, [FromBody]ObjectDefinitionGroup value)
        {
            logger.Debug("Start Put ObjectDefinitionGroup: " + id);
            if (value != null)
            {
                objectService.CreateOrUpdateObjectDefinitionGroup(value);
            }
            logger.Debug("End Put ObjectDefinitionGroup: " + id);
        }

        // DELETE: api/ObjectDefinitionGroup/5
        public void Delete(int id)
        {
            logger.Debug("Start Delete ObjectDefinitionGroup: " + id);
            objectService.SoftDeleteObjectDefintionGroup(id);
            logger.Debug("End Delete ObjectDefinitionGroup: " + id);
        }
    }
}
