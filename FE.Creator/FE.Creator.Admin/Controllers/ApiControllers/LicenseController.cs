using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Reflection;
using FE.Creator.ObjectRepository;
using System.Threading.Tasks;
using FE.Creator.ObjectRepository.ServiceModels;
using FE.Creator.Admin.Models;
using System.Runtime.Caching;
using FE.Creator.Admin.MVCExtension;
using FE.Creator.Cryptography;
using FE.Creator.ObjectRepository.EntityModels;
using System.Text;
using NLog;

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    [UnknownErrorFilter]
    [Authorize]
    public class LicenseController : ApiController
    {
        private static string SYS_RSA_PUBLIC_KEY = "BgIAAACkAABSU0ExAAQAAAEAAQBPkHSfs7Ukfng9Dz4EZZ1bDw5wCo4zKglQDlzOx01/b69bvLqxg2COkfKpegMJH8uDSGd8fvSSBKoWFu1RGnTomNUMHB7FRrbDAYQ0VAyUNfUcrZps8YlqgAjFGt3pF5GSoT7vGVVt3dKaRinvcPmlF3mk9qM/DHtqPfp4oA2Hqw==";
        private static string SYS_DEFAULT_LANGUAGE = "en-US";
        private static string SYS_LANG_CHINESE = "zh-CN";
        private static string SYS_LANG_ENGLISH = "en-US";
        private static string SYS_DEFAULT_DATEFORMAT = "MM/dd/yyyy";
        private static string SYS_DEFAULT_THEME = "skin-blue-light";
        private static int SYS_PULL_PUBLISHER_MESSAGE = 1;
        private static string SYS_PUBLISHER_URL = "http://localhost";
        private static string SYS_EMPTY_LICENSE_FILE = @"<?xml version=""1.0"" encoding=""utf-8""?><license></license>";
        IObjectService objectService = null;
        IRSACryptographyService cryptoGraphysvc = null;
        ISymmetricCryptographyService symmetricCryptoService = null;
        MemoryCache licenseCache = MemoryCache.Default;

        private static ILogger logger = LogManager.GetCurrentClassLogger(typeof(LicenseController));

        public LicenseController(IObjectService objectService,
            IRSACryptographyService cryptographysvc,
            ISymmetricCryptographyService symmetricCryptoService)
        {
            this.objectService = objectService;
            this.cryptoGraphysvc = cryptographysvc;
            this.symmetricCryptoService = symmetricCryptoService;
        }

        private string ReadResourceContent(string path)
        {
            logger.Debug("Start ReadResourceContent");
            logger.Debug(string.Format("path: {0}", path));
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(path)))
            {
                logger.Debug("End ReadResourceContent");
                return reader != null ? reader.ReadToEnd() : string.Empty;
            }
        }

        private int ResolveCategoryGroup(XElement config)
        {
            logger.Debug("Start ResolveCategoryGroup");
            string group = config.Attribute("group").Value;
            string groupKey = config.Attribute("key").Value;

            if (!this.objectService.IsObjectDefinitionGroupExists(group))
            {
                logger.Warn(string.Format("group {0} is not exists.", group));
                logger.Debug("End ResolveCategoryGroup");
                return this.objectService.CreateOrUpdateObjectDefinitionGroup(new ObjectRepository.ServiceModels.ObjectDefinitionGroup
                {
                    GroupKey = groupKey,
                    GroupName = group
                });
            }
            logger.Debug("End ResolveCategoryGroup");
            return this.objectService.GetObjectDefinitionGroupByName(group).GroupID;
        }
    

        private ObjectDefinitionField ParseObjectDefinitionField(XElement property)
        {
            string typeString = property.Attribute("type").Value;
            switch (typeString)
            {
                case "String":
                case "Integer":
                case "Long":
                case "Datetime":
                case "Number":
                case "Binary":
                    logger.Debug("Parse Binary field.");
                    PrimeDefinitionField field = new PrimeDefinitionField();
                    field.PrimeDataType = (ObjectRepository.EntityModels.PrimeFieldDataType)Enum.Parse(typeof(ObjectRepository.EntityModels.PrimeFieldDataType), typeString);
                    field.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    field.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    logger.Debug(string.Format("{0} = {1}", field.ObjectDefinitionFieldName, field.ObjectDefinitionFieldKey));
                    return field;
                case "File":
                    
                    ObjectDefinitionField fileField = new ObjectDefinitionField(ObjectRepository.EntityModels.GeneralObjectDefinitionFieldType.File);
                    fileField.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    fileField.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    logger.Debug("Parse File field " + fileField.ObjectDefinitionFieldName);

                    return fileField;
                case "ObjRef":
                    logger.Debug("Parse ObjRef field.");
                    ObjRefDefinitionField refField = new ObjRefDefinitionField();
                    refField.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    refField.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    refField.ReferedObjectDefinitionID = this.objectService.GetObjectDefinitionByName(property.Attribute("refName").Value).ObjectDefinitionID;

                    logger.Debug(string.Format("{0} = {1}", refField.ObjectDefinitionFieldName, refField.ReferedObjectDefinitionID));

                    return refField;
                case "SingleSelection":
                    SingleSDefinitionField ssField = new SingleSDefinitionField();
                    ssField.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    ssField.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    
                    foreach(XElement item in property.Descendants("choice"))
                    {
                        ssField.SelectionItems.Add(new DefinitionSelectItem()
                        {
                            SelectDisplayName = item.Attribute("displayName").Value,
                            SelectItemKey = item.Attribute("value").Value
                        });
                    }
                    logger.Debug("Parse SingleSelection field : " + ssField.ObjectDefinitionFieldName);
                    return ssField;
                default:
                    break;
            }

            return null;
        }

       private void ResolveEntity(int groupId, XElement entity)
        {
            logger.Debug("Start ResolveEntity");
            ObjectDefinition definition = new ObjectDefinition();
            
            definition.ObjectDefinitionKey = entity.Attribute("key").Value;
            definition.ObjectDefinitionGroupID = groupId;
            definition.ObjectDefinitionName = entity.Attribute("name").Value;
            definition.ObjectOwner = RequestContext.Principal.Identity.Name;
            definition.UpdatedBy = RequestContext.Principal.Identity.Name;

            var currentObjectDefinition = this.objectService.GetObjectDefinitionByName(definition.ObjectDefinitionName);
            definition.IsFeildsUpdateOnly = currentObjectDefinition != null;

            //if there is already a object there, do not register it.
            if (currentObjectDefinition == null)
            {
                logger.Warn(string.Format("NOT FOUND {0} in system, create new one", definition.ObjectDefinitionName));
                foreach (XElement prop in entity.Descendants("property"))
                {
                    ObjectDefinitionField objDefField = ParseObjectDefinitionField(prop);
                    if (objDefField != null)
                    {
                        definition.ObjectFields.Add(objDefField);
                    }
                }

                this.objectService.CreateORUpdateObjectDefinition(definition);
            }
            logger.Debug("End ResolveEntity");
        }

        private void ProcessConfig(string config)
        {
            XDocument configDoc = XDocument.Parse(config);
            XElement configElement = configDoc.Descendants("config").FirstOrDefault();
            if(configElement != null){
                logger.Info("process the system module config file.");
                int groupId = ResolveCategoryGroup(configElement);
                if (groupId > 0)
                {
                    foreach (XElement entity in configElement.Descendants("entity"))
                    {
                        ResolveEntity(groupId, entity);
                    }
                }
            }
        }

        private bool IsValidLicense(string license)
        {
            try
            {
                logger.Debug("Start IsValidLicense");
                XDocument licenseDoc = XDocument.Load(new StringReader(license));
                string grantlist = licenseDoc.Descendants("grantlist").FirstOrDefault().ToString();
                string productKey = licenseDoc.Descendants("productkey").FirstOrDefault().Value;

                logger.Debug("productKey = " + productKey);
                byte[] contentToVerified = UTF8Encoding.UTF8.GetBytes(grantlist);

                logger.Debug("SYS_RSA_PUBLIC_KEY = " + SYS_RSA_PUBLIC_KEY);
                bool isValid = cryptoGraphysvc.VerifySignedHash(contentToVerified,
                    Convert.FromBase64String(productKey),
                    SYS_RSA_PUBLIC_KEY);

                logger.Warn(string.Format("Product Key is Valid ? {0}", isValid));

                logger.Debug("End IsValidLicense");
                return isValid;
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private int GetAppObjectDefintionIdByName(string defName)
        {
            var objDefs = objectService.GetAllObjectDefinitions();
            var findObjDef = (from def in objDefs
                              where def.ObjectDefinitionName.Equals(defName, StringComparison.InvariantCultureIgnoreCase)
                              select def).FirstOrDefault();

            return findObjDef.ObjectDefinitionID;
        }
        private void UpdateSystemConfig(string license)
        {
            logger.Debug("Start UpdateSystemConfig");

            ServiceObject svObject = new ServiceObject();
            svObject.ObjectName = "FE Configuration";
            svObject.ObjectOwner = RequestContext.Principal.Identity.Name;
            svObject.OnlyUpdateProperties = false;
            svObject.UpdatedBy = RequestContext.Principal.Identity.Name;
            svObject.CreatedBy = RequestContext.Principal.Identity.Name;
            svObject.ObjectDefinitionId = GetAppObjectDefintionIdByName("AppConfig");

            logger.Debug(string.Format("svObject.ObjectName = {0}", svObject.ObjectName));
            logger.Debug(string.Format("svObject.ObjectDefinitionId = {0}", svObject.ObjectDefinitionId));
            logger.Debug(string.Format("svObject.ObjectOwner = {0}", svObject.ObjectOwner));

            //language
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "language",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = System.Threading.Thread.CurrentThread.CurrentUICulture
                    .TwoLetterISOLanguageName
                    .Equals(SYS_LANG_CHINESE, StringComparison.InvariantCultureIgnoreCase) ? SYS_LANG_CHINESE : SYS_LANG_ENGLISH
                }
            });

            //dateTimeFormat
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "dateTimeFormat",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = SYS_DEFAULT_DATEFORMAT
                }
            });

            //rsa key.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "cryptoSecurityKey",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = Convert.ToBase64String(symmetricCryptoService.getEncryptionKeys())
                }
            });

            //systemTheme.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "systemTheme",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = SYS_DEFAULT_THEME
                }
            });

            //pullMessageFromPublisher.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "pullMessageFromPublisher",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.Integer,
                    Value = SYS_PULL_PUBLISHER_MESSAGE
                }
            });
            //pullMessagePublisherUrl.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "pullMessagePublisherUrl",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = SYS_PUBLISHER_URL
                }
            });
            //systemVersion.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "systemVersion",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = Assembly.GetExecutingAssembly().GetName().Version.ToString(4)
                }
            });

            //license.
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "license",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = license
                }
            });

            objectService.CreateORUpdateGeneralObject(svObject);
            logger.Debug("End UpdateSystemConfig");
        }

        private void RegistApplication(string license)
        {
            logger.Debug("Start RegistApplication");
            string licenseMap = ReadResourceContent("FE.Creator.Admin.Config.Module.LicenseMaps.xml");
            logger.Debug(string.Format("licenseMap = {0}", licenseMap));
            XDocument document = XDocument.Parse(licenseMap);
            var files = from f in document.Descendants("module")
                        select new
                        {
                            LicenseId = f.Attribute("licenseId").Value,
                            FileUrl = f.Value
                        };
            
            foreach(var f in files)
            {
                string configureContent = ReadResourceContent(f.FileUrl);
                logger.Debug(string.Format("configure content = {0}", configureContent));
                ProcessConfig(configureContent);
            }

            UpdateSystemConfig(license);
            logger.Debug("End RegistApplication");
        }
        
        private string LoadLicenseFromConfig()
        {
           var objects =  objectService.GetServiceObjects(
                    GetAppObjectDefintionIdByName("AppConfig"),
                    new string[] {"license" },
                    1,
                    1);

            var licenseData = objects
                .FirstOrDefault()
                .GetPropertyValue<PrimeObjectField>("license")
                .GetStrongTypeValue<string>();
            logger.Debug(string.Format("license data from DB: {0}", licenseData));
            return IsValidLicense(licenseData)? licenseData : SYS_EMPTY_LICENSE_FILE;
        }

        private XDocument getCachedLicense()
        {
            XDocument license = licenseCache.Get("license") as XDocument;
            if(license == null)
            {
                logger.Debug("License is not in Cache, try in the DB.");
                string licenseContent = LoadLicenseFromConfig();
                license = XDocument.Load(new StringReader(licenseContent));

                if (license != null)
                {
                    logger.Debug(String.Format("get license from DB: {0}", license));
                    licenseCache.Add("license", license, DateTimeOffset.MaxValue);
                }
            }

            return license;
        }

        private void UpdateLicenseStatus(IEnumerable<LicensedModule> licModules)
        {
            XDocument licenseDoc = getCachedLicense();
            if (licenseDoc != null)
            {
                XElement licenseElement = licenseDoc.Descendants("grantlist").FirstOrDefault();
                var grantedList = from g in licenseDoc.Descendants("moudle")
                                  select g.Value;

                foreach (var lic in licModules)
                {
                    lic.ExpiredDate = DateTime.ParseExact(licenseElement.Attribute("expireddate").Value, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    lic.Licensed = grantedList.Contains(lic.ModuleId);
                }
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            logger.Debug("Start LicenseController.Post");
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                logger.Error("Unsupported media type");
                return BadRequest("Unsupported media type");
            }

            var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
            if(filesReadToProvider.Contents.Count > 0)
            {
                var stream = filesReadToProvider.Contents[0];
                var fileBytes = await stream.ReadAsByteArrayAsync();
                string license = System.Text.UTF8Encoding.UTF8.GetString(fileBytes);

                logger.Debug(string.Format("license : {0}", license));
                if (IsValidLicense(license))
                {
                    RegistApplication(license);
                }
            }
            logger.Debug("End LicenseController.Post");
            return this.Ok();
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetLicenseRegisterList()
        {
            string licenseMap = ReadResourceContent("FE.Creator.Admin.Config.Module.LicenseMaps.xml");
            XDocument document = XDocument.Parse(licenseMap);
            var files = (from f in document.Descendants("module")
                        select new LicensedModule
                        {
                            ModuleName = f.Attribute("name").Value,
                            ModuleDescription = f.Attribute("Description").Value,
                            ModuleId = f.Attribute("licenseId").Value,
                            Licensed = false,
                            ExpiredDate = DateTime.UtcNow
                        }).ToList();

            UpdateLicenseStatus(files);

            return this.Ok<IEnumerable<LicensedModule>>(await Task.FromResult<IEnumerable<LicensedModule>>(files));
        }
    }
}
