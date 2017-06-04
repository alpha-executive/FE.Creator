using FE.Creator.Cryptography;
using FE.Creator.ObjectRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    [Authorize]
    public class CryptographyController : ApiController
    {
        ICryptographyService cryptoservice = null;
        IObjectService objectService = null;

        private string GetSystemCryptographKeys()
        {
            return string.Empty;
        }
        public CryptographyController(ICryptographyService cryptoservice, IObjectService objectService)
        {
            this.cryptoservice = cryptoservice;
            this.objectService = objectService;
        }

        [HttpGet]
        public HttpResponseMessage GeneratePrivateKeyPairs()
        {
            HttpResponseMessage result = null;
            byte[] privateKey = cryptoservice.getEncryptionKeys();

            if (privateKey != null)
            {
                result = Request.CreateResponse(HttpStatusCode.OK);
                result.Content = new ByteArrayContent(privateKey);
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = "rsa.key";
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return result;
        }

        [HttpGet]
        public string EncryptData(string data)
        {
            byte[] bdata = System.Text.UTF8Encoding.Default.GetBytes(data);
            byte[] sdata = cryptoservice.EncryptData(bdata, GetSystemCryptographKeys(), true);

            return Convert.ToBase64String(sdata);
        }

        [HttpGet]
        public string DecryptData(string data)
        {
            byte[] bdata = Convert.FromBase64String(data);
            byte[] sdata = cryptoservice.DecryptData(bdata, GetSystemCryptographKeys(), true);

            return System.Text.UTF8Encoding.Default.GetString(sdata);
        }
    }
}
