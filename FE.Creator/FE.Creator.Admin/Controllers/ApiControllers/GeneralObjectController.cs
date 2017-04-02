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
    /// GET: api/objects/GeneralObject/FindServiceObjectsByFilter/{definitionname}/{parameters}?pageIndex=?&pageSize=?&filters=?
    ///        {parameters}: optional, perperties name, splited by comma.
    ///        {definitionname}: required, object defintion name
    ///        filters: query parameters, property filters, key,value;key,value format.
    ///       return: the founded general object list
    ///       
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
    [Authorize]
    public class GeneralObjectController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectController(IObjectService objectService)
        {
            this.objectService = objectService;
        }


        private ObjectDefinition FindObjectDefinitionByName(string defname)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defname, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

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
            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue || pageSize <= 0)
                pageSize = int.MaxValue;


            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID, parameters,pageIndex, pageSize, null);
            }

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
            var findObjDef = FindObjectDefinitionByName(definitionname);

            if (findObjDef != null)
            {
                int findObjectsCount = await this.CountObjects(findObjDef.ObjectDefinitionID);
             
                return this.Ok<int>(findObjectsCount);
            }

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

            if (string.IsNullOrEmpty(filters))
            {
                count = objectService.GetGeneralObjectCount(id);
            }
           else
            {
                List<ObjectKeyValuePair> filterKps = ParseFilterKeyValuePairs(filters);
                var fields = from kv in filterKps
                             select kv.KeyName;

                List<ServiceObject> foundObjects = await FilterServiceObjects(id, 
                    string.Join(",", fields.ToArray()),
                    1, 
                    int.MaxValue,
                    filterKps.Count > 0 ? filterKps : null);

                count = foundObjects.Count;
            }

            return count;
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, int pageIndex, int pageSize, string[] properties = null)
        {
            var objectList = objectService.GetServiceObjects(id, properties,pageIndex, pageSize);

            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        private bool IsFieldValueMatched(ServiceObjectField fieldValue, string value)
        {
            //if no filter value, then the record is matched.
            if (string.IsNullOrEmpty(value))
                return true;

            //if the field properties is null, then not matched.
            if (fieldValue == null)
                return false;

            return fieldValue.isFieldValueEqualAs(value);
        }

        private async Task<IHttpActionResult> FindServiceObjects(int id, string parameters = null, int? pageIndex = 1, int? pageSize = int.MaxValue, List<ObjectKeyValuePair> filters = null)
        {
            if (!pageIndex.HasValue || pageIndex <= 0)
                pageIndex = 1;

            if (!pageSize.HasValue || pageSize <= 0)
                pageSize = int.MaxValue;

            List<ServiceObject> foundObjects = await FilterServiceObjects(id, parameters, pageIndex, pageSize, filters);

            return this.Ok<IEnumerable<ServiceObject>>(foundObjects);
        }

        private async Task<List<ServiceObject>> FilterServiceObjects(int id, string parameters, int? pageIndex, int? pageSize, List<ObjectKeyValuePair> filters)
        {
            var objectList = await getAllServiceObjectAsync(id,
                            1,
                            int.MaxValue,
                            string.IsNullOrEmpty(parameters) ? null : parameters.Split(new char[] { ',' }));

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

                        isFilterMatched = kvp != null && isFilterMatched &&
                                                    IsFieldValueMatched((ServiceObjectField)kvp.Value, (string)f.Value);

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
            return await FindServiceObjects(id, parameters, pageIndex, pageSize, null);
        }

        /// <summary>
        ///  GET: api/custom/GeneralObject/FindServiceObject/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ServiceObject))]
        [HttpGet]
        public IHttpActionResult FindServiceObject(int id)
        {
            var obj = objectService.GetServiceObjectById(id, null);

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
                    kvp.Value = kpairs[1];
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
            var findObjDef = FindObjectDefinitionByName(definitionname);
            if (findObjDef != null)
            {
                List<ObjectKeyValuePair> filterKps = ParseFilterKeyValuePairs(filters);

                return await this.FindServiceObjects(findObjDef.ObjectDefinitionID,
                    parameters,
                    pageIndex,
                    pageSize,
                    filterKps.Count > 0 ? filterKps : null);
            }

            return this.NotFound();
        }

        private void EnsureServiceProperties(ServiceObject svcObject)
        {
            ObjectDefinition objDef =  objectService.GetObjectDefinitionById(svcObject.ObjectDefinitionId);
            List<ObjectKeyValuePair> removedProperties = new List<ObjectKeyValuePair>();

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
        }

        private ServiceObject InsertOrUpdateServiceObject(ServiceObject value, bool isUpdate)
        {
            if (value != null)
            {
                //only for create
                if (!isUpdate)
                {
                    value.CreatedBy = RequestContext.Principal.Identity.Name;
                    value.ObjectOwner = RequestContext.Principal.Identity.Name;
                }
           
                value.UpdatedBy = RequestContext.Principal.Identity.Name;
                EnsureServiceProperties(value);
                int objectId = objectService.CreateORUpdateGeneralObject(value);
                var properties = (from kv in value.Properties
                                  select kv.KeyName).ToArray<string>();
                
               return objectService.GetServiceObjectById(objectId, properties);
            }

            return value;
        }
        /// <summary>
        /// POST: api/GeneralObject
        /// </summary>
        [ResponseType(typeof(ServiceObject))]
        public IHttpActionResult Post([FromBody]ServiceObject value)
        {
            ServiceObject postResult = InsertOrUpdateServiceObject(value, false);
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
            ServiceObject putResult = InsertOrUpdateServiceObject(value, true);

            return this.Ok<ServiceObject>(putResult);
        }

        /// <summary>
        /// POST:  api/objects/edit/{definitionname}
        /// </summary>
        /// <param name="definitionname"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(ServiceObject))]
        public async Task<IHttpActionResult> Edit(string definitionname, [FromBody]ServiceObject obj)
        {
            return this.Ok<ServiceObject>(null);
        }


        /// <summary>
        /// DELETE: api/GeneralObject/5
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            objectService.DeleteServiceObject(id);
        }
    }
}
