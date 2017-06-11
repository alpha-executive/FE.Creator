using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FE.Creator.Cryptography
{
   public interface ISymmetricCryptographyService
    {
        byte[] getEncryptionKeys();

        byte[] EncryptData(byte[] data, string encryptKey);

        byte[] DecryptData(byte[] data, string decryptKey);
    }
}
