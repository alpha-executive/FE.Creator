;(function () {
  angular
        .module('ngObjectRepository')
        .factory("objectUtilService", objectUtilService);

  objectUtilService.$inject = ["$log", "ObjectRepositoryDataService"];

  function objectUtilService($log, ObjectRepositoryDataService) {
      return {
          getStringPropertyValue: getSimplePropertyValue,
          getIntegerPropertyValue: getSimplePropertyValue,
          getLongPropertyValue : getSimplePropertyValue, 
          getDateTimePropertyValue : getSimplePropertyValue,
          getNumberPropertyValue: getSimplePropertyValue,
          getBinaryPropertyValue: getSimplePropertyValue,
          getFilePropertyValue: getFilePropertyValue,
          getObjectRefPropertyValue: getObjectRefPropertyValue,
          getSingleSelectionPropertyValue: getSingleSelectionPropertyValue,
          parseServiceObject: parseServiceObject,
          convertAsServiceObject: convertAsServiceObject,
          addStringProperty: addStringProperty,
          addIntegerProperty: addIntegerProperty,
          addLongProperty: addLongProperty,
          addDateTimeProperty: addDateTimeProperty,
          addNumberProperty: addNumberProperty,
          addBinaryProperty: addBinaryProperty,
          addFileProperty: addFileProperty,
          addObjectRefProperty: addObjectRefProperty,
          addSingleSPropery: addSingleSPropery
      }

      function getSimplePropertyValue(sourceObj, propName) {
          for (var i = 0; i < sourceObj.properties.length; i++) {
              var prop = sourceObj.properties[i];
              if (prop.keyName == propName) {
                  return prop.value.value;
              }
          }

          return null;
      }

      function getFilePropertyValue(sourceObj, propName) {
          for (var i = 0; i < sourceObj.properties.length; i++) {
              var prop = sourceObj.properties[i];
              if (prop.keyName == propName) {
                  return prop.value;
              }
          }

          return {};
      }

      function getSingleSelectionPropertyValue(sourceObj, propName) {
          for (var i = 0; i < sourceObj.properties.length; i++) {
              var prop = sourceObj.properties[i];
              if (prop.keyName == propName) {
                  return prop;
              }
          }

          return null;
      }

      function getObjectRefPropertyValue(sourceObj, propName) {
          for (var i = 0; i < sourceObj.properties.length; i++) {
              var prop = sourceObj.properties[i];
              if (prop.keyName == propName) {
                  return ObjectRepositoryDataService.getServiceObject(prop.value.referedGeneralObjectID)
                   .then(function (data) {
                       return data;
                   });
              }
          }
    
          return {};
      }

      function createPrimaryProperty(primaryTypeIndex,propName, propValue) {
          //String,
          //Integer,
          //Long,
          //Datetime,
          //Number,
          //Binary

          var property = {
              keyName: propName,
              $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectKeyValuePair, FE.Creator.ObjectRepository",
              value: {
                  $type: "FE.Creator.ObjectRepository.ServiceModels.PrimeObjectField, FE.Creator.ObjectRepository",
                  primeDataType: primaryTypeIndex,
                  value: propValue
              }
          };

          return property;
      }

      function addStringProperty(sourceObj, propName, propValue) {
          if(sourceObj.properties == null)
          {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(0, propName, propValue);
          sourceObj.properties.push(property);
      }

      function addIntegerProperty(sourceObj, propName, propValue){
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(1, propName, propValue);
          sourceObj.properties.push(property);
      }

      function addLongProperty(sourceObj, propName, propValue) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(2, propName, propValue);
          sourceObj.properties.push(property);
      }

      function addDateTimeProperty(sourceObj, propName, propValue) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(3, propName, propValue);
          sourceObj.properties.push(property);
      }

      function addNumberProperty(sourceObj, propName, propValue){
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(4, propName, propValue);
          sourceObj.properties.push(property);
      }

      function addBinaryProperty(sourceObj, propName, propValue) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = createPrimaryProperty(5, propName, propValue);
          sourceObj.properties.push(property);
      }
      function assignProperties(sourceObj, targetObj) {
          if (sourceObj == null || targetObj == null)
              return;

          for (var pname in sourceObj) {
              targetObj[pname] = sourceObj[pname];
          }
      }
      function addFileProperty(sourceObj, propName, propValue) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = {
              keyName: propName,
              $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectKeyValuePair, FE.Creator.ObjectRepository",
              value: {
                  $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectFileField, FE.Creator.ObjectRepository"
              }
          };
         
          assignProperties(propValue, property.value);
          sourceObj.properties.push(property);
      }

      function addObjectRefProperty(sourceObj, propName, propValue) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = {
              keyName: propName,
              $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectKeyValuePair, FE.Creator.ObjectRepository",
              value: {
                  $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectReferenceField, FE.Creator.ObjectRepository"
              }
          };

          assignProperties(propValue, property.value);
          sourceObj.properties.push(property);
      }

      function addSingleSPropery(sourceObj, propName, propValue, selectionItems) {
          if (sourceObj.properties == null) {
              sourceObj.properties = [];
          }

          var property = {
              keyName: propName,
              $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectKeyValuePair, FE.Creator.ObjectRepository",
              value: {
                  $type: "FE.Creator.ObjectRepository.ServiceModels.SingleSelectionField, FE.Creator.ObjectRepository",
                  selectionItems: selectionItems
              }
          };

          assignProperties(propValue, property.value);
          sourceObj.properties.push(property);
      }

      function parseServiceObject(sourceObj) {

          if (sourceObj == null)
              return null;

          var retObj = {
              objectID : sourceObj.objectID,
              objectName : sourceObj.objectName,
              objectDefinitionId : sourceObj.objectDefinitionId,
              created: sourceObj.created,
              createdBy: sourceObj.createdBy,
              updated: sourceObj.updated,
              updatedBy: sourceObj.updatedBy,
              onlyUpdateProperties: sourceObj.onlyUpdateProperties,
              objectOwner: sourceObj.objectOwner,
              properties: []
          };

          for (var i = 0; i < sourceObj.properties.length; i++) {
              var prop = sourceObj.properties[i];
              retObj.properties[prop.keyName] = prop.value;
          }

          return retObj;
      }

      function convertAsServiceObject(sourceObj) {
          if (sourceObj == null)
              return null;

          var retObj = {
              objectID: sourceObj.objectID,
              objectName: sourceObj.objectName,
              objectDefinitionId: sourceObj.objectDefinitionId,
              created: sourceObj.created,
              createdBy: sourceObj.createdBy,
              updated: sourceObj.updated,
              updatedBy: sourceObj.updatedBy,
              onlyUpdateProperties: sourceObj.onlyUpdateProperties,
              objectOwner: sourceObj.objectOwner,
              properties: []
          };

          for (var prop in sourceObj.properties) {
              var svProperty = {
                  keyName: prop,
                  $type: "FE.Creator.ObjectRepository.ServiceModels.ObjectKeyValuePair, FE.Creator.ObjectRepository",
                  value: sourceObj.properties[prop]
              };

              retObj.properties.push(svProperty);
          }

          return retObj;
      }
  }
})();