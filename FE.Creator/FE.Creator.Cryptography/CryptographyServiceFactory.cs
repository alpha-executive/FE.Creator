using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.Cryptography
{
    public class CryptographyServiceFactory
    {
        private static object SyncRoot = new object();
        private static ICryptographyService rsaEncryptionService = null;
        public static ICryptographyService RSAEncryptionService
        {
            get
            {
                lock (SyncRoot)
                {
                    if (rsaEncryptionService == null)
                    {
                        rsaEncryptionService = new RSACryptographyService();
                    }
                }

                return rsaEncryptionService;
            }
        }
    }
}
