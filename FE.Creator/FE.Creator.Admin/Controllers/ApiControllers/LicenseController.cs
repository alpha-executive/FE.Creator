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

namespace FE.Creator.Admin.Controllers.ApiControllers
{
    public class LicenseController : ApiController
    {
        IObjectService objectService = null;

        public LicenseController(IObjectService objectService)
        {
            this.objectService = objectService;
        }

        private string ReadResourceContent(string path)
        {
            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(path)))
            {
                return reader != null ? reader.ReadToEnd() : string.Empty;
            }
        }

        private int ResolveCategoryGroup(XElement config)
        {
            string group = config.Attribute("group").Value;
            string groupKey = config.Attribute("key").Value;

            if (!this.objectService.IsObjectDefinitionGroupExists(group))
            {
               return this.objectService.CreateOrUpdateObjectDefinitionGroup(new ObjectRepository.ServiceModels.ObjectDefinitionGroup
                {
                    GroupKey = groupKey,
                    GroupName = group
                });
            }

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
                    PrimeDefinitionField field = new PrimeDefinitionField();
                    field.PrimeDataType = (ObjectRepository.EntityModels.PrimeFieldDataType)Enum.Parse(typeof(ObjectRepository.EntityModels.PrimeFieldDataType), typeString);
                    field.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    field.ObjectDefinitionFieldKey = property.Attribute("key").Value;

                    return field;
                case "File":
                    ObjectDefinitionField fileField = new ObjectDefinitionField(ObjectRepository.EntityModels.GeneralObjectDefinitionFieldType.File);
                    fileField.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    fileField.ObjectDefinitionFieldName = property.Attribute("name").Value;

                    return fileField;
                case "ObjRef":
                    ObjRefDefinitionField refField = new ObjRefDefinitionField();
                    refField.ObjectDefinitionFieldKey = property.Attribute("key").Value;
                    refField.ObjectDefinitionFieldName = property.Attribute("name").Value;
                    refField.ReferedObjectDefinitionID = this.objectService.GetObjectDefinitionByName(property.Attribute("refName").Value).ObjectDefinitionID;

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

                    return ssField;
                default:
                    break;
            }

            return null;
        }

       private void ResolveEntity(int groupId, XElement entity)
        {
            ObjectDefinition definition = new ObjectDefinition();
            definition.IsFeildsUpdateOnly = false;
            definition.ObjectDefinitionKey = entity.Attribute("key").Value;
            definition.ObjectDefinitionGroupID = groupId;
            definition.ObjectDefinitionName = entity.Attribute("name").Value;
            definition.ObjectOwner = RequestContext.Principal.Identity.Name;
            definition.UpdatedBy = RequestContext.Principal.Identity.Name;

            foreach(XElement prop in entity.Descendants("property"))
            {
                ObjectDefinitionField objDefField = ParseObjectDefinitionField(prop);
                if (objDefField != null)
                {
                    definition.ObjectFields.Add(objDefField);
                }
            }

            this.objectService.CreateORUpdateObjectDefinition(definition);
        }

        private void ProcessConfig(string config)
        {
            XDocument configDoc = XDocument.Parse(config);
            XElement configElement = configDoc.Descendants("config").FirstOrDefault();
            if(configElement != null){
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
        private void RegistApplication(string license)
        {
            string licenseMap = ReadResourceContent("FE.Creator.Admin.Config.Module.LicenseMaps.xml");
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
                ProcessConfig(configureContent);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            //150 9337 8913
            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent("form-data"))
            {
                return BadRequest("Unsupported media type");
            }

            var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();
            foreach (var stream in filesReadToProvider.Contents)
            {
                var fileBytes = await stream.ReadAsByteArrayAsync();
                string license = System.Text.UTF8Encoding.Default.GetString(fileBytes);

                RegistApplication(license);
            }

            return this.Ok();
        }
    }
}
