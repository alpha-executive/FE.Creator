using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FE.Creator.CipherMachine
{
    using FE.Creator.Cryptography;
    using Lang;
    using System.Configuration;
    using System.IO;
    using System.IO.Compression;
    using System.Security.Cryptography;

    public partial class CipherMain : Form
    {
        private static readonly string USER_PER_CONFIG_FILE = "user.config";
        private static readonly int MAX_STRING_LENGTH = 128;
        public CipherMain()
        {
            InitializeComponent();
        }

        private bool IsInputTextValid()
        {
            return !string.IsNullOrEmpty(txtInputText.Text)
                && txtInputText.Text.Length <= MAX_STRING_LENGTH;
        }

        private List<int> UpdateEncryptContent(byte[] content)
        {
            int maxValue = content.Length - 1;
            int maxLength = 6;

            List<int> currPickedIndex = new List<int>();
            Random rand = new Random(DateTime.Now.Millisecond);
            while(currPickedIndex.Count < maxLength)
            {
                int nextIndex = rand.Next(maxValue);
                if (!currPickedIndex.Contains(nextIndex))
                {
                    currPickedIndex.Add(nextIndex);
                }
            }

            foreach(int index in currPickedIndex)
            {
                content[index] = (byte)(content[index] ^ 255);
            }

            return currPickedIndex;
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            if (IsInputTextValid())
            {
                try
                {
                    IRSACryptographyService cryptService = CryptographyServiceFactory.RSACryptoService;
                    byte[] plainTextContent = UTF8Encoding.UTF8.GetBytes(txtInputText.Text);
                    byte[] encryptContent = cryptService.EncryptData(plainTextContent, Properties.Settings.Default.EncryptKey, true);

                    List<int> randomIndexes = UpdateEncryptContent(encryptContent);
                    string encryptedContentStr = Convert.ToBase64String(encryptContent);
                    Properties.Settings.Default.EncryptData.Add(encryptedContentStr);
                    Properties.Settings.Default.Save();

                    byte[] lengthBytes = BitConverter.GetBytes(Properties.Settings.Default.EncryptData.Count - 1);
                    byte[] updatedIndexs = (from idx in randomIndexes
                                         select (byte)idx).ToArray();
                    byte[] finalPass = new byte[lengthBytes.Length + updatedIndexs.Length];
                    Array.Copy(lengthBytes, finalPass, lengthBytes.Length);
                    Array.Copy(updatedIndexs, 0, finalPass, lengthBytes.Length, updatedIndexs.Length);
                    txtResult.Text = Convert.ToBase64String(finalPass);
                }
                catch(Exception ex)
                {
                    lblStatus.Text = CipherLang.APP_ENCRYPT_ERROR + ": " + ex.Message;
                }
               
            }
            else
            {
                lblStatus.Text = CipherLang.APP_INPUT_TEXT_INVALID;
            }
           
        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            IRSACryptographyService cryptService = CryptographyServiceFactory.RSACryptoService;
            if (!string.IsNullOrEmpty(txtInputText.Text))
            {
                try
                {
                    byte[] encryptContent = Convert.FromBase64String(txtInputText.Text);
                    int encryptKeyLength = BitConverter.ToInt32(encryptContent, 0);

                    string encryptedString = Properties.Settings.Default.EncryptData[encryptKeyLength];
                    byte[] encryptedData = Convert.FromBase64String(encryptedString);

                    for(int i=4; i<encryptContent.Length; i++)
                    {
                        encryptedData[encryptContent[i]] = (byte) (encryptedData[encryptContent[i]] ^ 255);
                    }

                    byte[] decryptContent = cryptService.DecryptData(encryptedData, Properties.Settings.Default.EncryptKey, true);
                    txtResult.Text = UTF8Encoding.UTF8.GetString(decryptContent);
                }
                catch(Exception ex)
                {
                    lblStatus.Text = CipherLang.APP_DECRYPT_ERROR + ": " + ex.Message;
                }
                
            }
            else
            {
                lblStatus.Text = CipherLang.APP_INPUT_TEXT_INVALID;
            }
            
        }

        private void CipherMain_Load(object sender, EventArgs e)
        {
            try
            {
                //var culture = new System.Globalization.CultureInfo("zh-CN");
                //System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                //System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                lblStatus.Text = CipherLang.APP_INITIALIZING;
                grpPanel.Text = CipherLang.LBL_INPUT_STRING;
                btnDecrypt.Text = CipherLang.BTN_DECRYPT;
                btnEncrypt.Text = CipherLang.BTN_ENCRYPT;
                btnCopyResult.Text = CipherLang.BTN_COPY;

                if (IsNeedMigration())
                {
                    this.Close();
                    return;
                }

                lblStatus.Text = CipherLang.APP_VERSION;

                if (string.IsNullOrEmpty(Properties.Settings.Default.EncryptKey))
                {
                    IRSACryptographyService cryptService = CryptographyServiceFactory.RSACryptoService;
                    byte[] keys = cryptService.getEncryptionKeys();
                    string encryptKey = Convert.ToBase64String(keys);
                    Properties.Settings.Default.EncryptKey = encryptKey;
                    Properties.Settings.Default.Save();
                }

                if (Properties.Settings.Default.EncryptData == null)
                {
                    Properties.Settings.Default.EncryptData = new System.Collections.Specialized.StringCollection();
                    Properties.Settings.Default.Save();
                }

                //using (StreamWriter writer = new StreamWriter(config.FilePath))
                //{
                //    writer.
                //}
            }
            catch (Exception ex)
            {
                lblStatus.Text = CipherLang.APP_INITIALIZE_FAILED + ": " + ex.Message;
            }
        }

        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private static bool IsNeedMigration()
        {
            bool needMigration = false;
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(USER_PER_CONFIG_FILE))
            {
               
                if (File.Exists(config.FilePath))
                {
                    needMigration = !CalculateMD5(USER_PER_CONFIG_FILE)
                        .Equals(CalculateMD5(config.FilePath));
                }
                else
                {
                    needMigration = true;
                }
            }

            if (needMigration)
            {
                MessageBox.Show(string.Format(CipherLang.APP_WARN_NEED_MIG, config.FilePath),
                CipherLang.APP_WARN_NEED_MIG_CAP, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Warning);
            }

            return needMigration;
        }

        private void btnCopyResult_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtResult.Text))
            {
                Clipboard.SetText(txtResult.Text, TextDataFormat.Text);
                lblStatus.Text = CipherLang.APP_RESULT_COPIED;
            }
        }

        private void CipherMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(config.FilePath))
            {
                //if there is any update to config file.
                if (File.Exists(USER_PER_CONFIG_FILE)
                    && CalculateMD5(config.FilePath)
                        .Equals(CalculateMD5(USER_PER_CONFIG_FILE)))
                {
                        return;
                }

                File.Copy(config.FilePath, USER_PER_CONFIG_FILE, true);               
            }
        }
    }
}
