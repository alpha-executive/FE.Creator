using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.Cryptography
{
    public class SHAService : ISHAService
    {
        private static ILogger logger = LogManager.GetCurrentClassLogger();
        public string CalculateSHA256(byte[] content)
        {
            using (var sha = SHA256.Create())
            {
                byte[] checksum = sha.ComputeHash(content);

                if (logger.IsDebugEnabled)
                {
                    logger.Debug("content : " + Convert.ToBase64String(content));
                    logger.Debug("chechsum : " + Convert.ToBase64String(checksum));
                }

                return Convert.ToBase64String(checksum);
            }
        }
    }
}
