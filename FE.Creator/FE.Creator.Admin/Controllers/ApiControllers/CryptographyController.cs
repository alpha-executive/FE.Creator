using FE.Creator.Admin.Models;
using FE.Creator.Cryptography;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    /// <summary>
    /// API to provide data cryptography services, include:
    ///  GET: /api/custom/Cryptography/GeneratePrivateKeyPairs
    ///       Generate RSA key pairs
    /// POST: /api/custom/Cryptography/EncryptData
    ///       Encrypt data with rsa key.
    ///       parameters: string data to be encrypted.
    /// POST: /api/custom/Cryptography/DecryptData
    ///       Decrypt data with rsa key.
    ///       parameters: data: base64 string data to be decrypted.
    /// </summary>
    [Authorize]
    public class CryptographyController : ApiController
    {
        ISymmetricCryptographyService cryptoservice = null;
        IRSACryptographyService rsaCryptoService = null;
        IObjectService objectService = null;

        private string GetSystemCryptographKeys()
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals("AppConfig", StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            var svcObjects = objectService.GetServiceObjects(findObjDef.ObjectDefinitionID, 
                new string[]{ "cryptoSecurityKey" }, 1, 1);
            string cryptoKey = svcObjects[0].GetPropertyValue<PrimeObjectField>("cryptoSecurityKey").GetStrongTypeValue<string>();

            return cryptoKey;
        }

        public CryptographyController(ISymmetricCryptographyService cryptoservice,
            IRSACryptographyService rsaCryptoService,
            IObjectService objectService)
        {
            this.cryptoservice = cryptoservice;
            this.objectService = objectService;
            this.rsaCryptoService = rsaCryptoService;
        }

        [HttpGet]
        public HttpResponseMessage GeneratePrivateKeyPairs()
        {
            HttpResponseMessage result = null;
            byte[] privateKey = rsaCryptoService.getEncryptionKeys();

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

        [HttpPost]
        [ResponseType(typeof(GenericDataModel))]
        public IHttpActionResult EncryptData([FromBody]GenericDataModel data)
        {
            byte[] bdata = System.Text.UTF8Encoding.Default.GetBytes(data.StringData);
            byte[] sdata = cryptoservice.EncryptData(bdata, GetSystemCryptographKeys());

            return this.Created<GenericDataModel>(Request.RequestUri, new GenericDataModel
            {
                StringData = Convert.ToBase64String(sdata)
            });
        }

        [HttpPost]
        [ResponseType(typeof(GenericDataModel))]
        public IHttpActionResult DecryptData([FromBody]GenericDataModel data)
        {
            byte[] bdata = Convert.FromBase64String(data.StringData);
            byte[] sdata = cryptoservice.DecryptData(bdata, GetSystemCryptographKeys());

            return this.Created<GenericDataModel>(Request.RequestUri,
                new GenericDataModel()
                {
                    StringData = System.Text.UTF8Encoding.Default.GetString(sdata)
                });
        }
    }
}
