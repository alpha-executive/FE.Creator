(function () {
    "use strict";

    /*angular controller decalration*/
    angular
        .module('ngObjectRepository')
        .directive("objectDefinitionGroupList", objectDefinitionGroupList)
        .directive("objectDefinitionGroup", objectDefinitionGroup);
        

    function objectDefinitionGroup() {
        return {
            require: '^^objectDefinitionGroupList',
            restrict: "E",
            scope: {
                currentGroup: "=group",
                onDelete: "&"
            },
            templateUrl: "/ngView/ObjectRepository/ObjectDefinitionGroup"
        };
    }

    function objectDefinitionGroupList()
    {
        return {
            restrict: "E",
            controller: ["$scope", "ObjectRepositoryDataService", function ($scope, ObjectRepositoryDataService) {
                var dfContext = $scope;
                dfContext.DefinitionGroups = [];
                dfContext.DeleteGroup = function (group) {
                    var index = dfContext.DefinitionGroups.indexOf(group);
                    if (index >= 0) {
                        dfContext.DefinitionGroups.splice(index, 1);
                    }
                }

                Activate();

                function Activate() {
                    return ObjectRepositoryDataService.getObjectDefinitionGroups()
                                .then(function (data) {
                                    dfContext.DefinitionGroups = data;

                                    return dfContext.DefinitionGroups;
                                });
                }
            }],
            transclude: true,
            templateUrl: "/ngView/ObjectRepository/ObjectDefinitionGroupList"
        };
    }
})();