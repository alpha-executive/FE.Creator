using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    public class GeneralObjectListController : ApiController
    {
        IObjectService objectService = null;

        public GeneralObjectListController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private Task<IEnumerable<ServiceObject>> getAllServiceObjectAsync(int id, string[] properties = null)
        {
            var objectList = objectService.GetAllSerivceObjects(id, properties);

            return Task.FromResult<IEnumerable<ServiceObject>>(objectList);
        }

        // GET: api/GeneralObjectList
        [ResponseType(typeof(IEnumerable<ServiceObject>))]
        public async Task<IHttpActionResult> Get(int id, string parameters)
         {
            var objectList = await getAllServiceObjectAsync(id,
                string.IsNullOrEmpty(parameters) ? null : parameters.Split(new char[] { ',' }));

            return this.Ok<IEnumerable<ServiceObject>>(objectList);
        }

    }
}
