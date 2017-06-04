using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.Cryptography
{
    internal class RSACryptographyService : ICryptographyService
    {
        /// <summary>
        /// Decrypt A encryption binary data 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="decryptKey">base64 formatted private/public key pairs</param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] DecryptData(byte[] data, string decryptKey, bool fOAEP)
        {
            byte[] keys = Convert.FromBase64String(decryptKey);
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                return RSA.Decrypt(data, fOAEP);
            }
        }

        public byte[] EncryptData(byte[] data, string encryptKey, bool fOAEP)
        {
            byte[] keys = Convert.FromBase64String(encryptKey);
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                return RSA.Encrypt(data, fOAEP);
            }
        }

        public byte[] ExtractPrivateKey(byte[] keys)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                if (RSA.PublicOnly)
                    throw new ArgumentException("Not a valid private key provided.");

                return RSA.ExportCspBlob(true);
            }
        }

        public byte[] ExtractPublicKey(byte[] keys)
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                return RSA.ExportCspBlob(false);
            }
        }

        public byte[] getEncryptionKeys()
        {
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
               return RSA.ExportCspBlob(true);
            }
        }
    }
}
