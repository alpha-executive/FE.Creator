(function () {
    "use strict";
    angular
         .module('ngObjectRepository')
         .controller("editObjectDefinitionGroupController", editObjectDefinitionGroupController)
         .config(function ($routeProvider, $locationProvider) {
             $routeProvider
              .when('/defgrps/:groupId', {
                  templateUrl: '/ngView/ObjectRepository/ObjectDefinitionGroupEdit',
                  controller: 'editObjectDefinitionGroupController',
              })
              .when('/defgrps/:iscreate/:groupId',{
                  templateUrl: '/ngView/ObjectRepository/ObjectDefinitionGroupEdit',
                  controller: 'editObjectDefinitionGroupController',
              })
             .when('/defgrps', {
                 template: "<object-definition-group-list></object-definition-group-list>"
             })
             .otherwise('/defgrps');

             // configure html5 to get links working on jsfiddle
             $locationProvider.html5Mode({
                 enabled: true,
                 requireBase: false
             });
         });

    editObjectDefinitionGroupController.$inject = ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService"];


    /*controller*/
    function editObjectDefinitionGroupController($scope, $route, $routeParams, $location, ObjectRepositoryDataService) {
        var scopeContext = $scope;
        scopeContext.$route = $route;
        scopeContext.$location = $location;
        scopeContext.$routeParams = $routeParams;

        scopeContext.currentGroup = {};
        scopeContext.SaveChange = SaveChange;
        scopeContext.ShowWarningMsg = false;

        //if it's update.
        if ($routeParams.iscreate != null && $routeParams.iscreate == "true") {
            LoadParentGroup();
        }
        else {
            Initialze();
        }

        function SaveChange() {
            try{
                ObjectRepositoryDataService.createOrUpdateDefinitionGroup(scopeContext.currentGroup.groupID,
                    scopeContext.currentGroup)
                    .then(function(data){
                            scopeContext.CurrMessageClass = "alert-success";
                            scopeContext.UpdateMessage = "Succeed Update the Definition Group";

                            //for create, we will update the currentGroup model.
                            if (scopeContext.currentGroup.groupID == null)
                                scopeContext.currentGroup = data;
                        });
            }
            catch(e)
            {
                scopeContext.CurrMessageClass = "alert-error";
                scopeContext.UpdateMessage = "Failed to update the Definition Group" + e;
            }
            scopeContext.ShowWarningMsg = true;
        }

        function LoadParentGroup() {
            return ObjectRepositoryDataService.getObjectDefinitionGroup($routeParams.groupId)
                       .then(function (data) {
                           scopeContext.currentGroup.parentGroup = data;

                           return scopeContext.currentGroup;
                       });
        }

        function Initialze() {
            return ObjectRepositoryDataService.getObjectDefinitionGroup($routeParams.groupId)
                        .then(function (data) {
                            scopeContext.currentGroup = data;

                            return scopeContext.currentGroup;
                        });
        }
    }


    /*directive*/
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
                onDelete: "&",
                drillDown: "&"
            },
            templateUrl: "/ngView/ObjectRepository/ObjectDefinitionGroup"
        };
    }

    function objectDefinitionGroupList()
    {
        return {
            restrict: "E",
            controller: ["$scope", "$route", "$routeParams", "$location", "ObjectRepositoryDataService", function ($scope, $route, $routeParams, $location, ObjectRepositoryDataService) {
                var dfContext = $scope;
                dfContext.DefinitionGroups = [];
                dfContext.parentGroupID = -1;
                dfContext.DeleteGroup = function (group) {
                    try {
                        ObjectRepositoryDataService.deleteDefinitionGroup(group.groupID);
                        var index = dfContext.DefinitionGroups.indexOf(group);
                        if (index >= 0) {
                            dfContext.DefinitionGroups.splice(index, 1);
                        }
                    }
                    catch (e) { }
                };

                dfContext.DrillDown = function (group) {
                    dfContext.parentGroupID = group.groupID;
                    Activate(group.groupID);
                };

                Activate(null);

                function Activate(parentGroupId) {
                    return ObjectRepositoryDataService.getObjectDefinitionGroups(parentGroupId)
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