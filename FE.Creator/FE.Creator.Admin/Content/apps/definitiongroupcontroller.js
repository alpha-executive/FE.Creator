(function () {
    "use strict";

    /*directive*/
    angular
        .module('ngObjectRepository')
        .directive("objectDefinitionGroupList", objectDefinitionGroupList)
        .directive("objectDefinitionGroup", objectDefinitionGroup);

    /*controller*/
    angular
         .module('ngObjectRepository')
         .controller("editObjectDefinitionGroupController", editObjectDefinitionGroupController);
    editObjectDefinitionGroupController.$inject = ["$scope", "ObjectRepositoryDataService"];

    function editObjectDefinitionGroupController($scope, ObjectRepositoryDataService) {
        var scopeContext = $scope;
        scopeContext.currentGroup = {};
        scopeContext.SaveChange = SaveChange;
        scopeContext.Initialzie = Initialzie;

        function SaveChange(){

        }

        function Initialzie() {
                 return ObjectRepositoryDataService.getObjectDefinitionGroup(1)
                        .then(function (data) {
                            $scope.currentGroup = data;

                            return $scope.currentGroup;
                        });
        }
    }

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
                    return ObjectRepositoryDataService.getObjectDefinitionGroups(null)
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