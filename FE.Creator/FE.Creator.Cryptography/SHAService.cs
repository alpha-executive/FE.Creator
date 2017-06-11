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
        public string CalculateSHA256(byte[] content)
        {
            using (var sha = SHA256.Create())
            {
                byte[] checksum = sha.ComputeHash(content);

                return Convert.ToBase64String(checksum);
            }
        }
    }
}
