﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FE.Creator.ObjectRepository.ServiceModels;
using FE.Creator.ObjectRepository.EntityModels;
using System.Data.Entity;
using FE.Creator.ObjectRepository.Utils;
using NLog;

namespace FE.Creator.ObjectRepository
{
    public class DefaultObjectService : IObjectService
    {
        private static object SyncRoot = new object();
        private static ILogger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Update the object field value according to the parameters provided in ObjectKeyValuePair
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldKvp"></param>
        /// <param name="isUpdate"></param>
        private static void UpdateObjectFieldValue(GeneralObjectField field, ObjectKeyValuePair fieldKvp, GeneralObjectDefinitionField fieldDefintion)
        {
            if (field is FileGeneralObjectField)
            {
                UpdateFileObjectField(field, fieldKvp);
            }
            else if (field is PrimeGeneralObjectField)
            {
                UpdatePrimeObjectField(field, fieldKvp, fieldDefintion);
            }
            else if (field is SingleSelectionGeneralObjectField)
            {
                UpdateSingleSelectionObjectField(field, fieldKvp);
            }
            else if(field is GeneralObjectReferenceField)
            {
                UpdateObjectReferenceField(field, fieldKvp);
            }
            else
            {
                logger.Error("Field Object Type is not supported currently : " + field.GetType().Name);
                throw new NotSupportedException("Field Object Type is not supported currently.");
            }
        }

        /// <summary>
        /// Update the object reference field value according to the parameters provided in ObjectKeyValuePair
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldKvp"></param>
        private static void UpdateObjectReferenceField(GeneralObjectField field, ObjectKeyValuePair fieldKvp)
        {
            GeneralObjectReferenceField f = field as GeneralObjectReferenceField;
            ObjectReferenceField fValue = fieldKvp.Value as ObjectReferenceField;

            f.ReferedGeneralObjectID = fValue.ReferedGeneralObjectID;
        }

        /// <summary>
        /// update the single selection object field value according to the parameters provided in ObjectKeyValuePair
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldKvp"></param>
        private static void UpdateSingleSelectionObjectField(GeneralObjectField field, ObjectKeyValuePair fieldKvp)
        {
            SingleSelectionGeneralObjectField f = field as SingleSelectionGeneralObjectField;
            SingleSelectionField fValue = fieldKvp.Value as SingleSelectionField;
            f.SelectedItemID = fValue.SelectedItemID;
        }

        /// <summary>
        /// upate the prime object field value according to the parameters provided in ObjectKeyValuePair
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldKvp"></param>
        private static void UpdatePrimeObjectField(GeneralObjectField field, ObjectKeyValuePair fieldKvp, GeneralObjectDefinitionField fieldDefintion)
        {
            PrimeGeneralObjectField f = field as PrimeGeneralObjectField;
            PrimeObjectField fValue = fieldKvp.Value as PrimeObjectField;
            PrimeObjectDefintionField defField = fieldDefintion as PrimeObjectDefintionField;

            switch (defField.PrimeDataType)
            {
                case PrimeFieldDataType.Integer:
                    f.IntegerValue = fValue.GetStrongTypeValue<int>();
                    break;
                case PrimeFieldDataType.Long:
                    f.LongValue = fValue.GetStrongTypeValue<long>();
                    break;
                case PrimeFieldDataType.Number:
                    f.NumberValue = fValue.GetStrongTypeValue<double>();
                    break;
                case PrimeFieldDataType.String:
                    f.StringValue = fValue.GetStrongTypeValue<string>();
                    break;
                case PrimeFieldDataType.Datetime:
                    f.DateTimeValue = fValue.GetStrongTypeValue<DateTime>();
                    break;
                case PrimeFieldDataType.Binary:
                    f.BinaryValue = fValue.GetStrongTypeValue<byte[]>();
                    break;
                default:
                    logger.Error(string.Format("{0} is not supported", defField.PrimeDataType.ToString()));
                    throw new NotSupportedException(string.Format("{0} is not supported", defField.PrimeDataType.ToString()));
            }
        }

        /// <summary>
        /// update the file object field value according to the parameters provided in ObjectKeyValuePair
        /// </summary>
        /// <param name="field"></param>
        /// <param name="fieldKvp"></param>
        /// <param name="isUpdate"></param>
        private static void UpdateFileObjectField(GeneralObjectField field, ObjectKeyValuePair fieldKvp)
        {
            ObjectFileField fValue = fieldKvp.Value as ObjectFileField;
            FileGeneralObjectField f = field as FileGeneralObjectField;
            f.FileName = fValue.FileName;
            f.FileFullPath = fValue.FileFullPath;
            f.FileExtension = fValue.FileExtension;
            f.FileCRC = fValue.FileCRC;
            f.FileSize = fValue.FileSize;
            f.FileUrl = fValue.FileUrl;
            f.Updated = fValue.Updated;
            f.Created = fValue.Created;
        }

        /// <summary>
        /// Parse the object field based on the object field definition.
        /// </summary>
        /// <param name="generalObject"></param>
        /// <param name="fieldDefintion"></param>
        /// <param name="fieldKvp"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        private static GeneralObjectField ParseObjectField(GeneralObject generalObject, GeneralObjectDefinitionField fieldDefintion, ObjectKeyValuePair fieldKvp)
        {
            if(fieldDefintion != null)
            {
                GeneralObjectField field = null;

                switch (fieldDefintion.GeneralObjectDefinitionFiledType)
                {
                    case GeneralObjectDefinitionFieldType.File:
                        field = new FileGeneralObjectField();
                        break;
                    case GeneralObjectDefinitionFieldType.PrimeType:
                        field = new PrimeGeneralObjectField();
                        break;
                    case GeneralObjectDefinitionFieldType.ObjectReference:
                        field = new GeneralObjectReferenceField();
                        break;
                    case GeneralObjectDefinitionFieldType.SingleSelection:
                        field = new SingleSelectionGeneralObjectField();
                        break;
                    default:
                        logger.Error("Field Object Type is not supported currently : " + field.GetType().Name);
                        throw new NotSupportedException("Field Object Type is not supported currently.");
                }

                //update the field values.
                UpdateObjectFieldValue(field, fieldKvp, fieldDefintion);

                field.GeneralObjectDefinitionField = fieldDefintion;
                field.GeneralObject = generalObject;

                return field;
            }

            throw new ArgumentNullException("fieldDefintion is null but it's required.");
        }

        /// <summary>
        /// Create a new General Object
        /// </summary>
        /// <param name="dboContext"></param>
        /// <param name="serviceObject"></param>
        /// <param name="goDefinition"></param>
        /// <returns></returns>
        private static int CreateGeneralObject(DBObjectContext dboContext, ServiceObject serviceObject, GeneralObjectDefinition goDefinition)
        {
            GeneralObject gobject = new GeneralObject();
            gobject.GeneralObjectName = serviceObject.ObjectName;
            gobject.ObjectOwner = serviceObject.ObjectOwner;
            gobject.GeneralObjectDefinition = goDefinition;
            gobject.Created = System.DateTime.Now;
            gobject.CreatedBy = serviceObject.CreatedBy;
            gobject.Updated = System.DateTime.Now;
            gobject.UpdatedBy = serviceObject.UpdatedBy;

            logger.Debug(string.Format("gobject.GeneralObjectName = {0}", serviceObject.ObjectName));
            logger.Debug(string.Format("gobject.ObjectOwner = {0}", serviceObject.ObjectOwner));
            logger.Debug(string.Format("gobject.GeneralObjectDefinition = {0}", goDefinition.GeneralObjectDefinitionName));
            logger.Debug(string.Format("gobject.CreatedBy = {0}", serviceObject.CreatedBy));
            logger.Debug(string.Format("gobject.UpdatedBy = {0}", serviceObject.UpdatedBy));

            logger.Debug("serviceObject.Properties.count = " + serviceObject.Properties.Count);
            foreach (ObjectKeyValuePair kvp in serviceObject.Properties)
            {
                var currentFieldDef = (from f in goDefinition.GeneralObjectDefinitionFields
                                       where f.GeneralObjectDefinitionFieldName.Equals(kvp.KeyName, StringComparison.InvariantCultureIgnoreCase)
                                       select f).FirstOrDefault();

                GeneralObjectField field = ParseObjectField(gobject, currentFieldDef, kvp);
                if (field != null)
                {
                    logger.Debug(string.Format("field = {0}, fieldId = {1}"
                        ,field.GeneralObjectDefinitionField.GeneralObjectDefinitionFieldName
                        ,field.GeneralObjectFieldID));

                    gobject.GeneralObjectFields.Add(field);
                }
            }

            dboContext.GeneralObjects.Add(gobject);
            dboContext.SaveChanges();

            return gobject.GeneralObjectID;
        }

        /// <summary>
        /// Update a general Object.
        /// </summary>
        /// <param name="currentObject"></param>
        /// <param name="dboContext"></param>
        /// <param name="serviceObject"></param>
        /// <param name="goDefinition"></param>
        /// <returns></returns>
        private static int UpdateGeneralObject(GeneralObject currentObject, DBObjectContext dboContext, ServiceObject serviceObject, GeneralObjectDefinition goDefinition)
        {
            logger.Debug(string.Format("serviceObject.OnlyUpdateProperties={0}", serviceObject.OnlyUpdateProperties));
            
            //if it's not properties update only, we will update the object information as well.
            if (!serviceObject.OnlyUpdateProperties)
            {
                currentObject.GeneralObjectName = serviceObject.ObjectName;
                currentObject.ObjectOwner = serviceObject.ObjectOwner;
                currentObject.GeneralObjectDefinition = goDefinition;
                currentObject.Updated = DateTime.Now;
                currentObject.UpdatedBy = serviceObject.UpdatedBy;
            }

            //load all the object fields of the current object.
            dboContext.Entry(currentObject).Collection(o => o.GeneralObjectFields).Load();

            foreach (ObjectKeyValuePair kvp in serviceObject.Properties)
            {
                var currentFieldDef = (from f in goDefinition.GeneralObjectDefinitionFields
                                       where f.GeneralObjectDefinitionFieldName.Equals(kvp.KeyName, StringComparison.InvariantCultureIgnoreCase)
                                       select f).FirstOrDefault();

                var currObjectField = (from f in currentObject.GeneralObjectFields
                                       where f.GeneralObjectDefinitionFieldID == currentFieldDef.GeneralObjectDefinitionFieldID
                                       select f).FirstOrDefault();

                logger.Debug(string.Format("currObjectField != null ? {0}", currObjectField != null));
                if(currObjectField != null) //update
                {
                    UpdateObjectFieldValue(currObjectField, kvp, currentFieldDef);
                }
                else //insert
                {
                    GeneralObjectField field = ParseObjectField(currentObject, currentFieldDef, kvp);
                    if (field != null)
                    {
                        currentObject.GeneralObjectFields.Add(field);
                    }
                }
            }

            dboContext.SaveChanges();

            return currentObject.GeneralObjectID;
        }


        /// <summary>
        /// Programing interface to update the GeneralObject instance.
        /// </summary>
        /// <param name="serviceObject"></param>
        /// <returns></returns>
        public int CreateORUpdateGeneralObject(ServiceObject serviceObject)
        {
            logger.Debug("Start CreateORUpdateGeneralObject");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    using (var trans = dboContext.Database.BeginTransaction())
                    {
                        try
                        {
                            GeneralObjectDefinition goDefinition = dboContext.GeneralObjectDefinitions.Find(serviceObject.ObjectDefinitionId);
                            dboContext.Entry(goDefinition).Collection(g => g.GeneralObjectDefinitionFields).Load();

                            int objectId = -1;
                            GeneralObject currObject = dboContext.GeneralObjects.Find(serviceObject.ObjectID);
                            if (currObject == null)
                            {
                                objectId = CreateGeneralObject(dboContext, serviceObject, goDefinition);
                            }
                            else
                            {
                                objectId = UpdateGeneralObject(currObject, dboContext, serviceObject, goDefinition);
                            }
                            logger.Debug("objectId : " + objectId);

                            trans.Commit();

                            return objectId;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            trans.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            logger.Debug("End CreateORUpdateGeneralObject");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Create or update a ObjectDefintion 
        /// </summary>
        /// <param name="objectDef"></param>
        /// <returns></returns>
        public int CreateORUpdateObjectDefinition(ObjectDefinition objectDef)
        {
            logger.Debug("Start CreateORUpdateObjectDefinition");
            lock (SyncRoot)
            {
                using (var dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    using (var trans = dboContext.Database.BeginTransaction())
                    {
                        try
                        {
                            GeneralObjectDefinition goDefinition = dboContext.GeneralObjectDefinitions.Find(objectDef.ObjectDefinitionID);
                            if (goDefinition != null) //update
                            {
                                UpdateGeneralObjectDefinition(objectDef, dboContext, goDefinition);
                            }
                            else //insert
                            {
                                goDefinition = CreateGeneralObjectDefinition(objectDef, dboContext);
                            }

                            trans.Commit();

                            return goDefinition.GeneralObjectDefinitionID;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            trans.Rollback();
                            throw ex;
                        }
                        finally
                        {
                            logger.Debug("End CreateORUpdateObjectDefinition");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Update GeneralObjectDefintion
        /// </summary>
        /// <param name="objectDef"></param>
        /// <param name="dboContext"></param>
        /// <param name="goDefinition"></param>
        private static void UpdateGeneralObjectDefinition(ObjectDefinition objectDef, DBObjectContext dboContext, GeneralObjectDefinition goDefinition)
        {
            dboContext.Entry(goDefinition).Collection(g => g.GeneralObjectDefinitionFields).Load();

            logger.Debug(string.Format("objectDef.IsFeildsUpdateOnly = {0}", objectDef.IsFeildsUpdateOnly));
            if (!objectDef.IsFeildsUpdateOnly)
            {
                goDefinition.GeneralObjectDefinitionGroupID = objectDef.ObjectDefinitionGroupID;
                goDefinition.GeneralObjectDefinitionKey = objectDef.ObjectDefinitionKey;
                goDefinition.GeneralObjectDefinitionName = objectDef.ObjectDefinitionName;
                goDefinition.ObjectOwner = objectDef.ObjectOwner;
                goDefinition.Updated = DateTime.Now;
                goDefinition.UpdatedBy = objectDef.UpdatedBy;
            }

            logger.Debug("objectDef.ObjectFields.count = " + objectDef.ObjectFields.Count);
            foreach (var f in objectDef.ObjectFields)
            {
                GeneralObjectDefinitionField field = (from fd in goDefinition.GeneralObjectDefinitionFields
                                                      where fd.GeneralObjectDefinitionFieldID == f.ObjectDefinitionFieldID
                                                      select fd).FirstOrDefault();

                bool needCreated = field == null;

                //for the selection field, we need to load the related select options.
                if(field != null && field is SingleSelectionDefinitionField)
                {
                    var sField = field as SingleSelectionDefinitionField;
                    dboContext.Entry(sField).Collection(sf => sf.SelectionItems).Load();
                }

                field = ObjectConverter.ConvertSvcField2ObjDefinitionField(f, field);
                if (needCreated)
                {
                    goDefinition.GeneralObjectDefinitionFields.Add(field);
                    //if there is new added item, we need to do the save change operation.
                    dboContext.SaveChanges();
                }
            }
            dboContext.SaveChanges();
        }

      

        /// <summary>
        /// Add new GeneralObjectDefinition to database
        /// </summary>
        /// <param name="objectDef"></param>
        /// <param name="dboContext"></param>
        /// <returns></returns>
        private static GeneralObjectDefinition CreateGeneralObjectDefinition(ObjectDefinition objectDef, DBObjectContext dboContext)
        {
            GeneralObjectDefinition goDefinition = new GeneralObjectDefinition();
            goDefinition.GeneralObjectDefinitionGroupID = objectDef.ObjectDefinitionGroupID;
            goDefinition.GeneralObjectDefinitionKey = objectDef.ObjectDefinitionKey;
            goDefinition.GeneralObjectDefinitionName = objectDef.ObjectDefinitionName;
            goDefinition.ObjectOwner = objectDef.ObjectOwner;
            goDefinition.Updated = DateTime.Now;
            goDefinition.Created = DateTime.Now;
            goDefinition.UpdatedBy = objectDef.UpdatedBy;

            logger.Debug(string.Format("goDefinition.GeneralObjectDefinitionGroupID = {0}", objectDef.ObjectDefinitionGroupID));
            logger.Debug(string.Format("goDefinition.GeneralObjectDefinitionKey = {0}", objectDef.ObjectDefinitionKey));
            logger.Debug(string.Format("goDefinition.GeneralObjectDefinitionName = {0}", objectDef.ObjectDefinitionName));
            logger.Debug(string.Format("goDefinition.ObjectOwner = {0}", objectDef.ObjectOwner));

            foreach (var f in objectDef.ObjectFields)
            {
                GeneralObjectDefinitionField field =  ObjectConverter.ConvertSvcField2ObjDefinitionField(f, null);
                goDefinition.GeneralObjectDefinitionFields.Add(field);
            }

            dboContext.GeneralObjectDefinitions.Add(goDefinition);
            dboContext.SaveChanges();

            return goDefinition;
        }

        /// <summary>
        /// Get all object Definitions.
        /// </summary>
        /// <returns></returns>
        public List<ObjectDefinition> GetAllObjectDefinitions()
        {
            logger.Debug("Start GetAllObjectDefinitions()");
            List<ObjectDefinition> retObjList = null;
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefList = dboContext.GeneralObjectDefinitions
                    .Where(g => g.IsDeleted == false)
                    .Include(d => d.GeneralObjectDefinitionFields)
                    .ToList();

                LoadSelctionItems(dboContext, objDefList);

                retObjList = ObjectConverter.ConvertToObjectDefinitionList(objDefList);
            }

            logger.Debug("End GetAllObjectDefinitions()");
            return retObjList;
        }

        public List<ObjectDefinition> GetObjectDefinitionsByGroup(int GroupId, int currentPage, int pageSize)
        {
            logger.Debug("Start GetObjectDefinitionsByGroup");
            List<ObjectDefinition> retObjList = null;

            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                IQueryable<GeneralObjectDefinition> query = dboContext.GeneralObjectDefinitions
                    .Where(g => g.IsDeleted == false && g.GeneralObjectDefinitionGroupID == GroupId)
                    .Include(d => d.GeneralObjectDefinitionFields);

                retObjList = ExecuteObjectDefinitionQuery(dboContext, GroupId, query);
            }

            logger.Debug("End GetObjectDefinitionsByGroup");
            return retObjList;
        }

        private static List<ObjectDefinition> ExecuteObjectDefinitionQuery(DBObjectContext dboContext, int GroupId, IQueryable<GeneralObjectDefinition> query)
        {
            logger.Debug("Start ExecuteObjectDefinitionQuery");
            List<ObjectDefinition> retObjList = null;

            var objDefList = query.ToList();

            LoadSelctionItems(dboContext, objDefList);

            retObjList = ObjectConverter.ConvertToObjectDefinitionList(objDefList);

            logger.Debug("End ExecuteObjectDefinitionQuery");
            return retObjList;
        }

        public List<ObjectDefinition> GetObjectDefinitionsExceptGroup(int GroupId)
        {
            logger.Debug("Start GetObjectDefinitionsExceptGroup");
            List<ObjectDefinition> retObjList = null;
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var query = dboContext.GeneralObjectDefinitions
                    .Where(g => g.IsDeleted == false && g.GeneralObjectDefinitionGroupID != GroupId)
                    .Include(d => d.GeneralObjectDefinitionFields);

                retObjList = ExecuteObjectDefinitionQuery(dboContext, GroupId, query);
            }

            logger.Debug("End GetObjectDefinitionsExceptGroup");
            return retObjList;
        }

        private static void LoadSelctionItems(DBObjectContext dboContext, List<GeneralObjectDefinition> objDefList)
        {
            logger.Debug("objDefList.count : " + objDefList.Count);
            foreach (var objDef in objDefList)
            {
                foreach (var field in objDef.GeneralObjectDefinitionFields)
                {
                    if (field is SingleSelectionDefinitionField)
                    {
                        dboContext.Entry(field).Collection("SelectionItems").Load();
                    }
                }
            }
        }

        public List<ServiceObject> GetAllSerivceObjects(int ObjDefId, string[] properties)
        {
            logger.Debug("Start GetAllSerivceObjects");
            List<ServiceObject> svsObjects = null;
            using (var dbContext = EntityContextFactory.GetDBObjectContext())
            {
                var objectList = dbContext.GeneralObjects.Where(o => o.GeneralObjectDefinitionID == ObjDefId && o.IsDeleted == false)
                                                            .Include(o => o.GeneralObjectFields.Select(f=>f.GeneralObjectDefinitionField))
                                                            .ToList();

                logger.Debug("objectList.count : " + objectList.Count);
                svsObjects = ObjectConverter.ConvertToServiceObjectList(objectList, properties);
            }
            logger.Debug("End GetAllSerivceObjects");
            return svsObjects;
        }

        public int GetGeneralObjectCount(int ObjDefId)
        {
            logger.Debug("Start GetGeneralObjectCount");
            using (var dbContext = EntityContextFactory.GetDBObjectContext())
            {
                var objectCount = dbContext.GeneralObjects
                                           .Where(o => o.GeneralObjectDefinitionID == ObjDefId && o.IsDeleted == false)
                                           .Count();

                logger.Debug("objectCount = " + objectCount);
                logger.Debug("End GetGeneralObjectCount");
                return objectCount;
            }
        }

        public ObjectDefinition GetObjectDefinitionById(int objDefId)
        {
            logger.Debug("Start GetObjectDefinitionById");
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefList = dboContext.GeneralObjectDefinitions
                                 .Where(d => d.GeneralObjectDefinitionID == objDefId)
                                 .Include(d => d.GeneralObjectDefinitionFields)
                                 .ToList();
                if (objDefList.Count > 0)
                {
                    LoadSelctionItems(dboContext, objDefList);
                }

                logger.Debug("objDefList.Count : " + objDefList.Count);

                logger.Debug("End GetObjectDefinitionById");

                return objDefList.Count > 0 ?
                    ObjectConverter.ConvertToObjectDefinitionList(objDefList).First() : null;
            }
        }

        public int GetObjectDefinitionCount()
        {
            logger.Debug("Start GetObjectDefinitionCount");
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefCount = dboContext.GeneralObjectDefinitions
                                 .Where(g=>g.IsDeleted == false)
                                 .Count();
                logger.Debug("objDefCount = " + objDefCount);
                logger.Debug("End GetObjectDefinitionCount");
                return objDefCount;
            }
        }

        public List<ObjectDefinition> GetObjectDefinitions(int currentPage, int pageSize)
        {
            logger.Debug("Start GetObjectDefinitions");
            List<ObjectDefinition> retObjList = null;
            int skipCount = currentPage > 1 ? (currentPage - 1) * pageSize : 0;
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefList = dboContext.GeneralObjectDefinitions
                                            .Where(g=>g.IsDeleted == false)
                                            .Include(d => d.GeneralObjectDefinitionFields)
                                            .OrderBy(o=>o.GeneralObjectDefinitionName)
                                            .Skip(skipCount)
                                            .Take(pageSize)
                                            .ToList();

                logger.Debug("objDefList.count = " + objDefList.Count);

                LoadSelctionItems(dboContext, objDefList);

                retObjList = ObjectConverter.ConvertToObjectDefinitionList(objDefList);
            }

            logger.Debug("End GetObjectDefinitions");
            return retObjList;
        }

        public ServiceObject GetServiceObjectById(int objectId, string[] properties)
        {
            logger.Debug("Start GetServiceObjectById");
            List<ServiceObject> svsObjects = null;
            using (var dbContext = EntityContextFactory.GetDBObjectContext())
            {
                var objectList = dbContext.GeneralObjects
                                            .Where(o => o.GeneralObjectID == objectId)
                                            .Include(o => o.GeneralObjectFields)
                                            .Include(o => o.GeneralObjectFields.Select(f => f.GeneralObjectDefinitionField))
                                            .ToList();

                svsObjects = ObjectConverter.ConvertToServiceObjectList(objectList, properties);
            }

            logger.Debug(string.Format("svsObjects!= null && svsObjects.Count ? {0}", svsObjects != null 
                && svsObjects.Count > 0));
            logger.Debug("End GetServiceObjectById");
            return svsObjects.Count > 0 ? svsObjects.First() : null;
        }

        public List<ServiceObject> GetServiceObjects(int ObjDefId, string[] properties, int currentPage, int pageSize)
        {
            logger.Debug("Start GetServiceObjects");
            List<ServiceObject> svsObjects = null;
            int skipCount = currentPage > 1 ? (currentPage - 1) * pageSize : 0;

            using (var dbContext = EntityContextFactory.GetDBObjectContext())
            {
                var objectList = dbContext.GeneralObjects
                                            .Where(o => o.GeneralObjectDefinitionID == ObjDefId && o.IsDeleted == false)
                                            .Include(o => o.GeneralObjectFields.Select(f => f.GeneralObjectDefinitionField))
                                            .OrderByDescending(o=>o.Created)
                                            .Skip(skipCount)
                                            .Take(pageSize)
                                            .ToList();

                svsObjects = ObjectConverter.ConvertToServiceObjectList(objectList, properties);
            }

            logger.Debug(string.Format("svsObjects != null ? {0}", svsObjects != null));
            logger.Debug(string.Format("svsObjects.count > 0 ? {0}", svsObjects.Count > 0));
            logger.Debug("End GetServiceObjects");

            return svsObjects;
        }

        public int SoftDeleteObjectDefinition(int objDefId, string updatedBy)
        {
            logger.Debug("Start SoftDeleteObjectDefinition");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var objDef = dboContext.GeneralObjectDefinitions
                                     .Where(d => d.GeneralObjectDefinitionID == objDefId)
                                     .FirstOrDefault();


                    logger.Debug(string.Format("objDef != null ? {0}", objDef != null));

                    if (objDef != null)
                    {
                        objDef.IsDeleted = true;
                        objDef.UpdatedBy = updatedBy;
                        dboContext.SaveChanges();
                    }

                    logger.Debug("End SoftDeleteObjectDefinition");
                    return objDef != null ? objDef.GeneralObjectDefinitionID : -1;
                }
            }
        }

        public int SoftDeleteServiceObject(int objectId, string updatedBy)
        {
            logger.Debug("Start SoftDeleteServiceObject");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var obj = dboContext.GeneralObjects
                                            .Where(o => o.GeneralObjectID == objectId)
                                            .FirstOrDefault();

                    logger.Debug(string.Format("obj != null ? {0}", obj != null));
                    if (obj != null)
                    {
                        obj.IsDeleted = true;
                        obj.Updated = DateTime.Now;
                        obj.UpdatedBy = updatedBy;

                        dboContext.SaveChanges();
                    }

                    logger.Debug("End SoftDeleteServiceObject");
                    return obj != null ? obj.GeneralObjectID : -1;
                }
            }
        }

        public List<ObjectDefinitionGroup> GetObjectDefinitionGroups(int? parentGroupId)
        {
            logger.Debug("Start GetObjectDefinitionGroups");
            List<ObjectDefinitionGroup> objList = null;
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefGroupList = dboContext.GeneralObjectDefinitionGroups
                         .Where(g=>g.IsDeleted == false)
                        .Include(g => g.ParentGroup)
                        .Include(g => g.ChildrenGroups)
                        .ToList()
                        .FindAll(s=> parentGroupId.HasValue ? s.ParentGroup != null && s.ParentGroup.GeneralObjectDefinitionGroupID == parentGroupId.Value : s.ParentGroup == null);

                objList = ObjectConverter.Convert2ObjectDefinitionGroupList(objDefGroupList);
                logger.Debug("objList.count = " + objList.Count);
            }

            logger.Debug("End GetObjectDefinitionGroups");
            return objList;
        }

      

        public ObjectDefinitionGroup GetObjectDefinitionGroupById(int defId)
        {
            logger.Debug("Start GetObjectDefinitionGroupById");
            List<ObjectDefinitionGroup> objList = null;

            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefGroupList = dboContext.GeneralObjectDefinitionGroups
                         .Where(g => g.GeneralObjectDefinitionGroupID == defId)
                        .Include(g => g.ParentGroup)
                        .Include(g => g.ChildrenGroups)
                        .ToList();
                objList = ObjectConverter.Convert2ObjectDefinitionGroupList(objDefGroupList);

                logger.Debug(string.Format("objList.Count > 0 ? {0}", objList.Count > 0));
                logger.Debug("End GetObjectDefinitionGroupById");
                return objList.Count > 0 ? objList.First() : null;
            }
        }

        public int CreateOrUpdateObjectDefinitionGroup(ObjectDefinitionGroup objGroup)
        {
            logger.Debug("Start CreateOrUpdateObjectDefinitionGroup");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var objDefGroup = dboContext.GeneralObjectDefinitionGroups
                             .Where(g => g.GeneralObjectDefinitionGroupID == objGroup.GroupID)
                            .Include(g => g.ParentGroup)
                            .Include(g => g.ChildrenGroups)
                            .FirstOrDefault();

                    logger.Debug(string.Format("objDefGroup == null ? {0}", objDefGroup == null));
                    if(objDefGroup == null)
                    {
                       //insert
                       objDefGroup = new GeneralObjectDefinitionGroup();
                       dboContext.GeneralObjectDefinitionGroups.Add(objDefGroup);
                    }

                    UpdateGeneralOjectRefGroupEntity(dboContext, objGroup, objDefGroup);
                    dboContext.SaveChanges();

                    logger.Debug("End CreateOrUpdateObjectDefinitionGroup");
                    return objDefGroup.GeneralObjectDefinitionGroupID;
                }
            }
        }

        private static void UpdateGeneralOjectRefGroupEntity(DBObjectContext dboContext, ObjectDefinitionGroup objGroup, GeneralObjectDefinitionGroup objDefGroup)
        {
            objDefGroup.GeneralObjectDefinitionGroupKey = objGroup.GroupKey;
            objDefGroup.GeneralObjectDefinitionGroupName = objGroup.GroupName;

            if (objGroup.ParentGroup != null)
            {
                objDefGroup.ParentGroup = dboContext.GeneralObjectDefinitionGroups.Find(objGroup.ParentGroup.GroupID);
            }
        }

        /// <summary>
        /// soft delete a object defintion.
        /// </summary>
        /// <param name="defId"></param>
        /// <returns></returns>
        public int SoftDeleteObjectDefintionGroup(int defId)
        {
            logger.Debug("Start SoftDeleteObjectDefintionGroup");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var objDefGroup = dboContext.GeneralObjectDefinitionGroups
                             .Where(g => g.GeneralObjectDefinitionGroupID == defId)
                            .Include(g => g.ParentGroup)
                            .Include(g => g.ChildrenGroups)
                            .FirstOrDefault();

                    logger.Debug(string.Format("objDefGroup != null ? {0}", objDefGroup != null));
                    if (objDefGroup != null)
                    {
                        objDefGroup.IsDeleted = true;
                        dboContext.SaveChanges();  
                    }

                    logger.Debug("End SoftDeleteObjectDefintionGroup");
                    return objDefGroup != null ? objDefGroup.GeneralObjectDefinitionGroupID : -1;
                }
            }
        }

        public void DeleteServiceObject(int serviceObjectId)
        {
            logger.Debug("Start DeleteServiceObject");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var generalObject = dboContext.GeneralObjects
                                                .Where(o => o.GeneralObjectID == serviceObjectId)
                                                .Include(f => f.GeneralObjectFields)
                                                .FirstOrDefault();
                    logger.Debug(string.Format("generalObject != null ? {0}", generalObject != null));
                    if(generalObject != null)
                    {
                        generalObject.GeneralObjectFields.ToList().ForEach(f=>dboContext.GeneralObjectFields.Remove(f));
                        dboContext.GeneralObjects.Remove(generalObject);
                    }

                    dboContext.SaveChanges();
                }
            }
            logger.Debug("End DeleteServiceObject");
        }

        public void DeleteObjectField(int objectFieldId)
        {
            logger.Debug("Start DeleteObjectField");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {

                    var field = dboContext.GeneralObjectFields.Find(objectFieldId);
                    if (field != null)
                    {
                        dboContext.GeneralObjectFields.Remove(field);
                    }

                    logger.Debug(string.Format("field != null ? {0}", field != null));
                    dboContext.SaveChanges();
                }
            }
            logger.Debug("End DeleteObjectField");
        }

        public void DeleteObjectDefinitionField(int fieldDefinitionId)
        {
            logger.Debug("Start DeleteObjectDefinitionField");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var objFields = dboContext.GeneralObjectFields
                                             .Include(f => f.GeneralObjectDefinitionField)
                                             .Where(f => f.GeneralObjectDefinitionFieldID == fieldDefinitionId)
                                             .ToList();

                    logger.Debug("objFields.count : " + objFields.Count);
                    //remove all the refered object fields firstly.
                    foreach(var field in objFields)
                    {
                        dboContext.GeneralObjectFields.Remove(field);
                    }

                    //find and remove the field definition.
                    var definitionField = dboContext.GeneralObjectDefinitionFields.Find(fieldDefinitionId);

                    logger.Debug(string.Format("definitionField != null ? {0}", definitionField != null));
                    if (definitionField != null)
                    {
                        //special deal with the selection items.
                        DeleteSingleSelectionItems(definitionField, dboContext);
                        dboContext.GeneralObjectDefinitionFields.Remove(definitionField);
                    }

                    dboContext.SaveChanges();
                }
            }
            logger.Debug("End DeleteObjectDefinitionField");
        }

        public void DeleteObjectDefinition(int objectDefinitionId)
        {
            logger.Debug("Start DeleteObjectDefinition");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var objectDefinition = dboContext.GeneralObjectDefinitions
                                                     .Include(f => f.GeneralObjectDefinitionFields)
                                                     .Include(o => o.GeneralObjects.Select(f=>f.GeneralObjectFields))
                                                     .Where(d => d.GeneralObjectDefinitionID == objectDefinitionId)
                                                     .FirstOrDefault();

                    logger.Debug(string.Format("objectDefinition != null ? {0}", objectDefinition != null));
                    if(objectDefinition != null)
                    {
                        //delete the objects bind to this definiton.
                        objectDefinition.GeneralObjects.ToList().ForEach(o =>
                        {
                            //delete the fields of the objects.
                            o.GeneralObjectFields.ToList().ForEach(f =>
                            {
                                dboContext.GeneralObjectFields.Remove(f);
                            });

                            dboContext.GeneralObjects.Remove(o);
                        });


                        objectDefinition.GeneralObjectDefinitionFields.ToList().ForEach(f =>
                        {
                            DeleteSingleSelectionItems(f, dboContext);

                            dboContext.GeneralObjectDefinitionFields.Remove(f);
                        });

                        //delete the object definition finally.
                        dboContext.GeneralObjectDefinitions.Remove(objectDefinition);
                    }

                    dboContext.SaveChanges();
                }
            }
            logger.Debug("End DeleteObjectDefinition");
        }

        private static void DeleteSingleSelectionItems(GeneralObjectDefinitionField f, DBObjectContext dboContext)
        {
            //deal with the dependency to selection items.
            if (f is SingleSelectionDefinitionField)
            {
                var sField = f as SingleSelectionDefinitionField;
                dboContext.Entry(sField).Collection(c => c.SelectionItems).Load();

                sField.SelectionItems.ToList().ForEach(l =>
                {
                    dboContext.GeneralObjectDefinitionSelectItems.Remove(l);
                });
            }
        }

        public void DeleteSingleSelectionFieldSelectionItem(int selectionItemId)
        {
            logger.Debug("Start DeleteSingleSelectionFieldSelectionItem");
            lock (SyncRoot)
            {
                using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
                {
                    var selectionItem =  dboContext.GeneralObjectDefinitionSelectItems.Find(selectionItemId);

                    logger.Debug(string.Format("selectionItem != null ? {0}", selectionItem != null));
                    if (selectionItem != null)
                    {
                        dboContext.GeneralObjectDefinitionSelectItems.Remove(selectionItem);
                    }

                    dboContext.SaveChanges();
                }
            }
            logger.Debug("End DeleteSingleSelectionFieldSelectionItem");
        }

        public bool IsObjectDefinitionGroupExists(string groupName)
        {
            logger.Debug("Start IsObjectDefinitionGroupExists");
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var groupCount = dboContext.GeneralObjectDefinitionGroups
                         .Where(g => g.IsDeleted == false && g.GeneralObjectDefinitionGroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase))
                         .Count();

                logger.Debug("groupCount = " + groupCount);
                logger.Debug("End IsObjectDefinitionGroupExists");
                return groupCount > 0;
            }
        }

        public ObjectDefinitionGroup GetObjectDefinitionGroupByName(string groupName)
        {
            logger.Debug("Start GetObjectDefinitionGroupByName");
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefGroupList = dboContext.GeneralObjectDefinitionGroups
                         .Where(g => g.IsDeleted == false && g.GeneralObjectDefinitionGroupName.Equals(groupName, StringComparison.InvariantCultureIgnoreCase))
                         .Include(g => g.ParentGroup)
                         .Include(g => g.ChildrenGroups)
                         .ToList();

                var objList = ObjectConverter.Convert2ObjectDefinitionGroupList(objDefGroupList);

                logger.Debug("objList.count = " + objList.Count);
                logger.Debug("End GetObjectDefinitionGroupByName");

                return objList.Count > 0 ? objList.First() : null;
            }
        }

        public ObjectDefinition GetObjectDefinitionByName(string definitionName)
        {
            logger.Debug("Start GetObjectDefinitionByName");
            using (DBObjectContext dboContext = EntityContextFactory.GetDBObjectContext())
            {
                var objDefList = dboContext.GeneralObjectDefinitions
                                 .Where(d => d.GeneralObjectDefinitionName.Equals(definitionName, StringComparison.InvariantCultureIgnoreCase))
                                 .Include(d => d.GeneralObjectDefinitionFields)
                                 .ToList();
                logger.Debug("objDefList.Count = " + objDefList.Count);
                if (objDefList.Count > 0)
                {
                    LoadSelctionItems(dboContext, objDefList);
                }

                logger.Debug("End GetObjectDefinitionByName");
                return objDefList.Count > 0 ?
                    ObjectConverter.ConvertToObjectDefinitionList(objDefList).First() : null;
            }
        }
    }
}
