using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.ApiControllers.Controllers
{
    using ObjectRepository.EntityModels;
    using ObjectRepository;
    using ObjectRepository.ServiceModels;
    using System.Threading.Tasks;
    using System.Web.Http.Description;
    using MVCExtension;
    using NLog;
    using System.Configuration;

    /// <summary>
    /// API for service objects
    ///  GET: api/objects/list/{definitionname}/{parameters}?pageSize=xx&pageIndex=xx
    ///         {parameters}: optional, peroperties name, split by comma.
    ///         {definitionname}: required, object definition name
    ///         pageSize and PageIndex: optional
    ///       return: the object list of the specific object definition, 
    ///  GET: api/custom/GeneralObject/CountObjects/id
    ///       return: the count of objects of the specific object definition, by definition id.
    ///  GET:  api/objects/CountObjects/{definitionname}
    ///       return: the count of objects of the specific object definition, by definition name.
    ///  GET: api/custom/GeneralObject/FindServiceObjects/{id}/parameters?pageIndex=xxx&pageSize=xxx
    ///         {parameters}: optional, peroperties name, split by comma.
    ///         {id}: required, object definition id
    ///         pageSize and PageIndex: optional
    ///       return: the object list of the specific object definition.
    ///  GET: api/custom/GeneralObject/FindServiceObject/{id}
    ///       return: find the general object by object id.
    ///  
    /// GET: /api/objects/GeneralObject/FindServiceObjectsByFilter/{definitionname}/{parameters}?pageIndex=?&pageSize=?&filters=?
    ///        {parameters}: optional, perperties name, splited by comma.
    ///        {definitionname}: required, object defintion name
    ///        filters: query parameters, property filters, key,value;key,value format.
    ///       return: the founded general object list
    ///
    /// Get: /api/objects/GeneralObject/FindSysObjectsByFilter/{definitionname}/{parameters}?pageIndex=?&pageSize=?&filters=?
    ///     get all the service objects by filters, it is not user specific.
    ///        {parameters}: optional, perperties name, splited by comma.
    ///        {definitionname}: required, object defintion name
    ///        filters: query parameters, property filters, key,value;key,value format.
    ///       return: the founded general object list
    ///  POST api/GeneralObject
    ///       creat new service object
    ///       required body parameter: ServiceObject value
    ///  
    ///  PUT: api/GeneralObject/{id}
    ///       update the service object
    ///       {id}, required service object id.
    ///  DELETE: api/GeneralObject/{id}
    ///        delete a service object
    ///        {id}, required service object id
    ///       
    /// </summary>
    [UnknownErrorFilter]
    [Authorize]
    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;
        ILogger logger = LogManager.GetCurrentClassLogger(typeof(GeneralObjectController));
        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
        }


        private ObjectDefinition FindObjectDefinitionByName(string defname)
        {
            logger.Debug("Start FindObjectDefinitionByName");

            var objDefs = objectService.GetAllObjectDefinitions();

            logger.Debug("objDefs.count : " + objDefs.Count);

            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defname, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            logger.Debug("End FindObjectDefinitionByName");
            return findObjDef;
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
            logger.Debug("Start GeneralObjectController.List");

            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue || pageSize <= 0)
                pageSize = int.MaxValue;


            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                logger.Debug("find object definition: " + findObjDef.ObjectDefinitionName);

                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID,
                    new ServiceRequestContext()
                    {
                        IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                        RequestUser = RequestContext.Principal.Identity.Name,
                        UserSenstiveForSharedData = false
                    },
                    parameters,
                    pageIndex, 
                    pageSize,
                    null);
            }

            logger.Error("object definition was not found");
            logger.Debug("End GeneralObjectController.List");

            return this.NotFound();
        }

        /// <summary>
        ///  api/objects/CountObjects/{definitionname}
        /// </summary>
        /// <param name="definitionname"></param>
        /// <returns></returns>
        [ResponseType(typeof(int))]
        [HttpGet]
        public async Task<IHttpActionResult> CountObjects(string definitionname)
        {
            logger.Debug("Start CountObjects");
            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                int findObjectsCount = await this.CountObjects(findObjDef.ObjectDefinitionID);
                logger.Debug("findObjectsCount : " + findObjectsCount);

                return this.Ok<int>(findObjectsCount);
            }

            logger.Error("findObjDef is null");
            logger.Debug("End CountObjects");

            return this.NotFound();
        }

        /// <summary>
        ///  Get the count of General objects of the specific object definition.
        ///  GET: api/custom/GeneralObject/CountObjectServices/{id}
        /// </summary>
        /// <param name="id">the id of the object definition</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<int> CountObjects(int id, string filters = null) {
            int count = 0;

            logger.Debug("Start CountObjects");
            if (string.IsNullOrEmpty(filters))
            {
                logger.Debug("no filters");
                count = objectService.GetGeneralObjectCount(id,
                    new ServiceRequestContext()
                    {
                        IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                        RequestUser = RequestContext.Principal.Identity.Name,
                        UserSenstiveForSharedData = false
                    });
            }
           else
            {
                logger.Debug("filters : " + filters);
                List<ObjectKeyValuePair> filterKps = ParseFilterKeyValuePairs(filters);
                var fields = from kv in filterKps
                             select kv.KeyName;

                List<ServiceObject> foundObjects = await FilterServiceObjects(id, 
                    string.Join(",", fields.ToArray()),
                    1, 
                    int.MaxValue,
                    filterKps.Count > 0 ? filterKps : null,
                     new ServiceRequestContext()
                     {
                         IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                         RequestUser = RequestContext.Principal.Identity.Name,
                         UserSenstiveForSharedData = false
                     });

                count = foundObjects.Count;
            }
            logger.Debug("find count : " + count);
            logger.Debug("End CountObjects");
            return count;
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, int pageIndex, int pageSize, ServiceRequestContext svcRequestContext, string[] properties = null)
        {
            logger.Debug("Start getAllServiceObjectAsync");
            var objectList = objectService.GetServiceObjects(id, 
                properties,
                pageIndex, 
                pageSize,
                svcRequestContext);

            logger.Debug("End getAllServiceObjectAsync");
            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        private bool IsFieldValueMatched(ServiceObjectField fieldValue, string value, bool isArrayValue)
        {
            //if no filter value, then the record is matched.
            if (string.IsNullOrEmpty(value))
                return true;

            //if the field properties is null, then not matched.
            if (fieldValue == null)
                return false;

            if (isArrayValue)
            {
                string[] fieldValues = value.Split(new char[] { ',' });
                var found = (from v in fieldValues
                             where fieldValue.isFieldValueEqualAs(v)
                             select v).Count();

                return found > 0;
            }

            return fieldValue.isFieldValueEqualAs(value);
        }


        private async Task<IHttpActionResult> FindServiceObjects(int id, ServiceRequestContext svcRequestContext, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue, List<ObjectKeyValuePair> filters = null)
        {
            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue || pageSize <= 0)
                pageSize = int.MaxValue;

            List<ServiceObject> foundObjects = await FilterServiceObjects(id, 
                parameters, 
                pageIndex, 
                pageSize, 
                filters,
                svcRequestContext);
            logger.Debug(string.Format("foundObjects {0}", foundObjects != null ? foundObjects.Count : 0));
            return this.Ok<IEnumerable<ServiceObject>>(foundObjects);
        }

        private async Task<List<ServiceObject>> FilterServiceObjects(int id, string parameters,
            int? pageIndex,
            int? pageSize,
            List<ObjectKeyValuePair> filters,
            ServiceRequestContext svcRequestContext)
        {
            logger.Debug("Start FilterServiceObjects");
            var objectList = await getAllServiceObjectAsync(id,
                            1,
                            int.MaxValue,
                            svcRequestContext,
                            string.IsNullOrEmpty(parameters) ? null : parameters.Split(new char[] { ',' }));
            logger.Debug(string.Format("filters : {0}",  filters));
            List<ServiceObject> foundObjects = new List<ServiceObject>();
            if (filters != null)
            {
                foreach (var obj in objectList)
                {
                    bool isFilterMatched = true;
                    foreach (var f in filters)
                    {
                        var kvp = (from k in obj.Properties
                                   where k.KeyName.Equals(f.KeyName, StringComparison.InvariantCultureIgnoreCase)
                                   select k).FirstOrDefault();

                        isFilterMatched = kvp != null && isFilterMatched 
                            && IsFieldValueMatched((ServiceObjectField)kvp.Value, (string)f.Value, f.IsArray);

                        //if there is a filter is not matched, then the complete record is not matched.
                        if (!isFilterMatched)
                            break;
                    }

                    if (isFilterMatched)
                        foundObjects.Add(obj);
                }
            }
            else
            {
                foundObjects.AddRange(objectList);
            }

            logger.Debug("End FilterServiceObjects");
            return foundObjects
                .Skip((pageIndex.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();
        }

        /// <summary>
        ///     GET: api/custom/GeneralObject/FindServiceObjects/{id}/parameters?pageIndex=xxx&pageSize=xxx
        /// </summary>
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        [HttpGet]
        public async Task<IHttpActionResult> FindServiceObjects(int id, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue)
        {
            return await FindServiceObjects(id,
                new ServiceRequestContext()
                {
                    IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                    RequestUser = RequestContext.Principal.Identity.Name,
                    UserSenstiveForSharedData = false
                },
                parameters, 
                pageIndex, 
                pageSize, 
                null);
        }

        /// <summary>
        ///  GET: api/custom/GeneralObject/FindServiceObject/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ServiceObject))]
        [HttpGet]
        public IHttpActionResult FindServiceObject(int id, string parameters = null)
        {
            logger.Debug("Start FindServiceObject");
            var obj = objectService.GetServiceObjectById(id, 
                    string.IsNullOrEmpty(parameters) 
                    ? null : parameters.Split(new char[] { ',' }),
                    new ServiceRequestContext()
                    {
                        IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                        RequestUser = RequestContext.Principal.Identity.Name,
                        UserSenstiveForSharedData = false
                    });
            logger.Debug(string.Format("obj : {0}", obj != null ? obj.ObjectName : string.Empty));
            logger.Debug("End FindServiceObject");
            return this.Ok<ServiceObject>(obj);
        }

        private List<ObjectKeyValuePair> ParseFilterKeyValuePairs(string filters)
        {
            List<ObjectKeyValuePair> filterKps = new List<ObjectKeyValuePair>();
            if (!string.IsNullOrEmpty(filters))
            {
                string[] keyValues = filters.Split(new char[] { ';' });
                foreach (string kv in keyValues)
                {
                    if (string.IsNullOrEmpty(kv))
                        continue;

                    string[] kpairs = kv.Split(new char[] { ',' });
                    ObjectKeyValuePair kvp = new ObjectKeyValuePair();
                    kvp.KeyName = kpairs[0];
                    kvp.Value = string.Join(",", kpairs.Skip(1).ToArray());
                    kvp.IsArray = kpairs.Length > 2;
                    filterKps.Add(kvp);
                }
            }

            return filterKps;
        }

        /// <summary>
        /// GET: api/custom/GeneralObject/FindServiceObjectsByFilter/{definitionname}/{parameters}?pageIndex=?&pageSize=?&filters=?
        /// </summary>
        /// <param name="definitionname"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ServiceObject))]
        public async Task<IHttpActionResult> FindServiceObjectsByFilter(string definitionname, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue, string filters = null)
        {
            logger.Debug("Start FindServiceObjectsByFilter");
            var findObjDef = FindObjectDefinitionByName(definitionname);
            if (findObjDef != null)
            {
                List<ObjectKeyValuePair> filterKps = ParseFilterKeyValuePairs(filters);

                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID,
                    new ServiceRequestContext()
                    {
                        IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                        RequestUser = RequestContext.Principal.Identity.Name,
                        UserSenstiveForSharedData = false
                    },
                    parameters,
                    pageIndex,
                    pageSize,
                    filterKps.Count > 0 ? filterKps : null);
            }

            logger.Error("findObjDef is null");
            logger.Debug("End FindServiceObjectsByFilter");

            return this.NotFound();
        }

        /// <summary>
        /// Get: /api/custom/GeneralObject/FindSysObjectsByFilter/{definitionname}/{parameters}?pageIndex=?&pageSize=?&filters=?
        ///     get all the service objects by filters, it is not user specific.
        /// </summary>
        /// <param name="definitionname"></param>
        /// <param name="parameters"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(ServiceObject))]
        public async Task<IHttpActionResult> FindSysObjectsByFilter(string definitionname, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue, string filters = null)
        {
            logger.Debug("Start FindServiceObjectsByFilter");
            var findObjDef = FindObjectDefinitionByName(definitionname);
            if (findObjDef != null)
            {
                List<ObjectKeyValuePair> filterKps = ParseFilterKeyValuePairs(filters);

                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID,
                   null,
                    parameters,
                    pageIndex,
                    pageSize,
                    filterKps.Count > 0 ? filterKps : null);
            }

            logger.Error("findObjDef is null");
            logger.Debug("End FindServiceObjectsByFilter");

            return this.NotFound();
        }

        private void EnsureServiceProperties(ServiceObject svcObject)
        {
            logger.Debug("Start EnsureServiceProperties");
            ObjectDefinition objDef = objectService.GetObjectDefinitionById(svcObject.ObjectDefinitionId);
            List<ObjectKeyValuePair> removedProperties = new List<ObjectKeyValuePair>();
            if (objDef != null)
            {
                logger.Debug("objDef : " + objDef.ObjectDefinitionName);
            }
            else
            {
                logger.Error("objDef is null");
            }

            logger.Debug("svcObject.Properties.Count: " + svcObject.Properties.Count);
            for(int i=0; i<svcObject.Properties.Count; i++)
            {
                ObjectKeyValuePair property = svcObject.Properties[i];
                var defField = (from f in objDef.ObjectFields
                                where f.ObjectDefinitionFieldName.Equals(property.KeyName, StringComparison.InvariantCultureIgnoreCase)
                                select f).FirstOrDefault();
                if(defField != null)
                {
                    switch (defField.GeneralObjectDefinitionFiledType)
                    {
                        case GeneralObjectDefinitionFieldType.PrimeType:
                            if(!(property.Value is PrimeObjectField))
                            {
                                removedProperties.Add(property);
                            }
                            ((PrimeObjectField)property.Value).PrimeDataType = ((PrimeDefinitionField)defField).PrimeDataType;
                            break;
                        case GeneralObjectDefinitionFieldType.File:
                            if (!(property.Value is ObjectFileField))
                            {
                                removedProperties.Add(property);
                            }
                            break;
                        case GeneralObjectDefinitionFieldType.ObjectReference:
                            if (!(property.Value is ObjectReferenceField))
                            {
                                removedProperties.Add(property);
                            }
                            break;
                        case GeneralObjectDefinitionFieldType.SingleSelection:
                            if (!(property.Value is SingleSelectionField))
                            {
                                removedProperties.Add(property);
                            }
                            break;
                        default:
                            //not supported yet.
                            logger.Error("Not supported property : " + property.KeyName);
                            removedProperties.Add(property);
                            break;
                    }
                }
                else
                {
                    removedProperties.Add(property);
                }
            }

            foreach(var p in removedProperties)
            {
                svcObject.Properties.Remove(p);
            }

            logger.Debug("End EnsureServiceProperties");
        }

        private ServiceObject InsertOrUpdateServiceObject(ServiceObject value, bool isUpdate)
        {
            logger.Debug("Start InsertOrUpdateServiceObject");
            if (value != null)
            {
                //only for create
                if (!isUpdate)
                {
                    logger.Debug("create new object");
                    value.CreatedBy = RequestContext.Principal.Identity.Name;
                    value.ObjectOwner = RequestContext.Principal.Identity.Name;
                }
           
                value.UpdatedBy = RequestContext.Principal.Identity.Name;
                EnsureServiceProperties(value);
                int objectId = objectService.CreateORUpdateGeneralObject(value);
                logger.Debug("new object id : " + objectId);
                var properties = (from kv in value.Properties
                                  select kv.KeyName).ToArray<string>();
                
               return objectService.GetServiceObjectById(objectId, 
                   properties,
                   new ServiceRequestContext()
                   {
                       IsDataCurrentUserOnly = bool.Parse(ConfigurationManager.AppSettings["IsDataForLoginUserOnly"]),
                       RequestUser = RequestContext.Principal.Identity.Name,
                       UserSenstiveForSharedData = false
                   });
            }

            logger.Error("value is null");
            logger.Debug("End InsertOrUpdateServiceObject");
            return value;
        }
        /// <summary>
        /// POST: api/GeneralObject
        /// </summary>
        [ResponseType(typeof(ServiceObject))]
        public IHttpActionResult Post([FromBody]ServiceObject value)
        {
            logger.Debug("Start GeneralObjectController.Post");
            ServiceObject postResult = InsertOrUpdateServiceObject(value, false);

            logger.Debug("End GeneralObjectController.Post");
            return this.Created<ServiceObject>(Request.RequestUri, postResult);
        }

        /// <summary>
        /// PUT: api/GeneralObject/5
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        [ResponseType(typeof(ServiceObject))]
        public IHttpActionResult Put(int id, [FromBody]ServiceObject value)
        {
            logger.Debug("Start GeneralObjectController.Put");
            ServiceObject putResult = InsertOrUpdateServiceObject(value, true);

            logger.Debug("End GeneralObjectController.Put");
            return this.Ok<ServiceObject>(putResult);
        }


        /// <summary>
        /// DELETE: api/GeneralObject/5
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            logger.Debug("Start GeneralObjectController.Delete");
            logger.Debug("delete object : " + id);
            objectService.DeleteServiceObject(id);
            logger.Debug("End GeneralObjectController.Delete");
        }
    }
}
