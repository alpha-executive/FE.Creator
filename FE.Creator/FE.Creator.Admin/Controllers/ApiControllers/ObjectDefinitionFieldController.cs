using FE.Creator.Admin.MVCExtension;
using FE.Creator.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    /// <summary>
    /// DELETE: api/ObjectDefinitionField/{id}
    ///      delete a object definition field by id
    /// </summary>
    [UnknownErrorFilter]
    [Authorize]
    public class ObjectDefinitionFieldController : ApiController
    {
        IObjectService objectService = null;


        public ObjectDefinitionFieldController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        // DELETE: api/ObjectDefinitionField/5
        public void Delete(int id)
        {
            objectService.DeleteObjectDefinitionField(id);
        }
    }
}
