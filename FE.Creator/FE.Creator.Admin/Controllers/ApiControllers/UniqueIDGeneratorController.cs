using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    /// <summary>
    /// Get a unique identifier for client side use.
    /// GET: /api/UniqueIDGenerator
    ///    return: a new GUID string.
    /// </summary>
    [Authorize]
    public class UniqueIDGeneratorController : ApiController
    {
        // GET: api/UniqueIDGenerator
        public string Get()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
