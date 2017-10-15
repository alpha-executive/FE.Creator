using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.Cryptography
{
    internal class RSACryptographyService : IRSACryptographyService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger(typeof(RSACryptographyService));
        /// <summary>
        /// Decrypt A encryption binary data 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="decryptKey">base64 formatted private/public key pairs</param>
        /// <param name="fOAEP"></param>
        /// <returns></returns>
        public byte[] DecryptData(byte[] data, string decryptKey, bool fOAEP)
        {
            logger.Debug("Start DecryptData");
            
            byte[] keys = Convert.FromBase64String(decryptKey);

            logger.Debug("data : " + Convert.ToBase64String(data));

            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                logger.Debug("End DecryptData");
                return RSA.Decrypt(data, fOAEP);
            }
        }

        public byte[] EncryptData(byte[] data, string encryptKey, bool fOAEP)
        {
            logger.Debug("Start EncryptData");
            byte[] keys = Convert.FromBase64String(encryptKey);
            logger.Debug("Public Key: " + encryptKey);
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);

                logger.Debug("End EncryptData");
                return RSA.Encrypt(data, fOAEP);
            }
        }

        public byte[] ExtractPrivateKey(byte[] keys)
        {
            logger.Debug("Start ExtractPrivateKey");
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);
                logger.Debug("Keys: " + Convert.ToBase64String(keys));

                if (RSA.PublicOnly)
                    throw new ArgumentException("Not a valid private key provided.");

                logger.Debug("End ExtractPrivateKey");
                return RSA.ExportCspBlob(true);
            }
        }

        public byte[] ExtractPublicKey(byte[] keys)
        {
            logger.Debug("Start ExtractPublicKey");
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                RSA.ImportCspBlob(keys);
                logger.Debug("Keys: " + Convert.ToBase64String(keys));

                logger.Debug("End ExtractPublicKey");
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

        /// <summary>
        /// Sign the bytes by SHA256 alg with RSA private key.
        /// </summary>
        /// <param name="dataToSign">data array to be signed.</param>
        /// <param name="privateKey">base64 string private key used to sign the data.</param>
        /// <returns></returns>
        public byte[] HashAndSignBytes(byte[] dataToSign, string privateKey)
        {
            logger.Debug("Start HashAndSignBytes");
            try
            {
                logger.Debug("dataToSign : " + Convert.ToBase64String(dataToSign));
                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.  
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportCspBlob(Convert.FromBase64String(privateKey));

                // Hash and sign the data. Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return RSAalg.SignData(dataToSign, new SHA256CryptoServiceProvider());
            }
            catch (CryptographicException e)
            {
                logger.Error(e);
                return null;
            }
            finally
            {
                logger.Debug("End HashAndSignBytes");
            }
        }

        public bool VerifySignedHash(byte[] dataToVerify, byte[] signedData, string publicKey)
        {
            try
            {
                logger.Debug("Start VerifySignedHash");
                logger.Debug("dataToVerify : " + Convert.ToBase64String(dataToVerify));
                logger.Debug("signedData : " + Convert.ToBase64String(signedData));
                logger.Debug("publicKey : " + publicKey);

                // Create a new instance of RSACryptoServiceProvider using the 
                // key from RSAParameters.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                RSAalg.ImportCspBlob(Convert.FromBase64String(publicKey));

                // Verify the data using the signature.  Pass a new instance of SHA1CryptoServiceProvider
                // to specify the use of SHA1 for hashing.
                return RSAalg.VerifyData(dataToVerify, new SHA256CryptoServiceProvider(), signedData);

            }
            catch (CryptographicException e)
            {
                logger.Error(e);
                return false;
            }
        }
    }
}
