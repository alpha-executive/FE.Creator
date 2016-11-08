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
            createOrUpdateObjectDefintion: createOrUpdateObjectDefintion,
            deleteObjectDefintionField: deleteObjectDefintionField,
            deleteObjectDefintion: deleteObjectDefintion,
            deleteSingleSelectionFieldItem: deleteSingleSelectionFieldItem
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
                logger.error('XHR Failed for getObjectDefinitionGroups - ' + error.data);
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
               logger.error('XHR Failed for getObjectDefinitionGroup - ' + error.data);
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
                logger.error('XHR Failed for createOrUpdateDefinitionGroup - ' + error.data);
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
                logger.error('XHR Failed for deleteDefinitionGroup - ' + error.data);
            }
        }

        //===============================================Object Defintion API ================
        function getObjectDefintionsbyGroup(groupId) {
            var url = "/api/custom/ObjectDefinition/GetObjectDefintionsByGroup";

            if (groupId != null)
                url = url + "/" + groupId;

            return $http.get(url)
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getObjectDefintionsbyGroup - ' + error.data);
            }
        }

        function getObjectDefinitionById(objectId) {
            return $http.get('/api/ObjectDefinition/' + objectId)
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getObjectDefintionsbyGroup - ' + error.data);
            }
        }

        function getLightWeightObjectDefinitions() {
            return $http.get('/api/custom/ObjectDefinition/Get')
                .then(complete)
                .catch(error);

            function complete(response) {
                return response.data;
            }

            function error(response) {
                logger.error('XHR Failed for getLightWeightObjectDefinitions - ' + error.data);
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
                logger.error('XHR Failed for createOrUpdateObjectDefintion - ' + error.data);
            }
        }

        function deleteObjectDefintionField(id) {
            return $http.delete("/api/ObjectDefinitionField/" + id)
                           .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteObjectDefintionField - ' + error.data);
            }
        }

        function deleteObjectDefintion(id) {
            return $http.delete("/api/ObjectDefinition/" + id)
                          .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteObjectDefintion - ' + error.data);
            }
        }

        function deleteSingleSelectionFieldItem(id) {
            return $http.delete("/api/SingleSelectionFieldItem/" + id)
                         .catch(error);

            function error(response) {
                logger.error('XHR Failed for deleteSingleSelectionFieldItem - ' + error.data);
            }
        }
    }

})();