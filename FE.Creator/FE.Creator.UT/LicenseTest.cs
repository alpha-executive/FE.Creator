using FE.Creator.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FE.Creator.UT
{
    [TestClass]
   public class LicenseTest
    {
        [TestMethod]
        public void EncryptLicenseTest()
        {
            IRSACryptographyService cservice = CryptographyServiceFactory.RSACryptoService;
            byte[] keys = cservice.getEncryptionKeys();
            string publicKey = Convert.ToBase64String(cservice.ExtractPublicKey(keys));
            string privateKey = Convert.ToBase64String(cservice.ExtractPrivateKey(keys));

            XDocument document = XDocument.Load(@"C:\Workspace\projects\devops\FE.Creator\FE.Creator.Admin\App_Data\License.xml");
            string xmlNode = document.Descendants("grantlist").FirstOrDefault().ToString();
           // string sha = CryptographyServiceFactory.SHAService.CalculateSHA256(UTF8Encoding.Default.GetBytes(xmlNode));

            byte[] hashContent = UTF8Encoding.UTF8.GetBytes(xmlNode);
            byte[] content = cservice.HashAndSignBytes(hashContent, privateKey);
            string strContent = Convert.ToBase64String(content);

            Assert.IsTrue(cservice.VerifySignedHash(hashContent, content, publicKey));
            Trace.WriteLine("Public Key: " + publicKey);
            Trace.WriteLine("Private Key: " + privateKey);
            Trace.WriteLine("License: " + strContent);
        }

        [TestMethod]
        public void SymmetricEncryptionTest()
        {
            ISymmetricCryptographyService service = CryptographyServiceFactory.SymmetricCryptoService;
            string key = Convert.ToBase64String(service.getEncryptionKeys());

            byte[] testcontent = UTF8Encoding.UTF8.GetBytes("Hello, Guys!");
            byte[] econtent = service.EncryptData(testcontent, key);
            byte[] dcontent = service.DecryptData(econtent, key);

            Assert.AreEqual(UTF8Encoding.UTF8.GetString(testcontent),
                UTF8Encoding.UTF8.GetString(dcontent));
        }
    }
}
