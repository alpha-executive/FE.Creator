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
            getObjectDefinitionById: getObjectDefinitionById
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
    }

})();