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
            getObjectDefinitionGroup: getObjectDefinitionGroup
        };

        //get the object defintion groups
        function getObjectDefinitionGroups() {
           return   $http.get("/api/ObjectDefinitionGroup")
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

            function error(response)
            {
                logger.error('XHR Failed for getObjectDefinitionGroup - ' + error.data);
            }
        }
    }

})();