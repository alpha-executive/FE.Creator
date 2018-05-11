using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FE.Creator.ObjectRepository.EntityModels;
using FE.Creator.ObjectRepository;
using FE.Creator.ObjectRepository.ServiceModels;

namespace FE.Creator.UT
{
    [TestClass]
    public class ObjectRepositoryTest
    {
        [TestMethod]
        public void CreateObjectDefGroupTest()
        {
            IObjectService service = new DefaultObjectService();

            service.CreateOrUpdateObjectDefinitionGroup(new ObjectRepository.ServiceModels.ObjectDefinitionGroup()
            {
                GroupKey = "GROUP_SERVER_OBJECT",
                GroupName = "SERVER OBJECT",
            });

            Assert.IsTrue(service.GetObjectDefinitionGroups(null, null).Count == 1);
        }

        [TestMethod]
        public void CreateObjectDefinitionTest()
        {
            IObjectService service = new DefaultObjectService();

            ObjectDefinition definition = new ObjectDefinition();
            definition.IsFeildsUpdateOnly = false;
            definition.ObjectDefinitionKey = "PERSON_TEST";
            definition.ObjectDefinitionGroupID = service.GetObjectDefinitionGroups(null, null)[0].GroupID;
            definition.ObjectDefinitionName = "Person";
            definition.ObjectOwner = "Admin";
            definition.UpdatedBy = "Admin";

            //Person name
            definition.ObjectFields.Add(new PrimeDefinitionField()
            {
                ObjectDefinitionFieldKey = "PERSON_NAME",
                ObjectDefinitionFieldName = "Person Name",
                PrimeDataType = PrimeFieldDataType.String
            });


            SingleSDefinitionField selectionField = new SingleSDefinitionField()
            {
                ObjectDefinitionFieldKey = "PERSON_SEX",
                ObjectDefinitionFieldName = "Person Sex"
            };
            selectionField.SelectionItems.Add(new DefinitionSelectItem()
            {
                SelectDisplayName = "Male",
                SelectItemKey = "Male"
            });
            selectionField.SelectionItems.Add(
                     new DefinitionSelectItem()
                     {
                         SelectItemKey = "Female",
                         SelectDisplayName = "Female"
                     });

            //Person Sex
            definition.ObjectFields.Add(selectionField);

            //Person Age
            definition.ObjectFields.Add(new PrimeDefinitionField()
            {
                ObjectDefinitionFieldKey = "PERSON_AGE",
                ObjectDefinitionFieldName = "Person AGE",
                PrimeDataType = PrimeFieldDataType.Integer
            });

            //person Image
            definition.ObjectFields.Add(new ObjectDefinitionField(GeneralObjectDefinitionFieldType.File)
            {
                GeneralObjectDefinitionFiledType = GeneralObjectDefinitionFieldType.File,
                ObjectDefinitionFieldKey = "PERSON_IMAGE",
                ObjectDefinitionFieldName = "Person Image"
            });

            //Manager
            definition.ObjectFields.Add(new ObjRefDefinitionField()
            {
                ObjectDefinitionFieldKey = "PERSON_MANAGER",
                ObjectDefinitionFieldName = "Person Manager",
                ReferedObjectDefinitionID = 1
            });

            int objdefintionId = service.CreateORUpdateObjectDefinition(definition);
            Assert.IsTrue(objdefintionId > 0);
        }

        [TestMethod]
        public void GetObjectDefinitionByGroupTest()
        {
            IObjectService service = new DefaultObjectService();
            var objectGroup = service.GetObjectDefinitionGroups(null, null)[0];
            var objectDefinition = service.GetObjectDefinitionsByGroup(objectGroup.GroupID, 1, 10, null);

            Assert.IsTrue(objectDefinition.Count > 0);
        }

        [TestMethod]
        public void CreateObjectTest()
        {
            IObjectService service = new DefaultObjectService();
            ServiceObject svObject = new ServiceObject();
            svObject.ObjectName = "Peter";
            svObject.ObjectOwner = "Admin";
            svObject.OnlyUpdateProperties = false;
            svObject.UpdatedBy = "Admin";
            svObject.CreatedBy = "Admin";
            svObject.ObjectDefinitionId = service.GetAllObjectDefinitions()[0].ObjectDefinitionID;

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Name",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = "Peter, Robert"
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Sex",
                Value = new SingleSelectionField()
                {
                    SelectedItemID = 5
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person AGE",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.Integer,
                    Value = 30
                }
            });
            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Image",
                Value = new ObjectFileField()
                {
                    FileCRC = "10001001",
                    FileExtension = ".docx",
                    FileFullPath = "c:\\location.docx",
                    FileName = "location.docx",
                    FileUrl = "http://www.url.com",
                    FileSize = 10,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                }
            });

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Manager",
                Value = new ObjectReferenceField()
                {
                    ReferedGeneralObjectID = 1
                }
            });

            int objId = service.CreateORUpdateGeneralObject(svObject);

            Assert.IsTrue(service.GetGeneralObjectCount(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, null) == 1);
        }

        [TestMethod]
        public void GetObjectTest()
        {
            IObjectService service = new DefaultObjectService();
            var objects = service.GetAllSerivceObjects(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, new string[] {
                   "Person Name",
                   "Person Sex",
                   "Person AGE",
                   "Person Image",
                   "Person Manager"
            }, null);

            var obj = objects[0];
            Assert.AreEqual(obj.ObjectName, "Peter");
            Assert.AreEqual(obj.GetPropertyValue<PrimeObjectField>("Person Name").GetStrongTypeValue<string>(), "Peter, Robert");
            Assert.AreEqual(obj.GetPropertyValue<SingleSelectionField>("Person Sex").SelectedItemID, 5);
            Assert.AreEqual(obj.GetPropertyValue<PrimeObjectField>("Person AGE").GetStrongTypeValue<int>(), 30);
            Assert.AreEqual(obj.GetPropertyValue<ObjectFileField>("Person Image").FileName, "location.docx");
            Assert.AreEqual(obj.GetPropertyValue<ObjectReferenceField>("Person Manager").ReferedGeneralObjectID, 1);


            obj = service.GetServiceObjectById(obj.ObjectID, new string[] {
                   "Person Name",
                   "Person Sex",
                   "Person Manager"
            }, null);
            Assert.AreEqual(obj.ObjectName, "Peter");
            Assert.AreEqual(obj.GetPropertyValue<PrimeObjectField>("Person Name").GetStrongTypeValue<string>(), "Peter, Robert");
            Assert.AreEqual(obj.GetPropertyValue<SingleSelectionField>("Person Sex").SelectedItemID, 5);
            Assert.AreEqual(obj.GetPropertyValue<ObjectReferenceField>("Person Manager").ReferedGeneralObjectID, 1);


            objects = service.GetServiceObjects(obj.ObjectDefinitionId, new string[] {
                   "Person Name",
                   "Person Sex",
                   "Person Manager"
            }, 1, 10, null);
            obj = objects[0];
            Assert.AreEqual(obj.ObjectName, "Peter");
            Assert.AreEqual(obj.GetPropertyValue<PrimeObjectField>("Person Name").GetStrongTypeValue<string>(), "Peter, Robert");
            Assert.AreEqual(obj.GetPropertyValue<SingleSelectionField>("Person Sex").SelectedItemID, 5);
            Assert.AreEqual(obj.GetPropertyValue<ObjectReferenceField>("Person Manager").ReferedGeneralObjectID, 1);
        }

        [TestMethod]
        public void UpdateObjectTest()
        {
            IObjectService service = new DefaultObjectService();
            ServiceObject svObject = service.GetAllSerivceObjects(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, null, null)[0];
            svObject.OnlyUpdateProperties = true;

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Name",
                Value = new PrimeObjectField()
                {
                    PrimeDataType = PrimeFieldDataType.String,
                    Value = "Peter, Robert - Updated"
                }
            });

            svObject.Properties.Add(new ObjectKeyValuePair()
            {
                KeyName = "Person Image",
                Value = new ObjectFileField()
                {
                    FileCRC = "10001001",
                    FileExtension = ".docx",
                    FileFullPath = "c:\\location-update.docx",
                    FileName = "location-update.docx",
                    FileUrl = "http://www.url.com",
                    FileSize = 10,
                    Created = DateTime.Now,
                    Updated = DateTime.Now
                }
            });

            int objectId = service.CreateORUpdateGeneralObject(svObject);
            var serviceobject = service.GetServiceObjectById(objectId, new string[]
             {
                "Person Name",
                "Person Image"
             }, null);

            Assert.AreEqual(serviceobject.GetPropertyValue<PrimeObjectField>("Person Name").GetStrongTypeValue<string>(), "Peter, Robert - Updated");
            Assert.AreEqual(serviceobject.GetPropertyValue<ObjectFileField>("Person Image").FileName, "location-update.docx");

        }

        [TestMethod]
        public void SoftDeleteObjectTest()
        {
            IObjectService service = new DefaultObjectService();
            ServiceObject svObject = service.GetAllSerivceObjects(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, null, null)[0];
            int objectId = service.SoftDeleteServiceObject(svObject.ObjectID, "Tester");
            Assert.AreEqual(objectId, svObject.ObjectID);
            Assert.AreEqual(service.GetGeneralObjectCount(svObject.ObjectDefinitionId, null), 0);
        }

        [TestMethod]
        public void DeleteObjectFieldTest()
        {
            IObjectService service = new DefaultObjectService();

            var objects = service.GetAllSerivceObjects(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, new string[] {
                   "Person Name"
            }, null);

            var obj = objects[0];
            Assert.IsNotNull(obj.GetPropertyValue<PrimeObjectField>("Person Name"));
            service.DeleteObjectField(obj.GetPropertyValue<PrimeObjectField>("Person Name").ObjectFieldID);
        }

        [TestMethod]
        public void DeleteObjectFieldDefinitionTest()
        {
            IObjectService service = new DefaultObjectService();

            var objects = service.GetAllSerivceObjects(service.GetAllObjectDefinitions()[0].ObjectDefinitionID, new string[] {
                   "Person Image"
            }, null);

            var obj = objects[0];
            Assert.IsNotNull(obj.GetPropertyValue<ObjectFileField>("Person Image"));

            service.DeleteObjectDefinitionField(4);
        }

        [TestMethod]
        public void DeleteObjectDefinitionTest()
        {
            IObjectService service = new DefaultObjectService();

            int definitionId = service.GetAllObjectDefinitions()[0].ObjectDefinitionID;
            var objects = service.GetAllSerivceObjects(definitionId, new string[] {
                   "Person Sex"
            }, null);

            var obj = objects[0];
            Assert.IsNotNull(obj.GetPropertyValue<SingleSelectionField>("Person Sex"));
            service.DeleteObjectDefinition(definitionId);
        }
    }
}
