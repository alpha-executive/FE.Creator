/**
    data service used by the application.
**/
(function ()
{
    "use strict";

    angular
        .module('ngObjectRepository')
        .factory("ObjectRepositoryDataService", repositoryDataService);

    repositoryDataService.$inject = ["$http", "$log"];

    function repositoryDataService($http, logger) {

        return {
            getObjectDefinitionGroups: getObjectDefinitionGroups,
            getObjectDefinitionGroup: getObjectDefinitionGroup,
            createOrUpdateDefinitionGroup: createOrUpdateDefinitionGroup,
            deleteDefinitionGroup: deleteDefinitionGroup,
            getObjectDefintionsbyGroup: getObjectDefintionsbyGroup,
            getObjectDefinitionById: getObjectDefinitionById,
            getLightWeightObjectDefinitions: getLightWeightObjectDefinitions,
            getCustomObjectDefinitions: getCustomObjectDefinitions,
            createOrUpdateObjectDefintion: createOrUpdateObjectDefintion,
            deleteObjectDefintionField: deleteObjectDefintionField,
            deleteObjectDefintion: deleteObjectDefintion,
            deleteSingleSelectionFieldItem: deleteSingleSelectionFieldItem,
            getServiceObjects: getServiceObjects,
            getServiceObject: getServiceObject,
            getServiceObjectCount: getServiceObjectCount,
            getServiceObjectsWithFilters: getServiceObjectsWithFilters,
            createOrUpdateServiceObject: createOrUpdateServiceObject,
            deleteServiceObject: deleteServiceObject,
            getUsers: getUsers,
            getUserIdByLoginName : getUserIdByLoginName,
            resetPassword: resetPassword,
            getLicencedModules: getLicencedModules,
            getUUID: getUUID,
            encryptData: encryptData,
            decryptData: decryptData
        };

        //get the object defintion groups
        function getObjectDefinitionGroups(parentGroupId) {

            var url = "/api/custom/ObjectDefinitionGroup/GetByParentId";

            if (parentGroupId != null)
                url = url + "/" + parentGroupId;

            return $http.get(url)
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response)
            {
                logger.error('XHR Failed for getObjectDefinitionGroups - ' + response.data);
                return response.data;
            }
        }

        //get the object defintion group by id.
        function getObjectDefinitionGroup(id){
           return $http.get("/api/ObjectDefinitionGroup/"+ id)
               .then(complete)
               .catch(error);


           function complete(response) {
               return response.data;
           }

           function error(response) {
               logger.error('XHR Failed for getObjectDefinitionGroup - ' + response.data);
               return response.data;
           }
        }

        //create or update
        function createOrUpdateDefinitionGroup(id, grpdata) {
            //create
            if (id == null) {
                return $http.post("/api/ObjectDefinitionGroup", grpdata)
                            .then(complete)
                            .catch(error);
            }//update
            else {
                return $http.put("/api/ObjectDefinitionGroup/" + id, grpdata)
                            .then(complete)
                            .catch(error);
            }

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for createOrUpdateDefinitionGroup - ' + response.data);
                return response.data;
            }
        }

        //delete
        function deleteDefinitionGroup(id)
        {
            return $http.delete("/api/ObjectDefinitionGroup/" + id)
                            .then(complete)
                            .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for deleteDefinitionGroup - ' + response.data);
                return response.data;
            }
        }

        //===============================================Object Defintion API ================
        function getObjectDefintionsbyGroup(groupId) {
            var url = "/api/custom/ObjectDefinition/FindObjectDefintionsByGroup";

            if (groupId != null)
                url = url + "/" + groupId;

            return $http.get(url)
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getObjectDefintionsbyGroup - ' + response.data);
                return response.data;
            }
        }

        function getObjectDefinitionById(objectId) {
            return $http.get('/api/custom/ObjectDefinition/FindObjectDefinition/' + objectId)
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getObjectDefintionsbyGroup - ' + response.data);
                return response.data;
            }
        }

        function getLightWeightObjectDefinitions() {
            return $http.get('/api/custom/ObjectDefinition/getSystemObjectDefinitions')
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getLightWeightObjectDefinitions - ' + response.data);
                return response.data;
            }
        }

        function getCustomObjectDefinitions() {
            return $http.get('/api/custom/ObjectDefinition/getCustomObjectDefinitions')
            .then(complete)
            .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getCustomObjectDefinitions - ' + response.data);
                return response.data;
            }
        }

        function createOrUpdateObjectDefintion(id, objdefdata) {
            //create
            if (id == null) {
                return $http.post("/api/ObjectDefinition", 
                       objdefdata)
                    .then(complete)
                    .catch(error);
            }//update
            else {
                return $http.put("/api/ObjectDefinition/" + id, objdefdata)
                            .then(complete)
                            .catch(error);
            }

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for createOrUpdateObjectDefintion - ' + response.data);
                return response.data;
            }
        }

        function deleteObjectDefintionField(id) {
            return $http.delete("/api/ObjectDefinitionField/" + id)
                           .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteObjectDefintionField - ' + response.data);
                return response.data;
            }
        }

        function deleteObjectDefintion(id) {
            return $http.delete("/api/ObjectDefinition/" + id)
                          .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteObjectDefintion - ' + response.data);
                return response.data;
            }
        }

        function deleteSingleSelectionFieldItem(id) {
            return $http.delete("/api/SingleSelectionFieldItem/" + id)
                         .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteSingleSelectionFieldItem - ' + response.data);

                return response.data;
            }
        }

        /*==========================Service Objects ===============================*/
        function getServiceObjects(objectDefintionId, properties, pageIndex, pageSize) {
            if (pageIndex == null)
            {
                pageIndex = 1;
            }
            if (pageSize == null) {
                pageSize = Number.MAX_SAFE_INTEGER;
            }

            var config = {
                method: 'GET',
                url: '/api/custom/GeneralObject/FindServiceObjects/' + objectDefintionId + "/" + properties + "?pageIndex="+pageIndex + "&pageSize=" + pageSize,
            };

            return $http(config)
              .then(complete)
              .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getServiceObjects - ' + response.data);
                return response.data;
            }
        }

        function getServiceObject(id, properties) {
            var url = properties != null ? '/api/custom/GeneralObject/FindServiceObject/' + id + "/" + properties
                : '/api/custom/GeneralObject/FindServiceObject/' + id;
            var config = {
                method: 'GET',
                url: url
            };

            return $http(config)
              .then(complete)
              .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getServiceObject - ' + response.data);
                return response.data;
            }
        }

        function getServiceObjectCount(id, filter) {
            var reqUrl = filter != null ? '/api/custom/GeneralObject/CountObjects/' + id + "?filters=" + filter
                : '/api/custom/GeneralObject/CountObjects/' + id;

            var config = {
                method: 'GET',
                url: reqUrl
            };

            return $http(config)
              .then(complete)
              .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getServiceObjectCount - ' + response.data);
                return response.data;
            }
        }

        function getServiceObjectsWithFilters(definitionName, parameters, pageIndex, pageSize, filters) {
            var url = '/api/objects/FindServiceObjectsByFilter/'+ definitionName;
            if (parameters != null)
                url = url + '/' + parameters + "?";
            else
                url = url + "?";

            if(pageIndex != null)
                url = url + "&pageIndex=" + pageIndex;

            if(pageSize != null)
                url = url + "&pageSize=" + pageSize;

            if(filters != null)
                url = url + "&filters=" + filters;
            
            var config = {
                method: 'GET',
                url: url
            };

            return $http(config)
              .then(complete)
              .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getServiceObjectsWithFilters - ' + response.data);
                return response.data;
            }
        }

        function createOrUpdateServiceObject(id, svcObject) {
            //create
            if (id == null) {
                return $http.post("/api/GeneralObject",
                       svcObject)
                    .then(complete)
                    .catch(error);
            }//update
            else {
                return $http.put("/api/GeneralObject/" + id, svcObject)
                            .then(complete)
                            .catch(error);
            }

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for createOrUpdateServiceObject - ' + response.data);
                return response.data;
            }
        }

        function deleteServiceObject(objectid) {
            return $http.delete("/api/GeneralObject/" + objectid)
                         .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteServiceObject - ' + response.data);

                return response.data;
            }
        }

        /*================User Service ==========================*/
        function getUsers() {
            return $http.get('/api/custom/SystemUser/List')
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getUsers - ' + response.data);
                return response.data;
            }
        }

        function getUserIdByLoginName() {
            return $http.get('/api/custom/SystemUser/GetUserIdByUserLoginName')
               .then(complete)
               .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getUserIdByLoginName - ' + response.data);
                return response.data;
            }
        }
        function resetPassword(userid) {
            return $http.post("/api/custom/SystemUser/ResetUserPassword/" + userid, userid)
                    .then(complete)
                    .catch(error);
             
            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for resetPassword - ' + response.data);
                return response.data;
            }
        }

        function getLicencedModules() {
            return $http.get('/api/custom/License/GetLicenseRegisterList')
              .then(complete)
              .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getLicencedModules - ' + response.data);
                return response.data;
            }
        }

        function getUUID() {
            return $http.get('/api/UniqueIDGenerator')
                          .then(complete)
                          .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getLicencedModules - ' + response.data);
                return response.data;
            }
        }

        function encryptData(data) {
            return $http.post("/api/custom/Cryptography/EncryptData",
                     data)
                  .then(complete)
                  .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for encryptData - ' + response.data);
                return response.data;
            }
        }

        function decryptData(data) {
            return $http.post("/api/custom/Cryptography/DecryptData",
                     data)
                  .then(complete)
                  .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for decryptData - ' + response.data);
                return response.data;
            }
        }
    }

})();