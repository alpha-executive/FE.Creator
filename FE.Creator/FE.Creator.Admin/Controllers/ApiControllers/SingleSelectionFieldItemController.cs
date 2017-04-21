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
    /// DELETE: api/SingleSelectionFieldItem/{id}
    ///      delete a selection item of the single selection field by id.
    /// </summary>
    [UnknownErrorFilter]
    [Authorize]
    public class SingleSelectionFieldItemController : ApiController
    {
        IObjectService objectService = null;

        public SingleSelectionFieldItemController(IObjectService objectService)
        {
            this.objectService = objectService;
        }


        // DELETE: api/SingleSelectionFieldItem/5
        public void Delete(int id)
        {
            this.objectService.DeleteSingleSelectionFieldSelectionItem(id);
        }
    }
}
